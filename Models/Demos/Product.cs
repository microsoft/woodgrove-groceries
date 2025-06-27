public class Product
{


    public required string Name { get; set; }
    public decimal Price { get; set; }
    public required string ImageUrl { get; set; }
    public required string Category { get; set; }
    public string Discount { get; set; } = "-"; // Default value if no discount is applied
    public string[] AllergyInfo { get; set; } = Array.Empty<string>(); // Default value if the product does not contain eggs

    // A constructor that intitializes the Discount property
    public Product()
    {
        // Return a random discount between 5% and 20%
        // However, 25% of the items will not have a discount
        Random random = new Random();
        int discountChance = random.Next(1, 5); // 1 in 4
        if (discountChance != 1)
        {
            Discount = $"{random.Next(5, 20)}%";
        }

    }
}

public static class ProductData
{
    public static List<Product> GetSampleProducts()
    {
        return new List<Product>
        {
            new Product
            {
                Category = "Vegetables",
                Name = "Organic baby spinach leaves",
                Price = 3.5m,
                ImageUrl = "/images/products/spinach.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Pasta & Grains",
                Name = "Italian whole wheat spaghetti 500 g",
                Price = 3.0m,
                ImageUrl = "/images/products/pasta.png",
                AllergyInfo = new[] { "Gluten", "Wheat" }
            },
            new Product
            {
                Category = "Dairy",
                Name = "Aged sharp cheddar cheese 200 g",
                Price = 5.5m,
                ImageUrl = "/images/products/cheese.png",
                AllergyInfo = new[] { "Milk", "Dairy" }
            },
            new Product
            {
                Category = "Pantry",
                Name = "Greek Olive Oil",
                Price = 12.0m,
                ImageUrl = "/images/products/olive-oil.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Bakery",
                Name = "Freshly baked blueberry muffins",
                Price = 3.0m,
                ImageUrl = "/images/products/blueberry-muffins.png",
                AllergyInfo = new[] { "Gluten", "Wheat", "Eggs", "Milk", "Dairy" }
            },
            new Product
            {
                Category = "Pantry",
                Name = "Homemade mixed nut granola 500 g",
                Price = 4.0m,
                ImageUrl = "/images/products/granola.png",
                AllergyInfo = new[] { "Tree Nuts", "Almonds", "Pecans", "Walnuts" }
            },
            new Product
            {
                Category = "Beverages",
                Name = "Pure natural coconut water",
                Price = 2.8m,
                ImageUrl = "/images/products/coconut-water.png",
                AllergyInfo = new[] { "Tree Nuts", "Coconut" }
            },
            new Product
            {
                Category = "Frozen",
                Name = "Margherita wood-fired pizza",
                Price = 3.75m,
                ImageUrl = "/images/products/pizza.png",
                AllergyInfo = new[] { "Gluten", "Wheat", "Milk", "Dairy" }
            },
            new Product
            {
                Category = "Snacks",
                Name = "70% cocoa dark chocolate bar",
                Price = 4.5m,
                ImageUrl = "/images/products/chocolate.png",
                AllergyInfo = new[] { "Milk", "Dairy", "Soy" }
            },
            new Product
            {
                Category = "Fruits",
                Name = "Organic bananas 1 kg",
                Price = 3.6m,
                ImageUrl = "/images/products/bananas.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Pantry",
                Name = "Pure maple syrup",
                Price = 1.5m,
                ImageUrl = "/images/products/maple-syrup.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Frozen",
                Name = "Strawberry ice cream",
                Price = 2.0m,
                ImageUrl = "/images/products/ice-cream.png",
                AllergyInfo = new[] { "Milk", "Dairy", "Eggs" }
            },
            new Product
            {
                Category = "Beverages",
                Name = "Six Bottles Of Water pack",
                Price = 3.0m,
                ImageUrl = "/images/products/water.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Beverages",
                Name = "Fresh orange juice 1 L",
                Price = 4.0m,
                ImageUrl = "/images/products/orange-juice.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Frozen",
                Name = "Frozen green peas 500 g",
                Price = 2.5m,
                ImageUrl = "/images/products/green-peas.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Dairy",
                Name = "Greek yogurt 500 g",
                Price = 4.8m,
                ImageUrl = "/images/products/greek-yogurt.png",
                AllergyInfo = new[] { "Milk", "Dairy" }
            },
            new Product
            {
                Category = "Fruits",
                Name = "Red apples 1 kg",
                Price = 3.9m,
                ImageUrl = "/images/products/red-apples.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Vegetables",
                Name = "Carrots 1 kg",
                Price = 2.0m,
                ImageUrl = "/images/products/carrots.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Pantry",
                Name = "Canned chickpeas 400 g",
                Price = 1.2m,
                ImageUrl = "/images/products/chickpeas.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Cleaning",
                Name = "Laundry detergent 2L",
                Price = 12.99m,
                ImageUrl = "/images/products/laundry-detergent.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Cleaning",
                Name = "Hand soap 250ml",
                Price = 2.99m,
                ImageUrl = "/images/products/hand-soap.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Cleaning",
                Name = "Glass cleaner 500ml",
                Price = 3.79m,
                ImageUrl = "/images/products/glass-cleaner.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Cleaning",
                Name = "Liquid dish soap 500ml",
                Price = 3.99m,
                ImageUrl = "/images/products/dish-soap.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Snacks",
                Name = "Salted roasted almonds 200 g",
                Price = 5.0m,
                ImageUrl = "/images/products/almonds.png",
                AllergyInfo = new[] { "Tree Nuts", "Almonds" }
            },
            new Product
            {
                Category = "Snacks",
                Name = "Potato chips 150 g",
                Price = 2.3m,
                ImageUrl = "/images/products/potato-chips.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Bakery",
                Name = "Chocolate chip cookies",
                Price = 3.5m,
                ImageUrl = "/images/products/cookies.png",
                AllergyInfo = new[] { "Gluten", "Wheat", "Eggs", "Milk", "Dairy", "Soy" }
            },
            new Product
            {
                Category = "Dairy",
                Name = "Butter 250 g",
                Price = 3.6m,
                ImageUrl = "/images/products/butter.png",
                AllergyInfo = new[] { "Milk", "Dairy" }
            },
            new Product
            {
                Category = "Frozen",
                Name = "Frozen mango chunks 500 g",
                Price = 3.8m,
                ImageUrl = "/images/products/frozen-mango.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Beverages",
                Name = "Green tea 20 bags",
                Price = 3.0m,
                ImageUrl = "/images/products/green-tea.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Pantry",
                Name = "Raw unfiltered local honey 500 g",
                Price = 12.0m,
                ImageUrl = "/images/products/honey.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Dairy",
                Name = "Unsweetened vanilla almond milk 1 L",
                Price = 4.3m,
                ImageUrl = "/images/products/almond-milk.png",
                AllergyInfo = new[] { "Tree Nuts", "Almonds" }
            },
            new Product
            {
                Category = "Frozen",
                Name = "Frozen organic berry blend",
                Price = 5.5m,
                ImageUrl = "/images/products/berries.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Fruits",
                Name = "Dark purple grapes 1.5 kg",
                Price = 11.25m,
                ImageUrl = "/images/products/grapes.jpg",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Vegetables",
                Name = "Organic sweet tomato 1 kg",
                Price = 2.75m,
                ImageUrl = "/images/products/tomatoes.jpg",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Bakery",
                Name = "French bread 400 g",
                Price = 13.0m,
                ImageUrl = "/images/products/bread.jpg",
                AllergyInfo = new[] { "Gluten", "Wheat" }
            },
            new Product
            {
                Category = "Dairy",
                Name = "Organic eggs 12 count",
                Price = 12.99m,
                ImageUrl = "/images/products/eggs.jpg",
                AllergyInfo = new[] { "Eggs" }
            },
            new Product
            {
                Category = "Fruits",
                Name = "Sweet corn 1 unit",
                Price = 5.25m,
                ImageUrl = "/images/products/corn.jpg",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Fruits",
                Name = "Watermelon 1 unit",
                Price = 12.5m,
                ImageUrl = "/images/products/watermelon.jpg",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Dairy",
                Name = "Organic sugar 2 packages",
                Price = 7.0m,
                ImageUrl = "/images/products/sugar.png",
                AllergyInfo = Array.Empty<string>()
            },
            new Product
            {
                Category = "Fruits",
                Name = "Oranges 1 kg",
                Price = 4.0m,
                ImageUrl = "/images/products/oranges.png",
                AllergyInfo = Array.Empty<string>()
            }
        };
    }
}