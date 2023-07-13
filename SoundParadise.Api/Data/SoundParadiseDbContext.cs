using Bogus;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Slugify;
using SoundParadise.Api.Models.Accumulators.AllSalesAccumulator;
using SoundParadise.Api.Models.Accumulators.ProductSalesAccumulator;
using SoundParadise.Api.Models.Accumulators.UserAccumulator;
using SoundParadise.Api.Models.Address;
using SoundParadise.Api.Models.Card;
using SoundParadise.Api.Models.Cart;
using SoundParadise.Api.Models.CartItem;
using SoundParadise.Api.Models.Category;
using SoundParadise.Api.Models.Comment;
using SoundParadise.Api.Models.Enums;
using SoundParadise.Api.Models.Image;
using SoundParadise.Api.Models.Log;
using SoundParadise.Api.Models.Order;
using SoundParadise.Api.Models.PaymentDetails;
using SoundParadise.Api.Models.Product;
using SoundParadise.Api.Models.ProductParameter;
using SoundParadise.Api.Models.Subcategory;
using SoundParadise.Api.Models.Token;
using SoundParadise.Api.Models.TokenJournal;
using SoundParadise.Api.Models.User;
using SoundParadise.Api.Models.Wishlist;
using SoundParadise.Api.Services;

namespace SoundParadise.Api.Data;

/// <summary>
///     Data base context.
/// </summary>
public sealed class SoundParadiseDbContext : DbContext
{
    private readonly IHostEnvironment _environment;
    private readonly HashingService _hashingService;

    /// <summary>
    ///     Constructor for SoundParadiseDbContext.
    /// </summary>
    /// <param name="options">Option.</param>
    /// <param name="environment">Environment.</param>
    /// <param name="hashingService">Hashing service.</param>
    public SoundParadiseDbContext(DbContextOptions<SoundParadiseDbContext> options, IHostEnvironment environment,
        HashingService hashingService) :
        base(options)
    {
        _environment = environment;
        _hashingService = hashingService;
    }

    /// <summary>
    ///     Dbset users.
    /// </summary>
    public DbSet<UserModel> Users { get; set; }

    /// <summary>
    ///     Dbset logs.
    /// </summary>
    public DbSet<LogModel> Logs { get; set; }

    /// <summary>
    ///     Dbset tokens users.
    /// </summary>
    public DbSet<TokenModel> Tokens { get; set; }

    /// <summary>
    ///     Dbset products..
    /// </summary>
    public DbSet<ProductModel> Products { get; set; }

    /// <summary>
    ///     Dbset Carts.
    /// </summary>
    public DbSet<CartModel> Carts { get; set; }

    /// <summary>
    ///     Dbset CartItems.
    /// </summary>
    public DbSet<CartItemModel> CartItems { get; set; }

    /// <summary>
    ///     Dbset Comments.
    /// </summary>
    public DbSet<CommentModel> Comments { get; set; }

    /// <summary>
    ///     Dbset Images.
    /// </summary>
    public DbSet<ImageModel> Images { get; set; }

    /// <summary>
    ///     Dbset subcategory.
    /// </summary>
    public DbSet<SubcategoryModel> Subcategories { get; set; }

    /// <summary>
    ///     Dbset categories.
    /// </summary>
    public DbSet<CategoryModel> Categories { get; set; }

    /// <summary>
    ///     Dbset parameters.
    /// </summary>
    public DbSet<ProductParameterModel> Parameters { get; set; }

    /// <summary>
    ///     Dbset orders.
    /// </summary>
    public DbSet<OrderModel> Orders { get; set; }

    /// <summary>
    ///     Dbset payments.
    /// </summary>
    public DbSet<PaymentModel> Payments { get; set; }

    /// <summary>
    ///     Dbset token journals.
    /// </summary>
    public DbSet<TokenJournalModel> TokenJournals { get; set; }

    /// <summary>
    ///     Dbset user accumulator.
    /// </summary>
    public DbSet<UserAccumulator> UserAccumulators { get; set; }

    /// <summary>
    ///     Dbset all sales accumulator.
    /// </summary>
    public DbSet<AllSalesAccumulator> AllSalesAccumulators { get; set; }

    /// <summary>
    ///     Dbset product sales accumulator.
    /// </summary>
    public DbSet<ProductSalesAccumulator> ProductSalesAccumulators { get; set; }

    /// <summary>
    ///     Dbset whishlist.
    /// </summary>
    public DbSet<WishlistModel> Wishlists { get; set; }

    /// <summary>
    ///     Dbset whislistproducts.
    /// </summary>
    public DbSet<WishlistProductsModel> WishlistProducts { get; set; }

    /// <summary>
    ///     Dbset delivery address.
    /// </summary>
    public DbSet<AddressModel> DeliveryAddresses { get; set; }

    /// <summary>
    ///     Dbset card model.
    /// </summary>
    public DbSet<CardModel> Cards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var uppercaseConverter = new ValueConverter<string, string>(
            s => s.ToUpper(),
            s => s,
            new ConverterMappingHints(255));

        var lowercaseConverter = new ValueConverter<string, string>(
            s => s.ToLower(),
            s => s,
            new ConverterMappingHints(255));

        var slugConverter = new ValueConverter<string, string>(
            s => new SlugHelper().GenerateSlug(s),
            s => s,
            new ConverterMappingHints(255));

        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.ToTable("users");
            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100)
                .HasConversion(uppercaseConverter)
                .UseCollation("Latin1_General_CI_AI");

            entity.HasOne(u => u.Image)
                .WithOne(i => i.User)
                .HasForeignKey<UserModel>(u => u.ImageId);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100)
                .HasConversion(uppercaseConverter)
                .UseCollation("Latin1_General_CI_AI");

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasConversion(uppercaseConverter)
                .UseCollation("Latin1_General_CI_AI");

            entity.Property(u => u.Surname)
                .IsRequired()
                .HasMaxLength(100)
                .HasConversion(uppercaseConverter)
                .UseCollation("Latin1_General_CI_AI");

            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.PhoneNumber).IsUnique();

            entity.Property(u => u.Role).HasColumnType("smallint");

            entity.Property(u => u.ConfirmationToken)
                .HasMaxLength(255);

            entity.HasOne(u => u.Cart)
                .WithOne(b => b.User)
                .HasForeignKey<CartModel>(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);

            entity.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId);

            entity.HasMany(u => u.DeliveryAddresses)
                .WithOne(d => d.User)
                .HasForeignKey(d => d.UserId);

            entity.HasOne(u => u.Wishlist)
                .WithOne(w => w.User)
                .HasForeignKey<WishlistModel>(w => w.UserId);

            entity.HasMany(u => u.Cards)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);
        });

        modelBuilder.Entity<CardModel>(entity =>
        {
            entity.ToTable("cards");

            entity.HasOne(c => c.User)
                .WithMany(u => u.Cards)
                .HasForeignKey(c => c.UserId);

            entity.HasIndex(c => c.EncryptedCardNumber).IsUnique();
        });

        modelBuilder.Entity<UserAccumulator>(entity =>
        {
            entity.ToTable("user_accumulator");

            entity.Property(p => p.Date)
                .IsRequired()
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<WishlistModel>(entity =>
        {
            entity.ToTable("wish_lists");

            entity.HasOne(p => p.User)
                .WithOne(j => j.Wishlist)
                .HasForeignKey<UserModel>(d => d.WishlistId);
        });

        modelBuilder.Entity<WishlistProductsModel>(entity =>
        {
            entity.ToTable("wish_list_products");

            entity.HasOne(wp => wp.Wishlist)
                .WithMany(w => w.WishlistProducts)
                .HasForeignKey(wp => wp.WishlistId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            entity.HasOne(wp => wp.Product)
                .WithMany(p => p.WishlistProducts)
                .HasForeignKey(wp => wp.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });


        modelBuilder.Entity<ProductSalesAccumulator>(entity =>
        {
            entity.ToTable("product_sales_accumulator");

            entity.Property(p => p.Date)
                .IsRequired()
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<AllSalesAccumulator>(entity =>
        {
            entity.ToTable("all_sales_accumulator");

            entity.Property(p => p.Date)
                .IsRequired()
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<LogModel>(entity =>
        {
            entity.ToTable("logs");

            entity.Property(l => l.Application)
                .HasMaxLength(100);

            entity.Property(l => l.Message)
                .HasColumnType("text");

            entity.Property(l => l.Level)
                .HasMaxLength(100);

            entity.Property(l => l.Exception)
                .HasColumnType("text");
        });

        modelBuilder.Entity<TokenModel>(entity =>
        {
            entity.ToTable("tokens");

            entity.Property(t => t.Token)
                .IsRequired();

            entity.Property(t => t.ExpirationDate)
                .IsRequired();
        });

        modelBuilder.Entity<OrderModel>(entity =>
        {
            entity.ToTable("orders");

            entity.Property(u => u.CustomerName)
                .IsRequired()
                .HasMaxLength(100)
                .HasConversion(uppercaseConverter)
                .UseCollation("Latin1_General_CI_AI");

            entity.Property(u => u.CustomerSurname)
                .IsRequired()
                .HasMaxLength(100)
                .HasConversion(uppercaseConverter)
                .UseCollation("Latin1_General_CI_AI");

            entity.HasOne(p => p.User)
                .WithMany(p => p.Orders)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Address)
                .WithMany(d => d.Orders)
                .HasForeignKey(o => o.DeliveryAddressId);

            entity.Property(u => u.DeliveryType)
                .HasColumnType("smallint");

            entity.Property(u => u.PaymentType)
                .HasColumnType("smallint");

            entity.Property(u => u.OrderStatus)
                .HasColumnType("smallint");

            entity.HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<PaymentModel>(p => p.OrderId);

            // entity.Property(o => o.TotalPrice)
            //     .HasComputedColumnSql("SELECT SUM(product.price * Cart_item.quantity) FROM Cart_item INNER JOIN product ON Cart_item.product_id = product.product_id WHERE Cart_item.order_id = order_id");
        });

        modelBuilder.Entity<PaymentModel>(entity =>
        {
            entity.ToTable("payments");
            entity.HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<OrderModel>(o => o.PaymentId);
        });

        modelBuilder.Entity<TokenJournalModel>(entity =>
        {
            entity.ToTable("token_journals");
            entity.HasKey(ut => ut.Id);

            entity.HasOne(ut => ut.User)
                .WithMany(u => u.TokenJournals)
                .HasForeignKey(ut => ut.UserId);

            entity.HasOne(ut => ut.Token)
                .WithMany(t => t.TokenJournals)
                .HasForeignKey(ut => ut.TokenId);
        });

        modelBuilder.Entity<CartModel>(entity =>
        {
            entity.ToTable("carts");

            entity.HasOne(u => u.User)
                .WithOne(b => b.Cart)
                .HasForeignKey<UserModel>(b => b.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(b => b.CartItems)
                .WithOne(bi => bi.Cart)
                .HasForeignKey(bi => bi.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItemModel>(entity =>
        {
            entity.ToTable("cart_items");

            entity.Property(bi => bi.Quantity)
                .IsRequired()
                .HasColumnType("smallint")
                .HasDefaultValue((short)1);

            entity.HasOne(bi => bi.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(bi => bi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(bi => bi.Order)
                .WithMany(o => o.CartItems)
                .HasForeignKey(bi => bi.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(bi => bi.Cart)
                .WithMany(b => b.CartItems)
                .HasForeignKey(bi => bi.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductModel>(entity =>
        {
            entity.ToTable("products");

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100)
                .UseCollation("Latin1_General_CI_AI");

            entity.Property(p => p.Description)
                .HasMaxLength(1000)
                .UseCollation("Latin1_General_CI_AI");

            entity.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            entity.HasMany(p => p.CartItems)
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductId);

            entity.HasOne(p => p.Seller)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.Images)
                .WithOne(u => u.Product)
                .HasForeignKey(u => u.ProductId);

            entity.HasMany(p => p.Comments)
                .WithOne(u => u.Product)
                .HasForeignKey(p => p.ProductId);

            entity.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .ValueGeneratedOnAdd();

            entity.Property(p => p.Slug)
                .HasMaxLength(100)
                .UseCollation("Latin1_General_CI_AI")
                .HasConversion(lowercaseConverter);

            entity.HasIndex(p => p.Slug)
                .IsUnique();
        });

        modelBuilder.Entity<CommentModel>(entity =>
        {
            entity.ToTable("comments");

            entity.Property(c => c.Content)
                .IsRequired()
                .UseCollation("Latin1_General_CI_AI")
                .HasMaxLength(1000);

            entity.Property(c => c.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<CategoryModel>(entity =>
        {
            entity.ToTable("categories");

            entity.Property(p => p.Name)
                .HasMaxLength(50)
                .UseCollation("Latin1_General_CI_AI")
                .HasConversion(uppercaseConverter);

            entity.HasMany(c => c.Subcategories)
                .WithOne(c => c.Category)
                .HasForeignKey(j => j.CategoryId);

            entity.HasOne(s => s.Image)
                .WithOne(i => i.Category)
                .HasForeignKey<ImageModel>(i => i.CategoryId);
        });

        modelBuilder.Entity<ProductParameterModel>(entity =>
        {
            entity.ToTable("parameters");

            entity.HasOne(p => p.Product)
                .WithMany(p => p.Parameters)
                .HasForeignKey(p => p.ProductId);
        });

        modelBuilder.Entity<SubcategoryModel>(entity =>
        {
            entity.ToTable("subcategory");

            entity.Property(p => p.Name)
                .HasMaxLength(50)
                .HasConversion(uppercaseConverter)
                .UseCollation("Latin1_General_CI_AI");

            entity.HasMany(c => c.Products)
                .WithOne(c => c.Subcategory)
                .HasForeignKey(j => j.SubcategoryId);

            entity.HasOne(s => s.Image)
                .WithOne(i => i.Subcategory)
                .HasForeignKey<ImageModel>(i => i.SubcategoryId);
        });

        modelBuilder.Entity<AddressModel>(entity =>
        {
            entity.ToTable("delivery_addresses");

            entity.Property(d => d.City)
                .HasMaxLength(50)
                .HasConversion(uppercaseConverter)
                .UseCollation("Latin1_General_CI_AI");

            // entity.Property(d => d.Street)
            //     .HasMaxLength(50)
            //     .HasConversion(uppercaseConverter)
            //     .UseCollation("Latin1_General_CI_AI");
            //
            // entity.Property(d => d.House)
            //     .HasMaxLength(10)
            //     .HasConversion(uppercaseConverter)
            //     .UseCollation("Latin1_General_CI_AI");
            //
            // entity.Property(d => d.Apartment)
            //     .HasMaxLength(10)
            //     .HasConversion(uppercaseConverter)
            //     .UseCollation("Latin1_General_CI_AI");
            //
            // entity.Property(d => d.PostalCode)
            //     .HasMaxLength(10)
            //     .HasConversion(uppercaseConverter)
            //     .UseCollation("Latin1_General_CI_AI");
            //
            //
            //
            // entity.Property(d => d.Country)
            //     .HasMaxLength(50)
            //     .HasConversion(uppercaseConverter)
            //     .UseCollation("Latin1_General_CI_AI");

            entity.Property(d => d.PostOfficeAddress)
                .HasMaxLength(50)
                .HasConversion(uppercaseConverter)
                .UseCollation("Latin1_General_CI_AI");
        });

        modelBuilder.Entity<ImageModel>(entity =>
        {
            entity.ToTable("images");

            entity.Property(p => p.Path)
                .HasConversion(lowercaseConverter);

            entity.HasOne<UserModel>(i => i.User)
                .WithOne(u => u.Image)
                .HasForeignKey<UserModel>(i => i.ImageId);

            entity.HasOne(s => s.Subcategory)
                .WithOne(i => i.Image)
                .HasForeignKey<SubcategoryModel>(i => i.ImageId);
        });

        if (_environment.IsDevelopment())
            SeedingData(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void SeedingData(ModelBuilder modelBuilder)
    {
        var bogus = new Faker("uk");

        _hashingService.CreatePasswordHash("Hashed_password1", out var passwordSalt, out var passwordHash);
        var sellerId = Guid.NewGuid();
        var sellerCartId = Guid.NewGuid();
        var sellerWishlistId = Guid.NewGuid();
        using var fileReader = File.OpenText("Data/Seeds/categories.json");
        using var jsonReader = new JsonTextReader(fileReader);
        var jsonData = JObject.Load(jsonReader);
        var productIds = new List<Guid>();
        var categories = (JObject)jsonData["categories"];

        foreach (var (categoryName, jToken) in categories)
        {
            var subcategories = (JObject)jToken["subcategories"];

            var categoryId = Guid.NewGuid();
            modelBuilder.Entity<CategoryModel>().HasData(
                new CategoryModel
                {
                    Id = categoryId,
                    Name = categoryName
                }
            );

            foreach (var (subcategoryName, value) in subcategories)
            {
                var subcategoryData = (JObject)value;

                var subcategoryId = Guid.NewGuid();
                var imageSubcategoryId = Guid.NewGuid();

                modelBuilder.Entity<SubcategoryModel>().HasData(
                    new SubcategoryModel
                    {
                        Id = subcategoryId,
                        CategoryId = categoryId,
                        Name = subcategoryName,
                        ImageId = imageSubcategoryId
                    }
                );
                var imageSubcategory = new ImageModel
                {
                    Id = imageSubcategoryId,
                    SubcategoryId = subcategoryId,
                    Path = subcategoryData["image"].ToString().ToLower()
                };

                modelBuilder.Entity<ImageModel>().HasData(imageSubcategory);

                var products = (JArray)subcategoryData["products"];
                foreach (var product in products)
                {
                    var productName = product["name"].ToString();
                    var productId = Guid.NewGuid();
                    productIds.Add(productId);
                    var imageId = Guid.NewGuid();
                    var cartId = Guid.NewGuid();
                    var userId = Guid.NewGuid();
                    var wishlistId = Guid.NewGuid();
                    var wishlistProductsId = Guid.NewGuid();

                    var imageModel = new ImageModel
                    {
                        Id = imageId,
                        ProductId = productId,
                        Path = product["image"].ToString().ToLower()
                    };

                    var productModel = new ProductModel
                    {
                        Id = productId,
                        Name = productName,
                        Description = bogus.Lorem.Paragraph(),
                        Price = bogus.Finance.Amount(100),
                        SellerId = sellerId,
                        Manufacturer = bogus.Company.CompanyName(),
                        SubcategoryId = subcategoryId,
                        Color = bogus.Commerce.Color().Transform(To.TitleCase),
                        Country = bogus.Address.Country(),
                        CreatedAt = DateTime.Now,
                        Quantity = (short)bogus.Random.Int(1, 10),
                        Weight = (short)bogus.Random.Int(1, 10),
                        Width = (short)bogus.Random.Int(1, 10),
                        Height = (short)bogus.Random.Int(1, 10),
                        Length = (short)bogus.Random.Int(1, 25),
                        IsNew = bogus.Random.Bool(),
                        InStock = true,
                        Rating = (short)bogus.Random.Decimal(3, 5),
                        RatingCount = (short)bogus.Random.Int(1, 10)
                    };

                    var wishlistProduct = new WishlistProductsModel
                    {
                        Id = wishlistProductsId,
                        ProductId = productId,
                        WishlistId = wishlistId
                    };

                    var wishlist = new WishlistModel
                    {
                        Id = wishlistId,
                        UserId = userId
                    };

                    modelBuilder.Entity<WishlistProductsModel>().HasData(wishlistProduct);

                    modelBuilder.Entity<WishlistModel>().HasData(wishlist);

                    var slugHelper = new SlugHelper();

                    productModel.Slug = slugHelper.GenerateSlug(productModel.Name + "-" + productModel.Id);
                    modelBuilder.Entity<ProductModel>().HasData(productModel);
                    modelBuilder.Entity<ImageModel>().HasData(imageModel);
                    var role = bogus.Random.Enum<UserRoleEnum>();


                    var user = new UserModel
                    {
                        Id = userId,
                        Username = bogus.Internet.UserName().ToUpper() + bogus.Random.Int(1, 1000),
                        Email = (Guid.NewGuid().ToString()[..8] + "@gmail.com").ToUpper(),
                        Name = bogus.Name.FirstName().ToUpper(),
                        Surname = bogus.Name.LastName().ToUpper(),
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        Role = role,
                        PhoneNumber = bogus.Phone.PhoneNumber("#########"),
                        CartId = cartId,
                        ConfirmationToken = null,
                        WishlistId = wishlistId
                    };
                    user.Confirm();

                    modelBuilder.Entity<UserModel>().HasData(user);

                    modelBuilder.Entity<CommentModel>().HasData(
                        new CommentModel
                        {
                            Id = Guid.NewGuid(),
                            ProductId = productId,
                            UserId = userId,
                            Content = bogus.Lorem.Paragraph(),
                            CreatedAt = DateTime.Now
                        }
                    );

                    modelBuilder.Entity<CartModel>().HasData(
                        new CartModel
                        {
                            Id = cartId,
                            UserId = userId
                        }
                    );

                    modelBuilder.Entity<CartItemModel>().HasData(
                        new CartItemModel
                        {
                            Id = Guid.NewGuid(),
                            CartId = cartId,
                            ProductId = productId,
                            Quantity = (short)bogus.Random.Int(1, 10)
                        }
                    );
                }
            }
        }

        var seller = new UserModel
        {
            Id = sellerId,
            Username = "soundparadisestore",
            Email = "soundparadisestore@gmail.com".ToUpper(),
            Name = "SoundParadise",
            Surname = "Store",
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Role = UserRoleEnum.Admin,
            PhoneNumber = bogus.Phone.PhoneNumber("#########"),
            CartId = sellerCartId,
            ConfirmationToken = null,
            WishlistId = sellerWishlistId
        };
        seller.Confirm();
        modelBuilder.Entity<UserModel>().HasData(seller);
        modelBuilder.Entity<CartModel>().HasData(
            new CartModel
            {
                Id = sellerCartId,
                UserId = sellerId
            }
        );

        modelBuilder.Entity<WishlistProductsModel>().HasData(new WishlistProductsModel
        {
            Id = Guid.NewGuid(),
            ProductId = productIds[0],
            WishlistId = sellerWishlistId
        });

        modelBuilder.Entity<WishlistModel>().HasData(new WishlistModel
        {
            Id = sellerWishlistId,
            UserId = sellerId
        });
    }
}