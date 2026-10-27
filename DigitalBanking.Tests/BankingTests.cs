using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalBanking.Tests;

using DigitalBanking.Application.DTOs;
using DigitalBanking.Application.Services;
using DigitalBanking.Domain.Exceptions;
using DigitalBanking.Infrastructure.Persistence;
using DigitalBanking.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class BankingTests
{
    // Método auxiliar para crear un DbContext aislado en memoria para cada prueba
    private BankingDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<BankingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new BankingDbContext(options);
    }

    // PRUEBA 1: Validar el algoritmo de generación del número de cuenta
    [Fact]
    public void GenerateNumber_ShouldFollowExpectedFormat()
    {
        // Arrange (Preparación)
        var generator = new AccountNumberGenerator();
        string todayFormat = DateTime.UtcNow.ToString("yyyyMMdd");

        // Act (Ejecución)
        string accountNumber = generator.GenerateNumber();

        // Assert (Verificación)
        Assert.StartsWith($"ACC-{todayFormat}-", accountNumber); // Verifica el prefijo ACC-YYYYMMDD-
        Assert.Equal(17, accountNumber.Length);                  // Formato: ACC-YYYYMMDD-XXXX (17 caracteres)
    }

    // PRUEBA 2: Validar un retiro exitoso cuando hay fondos suficientes
    [Fact]
    public async Task Withdraw_WithSufficientFunds_ShouldUpdateBalanceAndRecordTransaction()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var generator = new AccountNumberGenerator();
        var service = new BankingService(context, generator);

        // Crear cliente y cuenta con $500 iniciales
        var customer = await service.CreateCustomerAsync(new CreateCustomerDto("Carlos Lopez", DateTime.Now.AddYears(-25), "M", 3000));
        var account = await service.CreateAccountAsync(new CreateAccountDto(customer.Id, 500));

        // Act: Retirar $200
        var result = await service.WithdrawAsync(account.AccountNumber, 200);

        // Assert: El saldo resultante debe ser $300
        Assert.Equal(300, result.ResultingBalance);

        var currentBalance = await service.GetBalanceAsync(account.AccountNumber);
        Assert.Equal(300, currentBalance);
    }

    // PRUEBA 3: Validar que un retiro sea rechazado limpiamente por fondos insuficientes
    [Fact]
    public async Task Withdraw_WithInsufficientFunds_ShouldThrowBusinessException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var generator = new AccountNumberGenerator();
        var service = new BankingService(context, generator);

        // Crear cliente y cuenta con $100 iniciales
        var customer = await service.CreateCustomerAsync(new CreateCustomerDto("Ana Gomez", DateTime.Now.AddYears(-30), "F", 4000));
        var account = await service.CreateAccountAsync(new CreateAccountDto(customer.Id, 100));

        // Act & Assert: Intentar retirar $150 debe lanzar 'BusinessException'
        var exception = await Assert.ThrowsAsync<BusinessException>(() =>
            service.WithdrawAsync(account.AccountNumber, 150));

        // Comprobar mensaje de error de negocio
        Assert.Equal("Fondos insuficientes para realizar el retiro.", exception.Message);
        Assert.Equal(400, exception.StatusCode);
    }
}