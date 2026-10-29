namespace Lexicon.ProductList;

/// <summary>
/// Represents a product with an identifier, category, name, and price.
/// </summary>
public class Product(int id, string category, string name, decimal price)
{
    /// <summary>
    /// Gets the unique identifier of the product.
    /// </summary>
    public int Id { get; } = id;

    /// <summary>
    /// Gets the category of the product.
    /// </summary>
	public string Category { get; } = category;

    /// <summary>
    /// Gets the name of the product.
    /// </summary>
	public string Name { get; private set; } = name;

    /// <summary>
    /// Gets the price of the product.
    /// </summary>
	public decimal Price { get; private set; } = price;


    /// <summary>
    /// Updates the price of the product.
    /// </summary>
    /// <param name="newPrice">The new price to set for the product.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="newPrice"/> is less than zero.</exception>
	public void UpdatePrice(decimal newPrice)
	{
		if (newPrice < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(newPrice), "New price cannot be less than zero.");
		}

		Price = newPrice;
	}

    /// <summary>
    /// Updates the name of the product.
    /// </summary>
    /// <param name="newName">The new name to set for the product.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="newName"/> is null or empty.</exception>
	public void UpdateName(string newName)
	{
		if (string.IsNullOrWhiteSpace(newName))
		{
			throw new ArgumentException("New name cannot be null or empty.", nameof(newName));
		}

		Name = newName;
	}


	public override string? ToString()
	{
		const int maxWidth = 12;

		return $"{Id,maxWidth} | " +
		$"{Category?.PadRight(maxWidth)} | " +
		$"{Name?.PadRight(maxWidth)} | " +
		$"{Price,maxWidth}";
	}
}
