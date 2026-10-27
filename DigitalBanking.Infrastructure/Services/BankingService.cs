using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalBanking.Infrastructure.Services;

using DigitalBanking.Application.DTOs;
using DigitalBanking.Application.Interfaces;
using DigitalBanking.Application.Services;
using DigitalBanking.Domain.Entities;
using DigitalBanking.Domain.Exceptions;
using DigitalBanking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class BankingService : IBankingService
{
    private readonly BankingDbContext _context;
    private readonly IAccountNumberGenerator _generator;

    public BankingService(BankingDbContext context, IAccountNumberGenerator generator)
    {
        _context = context;
        _generator = generator;
    }

    public async Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            FullName = dto.FullName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            MonthlyIncome = dto.MonthlyIncome
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return new CustomerResponseDto(customer.Id, customer.FullName, customer.DateOfBirth, customer.Gender, customer.MonthlyIncome);
    }

    public async Task<AccountResponseDto> CreateAccountAsync(CreateAccountDto dto)
    {
        var customer = await _context.Customers.FindAsync(dto.CustomerId)
            ?? throw new BusinessException("El cliente especificado no existe.", 404);

        if (dto.InitialBalance < 0)
            throw new BusinessException("El saldo inicial no puede ser negativo.");

        var account = new Account
        {
            CustomerId = customer.Id,
            AccountNumber = _generator.GenerateNumber(),
            Balance = dto.InitialBalance
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return new AccountResponseDto(account.Id, account.AccountNumber, account.Balance, account.CustomerId);
    }

    public async Task<decimal> GetBalanceAsync(string accountNumber)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber)
            ?? throw new BusinessException($"La cuenta '{accountNumber}' no fue encontrada.", 404);

        return account.Balance;
    }

    public async Task<TransactionHistoryDto> DepositAsync(string accountNumber, decimal amount)
    {
        if (amount <= 0)
            throw new BusinessException("El monto a depositar debe ser mayor a cero.");

        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber)
            ?? throw new BusinessException($"La cuenta '{accountNumber}' no fue encontrada.", 404);

        account.Balance += amount;

        var transaction = new Transaction
        {
            AccountId = account.Id,
            Type = "Deposit",
            Amount = amount,
            Timestamp = DateTime.UtcNow,
            ResultingBalance = account.Balance
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return new TransactionHistoryDto(transaction.Id, transaction.Type, transaction.Amount, transaction.Timestamp, transaction.ResultingBalance);
    }

    public async Task<TransactionHistoryDto> WithdrawAsync(string accountNumber, decimal amount)
    {
        if (amount <= 0)
            throw new BusinessException("El monto a retirar debe ser mayor a cero.");

        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber)
            ?? throw new BusinessException($"La cuenta '{accountNumber}' no fue encontrada.", 404);

        // Rechazo limpio por saldo insuficiente
        if (account.Balance < amount)
            throw new BusinessException("Fondos insuficientes para realizar el retiro.", 400);

        account.Balance -= amount;

        var transaction = new Transaction
        {
            AccountId = account.Id,
            Type = "Withdrawal",
            Amount = amount,
            Timestamp = DateTime.UtcNow,
            ResultingBalance = account.Balance
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return new TransactionHistoryDto(transaction.Id, transaction.Type, transaction.Amount, transaction.Timestamp, transaction.ResultingBalance);
    }

    public async Task<IEnumerable<TransactionHistoryDto>> GetTransactionHistoryAsync(string accountNumber)
    {
        var account = await _context.Accounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber)
            ?? throw new BusinessException($"La cuenta '{accountNumber}' no fue encontrada.", 404);

        return account.Transactions
            .OrderBy(t => t.Timestamp)
            .Select(t => new TransactionHistoryDto(t.Id, t.Type, t.Amount, t.Timestamp, t.ResultingBalance));
    }
}