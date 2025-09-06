using Application.Interface;
using Application.Interface.Auth;
using Application.Interface.Invoice;
using Application.Mapping;
using Application.Services;
using Application.Validators.Invoices;
using Azure.Identity;
using Domain.Entities.JWT;
using ExpenseManagementSystemAPI.Middleware;
using Infrastructure.Options;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContext;
using Persistence.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Load Azure Key Vault
        var keyVaultName = builder.Configuration["KeyVaultName"] ?? "expensemgmt-dev-keyvault";
        var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
        builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            var sqlConn = builder.Configuration["AzureSql:ConnectionString"];
            options.UseSqlServer(sqlConn);
        });

        builder.Services.Configure<AzureDocumentIntelligenceSettings>(options =>
        {
            options.Endpoint = builder.Configuration["Ocr-Endpoint"];
            options.ApiKey = builder.Configuration["Ocr-ApiKey"];
        });
        builder.Services.Configure<AzureBlobSettings>(builder.Configuration.GetSection("AzureBlobSettings"));
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));



        #region Dependency Injection
        builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        builder.Services.AddScoped<IInvoiceService, InvoiceService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IFileStorageService, FileStorageService>();
        builder.Services.AddScoped<IOcrInvoiceService, OcrInvoiceService>();
        #endregion

        // Configure Identity and JWT Authentication
        builder.Services.AddIdentityAndJwtAuthentication(builder.Configuration);
        builder.Services.AddApplicationServices();
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
