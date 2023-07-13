using System.Net;
using System.Reflection;
using System.Text;
using CloudIpspSDK;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using SoundParadise.Api.Configuration;
using SoundParadise.Api.Constants;
using SoundParadise.Api.Data;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Middlewares;
using SoundParadise.Api.Models.Card;
using SoundParadise.Api.Models.Cart;
using SoundParadise.Api.Models.CartItem;
using SoundParadise.Api.Models.Category;
using SoundParadise.Api.Models.Comment;
using SoundParadise.Api.Models.Enums;
using SoundParadise.Api.Models.Order;
using SoundParadise.Api.Models.Product;
using SoundParadise.Api.Models.User;
using SoundParadise.Api.Models.Wishlist;
using SoundParadise.Api.Options;
using SoundParadise.Api.Services;
using SoundParadise.Api.Validators.Card;
using SoundParadise.Api.Validators.CartItem;
using SoundParadise.Api.Validators.User;
using SoundParadise.Web.Models.Address;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
IdentityModelEventSource.ShowPII = true;
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

var configuration = AppConfiguration.LoadConfiguration(builder.Environment.IsDevelopment());
var connectionString = configuration.ConnectionString;
var secretKey = configuration.SecretKey;
var identityServerAuthority = configuration.IdentityServerAuthority;
var encryption = configuration.EncryptionOptions;
var jwtBearer = configuration.JwtBearerOptions;
var blob = configuration.BlobOptions;
var smtp = configuration.SmtpOptions;
var urls = configuration.UrlsOptions;
var fondy = configuration.FondyConfig;

//Fondy

Config.MerchantId = int.Parse(fondy["MerchantId"]!);
Config.SecretKey = fondy["SecretKey"];

GlobalDiagnosticsContext.Set(NLogVariables.DatabaseConnectionString, connectionString);
GlobalDiagnosticsContext.Set(NLogVariables.DatabaseName, Assembly.GetEntryAssembly()?.GetName().Name);

builder.Services.AddControllers();

builder.Services.AddDbContext<SoundParadiseDbContext>(options =>
{
    options.UseSqlServer(connectionString);

    if (builder.Environment.IsDevelopment()) options.EnableSensitiveDataLogging();
});

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    // options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
    options.SerializerSettings.ContractResolver = new DefaultContractResolver
    {
        NamingStrategy = new CamelCaseNamingStrategy()
    };
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    options.SerializerSettings.Formatting = Formatting.Indented;
});

#region Crud services

builder.Services
    .AddScoped<IUserCrud, UserCrud>()
    .AddScoped<IProductCrud, ProductCrud>()
    .AddScoped<ICommentCrud, CommentCrud>()
    .AddScoped<ICartItemCrud, CartItemCrud>()
    .AddScoped<ICartCrud, CartCrud>()
    .AddScoped<ICategoryCrud, CategoryCrud>()
    .AddScoped<IOrderCrud, OrderCrud>()
    .AddScoped<IWishlistCrud, WishlistCrud>()
    .AddScoped<IAddressCrud, AddressCrud>()
    .AddScoped<ICardCrud, CardCrud>();

#endregion

#region Services

//Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // options.Cookie.Name = "ARRAffinity";
    // options.Cookie.Domain = urls["Server"];
    // options.Cookie.HttpOnly = true;
    // options.Cookie.IsEssential = true;
    // options.Cookie.SameSite = SameSiteMode.Strict;
    // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    // options.IdleTimeout = TimeSpan.FromMinutes(60); 
    options.Cookie.Name = ".AspNetCore.Session";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

//Versioning
builder.Services
    .AddApiVersioning(opt =>
    {
        opt.DefaultApiVersion = new ApiVersion(1, 0);
        opt.AssumeDefaultVersionWhenUnspecified = true;
        opt.ReportApiVersions = true;
        opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader(ApiVersions.ApiHeader),
            new MediaTypeApiVersionReader(ApiVersions.ApiHeader));
    })
    .AddVersionedApiExplorer(setup =>
    {
        setup.GroupNameFormat = "'v'VVV";
        setup.SubstituteApiVersionInUrl = true;
    });

//Swagger
builder.Services
    .AddSwaggerGen()
    .AddSwaggerGenNewtonsoftSupport()
    .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

//Options

builder.Services.Configure<EncryptionOptions>(encryption);
builder.Services.Configure<TokenOptions>(jwtBearer);
builder.Services.Configure<BlobOptions>(blob);
builder.Services.Configure<SmtpOptions>(smtp);
builder.Services.Configure<UrlsOptions>(urls);

//Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy(myAllowSpecificOrigins,
        policy =>
        {
            policy.AllowCredentials()
                .WithOrigins(urls["Client"], urls["Server"], "https://soundparadise.store")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(_ => true)
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .WithExposedHeaders("Set-Cookie");
        });
});

//Validation
builder.Services
    .AddScoped<UserAuthenticationDtoValidator>()
    .AddScoped<UserCreateDtoValidator>()
    .AddScoped<CartItemDtoValidator>()
    .AddScoped<CardDtoValidator>();

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

builder.Services
    .AddScoped<HashingService>()
    .AddScoped<CardEncryptionService>()
    .AddScoped<TokenService>()
    .AddScoped(typeof(ILoggingService<>), typeof(LoggingService<>))
    .AddScoped<SmtpService>()
    .AddScoped<ImageService>()
    .AddScoped<IPaymentService, PaymentService>();

#endregion

#region IdentityServer

builder.Services.AddIdentityServer()
    .AddInMemoryClients(configuration.AuthenticationClients)
    .AddInMemoryIdentityResources(configuration.AuthenticationIdentityResources)
    .AddInMemoryApiResources(configuration.AuthenticationApiResources)
    .AddInMemoryApiScopes(configuration.AuthenticationApiScopes)
    .AddDeveloperSigningCredential();

#endregion

#region Authentication & Authorization

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthPolicies.AdminPolicy, policy =>
        policy.RequireRole(UserRoleEnum.Admin.ToString()));

    options.AddPolicy(AuthPolicies.CustomerPolicy, policy =>
        policy.RequireRole(UserRoleEnum.Customer.ToString()));

    options.AddPolicy(AuthPolicies.SellerPolicy, policy =>
        policy.RequireRole(UserRoleEnum.Seller.ToString()));
});

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.Authority = identityServerAuthority;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

#endregion

var app = builder.Build();
app.UseMiddleware<TokenExpirationMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
    app.UseHsts();
app.UseCors(myAllowSpecificOrigins);
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseIdentityServer();
app.UseSession();

if (app.Environment.IsDevelopment())
{
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToLowerInvariant());
    });
}

app.MapControllerRoute(
    "default",
    "{controller}/{action=Index}/{id?}");

app.Run();