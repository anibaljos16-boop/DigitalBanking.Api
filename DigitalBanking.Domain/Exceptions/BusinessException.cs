using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalBanking.Domain.Exceptions;

public class BusinessException : Exception
{
    public int StatusCode { get; }

    public BusinessException(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }
}