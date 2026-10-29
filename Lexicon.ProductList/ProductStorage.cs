using System.Text.Json;

namespace Lexicon.ProductList;

/// <summary>
/// Saves products to, and loads products from, a JSON file.
/// </summary>
public static class ProductStorage
{
	/// <summary>
	/// Gets the name of the file used when no other path is given. It is relative to the working directory.
	/// </summary>
	public const string DefaultFileName = "products.json";

	private static readonly JsonSerializerOptions serializerOptions = new() { WriteIndented = true };

	/// <summary>
	/// Writes the products to a JSON file, replacing the file if it already exists.
	/// </summary>
	/// <param name="products">The products to save.</param>
	/// <param name="path">The path of the file to write.</param>
	public static Result Save(IReadOnlyList<Product> products, string path = DefaultFileName)
	{
		try
		{
			File.WriteAllText(path, JsonSerializer.Serialize(products, serializerOptions));

			return Result.Success($"Saved {products.Count} product(s) to `{Path.GetFullPath(path)}`.");
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			return Result.Error($"ERROR: Could not write the file. {ex.Message}");
		}
	}

	/// <summary>
	/// Reads products from a JSON file and checks that they are valid.
	/// </summary>
	/// <param name="path">The path of the file to read.</param>
	public static Result<IReadOnlyList<Product>> Load(string path = DefaultFileName)
	{
		if (!File.Exists(path))
			return Result<IReadOnlyList<Product>>.Error($"ERROR: No saved file found at `{Path.GetFullPath(path)}`.");

		List<Product>? products;

		try
		{
			products = JsonSerializer.Deserialize<List<Product>>(File.ReadAllText(path));
		}
		catch (JsonException)
		{
			return Result<IReadOnlyList<Product>>.Error("ERROR: The file is not valid product data.");
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			return Result<IReadOnlyList<Product>>.Error($"ERROR: Could not read the file. {ex.Message}");
		}

		if (products is null)
			return Result<IReadOnlyList<Product>>.Error("ERROR: The file is empty.");

		var validation = Validate(products);

		if (!validation.IsSuccess)
			return Result<IReadOnlyList<Product>>.Error(validation.Message);

		return Result<IReadOnlyList<Product>>.Success(products);
	}

	/// <summary>
	/// Checks that loaded products follow the same rules as products added through the menu.
	/// </summary>
	/// <param name="products">The products read from the file.</param>
	private static Result Validate(IReadOnlyList<Product> products)
	{
		if (products.Select(p => p.Id).Distinct().Count() != products.Count)
			return Result.Error("ERROR: Found duplicate product identifiers.");

		foreach (var product in products)
		{
			if (product.Id < ProductManager.InitialId)
				return Result.Error("ERROR: Invalid identifier for product.");

			if (string.IsNullOrWhiteSpace(product.Category))
				return Result.Error($"ERROR: Category for product `{product.Id}` is empty.");

			if (string.IsNullOrWhiteSpace(product.Name))
				return Result.Error($"ERROR: Name for product `{product.Id}` in category `{product.Category}` is empty.");

			if (product.Price < 0)
				return Result.Error($"ERROR: Price for product {product.Id} ({product.Name}, category {product.Category}) is negative.");
		}

		return Result.Success();
	}
}
