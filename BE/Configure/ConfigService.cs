using AutoMapper;
using Azure.Storage.Blobs;
using BE.AutoMapper;
using BE.Services.Conversation;
using BE.Services.FriendRequest;
using BE.Services.FriendShip;
using BE.Services.Group;
using BE.Services.GroupMember;
using BE.Services.GroupRequest;
using BE.Services.Mail;
using BE.Services.MediaFile;
using BE.Services.Message;
using BE.Services.MessageReaction;
using BE.Services.OTP;
using BE.Services.ReactionType;
using BE.Services.Redis;
using BE.Services.SMS;
using BE.Services.User;
using ENTITIES.DbContent;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MODELS.USER.Requests;
using StackExchange.Redis;
using System.Net.WebSockets;

namespace BE.Configure
{
    public static class ConfigService
    {
        public static void Config(this WebApplicationBuilder builder)
        {
            // SYSTEM
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Cấu hình DbContext
            builder.Services.AddDbContext<LINKUContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("LinkU"))
                // Hiển thị log SQL
                //.EnableSensitiveDataLogging();
                .LogTo(_ => { });
            }, ServiceLifetime.Scoped);

            // Cấu hình Azure Blob Storage
            builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration["AzureBlobStorage:ConnectionString"]));

            // Thêm AutoMapper
            var mappigConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappigConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);

            // Tắt tự động validate model
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // Thêm Fluent Validation
            builder.Services.AddMvc(config => { })
                .AddFluentValidation(config =>
                {
                    config.ImplicitlyValidateChildProperties = true;
                    config.DisableDataAnnotationsValidation = true;
                    config.RegisterValidatorsFromAssemblyContaining<LoginRequestValidator>();
                })
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            //
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "LinkU", Version = "v1" });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Nhập Token của bạn ở đây",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials().SetIsOriginAllowed((hosts) => true);
                    });
            });

            // Appsetting
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            // Redis
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                string redisUrl = builder.Configuration["Redis:ConnectionString"];
                var uri = new Uri(redisUrl);
                var userInfoParts = uri.UserInfo.Split(':');
                if (userInfoParts.Length != 2)
                {
                    throw new InvalidOperationException("REDIS_URL is not in the expected format ('redis://user:password@host:port')");
                }

                var configurationOptions = new ConfigurationOptions
                {
                    EndPoints = { { uri.Host, uri.Port } },
                    Password = userInfoParts[1],
                    //Ssl = true,
                    Ssl = false
                };
                configurationOptions.CertificateValidation += (sender, cert, chain, errors) => true;
                return ConnectionMultiplexer.Connect(configurationOptions);
            });

            // Register service
            RegisterService(builder);
        }

        public static void RegisterService(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUSERService, USERService>();
            builder.Services.AddScoped<IMESSAGEService, MESSAGEService>();
            builder.Services.AddScoped<IMAILService, MAILService>();
            builder.Services.AddScoped<ISMSService, SMSService>();
            builder.Services.AddScoped<IOTPService, OTPService>();
            //builder.Services.AddTransient<IMESSAGELISTService, MESSAGELISTService>();
            builder.Services.AddScoped<IFRIENDREQUESTService, FRIENDREQUESTService>();
            builder.Services.AddScoped<IFRIENDSHIPService, FIRENDSHIPService>();
            builder.Services.AddScoped<IMEDIAFILEService, MEDIAFILEService>();
            builder.Services.AddScoped<ICONVERSATIONService, CONVERSATIONService>();
            builder.Services.AddScoped<IGROUPService, GROUPService>();
            builder.Services.AddScoped<IGROUPMEMBERService, GROUPMEMBERService>();
            builder.Services.AddScoped<IGROUPREQUESTService, GROUPREQUESTService>();
            builder.Services.AddScoped<IREACTIONTYPEService, REACTIONTYPEService>();
            builder.Services.AddScoped<IMESSAGEREACTIONService, MESSAGEREACTIONService>();
            builder.Services.AddScoped<IREDISService, REDISService>();
        }
    }
}
