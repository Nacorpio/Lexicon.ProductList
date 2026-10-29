using System.Globalization;

namespace Lexicon.ProductList;

/// <summary>
/// Contains one handler per menu option. Each handler runs the interaction for its option and returns a <see cref="Result"/> to show above the menu.
/// </summary>
public static class MenuHandlers
{
    /// <summary>
    /// Handles adding a new product. Prompts the user for category, name, and price, and adds the product to the manager.
    /// </summary>
    /// <param name="productManager">The product manager to add the product to.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
    public static Result Add(ProductManager productManager)
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

    /// <summary>
    /// Handles viewing all products. Displays the product list in a table format.
    /// </summary>
    /// <param name="productManager">The product manager containing the products to view.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
	public static Result View(ProductManager productManager)
	{
		var productEntries = productManager.Entries;

		if (productEntries.Count == 0)
			return Result.Error("No products available.");

		ConsoleHelper.DrawProductTable("Product List", productEntries);
		ConsoleHelper.PauseBeforeReturn();

		return Result.Success();
	}

    /// <summary>
    /// Handles searching for products by a query string. Displays the search results in a table format.
    /// </summary>
    /// <param name="productManager">The product manager containing the products to search.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
	public static Result Search(ProductManager productManager)
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
		ConsoleHelper.PauseBeforeReturn();

		return Result.Success();
	}

    /// <summary>
    /// Handles deleting a product by its ID. Prompts the user for confirmation before deletion.
    /// </summary>
    /// <param name="productManager">The product manager containing the products to delete.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
	public static Result Delete(ProductManager productManager)
	{
		var productEntries = productManager.Entries;

		if (productEntries.Count == 0)
			return Result.Error("No products available.");

		ConsoleHelper.DrawProductTable("Delete Product", productEntries);
		Console.WriteLine();

		if (!ConsoleHelper.TryHandleInput("Enter the ID of the product to delete", "product ID",
			ConsoleHelper.ConvertIdInput, out var id))
		{
			return Result.Success();
		}

		var product = productManager.GetProductById(id);

		if (product is null)
			return Result.Error($"No product with ID {id} was found.");

		if (!ConsoleHelper.Confirm($"Delete `{product.Name}` (ID {product.Id})?"))
			return Result.Success("Deletion cancelled.");

		productManager.Remove(product);

		return Result.Success($"Product `{product.Name}` deleted successfully.");
	}

	/// <summary>
	/// Handles editing a product's category, name and price. Pressing Enter at a prompt keeps the current value.
	/// </summary>
	/// <param name="productManager">The product manager containing the products to edit.</param>
	/// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
	public static Result Edit(ProductManager productManager)
	{
		var productEntries = productManager.Entries;

		if (productEntries.Count == 0)
			return Result.Error("No products available.");

		ConsoleHelper.DrawProductTable("Edit Product", productEntries);
		Console.WriteLine();

		if (!ConsoleHelper.TryHandleInput("Enter the ID of the product to edit", "product ID",
			ConsoleHelper.ConvertIdInput, out var id))
		{
			return Result.Success();
		}

		var product = productManager.GetProductById(id);

		if (product is null)
			return Result.Error($"No product with ID {id} was found.");

		Console.WriteLine();
		Console.WriteLine("Press Enter at a prompt to keep the current value.");

		if (!ConsoleHelper.TryHandleInput("Enter the new category", "product category",
			ConsoleHelper.ConvertTextInput, out var category, product.Category))
		{
			return Result.Success();
		}

		if (!ConsoleHelper.TryHandleInput("Enter the new name", "product name",
			ConsoleHelper.ConvertTextInput, out var name, product.Name))
		{
			return Result.Success();
		}

		if (!ConsoleHelper.TryHandleInput("Enter the new price", "product price",
			ConsoleHelper.ConvertPriceInput, out var price, product.Price.ToString(CultureInfo.InvariantCulture)))
		{
			return Result.Success();
		}

		if (category == product.Category && name == product.Name && price == product.Price)
			return Result.Success("No changes made.");

		if (!ConsoleHelper.Confirm($"Save changes to product {product.Id}?"))
			return Result.Success("Edit cancelled.");

		product.UpdateCategory(category);
		product.UpdateName(name);
		product.UpdatePrice(price);

		return Result.Success($"Product `{product.Name}` updated successfully.");
	}

	/// <summary>
	/// Handles showing statistics: the most expensive and cheapest product, the average price and the number of products per category.
	/// </summary>
	/// <param name="productManager">The product manager containing the products to summarise.</param>
	/// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
	public static Result Statistics(ProductManager productManager)
	{
		var statistics = productManager.GetStatistics();

		if (statistics is null)
			return Result.Error("No products available.");

		ConsoleHelper.DrawStatistics(statistics);
		ConsoleHelper.PauseBeforeReturn();

		return Result.Success();
	}

    /// <summary>
    /// Handles saving the current list of products to storage.
    /// </summary>
    /// <param name="productManager">The product manager containing the products to save.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
	public static Result Save(ProductManager productManager)
	{
		var productEntries = productManager.Entries;

		if (productEntries.Count == 0)
			return Result.Error("No products to save.");

		return ProductStorage.Save(productEntries);
	}

    /// <summary>
    /// Handles loading products from storage. Replaces the current list of products if confirmed by the user.
    /// </summary>
    /// <param name="productManager">The product manager to load the products into.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
	public static Result Load(ProductManager productManager)
	{
		if (productManager.Count > 0
			&& !ConsoleHelper.Confirm($"Loading replaces the {productManager.Count} current product(s). Continue?"))
		{
			return Result.Success("Load cancelled.");
		}

		var loaded = ProductStorage.Load();

		if (!loaded.IsSuccess)
			return Result.Error(loaded.Text);

		productManager.ReplaceAll(loaded.Value);

		return Result.Success($"Loaded {loaded.Value.Count} product(s).");
	}
}
