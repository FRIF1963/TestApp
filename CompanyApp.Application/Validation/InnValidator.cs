using System.Text.RegularExpressions;
using CompanyApp.Application.Exceptions;

namespace CompanyApp.Application.Validation;

public static class InnValidator
{
    private static readonly Regex InnPattern = new(@"^\d{10}$|^\d{12}$", RegexOptions.Compiled);

    public static void Validate(string inn)
    {
        if (string.IsNullOrWhiteSpace(inn))
        {
            throw new ValidationException("ИНН обязателен для заполнения.");
        }

        if (!InnPattern.IsMatch(inn.Trim()))
        {
            throw new ValidationException("ИНН должен содержать 10 или 12 цифр.");
        }
    }
}
