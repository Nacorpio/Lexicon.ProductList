using System.Collections;
using System;

namespace Lexicon.ProductList;

/// <summary>
/// Provides functionality to manage a collection of products, including adding, removing, and querying products by various criteria.
/// </summary>
public sealed class ProductManager
{
	/// <summary>
	/// Gets the initial identifier value for products.
	/// </summary>
	public const int InitialId = 1001;

	private readonly List<Product> _products = [];

	/// <summary>
	/// Gets the total number of products currently contained in the <see cref="ProductManager"/>.
	/// </summary>
	public int Count => _products.Count;

	/// <summary>
	/// Gets the identifier that will be assigned to the next added product.
	/// </summary>
	private int NextId => InitialId + Count;

	/// <summary>
	/// Gets a collection of the product entries in the <see cref="ProductManager"/>.
	/// </summary>
	public IReadOnlyList<Product> Entries => [.. _products];

	public Product AddProduct(string category, string name, decimal price)
	{
		var product = new Product(NextId, category, name, price);

		_products.Add(product);

		return product;
	}

	/// <summary>
	/// Calculates the total value of all products currently contained in the <see cref="ProductManager"/>.
	/// </summary>
	public decimal CalculateTotal() => _products.Sum(p => p.Price);

    /// <summary>
    /// Determines whether the specified product exists in the <see cref="ProductManager"/>.	
    /// </summary>
    /// <param name="product">The product to check for existence.</param>
    /// <returns><c>true</c> if the product exists; otherwise, <c>false</c>.</returns>
    public bool Contains(Product product)
	{
		return _products.Any(p => p.Id == product.Id);
	}

	/// <summary>
	/// Removes the specified product from the <see cref="ProductManager"/>.
	/// </summary>
	/// <param name="product">The product to remove.</param>
	/// <returns><c>true</c> if the product was successfully removed; otherwise, <c>false</c>.</returns>
	public bool Remove (Product product)
	{
		return _products.RemoveAll(p => p.Id == product.Id) > 0;
	}

	/// <summary>
	/// Returns a product with the specified unique identifier.
	/// </summary>
	/// <param name="id">The identifier of the product.</param>
	/// <returns>The product with the given identifier, if found; otherwise, <c>null</c>.</returns>
	public Product? GetProductById(int id)
	{
		return _products.FirstOrDefault(p => p.Id == id);
	}

    /// <summary>
    /// Finds all products that match the specified query in either their name or category.
    /// </summary>
    /// <param name="query">The search query to match against product names and categories.</param>
    /// <returns>A read-only list of products that match the specified query.</returns>
    public IReadOnlyList<Product> FindProductsByQuery (string query)
	{
		return [.. _products
			.Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
				|| p.Category.Contains(query, StringComparison.OrdinalIgnoreCase))];
	}

    /// <summary>
    /// Gets all products that belong to the specified category.
    /// </summary>
    /// <param name="category">The category to filter products by.</param>
    /// <returns>A read-only list of products in the specified category.</returns>
    public IReadOnlyList<Product> GetProductsByCategory(string category)
	{
		return [.. _products
			.Where(p => string
			.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase))];
	}

	/// <summary>
	/// Gets all products that have the specified name.
	/// </summary>
	/// <param name="name">The name to filter products by.</param>
	/// <returns>A read-only list of products with the specified name.</returns>
	public IReadOnlyList<Product> GetProductsByName(string name)
	{
		return [.. _products
			.Where(p => string
			.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))];
	}
}