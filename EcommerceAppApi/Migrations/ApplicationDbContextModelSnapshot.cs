using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using EcommerceAppApi.Domain.Entities;
using EcommerceAppApi.Infrastructure.Data;

#nullable disable

namespace EcommerceAppApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "10.0.7");

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.Address", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<int>("UserId").HasColumnType("INTEGER");
                    b.Property<string>("FullName").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("Street").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("City").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("State").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("ZipCode").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("Country").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("PhoneNumber").IsRequired().HasColumnType("TEXT");
                    b.Property<bool>("IsDefault").HasColumnType("INTEGER");
                    b.HasKey("Id");
                    b.HasIndex("UserId");
                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.Brand", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<string>("Name").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("Slug").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("LogoUrl").HasColumnType("TEXT");
                    b.Property<string>("Description").HasColumnType("TEXT");
                    b.Property<string>("Website").HasColumnType("TEXT");
                    b.HasKey("Id");
                    b.HasIndex("Slug").IsUnique();
                    b.ToTable("Brands");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.BrandFollower", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<int>("UserId").HasColumnType("INTEGER");
                    b.Property<int>("BrandId").HasColumnType("INTEGER");
                    b.Property<DateTime>("FollowedAt").HasColumnType("TEXT");
                    b.HasKey("Id");
                    b.HasIndex("BrandId");
                    b.HasIndex("UserId");
                    b.ToTable("BrandFollowers");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.Cart", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<int>("UserId").HasColumnType("INTEGER");
                    b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
                    b.Property<DateTime?>("UpdatedAt").HasColumnType("TEXT");
                    b.HasKey("Id");
                    b.HasIndex("UserId").IsUnique();
                    b.ToTable("Carts");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.CartItem", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<int>("CartId").HasColumnType("INTEGER");
                    b.Property<int>("ProductId").HasColumnType("INTEGER");
                    b.Property<int>("Quantity").HasColumnType("INTEGER");
                    b.Property<DateTime>("AddedAt").HasColumnType("TEXT");
                    b.HasKey("Id");
                    b.HasIndex("CartId");
                    b.HasIndex("ProductId");
                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.Category", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<string>("Name").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("Slug").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("Description").HasColumnType("TEXT");
                    b.Property<string>("ImageUrl").HasColumnType("TEXT");
                    b.Property<int?>("ParentCategoryId").HasColumnType("INTEGER");
                    b.Property<int>("DisplayOrder").HasColumnType("INTEGER");
                    b.HasKey("Id");
                    b.HasIndex("Slug").IsUnique();
                    b.HasIndex("ParentCategoryId");
                    b.ToTable("Categories");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.Order", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<string>("OrderNumber").IsRequired().HasColumnType("TEXT");
                    b.Property<int>("UserId").HasColumnType("INTEGER");
                    b.Property<decimal>("Subtotal").HasPrecision(18, 2).HasColumnType("TEXT(18,2)");
                    b.Property<decimal>("ShippingCost").HasPrecision(18, 2).HasColumnType("TEXT(18,2)");
                    b.Property<decimal>("TaxAmount").HasPrecision(18, 2).HasColumnType("TEXT(18,2)");
                    b.Property<decimal>("DiscountAmount").HasPrecision(18, 2).HasColumnType("TEXT(18,2)");
                    b.Property<decimal>("TotalAmount").HasPrecision(18, 2).HasColumnType("TEXT(18,2)");
                    b.Property<int>("Status").HasColumnType("INTEGER");
                    b.Property<string>("CouponCode").HasColumnType("TEXT");
                    b.Property<string>("PaymentMethod").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("PaymentTransactionId").HasColumnType("TEXT");
                    b.Property<string>("ShippingAddressJson").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("TrackingNumber").HasColumnType("TEXT");
                    b.Property<string>("TrackingUrl").HasColumnType("TEXT");
                    b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
                    b.Property<DateTime?>("UpdatedAt").HasColumnType("TEXT");
                    b.Property<DateTime?>("ShippedAt").HasColumnType("TEXT");
                    b.Property<DateTime?>("DeliveredAt").HasColumnType("TEXT");
                    b.HasKey("Id");
                    b.HasIndex("OrderNumber").IsUnique();
                    b.HasIndex("UserId");
                    b.ToTable("Orders");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.OrderItem", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<int>("OrderId").HasColumnType("INTEGER");
                    b.Property<int>("ProductId").HasColumnType("INTEGER");
                    b.Property<string>("ProductName").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("ProductImageUrl").HasColumnType("TEXT");
                    b.Property<decimal>("UnitPrice").HasPrecision(18, 2).HasColumnType("TEXT(18,2)");
                    b.Property<int>("Quantity").HasColumnType("INTEGER");
                    b.Property<decimal>("TotalPrice").HasPrecision(18, 2).HasColumnType("TEXT(18,2)");
                    b.HasKey("Id");
                    b.HasIndex("OrderId");
                    b.HasIndex("ProductId");
                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.OrderStatusHistory", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<int>("OrderId").HasColumnType("INTEGER");
                    b.Property<int>("Status").HasColumnType("INTEGER");
                    b.Property<string>("Note").HasColumnType("TEXT");
                    b.Property<DateTime>("ChangedAt").HasColumnType("TEXT");
                    b.HasKey("Id");
                    b.HasIndex("OrderId");
                    b.ToTable("OrderStatusHistories");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.Product", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<string>("Name").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("Slug").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("Description").IsRequired().HasColumnType("TEXT");
                    b.Property<decimal>("Price").HasPrecision(18, 2).HasColumnType("TEXT(18,2)");
                    b.Property<decimal?>("CompareAtPrice").HasPrecision(18, 2).HasColumnType("TEXT(18,2)");
                    b.Property<int>("StockQuantity").HasColumnType("INTEGER");
                    b.Property<int?>("CategoryId").HasColumnType("INTEGER");
                    b.Property<int?>("BrandId").HasColumnType("INTEGER");
                    b.Property<string>("MainImageUrl").HasColumnType("TEXT");
                    b.Property<string>("Sku").HasColumnType("TEXT");
                    b.Property<bool>("IsActive").HasColumnType("INTEGER");
                    b.Property<bool>("IsFeatured").HasColumnType("INTEGER");
                    b.Property<double?>("AverageRating").HasColumnType("REAL");
                    b.Property<int>("TotalReviews").HasColumnType("INTEGER");
                    b.Property<int>("SalesCount").HasColumnType("INTEGER");
                    b.Property<string>("SpecsJson").HasColumnType("TEXT");
                    b.Property<string>("FeaturesJson").HasColumnType("TEXT");
                    b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
                    b.Property<DateTime?>("UpdatedAt").HasColumnType("TEXT");
                    b.HasKey("Id");
                    b.HasIndex("BrandId");
                    b.HasIndex("CategoryId");
                    b.HasIndex("Sku").IsUnique();
                    b.HasIndex("Slug").IsUnique();
                    b.ToTable("Products");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.ProductImage", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<int>("ProductId").HasColumnType("INTEGER");
                    b.Property<string>("ImageUrl").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("AltText").HasColumnType("TEXT");
                    b.Property<int>("DisplayOrder").HasColumnType("INTEGER");
                    b.HasKey("Id");
                    b.HasIndex("ProductId");
                    b.ToTable("ProductImages");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.Review", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<int>("ProductId").HasColumnType("INTEGER");
                    b.Property<int>("UserId").HasColumnType("INTEGER");
                    b.Property<int>("Rating").HasColumnType("INTEGER");
                    b.Property<string>("Comment").IsRequired().HasColumnType("TEXT");
                    b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
                    b.Property<DateTime?>("UpdatedAt").HasColumnType("TEXT");
                    b.HasKey("Id");
                    b.HasIndex("ProductId");
                    b.HasIndex("UserId");
                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.Settings", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<string>("StoreName").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("Email").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("Currency").IsRequired().HasColumnType("TEXT");
                    b.Property<decimal>("TaxRate").HasColumnType("TEXT");
                    b.Property<bool>("RecommendationEnabled").HasColumnType("INTEGER");
                    b.Property<bool>("ForecastingEnabled").HasColumnType("INTEGER");
                    b.Property<string>("RetrainSchedule").IsRequired().HasColumnType("TEXT");
                    b.HasKey("Id");
                    b.ToTable("Settings");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<string>("Name").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("Email").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("PasswordHash").IsRequired().HasColumnType("TEXT");
                    b.Property<string>("PhoneNumber").IsRequired().HasColumnType("TEXT");
                    b.Property<int>("Role").HasColumnType("INTEGER");
                    b.Property<bool>("Blocked").HasColumnType("INTEGER");
                    b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
                    b.Property<DateTime?>("UpdatedAt").HasColumnType("TEXT");
                    b.HasKey("Id");
                    b.HasIndex("Email").IsUnique();
                    b.ToTable("Users");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.UserRefreshToken", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<int>("UserId").HasColumnType("INTEGER");
                    b.Property<string>("Token").IsRequired().HasColumnType("TEXT");
                    b.Property<DateTime>("ExpiresAt").HasColumnType("TEXT");
                    b.Property<bool>("IsRevoked").HasColumnType("INTEGER");
                    b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
                    b.HasKey("Id");
                    b.HasIndex("UserId");
                    b.ToTable("UserRefreshTokens");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.Wishlist", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<int>("UserId").HasColumnType("INTEGER");
                    b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
                    b.HasKey("Id");
                    b.HasIndex("UserId").IsUnique();
                    b.ToTable("Wishlists");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.WishlistItem", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                    b.Property<int>("WishlistId").HasColumnType("INTEGER");
                    b.Property<int>("ProductId").HasColumnType("INTEGER");
                    b.Property<DateTime>("AddedAt").HasColumnType("TEXT");
                    b.HasKey("Id");
                    b.HasIndex("ProductId");
                    b.HasIndex("WishlistId");
                    b.ToTable("WishlistItems");
                });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.Address", b =>
                { b.HasOne("EcommerceAppApi.Domain.Entities.User", "User").WithMany("Addresses").HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired(); });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.BrandFollower", b =>
                { b.HasOne("EcommerceAppApi.Domain.Entities.Brand", "Brand").WithMany("Followers").HasForeignKey("BrandId").OnDelete(DeleteBehavior.Cascade).IsRequired();
                  b.HasOne("EcommerceAppApi.Domain.Entities.User", "User").WithMany("FollowedBrands").HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired(); });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.Cart", b =>
                { b.HasOne("EcommerceAppApi.Domain.Entities.User", "User").WithOne("Cart").HasForeignKey("Cart").OnDelete(DeleteBehavior.Cascade).IsRequired(); });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.CartItem", b =>
                { b.HasOne("EcommerceAppApi.Domain.Entities.Cart", "Cart").WithMany("Items").HasForeignKey("CartId").OnDelete(DeleteBehavior.Cascade).IsRequired();
                  b.HasOne("EcommerceAppApi.Domain.Entities.Product", "Product").WithMany("CartItems").HasForeignKey("ProductId").OnDelete(DeleteBehavior.Cascade).IsRequired(); });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.Category", b =>
                { b.HasOne("EcommerceAppApi.Domain.Entities.Category", "ParentCategory").WithMany("SubCategories").HasForeignKey("ParentCategoryId"); });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.Order", b =>
                { b.HasOne("EcommerceAppApi.Domain.Entities.User", "User").WithMany("Orders").HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired(); });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.OrderItem", b =>
                { b.HasOne("EcommerceAppApi.Domain.Entities.Order", "Order").WithMany("Items").HasForeignKey("OrderId").OnDelete(DeleteBehavior.Cascade).IsRequired();
                  b.HasOne("EcommerceAppApi.Domain.Entities.Product", "Product").WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.Cascade).IsRequired(); });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.OrderStatusHistory", b =>
                { b.HasOne("EcommerceAppApi.Domain.Entities.Order", "Order").WithMany("StatusHistory").HasForeignKey("OrderId").OnDelete(DeleteBehavior.Cascade).IsRequired(); });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.Product", b =>
                { b.HasOne("EcommerceAppApi.Domain.Entities.Brand", "Brand").WithMany("Products").HasForeignKey("BrandId");
                  b.HasOne("EcommerceAppApi.Domain.Entities.Category", "Category").WithMany("Products").HasForeignKey("CategoryId"); });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.ProductImage", b =>
                { b.HasOne("EcommerceAppApi.Domain.Entities.Product", "Product").WithMany("Images").HasForeignKey("ProductId").OnDelete(DeleteBehavior.Cascade).IsRequired(); });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.Review", b =>
                { b.HasOne("EcommerceAppApi.Domain.Entities.Product", "Product").WithMany("Reviews").HasForeignKey("ProductId").OnDelete(DeleteBehavior.Cascade).IsRequired();
                  b.HasOne("EcommerceAppApi.Domain.Entities.User", "User").WithMany("Reviews").HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired(); });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.UserRefreshToken", b =>
                { b.HasOne("EcommerceAppApi.Domain.Entities.User", "User").WithMany("RefreshTokens").HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired(); });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.Wishlist", b =>
                { b.HasOne("EcommerceAppApi.Domain.Entities.User", "User").WithOne("Wishlist").HasForeignKey("Wishlist").OnDelete(DeleteBehavior.Cascade).IsRequired(); });

            modelBuilder.Entity("EcommerceAppApi.Domain.Entities.WishlistItem", b =>
                { b.HasOne("EcommerceAppApi.Domain.Entities.Wishlist", "Wishlist").WithMany("Items").HasForeignKey("WishlistId").OnDelete(DeleteBehavior.Cascade).IsRequired();
                  b.HasOne("EcommerceAppApi.Domain.Entities.Product", "Product").WithMany("WishlistItems").HasForeignKey("ProductId").OnDelete(DeleteBehavior.Cascade).IsRequired(); });

#pragma warning restore 612, 618
        }
    }
}
