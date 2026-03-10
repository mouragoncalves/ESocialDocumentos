using ESocial.Application.Interfaces;
using ESocial.Domain.Repositories;
using ESocial.Infrastructure.Persistence;
using ESocial.Infrastructure.Persistence.Repositories;
using ESocial.Infrastructure.Validation;
using ESocial.Infrastructure.WebService.Adapters;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger (Swashbuckle) — disponível apenas em Development
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "eSocial API",
        Version = "v1",
        Description = "API de integração com o eSocial — envio e consulta de eventos trabalhistas.",
        Contact = new OpenApiContact { Name = "Stark Industries TI" }
    });
});

// MediatR — registra todos os handlers do assembly Application
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ESocial.Application.UseCases.EnviarLote.EnviarLoteHandler).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(ESocial.Application.UseCases.EnviarLote.EnviarLoteHandler).Assembly);

// EF Core + MySQL (Pomelo)
var connectionString = builder.Configuration.GetConnectionString("Default");
if (!string.IsNullOrWhiteSpace(connectionString))
    builder.Services.AddDbContext<ESocialDbContext>(options =>
        options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

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
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "eSocial API v1");
        options.RoutePrefix = "swagger";
    });
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.MapControllers();

app.Run();
