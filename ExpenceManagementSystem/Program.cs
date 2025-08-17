using Application.Interface;
using Application.Interface.Auth;
using Application.Interface.Invoice;
using Application.Mapping;
using Application.Services;
using Application.Validators.Invoices;
using Domain.Entities.JWT;
using ExpenseManagementSystemAPI.Middleware;
using Infrastructure.Options;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContext;
using Persistence.Repository;
using System;
using System.Text;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
        builder.Services.Configure<AzureBlobSettings>(builder.Configuration.GetSection("AzureBlobSettings"));
        builder.Services.Configure<AzureDocumentIntelligenceSettings>(builder.Configuration.GetSection("AzureDocumentIntelligenceSettings"));



        #region Dependency Injection
        builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        builder.Services.AddScoped<IInvoiceService, InvoiceService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IFileStorageService, FileStorageService>();
        builder.Services.AddScoped<IOcrInvoiceService, OcrInvoiceService>();
        #endregion

        // Configure Identity and JWT Authentication
        builder.Services.AddIdentityAndJwtAuthentication(builder.Configuration);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

 
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
