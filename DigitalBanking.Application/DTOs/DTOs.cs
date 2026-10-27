using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalBanking.Application.DTOs;

public record CreateCustomerDto(string FullName, DateTime DateOfBirth, string Gender, decimal MonthlyIncome);
public record CustomerResponseDto(Guid Id, string FullName, DateTime DateOfBirth, string Gender, decimal MonthlyIncome);

public record CreateAccountDto(Guid CustomerId, decimal InitialBalance);
public record AccountResponseDto(Guid Id, string AccountNumber, decimal Balance, Guid CustomerId);

public record TransactionDto(decimal Amount);
public record TransactionHistoryDto(Guid Id, string TransactionType, decimal Amount, DateTime Timestamp, decimal ResultingBalance);
