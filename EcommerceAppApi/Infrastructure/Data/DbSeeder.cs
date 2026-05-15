using EcommerceAppApi.Domain.Entities;
using EcommerceAppApi.Domain.Enums;

namespace EcommerceAppApi.Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Categories.Any()) return;

        var categories = new List<Category>
        {
            new() { Name = "Electronics", Slug = "electronics", Description = "Electronic devices and accessories", DisplayOrder = 1 },
            new() { Name = "Clothing", Slug = "clothing", Description = "Fashion and apparel", DisplayOrder = 2 },
            new() { Name = "Books", Slug = "books", Description = "Books and literature", DisplayOrder = 3 },
            new() { Name = "Home & Living", Slug = "home-living", Description = "Home decor and living essentials", DisplayOrder = 4 },
            new() { Name = "Sports", Slug = "sports", Description = "Sports equipment and gear", DisplayOrder = 5 },
        };
        context.Categories.AddRange(categories);

        var brands = new List<Brand>
        {
            new() { Name = "NovaTech", Slug = "novatech", Description = "Leading technology brand", LogoUrl = "NT", Website = "https://novatech.example.com" },
            new() { Name = "SoundWave", Slug = "soundwave", Description = "Premium audio equipment", LogoUrl = "SW", Website = "https://soundwave.example.com" },
            new() { Name = "StyleCraft", Slug = "stylecraft", Description = "Contemporary fashion brand", LogoUrl = "SC", Website = "https://stylecraft.example.com" },
            new() { Name = "PageTurner", Slug = "pageturner", Description = "Publishing house", LogoUrl = "PT", Website = "https://pageturner.example.com" },
            new() { Name = "CozyHome", Slug = "cozyhome", Description = "Home and living essentials", LogoUrl = "CH", Website = "https://cozyhome.example.com" },
            new() { Name = "FitGear", Slug = "fitgear", Description = "Sports and fitness equipment", LogoUrl = "FG", Website = "https://fitgear.example.com" },
        };
        context.Brands.AddRange(brands);

        context.SaveChanges();

        var products = new List<Product>
        {
            new() { Name = "NovaSound Pro Wireless Headphones", Slug = "novasound-pro-wireless-headphones", Description = "Premium wireless headphones with active noise cancelation. Experience studio-quality sound with cutting-edge noise cancelation technology.", Price = 249.99m, CompareAtPrice = 349.99m, StockQuantity = 48, SalesCount = 520, CategoryId = categories[0].Id, BrandId = brands[0].Id, MainImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400&h=400&fit=crop", Sku = "NT-HP-001", IsActive = true, IsFeatured = true, AverageRating = 4.8, TotalReviews = 3, CreatedAt = new DateTime(2026, 4, 2), SpecsJson = "{\"Battery\":\"42 hours\",\"Connectivity\":\"Bluetooth 5.3\",\"Warranty\":\"2 years\",\"Weight\":\"250g\"}", FeaturesJson = "[\"ANC\",\"Fast charge\",\"Multipoint pairing\",\"Foldable case\"]" },
            new() { Name = "SmartView 4K Ultra HD Monitor", Slug = "smartview-4k-ultra-hd-monitor", Description = "27-inch 4K UHD monitor with HDR10+ support for stunning visuals. Perfect for creators and professionals.", Price = 449.99m, CompareAtPrice = 599.99m, StockQuantity = 30, SalesCount = 310, CategoryId = categories[0].Id, BrandId = brands[0].Id, MainImageUrl = "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=400&h=400&fit=crop", Sku = "NT-MN-001", IsActive = true, IsFeatured = true, AverageRating = 4.7, TotalReviews = 3, CreatedAt = new DateTime(2026, 3, 28) },
            new() { Name = "Quantum Mechanical Keyboard", Slug = "quantum-mechanical-keyboard", Description = "RGB mechanical keyboard with hot-swappable switches. Customize every key to your liking.", Price = 159.99m, CompareAtPrice = 199.99m, StockQuantity = 75, SalesCount = 210, CategoryId = categories[0].Id, BrandId = brands[0].Id, MainImageUrl = "https://images.unsplash.com/photo-1618384887929-16ec33fab9ef?w=400&h=400&fit=crop", Sku = "NT-KB-001", IsActive = true, IsFeatured = false, AverageRating = 4.3, TotalReviews = 3, CreatedAt = new DateTime(2026, 4, 10) },
            new() { Name = "BassBlast Portable Speaker", Slug = "bassblast-portable-speaker", Description = "Waterproof portable speaker with 360-degree sound. Take the party anywhere you go.", Price = 79.99m, CompareAtPrice = 99.99m, StockQuantity = 120, SalesCount = 420, CategoryId = categories[0].Id, BrandId = brands[1].Id, MainImageUrl = "https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?w=400&h=400&fit=crop", Sku = "SW-SP-001", IsActive = true, IsFeatured = true, AverageRating = 4.6, TotalReviews = 3, CreatedAt = new DateTime(2026, 4, 5) },
            new() { Name = "Urban Denim Jacket", Slug = "urban-denim-jacket", Description = "Classic denim jacket with a modern fit. A wardrobe essential that never goes out of style.", Price = 89.99m, CompareAtPrice = 129.99m, StockQuantity = 60, SalesCount = 650, CategoryId = categories[1].Id, BrandId = brands[2].Id, MainImageUrl = "https://images.unsplash.com/photo-1576995853123-5a10305d93c0?w=400&h=400&fit=crop", Sku = "SC-DJ-001", IsActive = true, IsFeatured = true, AverageRating = 4.4, TotalReviews = 3, CreatedAt = new DateTime(2026, 4, 1) },
            new() { Name = "Merino Wool Sweater", Slug = "merino-wool-sweater", Description = "Luxuriously soft merino wool sweater for ultimate comfort. Perfect for any occasion.", Price = 119.99m, CompareAtPrice = 159.99m, StockQuantity = 45, SalesCount = 190, CategoryId = categories[1].Id, BrandId = brands[2].Id, MainImageUrl = "https://images.unsplash.com/photo-1620799140408-edc6dcb6d633?w=400&h=400&fit=crop", Sku = "SC-SW-001", IsActive = true, IsFeatured = false, AverageRating = 4.2, TotalReviews = 3, CreatedAt = new DateTime(2026, 3, 25) },
            new() { Name = "The Art of Clean Code", Slug = "the-art-of-clean-code", Description = "Master the principles of writing clean, maintainable code. A must-read for every developer.", Price = 34.99m, CompareAtPrice = 44.99m, StockQuantity = 200, SalesCount = 255, CategoryId = categories[2].Id, BrandId = brands[3].Id, MainImageUrl = "https://images.unsplash.com/photo-1532012197267-da84d127e765?w=400&h=400&fit=crop", Sku = "PT-BK-001", IsActive = true, IsFeatured = true, AverageRating = 4.8, TotalReviews = 3, CreatedAt = new DateTime(2026, 3, 15) },
            new() { Name = "Scented Soy Candle Collection", Slug = "scented-soy-candle-collection", Description = "Hand-poured soy candles with natural essential oils. Set of 3 premium scents.", Price = 44.99m, CompareAtPrice = 59.99m, StockQuantity = 90, SalesCount = 175, CategoryId = categories[3].Id, BrandId = brands[4].Id, MainImageUrl = "https://images.unsplash.com/photo-1603006905003-be475563bc59?w=400&h=400&fit=crop", Sku = "CH-CD-001", IsActive = true, IsFeatured = true, AverageRating = 4.5, TotalReviews = 3, CreatedAt = new DateTime(2026, 4, 8) },
            new() { Name = "Bamboo Kitchen Organizer Set", Slug = "bamboo-kitchen-organizer-set", Description = "Eco-friendly bamboo kitchen organizers. Sustainable storage solutions for your kitchen.", Price = 39.99m, CompareAtPrice = 54.99m, StockQuantity = 65, SalesCount = 295, CategoryId = categories[3].Id, BrandId = brands[4].Id, MainImageUrl = "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=400&h=400&fit=crop", Sku = "CH-KT-001", IsActive = true, IsFeatured = false, AverageRating = 4.1, TotalReviews = 3, CreatedAt = new DateTime(2026, 4, 12) },
            new() { Name = "Premium Yoga Mat", Slug = "premium-yoga-mat", Description = "Extra-thick non-slip yoga mat for ultimate comfort. Perfect for yoga, pilates, and stretching.", Price = 69.99m, CompareAtPrice = 89.99m, StockQuantity = 55, SalesCount = 405, CategoryId = categories[4].Id, BrandId = brands[5].Id, MainImageUrl = "https://images.unsplash.com/photo-1601925260368-ae2f83cf8b7f?w=400&h=400&fit=crop", Sku = "FG-YM-001", IsActive = true, IsFeatured = true, AverageRating = 4.6, TotalReviews = 3, CreatedAt = new DateTime(2026, 4, 3) },
            new() { Name = "Adjustable Dumbbell Set", Slug = "adjustable-dumbbell-set", Description = "Space-saving adjustable dumbbells from 5-52.5 lbs. Replace 15 sets of dumbbells with one.", Price = 349.99m, CompareAtPrice = 449.99m, StockQuantity = 20, SalesCount = 330, CategoryId = categories[4].Id, BrandId = brands[5].Id, MainImageUrl = "https://images.unsplash.com/photo-1638536532686-d610adfc8e5c?w=400&h=400&fit=crop", Sku = "FG-DB-001", IsActive = true, IsFeatured = false, AverageRating = 4.8, TotalReviews = 3, CreatedAt = new DateTime(2026, 4, 6) },
            new() { Name = "Wireless Charging Pad", Slug = "wireless-charging-pad", Description = "Fast wireless charger compatible with all Qi devices. Sleek and minimalist design.", Price = 29.99m, CompareAtPrice = 39.99m, StockQuantity = 150, SalesCount = 160, CategoryId = categories[0].Id, BrandId = brands[0].Id, MainImageUrl = "https://images.unsplash.com/photo-1583863788434-e58a36330cf0?w=400&h=400&fit=crop", Sku = "NT-WC-001", IsActive = true, IsFeatured = false, AverageRating = 4.0, TotalReviews = 3, CreatedAt = new DateTime(2026, 4, 14) },
            new() { Name = "Slim Fit Chinos", Slug = "slim-fit-chinos", Description = "Stretch comfort slim fit chinos for a sharp casual look. Available in multiple colors.", Price = 59.99m, StockQuantity = 80, SalesCount = 135, CategoryId = categories[1].Id, BrandId = brands[2].Id, MainImageUrl = "https://images.unsplash.com/photo-1473966968604-f19f4b563cfd?w=400&h=400&fit=crop", Sku = "SC-CH-001", IsActive = true, IsFeatured = false, AverageRating = 4.3, TotalReviews = 3, CreatedAt = new DateTime(2026, 4, 9) },
            new() { Name = "Data Structures & Algorithms", Slug = "data-structures-algorithms", Description = "Comprehensive guide to mastering data structures and algorithms. With practical examples.", Price = 44.99m, StockQuantity = 180, SalesCount = 345, CategoryId = categories[2].Id, BrandId = brands[3].Id, MainImageUrl = "https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?w=400&h=400&fit=crop", Sku = "PT-BK-002", IsActive = true, IsFeatured = false, AverageRating = 4.7, TotalReviews = 3, CreatedAt = new DateTime(2026, 3, 20) },
            new() { Name = "Microfiber Cleaning Cloth Set", Slug = "microfiber-cleaning-cloth-set", Description = "Premium microfiber cleaning cloths. Lint-free and scratch-free cleaning for all surfaces.", Price = 14.99m, StockQuantity = 300, SalesCount = 880, CategoryId = categories[3].Id, BrandId = brands[4].Id, MainImageUrl = "https://images.unsplash.com/photo-1585421514284-efb74c2b69ba?w=400&h=400&fit=crop", Sku = "CH-MC-001", IsActive = true, IsFeatured = false, AverageRating = 4.2, TotalReviews = 3, CreatedAt = new DateTime(2026, 4, 11) },
            new() { Name = "Resistance Bands Set", Slug = "resistance-bands-set", Description = "Set of 5 resistance bands with different strengths. Perfect for home workouts.", Price = 24.99m, CompareAtPrice = 34.99m, StockQuantity = 100, SalesCount = 415, CategoryId = categories[4].Id, BrandId = brands[5].Id, MainImageUrl = "https://images.unsplash.com/photo-1598289431512-b97b0917affc?w=400&h=400&fit=crop", Sku = "FG-RB-001", IsActive = true, IsFeatured = false, AverageRating = 4.4, TotalReviews = 3, CreatedAt = new DateTime(2026, 4, 7) },
        };
        context.Products.AddRange(products);
        context.SaveChanges();

        var adminUser = new User
        {
            Name = "Admin",
            Email = "admin@shop.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            PhoneNumber = "+8801712345678",
            Role = Role.Admin,
            CreatedAt = new DateTime(2026, 1, 1)
        };
        context.Users.Add(adminUser);

        var users = new List<User>
        {
            new() { Name = "Ayesha Rahman", Email = "ayesha@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"), PhoneNumber = "+8801712345679", Role = Role.User, CreatedAt = new DateTime(2026, 2, 1) },
            new() { Name = "Tanvir Hasan", Email = "tanvir@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"), PhoneNumber = "+8801712345680", Role = Role.User, CreatedAt = new DateTime(2026, 2, 15) },
            new() { Name = "Nusrat Jahan", Email = "nusrat@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"), PhoneNumber = "+8801712345681", Role = Role.User, CreatedAt = new DateTime(2026, 3, 1) },
            new() { Name = "Rafi Ahmed", Email = "rafi@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"), PhoneNumber = "+8801712345682", Role = Role.User, CreatedAt = new DateTime(2026, 3, 10) },
        };
        context.Users.AddRange(users);
        context.SaveChanges();

        foreach (var user in users)
        {
            context.Carts.Add(new Cart { UserId = user.Id, CreatedAt = DateTime.UtcNow });
            context.Wishlists.Add(new Wishlist { UserId = user.Id, CreatedAt = DateTime.UtcNow });
        }

        context.Addresses.AddRange(
            new Address { UserId = adminUser.Id, FullName = "Admin", Street = "123 Admin St", City = "Dhaka", State = "Dhaka", ZipCode = "1205", Country = "Bangladesh", PhoneNumber = "+8801712345678", IsDefault = true },
            new Address { UserId = users[0].Id, FullName = "Ayesha Rahman", Street = "45 Gulshan Ave", City = "Dhaka", State = "Dhaka", ZipCode = "1212", Country = "Bangladesh", PhoneNumber = "+8801712345679", IsDefault = true },
            new Address { UserId = users[0].Id, FullName = "Ayesha Rahman", Street = "12 Uttara Rd", City = "Dhaka", State = "Dhaka", ZipCode = "1230", Country = "Bangladesh", PhoneNumber = "+8801712345679", IsDefault = false },
            new Address { UserId = users[1].Id, FullName = "Tanvir Hasan", Street = "78 Banani Dr", City = "Dhaka", State = "Dhaka", ZipCode = "1213", Country = "Bangladesh", PhoneNumber = "+8801712345680", IsDefault = true },
            new Address { UserId = users[2].Id, FullName = "Nusrat Jahan", Street = "56 Dhanmondi Rd", City = "Dhaka", State = "Dhaka", ZipCode = "1209", Country = "Bangladesh", PhoneNumber = "+8801712345681", IsDefault = true },
            new Address { UserId = users[3].Id, FullName = "Rafi Ahmed", Street = "34 Mirpur Rd", City = "Dhaka", State = "Dhaka", ZipCode = "1216", Country = "Bangladesh", PhoneNumber = "+8801712345682", IsDefault = true }
        );
        context.SaveChanges();

        var orderStatuses = new[] { OrderStatus.Pending, OrderStatus.Processing, OrderStatus.Shipped, OrderStatus.Delivered, OrderStatus.Cancelled, OrderStatus.Refunded };
        var paymentMethods = new[] { "Credit Card", "PayPal", "Cash on Delivery" };
        var rng = new Random(42);

        for (int i = 0; i < 6; i++)
        {
            var user = users[i % users.Count];
            var orderProducts = products.OrderBy(_ => rng.Next()).Take(rng.Next(1, 4)).ToList();
            var subtotal = orderProducts.Sum(p => p.Price);
            var shipping = subtotal >= 200 ? 0 : 8.50m;
            var discount = i == 0 ? subtotal * 0.10m : 0;
            var tax = (subtotal - discount) * 0.08m;
            var total = subtotal + shipping + tax - discount;

            var order = new Order
            {
                OrderNumber = $"ORD-{74291 + i}",
                UserId = user.Id,
                Subtotal = subtotal,
                ShippingCost = shipping,
                TaxAmount = tax,
                DiscountAmount = discount,
                TotalAmount = total,
                Status = orderStatuses[i],
                CouponCode = discount > 0 ? "SMART10" : null,
                PaymentMethod = paymentMethods[i % paymentMethods.Length],
                ShippingAddressJson = $"{{\"FullName\":\"{user.Name}\",\"Street\":\"123 Main St\",\"City\":\"Dhaka\",\"State\":\"Dhaka\",\"ZipCode\":\"1205\",\"Country\":\"Bangladesh\",\"PhoneNumber\":\"+8801712345678\",\"IsDefault\":true}}",
                CreatedAt = DateTime.UtcNow.AddDays(-(6 - i) * 2),
            };

            foreach (var p in orderProducts)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    ProductImageUrl = p.MainImageUrl,
                    UnitPrice = p.Price,
                    Quantity = rng.Next(1, 3),
                    TotalPrice = p.Price * rng.Next(1, 3)
                });
            }

            order.StatusHistory.Add(new OrderStatusHistory
            {
                Status = order.Status,
                Note = "Order placed",
                ChangedAt = order.CreatedAt
            });

            context.Orders.Add(order);
        }
        context.SaveChanges();

        foreach (var product in products)
        {
            var reviewers = users.OrderBy(_ => rng.Next()).Take(3).ToList();
            foreach (var reviewer in reviewers)
            {
                context.Reviews.Add(new Review
                {
                    ProductId = product.Id,
                    UserId = reviewer.Id,
                    Rating = rng.Next(3, 6),
                    Comment = GetReviewComment(rng.Next(0, 5)),
                    CreatedAt = DateTime.UtcNow.AddDays(-rng.Next(1, 30))
                });
            }
        }
        context.SaveChanges();

        context.Settings.Add(new Settings());
        context.SaveChanges();
    }

    private static string GetReviewComment(int index) => index switch
    {
        0 => "Excellent product! Highly recommended.",
        1 => "Great quality for the price. Very satisfied.",
        2 => "Good product but could be better.",
        3 => "Amazing! Exceeded my expectations.",
        4 => "Decent quality. Fast shipping.",
        _ => "Would buy again!"
    };
}
