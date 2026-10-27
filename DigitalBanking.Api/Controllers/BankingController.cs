namespace DigitalBanking.Api.Controllers;

using DigitalBanking.Application.DTOs;
using DigitalBanking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BankingController : ControllerBase
{
    private readonly IBankingService _bankingService;

    public BankingController(IBankingService bankingService)
    {
        _bankingService = bankingService;
    }

    [HttpPost("customers")]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
    {
        var result = await _bankingService.CreateCustomerAsync(dto);
        return CreatedAtAction(nameof(CreateCustomer), new { id = result.Id }, result);
    }

    [HttpPost("accounts")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto dto)
    {
        var result = await _bankingService.CreateAccountAsync(dto);
        return Ok(result);
    }

    [HttpGet("accounts/{accountNumber}/balance")]
    public async Task<IActionResult> GetBalance(string accountNumber)
    {
        var balance = await _bankingService.GetBalanceAsync(accountNumber);
        return Ok(new { AccountNumber = accountNumber, Balance = balance });
    }

    [HttpPost("accounts/{accountNumber}/deposit")]
    public async Task<IActionResult> Deposit(string accountNumber, [FromBody] TransactionDto dto)
    {
        var result = await _bankingService.DepositAsync(accountNumber, dto.Amount);
        return Ok(result);
    }

    [HttpPost("accounts/{accountNumber}/withdraw")]
    public async Task<IActionResult> Withdraw(string accountNumber, [FromBody] TransactionDto dto)
    {
        var result = await _bankingService.WithdrawAsync(accountNumber, dto.Amount);
        return Ok(result);
    }

    [HttpGet("accounts/{accountNumber}/history")]
    public async Task<IActionResult> GetHistory(string accountNumber)
    {
        var history = await _bankingService.GetTransactionHistoryAsync(accountNumber);
        return Ok(history);
    }
}
