using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TestMaster.Controllers;
using TestMaster.Data;
using TestMaster.Exceptions;
using TestMaster.Identity;
using TestMaster.Services.QuestionService;
using TestMaster.Services.QuestionService.QuestionBankService;
using TestMaster.Services.TestService;
using TestMaster.Services.UserService;
using TestMaster.Validations.Users;

namespace Microsoft.Extensions.DependencyInjection;

public static class Dependencies
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddAPIVersioning()
               .AddDataBase()
               .AddHttps()
               .AddExceptionHandler()
               .AddResponseCompression()
               .AddRateLimiter()
               .AddJWTAuthentication(configuration)
               .AddMemoryCaching()
               .AddValidation()
               .AddAuthentications()
               .AddCORS()
               .AddProblemDetail()
               .AddBusinessServices()
               .AddControllerWithJsonConfiguration()
               .AddSwaggerGen();
        return service;
    }
    public static IServiceCollection AddDataBase(this IServiceCollection service)
    {
        service.AddDbContext<AppDbContext>(op =>
        {
            op.UseSqlServer("server = .; Database = TestMaster; Integrated security = SSPI; TrustServerCertificate = true;");
        });
        return service;
    }
    public static IServiceCollection AddHttps(this IServiceCollection service)
    {
        service.AddHttpsRedirection(options =>
        {
            options.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
        });
        return service;
    }
    public static IServiceCollection AddControllerWithJsonConfiguration(this IServiceCollection service)
    {
        service.AddControllers()
               .AddApplicationPart(typeof(QuestionBanksController).Assembly)
               .AddJsonOptions(Options =>
                {
                    Options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    Options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });
        return service;
    }
    public static IServiceCollection AddExceptionHandler(this IServiceCollection service)
    {
        service.AddExceptionHandler<GlobalExceptionHandler>();
        return service;
    }
    public static IServiceCollection AddResponseCompression(this IServiceCollection service)
    {
        service.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.MimeTypes = ["application/json"];
            options.Providers.Add<GzipCompressionProvider>();
            options.Providers.Add<BrotliCompressionProvider>();
        });
        return service;
    }
    public static IServiceCollection AddRateLimiter(this IServiceCollection service)
    {
        service.AddRateLimiter(options =>
        {
            options.AddSlidingWindowLimiter("SlidingWindow", limiterOptions =>
            {
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.PermitLimit = 5;
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 5;
                limiterOptions.SegmentsPerWindow = 6;
                limiterOptions.AutoReplenishment = true;
            });
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsync("Too many request. Please wait a moment before trying again.");
            };
        });
        return service;
    }
    public static IServiceCollection AddJWTAuthentication(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var Config = configuration.GetSection("JWT");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Config["Issuer"],
                ValidAudience = Config["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["Key"]!.ToString()))
            };
        });
        return service;
    }
    public static IServiceCollection AddMemoryCaching(this IServiceCollection service)
    {
        service.AddMemoryCache(options =>
        {
            options.SizeLimit = 100;
        });
        return service;
    }
    public static IServiceCollection AddAPIVersioning(this IServiceCollection service)
    {
        service.AddApiVersioning(option =>
        {
            option.AssumeDefaultVersionWhenUnspecified = true;
            option.DefaultApiVersion = new ApiVersion(1, 0);
            option.ReportApiVersions = true;
            option.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddVersionedApiExplorer(option =>
        {
            option.GroupNameFormat = "'v'VVVV";
            option.SubstituteApiVersionInUrl = true;
        });
        return service;
    }
    public static IServiceCollection AddValidation(this IServiceCollection service)
    {
        service.AddValidatorsFromAssemblyContaining<UserValidator>();
        service.AddFluentValidationAutoValidation();
        service.AddFluentValidationClientsideAdapters();
        return service;
    }
    public static IServiceCollection AddAuthentications(this IServiceCollection service)
    {
        service.AddAuthorization(options =>
        {
            options.AddPolicy("Activate", policy =>
            {
                policy.RequireClaim("Activate", "true");
            });
        });
        return service;
    }
    public static IServiceCollection AddCORS(this IServiceCollection service)
    {
        service.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyMethod();
            });
        });
        return service;
    }
    public static IServiceCollection AddProblemDetail(this IServiceCollection service)
    {
        service.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = (context) =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
            };
        });
        return service;
    }
    public static IServiceCollection AddBusinessServices(this IServiceCollection service)
    {
        service.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });
        service.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        service.AddScoped<JwtTokenProvider>();
        service.AddScoped<IUserService, UserService>();
        service.AddScoped<IQuestionService, QuestionService>();
        service.AddScoped<ITestService, TestService>();
        service.AddScoped<IQuestionBankService, QuestionBankService>();

        return service;
    }
    public static IServiceCollection AddSwaggerGen(this IServiceCollection service)
    {
        service.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "TestMaster API V1",
                Version = "v1.0",
                Description = "TestMaster Web API"
            });

            options.SwaggerDoc("v2", new OpenApiInfo
            {
                Title = "TestMaster API V2",
                Version = "v2.0",
                Description = "TestMaster Web API V2"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    Array.Empty<string>()
                }
            });

            options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                return true;
            });

            try
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            }
            catch
            {
            }
        }).AddEndpointsApiExplorer();
        return service;
    }
}