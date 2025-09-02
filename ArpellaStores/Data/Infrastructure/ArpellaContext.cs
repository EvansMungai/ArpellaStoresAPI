using ArpellaStores.Features.Authentication.Models;
using ArpellaStores.Features.DeliveryTrackingManagement.Models;
using ArpellaStores.Features.FinalPriceManagement.Models;
using ArpellaStores.Features.GoodsInformationManagement.Models;
using ArpellaStores.Features.InventoryManagement.Models;
using ArpellaStores.Features.OrderManagement.Models;
using ArpellaStores.Features.SettingManagement.Models;
using ArpellaStores.Features.SmsManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Data.Infrastructure;

public partial class ArpellaContext : IdentityDbContext<User>
{
    private readonly string _connectionString;
    public ArpellaContext()
    {
    }

    public ArpellaContext(DbContextOptions<ArpellaContext> options, IConfiguration configuration)
        : base(options)
    {
        //_connectionString = configuration.GetConnectionString("arpellaDB");
        _connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__arpellaDB");
        ChangeTracker.LazyLoadingEnabled = false;
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }
    public virtual DbSet<Deliverytracking> Deliverytrackings { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Flashsale> Flashsales { get; set; }
    public virtual DbSet<Goodsinfo> Goodsinfos { get; set; }
    public virtual DbSet<Inventory> Inventories { get; set; }
    public virtual DbSet<Invoice> Invoices { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<Orderitem> Orderitems { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Productimage> Productimages { get; set; }
    public virtual DbSet<Restocklog> Restocklogs { get; set; }
    public virtual DbSet<Setting> Settings { get; set; }
    public virtual DbSet<SmsTemplate> SmsTemplates{ get; set; }
    public virtual DbSet<Subcategory> Subcategories { get; set; }
    public virtual DbSet<Supplier> Suppliers { get; set; }

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
        modelBuilder.Entity<Deliverytracking>(entity =>
        {
            entity.HasKey(e => e.DeliveryId).HasName("PRIMARY");

            entity.ToTable("deliverytracking");

            entity.HasIndex(e => e.OrderId, "orderId");

            entity.HasIndex(e => e.Username, "username");

            entity.Property(e => e.DeliveryId).HasColumnName("deliveryId");
            entity.Property(e => e.DeliveryAgent)
                .HasMaxLength(30)
                .HasColumnName("deliveryAgent");
            entity.Property(e => e.LastUpdated)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("lastUpdated");
            entity.Property(e => e.OrderId)
                .HasMaxLength(30)
                .HasColumnName("orderId");
            entity.Property(e => e.Status)
                .HasColumnType("enum('Pending','Processing','Delivering','Delivered')")
                .HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(30)
                .HasColumnName("username");

            entity.HasOne(d => d.Order).WithMany(p => p.Deliverytrackings)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("deliverytracking_ibfk_1");

            entity.HasOne(d => d.UsernameNavigation).WithMany(p => p.Deliverytrackings)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.Username)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("deliverytracking_ibfk_2");
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
        modelBuilder.Entity<Goodsinfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("goodsinfo");

            entity.HasIndex(e => e.ProductId, "productId");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ItemCode)
                .HasMaxLength(30)
                .HasColumnName("itemCode");
            entity.Property(e => e.ItemDescription)
                .HasMaxLength(30)
                .HasColumnName("itemDescription");
            entity.Property(e => e.ProductId)
                .HasMaxLength(150)
                .HasColumnName("productId");
            entity.Property(e => e.TaxRate)
                .HasPrecision(10, 2)
                .HasColumnName("taxRate");
            entity.Property(e => e.UnitMeasure)
                .HasMaxLength(30)
                .HasColumnName("unitMeasure");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PRIMARY");

            entity.ToTable("inventory");

            entity.HasIndex(e => e.InvoiceNumber, "invoiceNumber");

            entity.HasIndex(e => e.ProductId)
                .IsUnique()
                .HasDatabaseName("unique_product_id");

            entity.HasIndex(e => e.SupplierId, "supplierId");

            entity.Property(e => e.InventoryId).HasColumnName("inventoryId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("createdAt");
            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(30)
                .HasColumnName("invoiceNumber");
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
            entity.Property(e => e.SupplierId).HasColumnName("supplierId");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.InvoiceNumberNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.InvoiceNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventory_ibfk_2");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PRIMARY");

            entity.ToTable("invoices");

            entity.HasIndex(e => e.SupplierId, "supplierId");

            entity.Property(e => e.InvoiceId)
                .HasMaxLength(30)
                .HasColumnName("invoiceId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("createdAt");
            entity.Property(e => e.SupplierId).HasColumnName("supplierId");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("totalAmount");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Orderid).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.UserId, "userId");

            entity.Property(e => e.Orderid)
                .HasMaxLength(30)
                .HasColumnName("orderid");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(30)
                .HasColumnName("phoneNumber");
            entity.Property(e => e.BuyerPin)
                .HasMaxLength(30)
                .HasColumnName("buyerPin");
            entity.Property(e => e.Latitude)
                .HasPrecision(9, 6)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasPrecision(9, 6)
                .HasColumnName("longitude");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");
            entity.Property(e => e.UserId)
                .HasMaxLength(30)
                .HasColumnName("userId");
            entity.Property(e => e.OrderPaymentType)
                .HasColumnType("enum('Cash','Mpesa')")
                .HasColumnName("orderPaymentType");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("createdAt");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updatedAt");
            entity.Property(e => e.OrderSource)
                .HasColumnType("enum('Ecommerce','POS')")
                .HasColumnName("orderSource");

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
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("orderitems_ibfk_2");

            entity.HasOne(d => d.Product).WithMany(p => p.Orderitems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orderitems_ibfk_1");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PRIMARY");

            entity.ToTable("payments");

            entity.HasIndex(e => e.Orderid, "orderid");

            entity.Property(e => e.PaymentId).HasColumnName("paymentId");
            entity.Property(e => e.Orderid)
                .HasMaxLength(30)
                .HasColumnName("orderid");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(30)
                .HasColumnName("transactionId");

            entity.HasOne(d => d.Order).WithMany()
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("payments_ibfk_1");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("products");

            entity.HasIndex(e => e.InventoryId, "AK_products_inventoryId");

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
                .HasMaxLength(150)
                .HasColumnName("inventoryId");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.PriceAfterDiscount)
                .HasPrecision(10, 2)
                .HasColumnName("price_after_discount");
            entity.Property(e => e.PurchaseCap).HasColumnName("purchaseCap");
            entity.Property(e => e.Subcategory).HasColumnName("subcategory");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updatedAt");
            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Category)
                .HasConstraintName("products_ibfk_1");

            entity.HasOne(d => d.SubcategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Subcategory)
                .HasConstraintName("products_ibfk_6");
            entity.HasOne(d => d.IdNavigation)
                  .WithMany(p => p.Products)
                  .HasForeignKey(d => d.InventoryId)
                  .HasPrincipalKey(p => p.ProductId)
                  .HasConstraintName("products_ibfk_7");
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
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.HasOne(d => d.Product)
                .WithMany(p => p.Productimages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("productimages_ibfk_1");
        });

        modelBuilder.Entity<Restocklog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PRIMARY");

            entity.ToTable("restocklog");

            entity.HasIndex(e => e.InvoiceNumber, "invoiceNumber");

            entity.HasIndex(e => e.SupplierId, "supplierId");

            entity.Property(e => e.LogId).HasColumnName("logId");
            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(30)
                .HasColumnName("invoiceNumber");
            entity.Property(e => e.ProductId)
                .HasMaxLength(30)
                .HasColumnName("productId");
            entity.Property(e => e.PurchasePrice)
                .HasPrecision(10, 2)
                .HasColumnName("purchasePrice");
            entity.Property(e => e.RestockDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("restockDate");
            entity.Property(e => e.RestockQuantity).HasColumnName("restockQuantity");
            entity.Property(e => e.SupplierId).HasColumnName("supplierId");

            entity.HasOne(d => d.InvoiceNumberNavigation).WithMany(p => p.Restocklogs)
                .HasForeignKey(d => d.InvoiceNumber)
                .HasConstraintName("restocklog_ibfk_1");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Restocklogs)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("restocklog_ibfk_2");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("settings");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SettingName)
                .HasMaxLength(30)
                .HasColumnName("settingName");
            entity.Property(e => e.SettingValue)
                .HasMaxLength(50)
                .HasColumnName("settingValue");
        });

        modelBuilder.Entity<SmsTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("smstemplates");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(300)
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("createdAt");
            entity.Property(e => e.TemplateType)
                .HasMaxLength(100)
                .HasColumnName("templateType");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updatedAt");
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

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("suppliers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.KraPin)
                .HasMaxLength(30)
                .HasColumnName("kraPin");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(50)
                .HasColumnName("supplierName");
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
