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

	if (option == MenuOption.Exit)
		break;

	Console.Clear();

	status = option switch
	{
		MenuOption.Add => MenuHandlers.Add(productManager),
		MenuOption.View => MenuHandlers.View(productManager),
		MenuOption.Search => MenuHandlers.Search(productManager),
		MenuOption.Edit => MenuHandlers.Edit(productManager),
		MenuOption.Delete => MenuHandlers.Delete(productManager),
		MenuOption.Statistics => MenuHandlers.Statistics(productManager),
		MenuOption.Save => MenuHandlers.Save(productManager),
		MenuOption.Load => MenuHandlers.Load(productManager),

		_ => Result.Error("ERROR: Invalid menu option.")
	};
}

static void DrawMenu()
{
	Console.WriteLine("1. Add Product");
	Console.WriteLine("2. View Products");
	Console.WriteLine("3. Search Products");
	Console.WriteLine("4. Edit Product");
	Console.WriteLine("5. Delete Product");
	Console.WriteLine("6. Statistics");
	Console.WriteLine("7. Save Products");
	Console.WriteLine("8. Load Products");
	Console.WriteLine("9. Exit");

	Console.WriteLine();
}
