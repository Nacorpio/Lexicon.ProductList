using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Lexicon.ProductList;

public static class ConsoleHelper
{
	private static readonly CultureInfo cultureInfo = CultureInfo.GetCultureInfo("sv-SE");

    /// <summary>
    /// Draws a horizontal line of the specified width.
    /// </summary>
    /// <param name="totalMinWidth">The total width of the line.</param>
	public static void DrawLine(int totalMinWidth)
	{
		Console.WriteLine(new string('-', totalMinWidth));
	}

    /// <summary>
    /// Draws the footer for the product table, showing total and average prices.
    /// </summary>
    /// <param name="productEntries">The list of products to calculate totals for.</param>
    private static void DrawProductFooter(IReadOnlyList<Product> productEntries)
	{
		Console.WriteLine($"TOTAL PRICE: {productEntries.Sum(p => p.Price).ToString("C", cultureInfo)}");
		Console.WriteLine($"AVERAGE PRICE: {productEntries.Average(p => p.Price).ToString("C", cultureInfo)}");
	}

    /// <summary>
	/// Draws a table of products with the specified title and optional row color.
    /// </summary>
    /// <param name="title">The title of the product table.</param>
    /// <param name="productEntries">The list of products to display.</param>
    /// <param name="rowColor">The optional color for the table rows.</param>
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
	}

   
	public static void DrawStatistics(ProductStatistics statistics)
	{
		Console.WriteLine("========= STATISTICS =========");
		Console.WriteLine();

		Console.WriteLine("Most Expensive Product:");
		Console.WriteLine(FormatProductPrice(statistics.MostExpensive));
		Console.WriteLine();

		Console.WriteLine("Cheapest Product:");
		Console.WriteLine(FormatProductPrice(statistics.Cheapest));
		Console.WriteLine();

		Console.WriteLine("Average Price:");
		Console.WriteLine(statistics.AveragePrice.ToString("C", cultureInfo));
		Console.WriteLine();

		Console.WriteLine("Products per Category:");

		foreach (var (category, count) in statistics.ProductsPerCategory)
			Console.WriteLine($"{category}: {count}");
	}

	private static string FormatProductPrice(Product product) =>
		$"{product.Name} - {product.Price.ToString("C", cultureInfo)}";


	/// <summary>
	/// Pauses the console and waits for the user to press any key before returning.
	/// </summary>
	/// <remarks>
	/// This method is typically used to give the user time to read messages before returning to the main menu.
	/// </remarks>
	public static void PauseBeforeReturn()
	{
		Console.WriteLine();
		Console.WriteLine("Press any key to go back");

		Console.ReadLine();
	}

    /// <summary>
    /// Prints text to the console with an optional color.
    /// </summary>
    /// <param name="text">The text to print.</param>
    /// <param name="color">The optional color for the text.</param>
	public static void Print(string text, ConsoleColor? color = null)
	{
		if (color is { } foreground)
			Console.ForegroundColor = foreground;

		Console.Write(text);

		if (color is not null)
			Console.ResetColor();
	}

    /// <summary>
    /// Prints a line of text to the console with an optional color.
    /// </summary>
    /// <param name="text">The text to print.</param>
    /// <param name="color">The optional color for the text.</param>
	public static void PrintLine(string text, ConsoleColor? color = null)
	{
		Print(text, color);
		Console.WriteLine();
	}

    /// <summary>
    /// Prints the result message to the console with color indicating success or error.
    /// </summary>
    /// <param name="result">The result to print.</param>
	public static void PrintResult(Result result)
	{
		if (result.IsSuccess)
		{
			PrintLine(result.Message, ConsoleColor.Green);
			return;
		}

		PrintLine(result.Message, ConsoleColor.Red);
	}

	/// <summary>
	/// Asks a yes/no question and returns whether the user confirmed.
	/// </summary>
	/// <param name="question">The question to show, without the answer hint.</param>
	/// <returns><c>true</c> if the user confirmed; otherwise, <c>false</c>.</returns>
	public static bool Confirm(string question)
	{
		Print($"{question} (y/n): ");

		var input = Console.ReadLine();

		if (string.IsNullOrWhiteSpace(input))
			return false;

		input = input.Trim().ToLowerInvariant();

		return input is "y" or "yes";
	}

    /// <summary>
    /// Converts the input string to a text result.
    /// </summary>
    /// <param name="input">The input string to convert.</param>
    /// <returns>A <see cref="Result{T}"/> containing the converted text.</returns>
    public static Result<string> ConvertTextInput(string input) => Result<string>.Success(input);

    /// <summary>
    /// Converts the input string to an ID result.
    /// </summary>
    /// <param name="input">The input string to convert.</param>
    /// <returns>A <see cref="Result{T}"/> containing the converted ID.</returns>
    public static Result<int> ConvertIdInput(string input)
	{
		if (!int.TryParse(input, out var id))
			return Result<int>.Error("ERROR: Invalid ID. Please enter a number.");

		return Result<int>.Success(id);
	}

    /// <summary>
    /// Converts the input string to a menu option result.
    /// </summary>
    /// <param name="input">The input string to convert.</param>
    /// <returns>A <see cref="Result{T}"/> containing the converted menu option.</returns>
    public static Result<MenuOption> ConvertMenuOptionInput(string input)
    {
        if (!int.TryParse(input, out var number))
        {
            return Result<MenuOption>.Error("ERROR: Invalid menu option. Please enter a number.");
        }

        var option = (MenuOption)number;

        if (!Enum.IsDefined(option))
        {
            return Result<MenuOption>.Error("ERROR: Menu option out of range. Please select a valid option.");
        }

        return Result<MenuOption>.Success(option);
    }

    /// <summary>
    /// Converts the input string to a price result.
    /// </summary>
    /// <param name="input">The input string to convert.</param>
    /// <returns>A <see cref="Result{T}"/> containing the converted price.</returns>
    public static Result<decimal> ConvertPriceInput(string input)
    {
        const NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;

        if (!decimal.TryParse(input.Replace(',', '.'), style, CultureInfo.InvariantCulture, out var price))
            return Result<decimal>.Error("ERROR: Invalid price. Please enter a numeric value.");

        if (price < 0)
            return Result<decimal>.Error("ERROR: Price cannot be negative.");

        return Result<decimal>.Success(price);
    }

    /// <summary>
    /// Handles user input for a specific field, using a conversion function to validate and convert the input.
    /// </summary>
    /// <param name="message">The message to display to the user.</param>
    /// <param name="fieldName">The name of the field being input.</param>
    /// <param name="convertor">The function to convert and validate the input.</param>
    /// <param name="value">The output value if the input is valid.</param>
    /// <returns>True if the input was successfully handled and converted; otherwise, false.</returns>
    public static bool TryHandleInput<T>(
        string message, string fieldName,
        Func<string, Result<T>> convertor,
        [MaybeNullWhen(false)] out T value,
        string? defaultValue = null)
    {
        while (true)
        {
            Console.Write($"{message} (");

            if (defaultValue != null)
                Console.Write($"default: `{defaultValue}` | ");

            Console.Write($"`q` to exit): ");

            var input = Console.ReadLine()?.Trim();

            if (input is null || string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
            {
                value = default;
                return false;
            }

			if (input.Length is 0 && defaultValue is not null)
				input = defaultValue;

			var result = input.Length is 0
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
