using ArpellaStores.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Data;

public partial class ArpellaContext : IdentityDbContext<User>
{
    private readonly string _connectionString;
    public ArpellaContext()
    {
    }

    public ArpellaContext(DbContextOptions<ArpellaContext> options, IConfiguration configuration)
        : base(options)
    {
        _connectionString = configuration.GetConnectionString("arpellaDB");
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Flashsale> Flashsales { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Orderitem> Orderitems { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Productimage> Productimages { get; set; }

    public virtual DbSet<Subcategory> Subcategories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
        }
    }
    //=> optionsBuilder.UseMySql(_connectionString, Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.2.0-mysql")).UseExceptionProcessor();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(iul => new { iul.LoginProvider, iul.ProviderKey });
        modelBuilder.Entity<IdentityUserRole<string>>().HasKey(iur => new { iur.UserId, iur.RoleId });
        modelBuilder.Entity<IdentityUserToken<string>>().HasKey(iut => new { iut.UserId, iut.LoginProvider, iut.Name });


        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.CouponId).HasName("PRIMARY");

            entity.ToTable("coupons");

            entity.HasIndex(e => e.CouponCode, "coupon_code").IsUnique();

            entity.Property(e => e.CouponId).HasColumnName("coupon_id");
            entity.Property(e => e.CouponCode)
                .HasMaxLength(50)
                .HasColumnName("coupon_code");
            entity.Property(e => e.DiscountType)
                .HasColumnType("enum('percentage','fixed')")
                .HasColumnName("discount_type");
            entity.Property(e => e.DiscountValue)
                .HasPrecision(10, 2)
                .HasColumnName("discount_value");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.UsageCount)
                .HasDefaultValueSql("'0'")
                .HasColumnName("usage_count");
            entity.Property(e => e.UsageLimit)
                .HasDefaultValueSql("'1'")
                .HasColumnName("usage_limit");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PRIMARY");

            entity.ToTable("discounts");

            entity.HasIndex(e => e.DiscountCode, "discount_code").IsUnique();

            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.DiscountCode)
                .HasMaxLength(50)
                .HasColumnName("discount_code");
            entity.Property(e => e.DiscountType)
                .HasColumnType("enum('percentage','fixed','flash_sale')")
                .HasColumnName("discount_type");
            entity.Property(e => e.DiscountValue)
                .HasPrecision(10, 2)
                .HasColumnName("discount_value");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
        });

        modelBuilder.Entity<Flashsale>(entity =>
        {
            entity.HasKey(e => e.FlashSaleId).HasName("PRIMARY");

            entity.ToTable("flashsales");

            entity.HasIndex(e => e.ProductId, "flashsales_ibfk_1");

            entity.Property(e => e.FlashSaleId).HasColumnName("flash_sale_id");
            entity.Property(e => e.DiscountValue)
                .HasPrecision(10, 2)
                .HasColumnName("discount_value");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("start_time");

            entity.HasOne(d => d.Product).WithMany(p => p.Flashsales)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("flashsales_ibfk_1");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PRIMARY");

            entity.ToTable("inventory");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.InventoryId).HasColumnName("inventoryId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("createdAt");
            entity.Property(e => e.ProductId)
                .HasMaxLength(30)
                .HasColumnName("product_id");
            entity.Property(e => e.StockPrice)
                .HasPrecision(10, 2)
                .HasColumnName("stock_price");
            entity.Property(e => e.StockQuantity)
                .HasDefaultValueSql("'0'")
                .HasColumnName("stock_quantity");
            entity.Property(e => e.StockThreshold).HasColumnName("stock_threshold");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.UserId, "userId");

            entity.Property(e => e.OrderId)
                .HasMaxLength(30)
                .HasColumnName("orderid");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");
            entity.Property(e => e.UserId)
                .HasMaxLength(30)
                .HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("orders_ibfk_1");
        });

        modelBuilder.Entity<Orderitem>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("orderitems");

            entity.HasIndex(e => e.ProductId, "orderitems_ibfk_1");

            entity.Property(e => e.OrderId)
                .HasMaxLength(30)
                .HasColumnName("orderid");
            entity.Property(e => e.ProductId).HasColumnName("productid");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.Orderitems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orderitems_ibfk_2");

            entity.HasOne(d => d.Product).WithMany(p => p.Orderitems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orderitems_ibfk_1");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("payments");

            entity.HasIndex(e => e.OrderId, "orderid");

            entity.Property(e => e.OrderId)
                .HasMaxLength(30)
                .HasColumnName("orderid");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(30)
                .HasColumnName("transactionId");

            entity.HasOne(d => d.Order).WithMany()
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("payments_ibfk_1");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("products");

            entity.HasIndex(e => e.InventoryId, "inventoryId");

            entity.HasIndex(e => e.Category, "products_ibfk_1");

            entity.HasIndex(e => e.Subcategory, "subcategory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Barcodes)
                .HasMaxLength(255)
                .HasColumnName("barcodes");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("createdAt");
            entity.Property(e => e.DiscountQuantity).HasColumnName("discount_quantity");
            entity.Property(e => e.InventoryId)
                .HasMaxLength(30)
                .HasColumnName("inventoryId");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.PriceAfterDiscount)
                .HasPrecision(10, 2)
                .HasColumnName("price_after_discount");
            entity.Property(e => e.PurchaseCap).HasColumnName("purchaseCap");
            entity.Property(e => e.Subcategory).HasColumnName("subcategory");
            entity.Property(e => e.TaxRate)
                .HasPrecision(10, 2)
                .HasColumnName("tax_rate");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Category)
                .HasConstraintName("products_ibfk_1");

            entity.HasOne(p => p.IdNavigation)
                  .WithMany(i => i.Products)
                  .HasForeignKey(p => p.InventoryId) 
                  .HasPrincipalKey(i => i.ProductId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("products_ibfk_5");

            entity.HasOne(d => d.SubcategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Subcategory)
                .HasConstraintName("products_ibfk_6");
        });

        modelBuilder.Entity<Productimage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PRIMARY");

            entity.ToTable("productimages");

            entity.HasIndex(e => e.ProductId, "productimages_ibfk_1");

            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.IsPrimary)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_primary");
            entity.Property(e => e.ProductId)
                .HasMaxLength(30)
                .HasColumnName("product_id");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.HasOne(d => d.Product) 
                .WithMany(p => p.Productimages) 
                .HasForeignKey(d => d.ProductId) 
                .HasPrincipalKey(p => p.InventoryId) 
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("productimages_ibfk_1");

        });

        modelBuilder.Entity<Subcategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("subcategories");

            entity.HasIndex(e => e.CategoryId, "subcategories_ibfk_1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("categoryId");
            entity.Property(e => e.SubcategoryName)
                .HasMaxLength(50)
                .HasColumnName("subcategoryName");

            entity.HasOne(d => d.Category).WithMany(p => p.Subcategories)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("subcategories_ibfk_1");
        });

        //modelBuilder.Entity<User>(entity =>
        //{
        //    entity.HasKey(e => e.Id).HasName("PRIMARY");

        //    entity.ToTable("users");

        //    entity.Property(e => e.Id)
        //        .HasMaxLength(30)
        //        .HasColumnName("id");
        //    entity.Property(e => e.Password)
        //        .HasMaxLength(200)
        //        .HasColumnName("password");
        //    entity.Property(e => e.Phone)
        //        .HasMaxLength(15)
        //        .HasColumnName("phone");
        //    entity.Property(e => e.Role)
        //        .HasMaxLength(20)
        //        .HasColumnName("role");
        //    entity.Property(e => e.Username)
        //        .HasMaxLength(50)
        //        .HasColumnName("username");
        //});

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
