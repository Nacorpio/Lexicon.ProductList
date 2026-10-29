# Product List Management System

Menu-based console application in which a collection of products can be managed and exported to JSON format. The program displays menus and information in a clean, user-friendly format.

## Features

- Add a product (category, name, price)
- View all products in a table sorted by price
- Search products by name or category
- Edit a product's category, name or price by ID, with confirmation
- Delete a product by ID, with confirmation
- Show statistics: most expensive and cheapest product, average price and products per category
- Save the products to a JSON file and load them again

## Getting started

Requirements: .NET 10 SDK.

```bash
dotnet run --project Lexicon.ProductList
```

## Usage

The main menu has nine options. Enter the number of the option and press Enter.

1. **Add Product:** asks for a category, a name and a price, in that order. The product gets an ID automatically.
2. **View Products:** shows all products in a table, sorted by price (lowest first) and then by name. Below the table are the total and the average price.
3. **Search Products:** asks for a search term and shows the matching products in green, with the total and average price for the matches only.
4. **Edit Product:** shows all products and asks for the ID of the product to edit. It then asks for a new category, name and price. The current value is shown at each prompt, and pressing Enter keeps it. Before anything is changed you are asked to confirm, so cancelling halfway leaves the product as it was.
5. **Delete Product:** shows all products, asks for the ID of the product to delete and asks you to confirm with `y` or `yes`. Any other answer cancels the deletion.
6. **Statistics:** shows the most expensive product, the cheapest product, the average price and the number of products in each category.
7. **Save Products:** writes all products to `products.json`. Saving an empty list is refused, so an existing file is not wiped by accident.
8. **Load Products:** reads the products from `products.json` and replaces the current list. If there are products in the list, you are asked to confirm first.
9. **Exit:** closes the program.

Type `q` at any prompt to cancel and go back to the menu. After a table is shown, press Enter to return to the menu.

`products.json` is created in the folder the program is started from, and the full path is shown after saving. The file is listed in `.gitignore`, so saved data is not committed.

## Design decisions

- **Price accepts both `.` and `,`.** Swedish users type `12,50` and others type `12.50`. Both are read the same way, and prices are shown with two decimals.
- **A price of 0 is allowed, a negative price is not.** Free items are valid products, but a negative price has no meaning here.
- **IDs start at 1001 and are assigned automatically.** The next ID is the highest existing ID plus one, so IDs stay unique after deletions. If the newest product is deleted, its ID is used again.
- **`q` cancels input, so a product cannot be named `q`.** One cancel key works at every prompt, which keeps the interface simple.
- **Search is case-insensitive and matches part of the name or the category.** Searching `lap` finds `Laptop`, and searching `phone` finds everything in the category `Phones`.
- **Editing keeps the current value by default.** Pressing Enter at an edit prompt sends the old value through the same checks as typed input, so the rules for price and text are the same as when adding. Nothing is changed until all three answers are given and confirmed. In the price prompt the current price is shown with a decimal point (for example `299.5`), although the table uses the Swedish format.
- **Statistics treat categories case-insensitively.** `Food` and `food` are counted as one category, the same way search and category lookups work. If several products share the highest or lowest price, the one that comes first alphabetically is shown.
- **Loaded files are validated.** A file can be edited by hand, so loading rejects duplicate IDs, empty names or categories and negative prices, with a message that names the product. A failed load leaves the current list untouched.
- **Handlers return a `Result`.** The menu loop prints the message once above the menu, so success and error messages are shown the same way everywhere.

## Project structure

| File | Responsibility |
| --- | --- |
| `Program.cs` | Menu loop and menu drawing |
| `MenuOption.cs` | The menu options and the number the user types for each |
| `MenuHandlers.cs` | One handler per menu option |
| `ConsoleHelper.cs` | Input prompts, parsing, confirmation, table and statistics drawing |
| `ProductManager.cs` | Stores products, assigns IDs, searches, calculates statistics |
| `ProductStatistics.cs` | The figures shown on the statistics screen |
| `ProductStorage.cs` | Saves and loads products as JSON and validates loaded data |
| `Product.cs` | Product data |
| `Result.cs` | Success/error result used by handlers and parsers |
