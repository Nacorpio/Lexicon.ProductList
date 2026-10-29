using System.Globalization;
using Lexicon.ProductList;

ProductManager productManager = new();
Result? status = null;

while (true)
{
	Console.Clear();

	if (status is {} result && !string.IsNullOrEmpty(result.Message))
	{
		PrintResult(result);
		Console.WriteLine();
	}

	status = null;

	DrawMenu();
	
	if (!ConsoleHelper.TryHandleInput("Enter your choice", "option", ConsoleHelper.ConvertMenuOptionInput, out var option))
		break;

	if (option == 4)
		break;

	Console.Clear();

	status = option switch
	{
		1 => HandleProductAdd(),
		2 => HandleViewProducts(),
		3 => HandleSearchProducts(),
		_ => Result.Error("ERROR: Invalid menu option.")
	};
}

static void PrintLine (string text, ConsoleColor color)
{
	Console.ForegroundColor = color;
	Console.WriteLine(text);
	Console.ResetColor();
}

static void PrintResult(Result result)
{
	if (result.IsSuccess)
	{
		PrintLine(result.Message, ConsoleColor.Green);
		return;
	}

	PrintLine(result.Message, ConsoleColor.Red);
}

Result HandleProductAdd()
{
	if (!ConsoleHelper.TryHandleInput("Enter the product category", "product category",
		ConsoleHelper.ConvertTextInput, out var category))
	{
		return Result.Success();
	}

	if (!ConsoleHelper.TryHandleInput<string>("Enter a name for the product", "product name",
		ConsoleHelper.ConvertTextInput, out var productName))
	{
		return Result.Success();
	}

	if (!ConsoleHelper.TryHandleInput("Enter the product price", "product price",
		ConsoleHelper.ConvertPriceInput, out var price))
	{
		return Result.Success();
	}

	var product = productManager.AddProduct(category, productName, price);

	return Result.Success($"Product `{product.Name}` added successfully.");
}

Result HandleViewProducts()
{
	var productEntries = productManager.Entries;

	if (productEntries.Count == 0)
		return Result.Error("No products available.");

	DrawProductTable("Product List", productEntries);
	return Result.Success();
}

Result HandleSearchProducts()
{
	if (!ConsoleHelper.TryHandleInput("Enter the search query", "product name",
		ConsoleHelper.ConvertTextInput, out var searchTerm))
	{
		return Result.Success();
	}

	var searchResults = productManager.FindProductsByQuery(searchTerm);

	if (searchResults.Count == 0)
		return Result.Error("No products found matching the search term.");

	DrawProductTable($"Search Results for '{searchTerm}'", searchResults);

	return Result.Success();
}

static void DrawMenu()
{
	Console.WriteLine("1. Add Product");
	Console.WriteLine("2. View Products");
	Console.WriteLine("3. Search Products");
	Console.WriteLine("4. Exit");

	Console.WriteLine();
}

static void DrawLine(int totalMinWidth)
{
	Console.WriteLine(new string('-', totalMinWidth));
}

static void DrawProductTable(string title, IReadOnlyList<Product> productEntries)
{
	var culture = CultureInfo.GetCultureInfo("sv-SE");
	
	var idMinWidth = Math.Max("Id".Length, productEntries.Max(p => p.Id.ToString().Length)) + 3;
	var nameMinWidth = Math.Max("Name".Length, productEntries.Max(p => p.Name.Length)) + 3;
	var categoryMinWidth = Math.Max("Category".Length, productEntries.Max(p => p.Category.Length)) + 3;
	var priceMinWidth = Math.Max("Price".Length, productEntries.Max(p => p.Price.ToString("C", culture).Length)) + 3;

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
		Console.Write($" {product.Id.ToString().PadRight(idMinWidth)} |");
		Console.Write($" {product.Name.PadRight(nameMinWidth)} |");
		Console.Write($" {product.Category.PadRight(categoryMinWidth)} |");
		Console.Write($" {product.Price.ToString("C", culture).PadRight(priceMinWidth)} |");

		Console.WriteLine();
	}

	DrawLine(totalMinWidth);

	Console.WriteLine();

	Console.WriteLine($"TOTAL PRICE: {productEntries.Sum(p => p.Price).ToString("C", culture)}");

	Console.WriteLine();
	Console.WriteLine("Press any key to go back");

	Console.ReadLine();
}
