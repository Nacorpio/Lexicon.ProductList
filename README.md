# Product List Management System

Menu-based console application in which a collection of products can be managed and exported to JSON format. The program displays menus and information in a clean, user-friendly format.

## Features

- Add a product (category, name, price)
- View all products in a table sorted by price
- Search products by name or category

## Getting started

Requirements: .NET 10 SDK.

```bash
dotnet run --project Lexicon.ProductList
```

## Usage

The main menu has four options. Enter the number of the option and press Enter.

1. **Add Product:** asks for a category, a name and a price, in that order. The product gets an ID automatically.
2. **View Products:** shows all products in a table, sorted by price (lowest first) and then by name. Below the table are the total and the average price.
3. **Search Products:** asks for a search term and shows the matching products in green, with the total and average price for the matches only.
4. **Exit:** closes the program.

Type `q` at any prompt to cancel and go back to the menu. After a table is shown, press Enter to return to the menu.

## Design decisions

- **Price accepts both `.` and `,`.** Swedish users type `12,50` and others type `12.50`. Both are read the same way, and prices are shown with two decimals.
- **A price of 0 is allowed, a negative price is not.** Free items are valid products, but a negative price has no meaning here.
- **IDs start at 1001 and are assigned automatically.** The user never types an ID, so IDs cannot be duplicated by mistake.
- **`q` cancels input, so a product cannot be named `q`.** One cancel key works at every prompt, which keeps the interface simple.
- **Search is case-insensitive and matches part of the name or the category.** Searching `lap` finds `Laptop`, and searching `phone` finds everything in the category `Phones`.
- **Handlers return a `Result`.** The menu loop prints the message once above the menu, so success and error messages are shown the same way everywhere.

## Project structure

| File | Responsibility |
| --- | --- |
| `Program.cs` | Menu loop and menu handlers |
| `ConsoleHelper.cs` | Input prompts, parsing and table drawing |
| `ProductManager.cs` | Stores products, assigns IDs, searches |
| `Product.cs` | Product data |
| `Result.cs` | Success/error result used by handlers and parsers |
