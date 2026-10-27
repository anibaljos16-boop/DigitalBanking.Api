using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalBanking.Application.Services;

public interface IAccountNumberGenerator
{
    string GenerateNumber();
}

public class AccountNumberGenerator : IAccountNumberGenerator
{
    private static readonly Random _random = new();

    public string GenerateNumber()
    {
        string datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        int randomDigits = _random.Next(0, 10000);
        return $"ACC-{datePart}-{randomDigits:D4}";
    }
}