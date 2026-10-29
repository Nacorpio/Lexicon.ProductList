using System.Globalization;
using System.Text.RegularExpressions;
using Lexicon.ProductList;

ProductManager productManager = new();
Result? status = null;

while (true)
{
	Console.Clear();

	if (status is {} result && !string.IsNullOrEmpty(result.Message))
	{
		ConsoleHelper.PrintResult(result);
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
		1 => HandleProductAdd(productManager),
		2 => HandleViewProducts(productManager),
		3 => HandleSearchProducts(productManager),

		_ => Result.Error("ERROR: Invalid menu option.")
	};
}

static Result HandleProductAdd(ProductManager productManager)
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

static Result HandleViewProducts(ProductManager productManager)
{
	var productEntries = productManager.Entries;

	if (productEntries.Count == 0)
		return Result.Error("No products available.");

	ConsoleHelper.DrawProductTable("Product List", productEntries);

	return Result.Success();
}

static Result HandleSearchProducts(ProductManager productManager)
{
	if (!ConsoleHelper.TryHandleInput("Enter the search query", "product name",
		ConsoleHelper.ConvertTextInput, out var searchTerm))
	{
		return Result.Success();
	}

	var searchResults = productManager.FindProductsByQuery(searchTerm);

	if (searchResults.Count == 0)
		return Result.Error("No products found matching the search term.");

	ConsoleHelper.DrawProductTable($"Search Results for '{searchTerm}'", searchResults, ConsoleColor.Green);

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
