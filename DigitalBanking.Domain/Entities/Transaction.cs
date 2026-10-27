using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalBanking.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AccountId { get; set; }
    public string Type { get; set; } = string.Empty; // "Deposit" o "Withdrawal"
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public decimal ResultingBalance { get; set; }
    public Account Account { get; set; } = null!;
}
