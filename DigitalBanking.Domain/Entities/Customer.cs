using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace DigitalBanking.Domain.Entities;

public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public decimal MonthlyIncome { get; set; }
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
