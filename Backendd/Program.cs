using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Backendd.ModelFromDB;
using Backendd.Service;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký Controllers và Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Backendd API", Version = "v1" });
    c.OperationFilter<FileUploadOperationFilter>();
});

// 2. Cấu hình DbContext
builder.Services.AddDbContext<DBCnhom1>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("cnn")));

// 3. Cấu hình FormOptions và Kestrel để cho phép upload file lớn (tới 10GB)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10L * 1024L * 1024L * 1024L;
});
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 10L * 1024L * 1024L * 1024L;
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(30);
});

// 4. Cấu hình JWT Authentication
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// 5. Cấu hình Hangfire với SQL Server Storage
builder.Services.AddHangfire(config => config
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
builder.Services.AddHangfireServer();

// 6. Cấu hình SignalR
builder.Services.AddSignalR();

// 7. Đăng ký HttpClient và VideoProcessingService
builder.Services.AddHttpClient();
builder.Services.AddScoped<VideoProcessingService>();

// 8. Cấu hình CORS để cho phép origin của client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost7180", policy =>
    {
        policy.WithOrigins("https://localhost:7180") // Add the client origin
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR
    });
});

var app = builder.Build();

// 9. Sử dụng Swagger trong môi trường Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backendd API v1");
    });
}

// 10. Sử dụng CORS trước các middleware khác
app.UseCors("AllowLocalhost7180");

// 11. Bắt buộc sử dụng HTTPS (optional, comment out if using HTTP for development)
app.UseHttpsRedirection();

// 12. Thêm middleware Authentication và Authorization
app.UseAuthentication();
app.UseAuthorization();

// 13. Sử dụng Hangfire Dashboard
app.UseHangfireDashboard();

// 14. Map SignalR Hub
app.MapHub<VideoProcessingHub>("/videoProcessingHub");

// 15. Map Controllers endpoints
app.MapControllers();

app.Run();

/// <summary>
/// Operation Filter giúp Swagger tạo schema chính xác cho các endpoint nhận file upload thông qua IFormFile.
/// </summary>
public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParams = context.ApiDescription.ParameterDescriptions
            .Where(p => p.ModelMetadata != null && p.ModelMetadata.ModelType == typeof(IFormFile))
            .ToList();

        if (!fileParams.Any())
            return;

        foreach (var param in fileParams)
        {
            var existingParam = operation.Parameters.FirstOrDefault(p => p.Name == param.Name);
            if (existingParam != null)
                operation.Parameters.Remove(existingParam);
        }

        operation.RequestBody = new OpenApiRequestBody
        {
            Content =
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["video"] = new OpenApiSchema
                            {
                                Description = "File upload",
                                Type = "string",
                                Format = "binary"
                            }
                        },
                        Required = new HashSet<string> { "video" }
                    }
                }
            }
        };
    }
}