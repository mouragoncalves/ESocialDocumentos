using ESocial.Application.Interfaces;
using ESocial.Domain.Repositories;
using ESocial.Infrastructure.Persistence;
using ESocial.Infrastructure.Persistence.Repositories;
using ESocial.Infrastructure.Validation;
using ESocial.Infrastructure.WebService.Adapters;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// MediatR — registra todos os handlers do assembly Application
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ESocial.Application.UseCases.EnviarLote.EnviarLoteHandler).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(ESocial.Application.UseCases.EnviarLote.EnviarLoteHandler).Assembly);

// EF Core + MySQL (Pomelo)
var connectionString = builder.Configuration.GetConnectionString("Default")!;
builder.Services.AddDbContext<ESocialDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Repositories
builder.Services.AddScoped<ILoteEventosRepository, LoteEventosRepository>();
builder.Services.AddScoped<IEmpregadorRepository, EmpregadorRepository>();

// Certificado digital
var certConfig = builder.Configuration
    .GetSection("ESocial:Certificado")
    .Get<CertificadoConfiguration>() ?? new CertificadoConfiguration();
builder.Services.AddSingleton(certConfig);

// WebService adapter
builder.Services.AddScoped<IESocialWebService, ESocialWebServiceAdapter>();

// XSD Validator
var schemasPath = builder.Configuration["ESocial:SchemasPath"] ?? string.Empty;
builder.Services.AddSingleton<IXmlValidator>(new XsdValidator(schemasPath));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "eSocial API";
        options.Theme = ScalarTheme.DeepSpace;
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
