using DigitalBanking.Api.Middlewares;
using DigitalBanking.Application.Interfaces;
using DigitalBanking.Application.Services;
using DigitalBanking.Infrastructure.Persistence;
using DigitalBanking.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar la base de datos SQLite
builder.Services.AddDbContext<BankingDbContext>(options =>
    options.UseSqlite("Data Source=banking.db"));

// 2. Inyección de dependencias
builder.Services.AddScoped<IAccountNumberGenerator, AccountNumberGenerator>();
builder.Services.AddScoped<IBankingService, BankingService>();

// 3. Manejador Global de Excepciones
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4. Configurar Middleware
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Digital Banking API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseAuthorization();
app.MapControllers();

// 5. Inicialización de la Base de Datos SQLite
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BankingDbContext>();
    db.Database.EnsureCreated();
}

app.Run();