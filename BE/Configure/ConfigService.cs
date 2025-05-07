using AutoMapper;
using Azure.Storage.Blobs;
using BE.AutoMapper;
using BE.Services.FriendRequest;
using BE.Services.FriendShip;
using BE.Services.Mail;
using BE.Services.MediaFile;
using BE.Services.Message;
using BE.Services.MessageList;
using BE.Services.OTP;
using BE.Services.SMS;
using BE.Services.User;
using ENTITIES.DbContent;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MODELS.USER.Requests;

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
                       .EnableSensitiveDataLogging();
            });

            // Cấu hình Azure Blob Storage
            builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration["AzureBlobStorage:ConectionString"]));

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

            // Register service
            RegisterService(builder);
        }

        public static void RegisterService(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IUSERService, USERService>();
            builder.Services.AddTransient<IMESSAGEService, MESSAGEService>();
            builder.Services.AddTransient<IMAILService, MAILService>();
            builder.Services.AddTransient<ISMSService, SMSService>();
            builder.Services.AddTransient<IOTPService, OTPService>();
            builder.Services.AddTransient<IMESSAGELISTService, MESSAGELISTService>();
            builder.Services.AddTransient<IFRIENDREQUESTService, FRIENDREQUESTService>();
            builder.Services.AddTransient<IFRIENDSHIPService, FIRENDSHIPService>();
            builder.Services.AddScoped<IMEDIAFILEService, MEDIAFILEService>();
        }
    }
}
