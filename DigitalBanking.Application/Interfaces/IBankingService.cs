using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalBanking.Application.Interfaces;

using DigitalBanking.Application.DTOs;

public interface IBankingService
{
    Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto dto);
    Task<AccountResponseDto> CreateAccountAsync(CreateAccountDto dto);
    Task<decimal> GetBalanceAsync(string accountNumber);
    Task<TransactionHistoryDto> DepositAsync(string accountNumber, decimal amount);
    Task<TransactionHistoryDto> WithdrawAsync(string accountNumber, decimal amount);
    Task<IEnumerable<TransactionHistoryDto>> GetTransactionHistoryAsync(string accountNumber);
}
