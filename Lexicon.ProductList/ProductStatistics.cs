namespace Lexicon.ProductList;

/// <summary>
/// Holds summary figures calculated from a non-empty list of products.
/// </summary>
/// <param name="MostExpensive">The product with the highest price.</param>
/// <param name="Cheapest">The product with the lowest price.</param>
/// <param name="AveragePrice">The average price of all products.</param>
/// <param name="ProductsPerCategory">The number of products in each category.</param>
public sealed record ProductStatistics(
	Product MostExpensive,
	Product Cheapest,
	decimal AveragePrice,
	IReadOnlyList<(string Category, int Count)> ProductsPerCategory);
