using AutoMapper;
using BE.AutoMapper;
using ENTITIES.DbContent;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MODELS.USER.Requests;

namespace BE.Configure
{
    public static class ConfigService
    {
        public static void Config(this WebApplicationBuilder builder)
        {
            SystemSetting(builder);


        }

        public static void SystemSetting(WebApplicationBuilder builder)
        {
            // Cấu hình DbContext
            builder.Services.AddDbContext<LINKUContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("LinkU"));
            });

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
            builder.Services.AddMvc(config => {  })
                .AddFluentValidation(config =>
                {
                    config.ImplicitlyValidateChildProperties = true;
                    config.DisableDataAnnotationsValidation = true;
                    config.RegisterValidatorsFromAssemblyContaining<LoginRequestValidator>();
                })
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        }
    }
}
