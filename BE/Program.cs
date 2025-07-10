using BE.Configure;
using BE.Hubs;
using MODELS.MAIL.Dtos;
using MODELS.SMS.Dtos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Config hệ thống và service
builder.Config();
// Config authentication
builder.ExecuteConfigAuthentication();
// Add SignalR
builder.Services.AddSignalR();
// Add Cors
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
// Config Email setting
builder.Services.Configure<MODELMailSettings>(builder.Configuration.GetSection("MailSettings"));
// Config SMS setting
builder.Services.Configure<SMSoptions>(builder.Configuration.GetSection("SmsSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();
// Truy cập hình ảnh
app.UseStaticFiles();
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<MessageHub>("/messageHub");
app.MapControllers();

app.Run();
