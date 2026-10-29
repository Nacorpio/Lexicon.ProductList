using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Lexicon.ProductList;

public static class ConsoleHelper
{
	private static readonly CultureInfo cultureInfo = CultureInfo.GetCultureInfo("sv-SE");

	public static void DrawLine(int totalMinWidth)
	{
		Console.WriteLine(new string('-', totalMinWidth));
	}

	private static void DrawProductFooter(IReadOnlyList<Product> productEntries)
	{
		Console.WriteLine($"TOTAL PRICE: {productEntries.Sum(p => p.Price).ToString("C", cultureInfo)}");
		Console.WriteLine($"AVERAGE PRICE: {productEntries.Average(p => p.Price).ToString("C", cultureInfo)}");
	}

	public static void DrawProductTable(string title, IReadOnlyList<Product> productEntries, ConsoleColor? rowColor = null)
	{
		var idMinWidth = Math.Max("Id".Length, productEntries.Max(p => p.Id.ToString().Length)) + 3;
		var nameMinWidth = Math.Max("Name".Length, productEntries.Max(p => p.Name.Length)) + 3;
		var categoryMinWidth = Math.Max("Category".Length, productEntries.Max(p => p.Category.Length)) + 3;
		var priceMinWidth = Math.Max("Price".Length, productEntries.Max(p => p.Price.ToString("C", cultureInfo).Length)) + 3;

		var totalMinWidth = idMinWidth + nameMinWidth + categoryMinWidth + priceMinWidth + 12;

		Console.WriteLine($"--- {title} ({productEntries.Count} product(s)) ---");
		Console.WriteLine();

		DrawLine(totalMinWidth);

		Console.Write($" {"Id".PadRight(idMinWidth)} |");
		Console.Write($" {"Name".PadRight(nameMinWidth)} |");
		Console.Write($" {"Category".PadRight(categoryMinWidth)} |");
		Console.Write($" {"Price".PadRight(priceMinWidth)} |");

		Console.WriteLine();

		DrawLine(totalMinWidth);

		foreach (var product in productEntries.OrderBy(p => p.Price).ThenBy(p => p.Name))
		{
			var row =
				$" {product.Id.ToString().PadRight(idMinWidth)} |" +
				$" {product.Name.PadRight(nameMinWidth)} |" +
				$" {product.Category.PadRight(categoryMinWidth)} |" +
				$" {product.Price.ToString("C", cultureInfo).PadRight(priceMinWidth)} |";

			PrintLine(row, rowColor);
		}

		DrawLine(totalMinWidth);

		Console.WriteLine();

		DrawProductFooter(productEntries);

		Console.WriteLine();
		Console.WriteLine("Press any key to go back");

		Console.ReadLine();
	}

	public static void Print(string text, ConsoleColor? color = null)
	{
		if (color is { } foreground)
			Console.ForegroundColor = foreground;

		Console.Write(text);

		if (color is not null)
			Console.ResetColor();
	}

	public static void PrintLine(string text, ConsoleColor? color = null)
	{
		Print(text, color);
		Console.WriteLine();
	}

	public static void PrintResult(Result result)
	{
		if (result.IsSuccess)
		{
			PrintLine(result.Message, ConsoleColor.Green);
			return;
		}

		PrintLine(result.Message, ConsoleColor.Red);
	}

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
