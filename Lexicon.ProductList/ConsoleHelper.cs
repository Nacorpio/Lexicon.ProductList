using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Lexicon.ProductList;

public static class ConsoleHelper
{
    public static Result<string> ConvertTextInput(string input) => Result<string>.Success(input);

    public static Result<int> ConvertMenuOptionInput(string input)
    {
        if (!int.TryParse(input, out var result))
        {
            return Result<int>.Error("ERROR: Invalid menu option. Please enter a number.");
        }

        if (result < 1 || result > 4)
        {
            return Result<int>.Error("ERROR: Menu option out of range. Please select a valid option.");
        }

        return Result<int>.Success(result);
    }

    public static Result<decimal> ConvertPriceInput(string input)
    {
        const NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;

        if (!decimal.TryParse(input.Replace(',', '.'), style, CultureInfo.InvariantCulture, out var price))
            return Result<decimal>.Error("ERROR: Invalid price. Please enter a numeric value.");

        if (price < 0)
            return Result<decimal>.Error("ERROR: Price cannot be negative.");

        return Result<decimal>.Success(price);
    }

    public static bool TryHandleInput<T>(
        string message, string fieldName,
        Func<string, Result<T>> convertor,
        [MaybeNullWhen(false)] out T value)
    {
        while (true)
        {
            Console.Write($"{message} (`q` to exit): ");

            var input = Console.ReadLine()?.Trim();

            if (input is null || string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
            {
                value = default;
                return false;
            }

            var result = input.Length == 0
                ? Result<T>.Error($"ERROR: The {fieldName.ToLower()} cannot be empty.")
                : convertor(input);

            if (result.IsSuccess)
            {
                value = result.Value;
                return true;
            }

            Console.WriteLine(result.Text);
        }
    }
}
