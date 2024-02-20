using System.Text;
using AspNetCore.Authentication.ApiKey;
using Dapper;
using JenkinsPlayground.Server.Data;
using JenkinsPlayground.Server.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace JenkinsPlayground.Server;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        StaticConfiguration.Initialize(Configuration);
        var dbConnectionString = StaticConfiguration.ConnectionStringsJenkinsPlaygroundDB;

        services.AddCors();
        services.AddControllers();
        
        // configure jwt authentication
        var key = Encoding.ASCII.GetBytes(StaticConfiguration.AppSettingsSecret);
        services
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            })
            .AddApiKeyInQueryParams<ApiKeyProvider>(x =>
            {
                x.Realm = "ApiKey";
                x.KeyName = "apiKey";
            });
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                "AuthPolicy",
                policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.AuthenticationSchemes.Add(ApiKeyDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                }
            );

            options.DefaultPolicy = options.GetPolicy("AuthPolicy");
        });
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Version = "v1",
                    Title = "JenkinsPlayground",
                    Description = "JenkinsPlayground Server API",
                    TermsOfService = new Uri("https://playground.hr"),
                }
            );

            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http
                }
            );

            c.AddSecurityDefinition(
                "apiKey",
                new OpenApiSecurityScheme
                {
                    Name = "apiKey",
                    Scheme = "apiKey",
                    In = ParameterLocation.Query,
                    Type = SecuritySchemeType.ApiKey
                }
            );

            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "apiKey"
                            }
                        },
                        new List<string>()
                    }
                }
            );

            c.CustomSchemaIds(type => type.ToString());
        });
        
        // configure DI for application services
        services.AddDbContext<JenkinsPlaygroundContext>(options => options.UseNpgsql(dbConnectionString));
        services.AddHttpContextAccessor();
        services.AddAutoMapper(typeof(Mappings.Mappings));
        services.AddSingleton<DateTimeProvider>();
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env,
        JenkinsPlaygroundContext jenkinsPlaygroundContext
    )
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            //NpgsqlLogManager.IsParameterLoggingEnabled = true;
        }

        app.UseExceptionHandler("/error");

        app.UseRouting();

        // global cors policy
        app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

        app.UseAuthentication();
        app.UseAuthorization();

        //app.UsePermissionLevel();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", context => context.Response.WriteAsync("Jenkins Playground API"));
            endpoints.MapGet("/health", context => context.Response.WriteAsync("ok"));
            endpoints.MapGet(
                "/version",
                context =>
                    context.Response.WriteAsync(
                        File.Exists("version.txt")
                            ? File.ReadAllText("version.txt")
                            : "non-production"
                    )
            );
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Jenkins Playground Server");
        });

        if (jenkinsPlaygroundContext.Database.GetPendingMigrations().Any())
        {
            Console.WriteLine("Applying DB migrations");
            jenkinsPlaygroundContext.Database.Migrate();
        }
    }
}