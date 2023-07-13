namespace SoundParadise.Api.Configuration;

public class AppConfiguration
{
    public string ConnectionString { get; set; }
    public string SecretKey { get; set; }
    public string IdentityServerAuthority { get; set; }

    public IConfigurationSection AuthenticationClients { get; set; }

    public IConfigurationSection AuthenticationIdentityResources { get; set; }

    public IConfigurationSection AuthenticationApiResources { get; set; }

    public IConfigurationSection AuthenticationApiScopes { get; set; }

    public IConfigurationSection EncryptionOptions { get; set; }

    public IConfigurationSection JwtBearerOptions { get; set; }

    public IConfigurationSection BlobOptions { get; set; }

    public IConfigurationSection SmtpOptions { get; set; }

    public IConfigurationSection UrlsOptions { get; set; }

    public IConfigurationSection FondyConfig { get; set; }

    public static AppConfiguration LoadConfiguration(bool isDevelopment)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(!isDevelopment
                ? "appsettings.json"
                : "appsettings.Development.json")
            .AddEnvironmentVariables()
            .Build();

        var appConfiguration = new AppConfiguration
        {
            ConnectionString = configuration.GetConnectionString("SoundParadiseDb"),
            SecretKey = configuration["JwtBearer:SecretKey"],
            IdentityServerAuthority = configuration["Authentication:Authority"],
            AuthenticationClients = configuration.GetSection("Authentication:Clients"),
            AuthenticationIdentityResources = configuration.GetSection("Authentication:IdentityResources"),
            AuthenticationApiResources = configuration.GetSection("Authentication:ApiResources"),
            AuthenticationApiScopes = configuration.GetSection("Authentication:ApiScopes"),
            JwtBearerOptions = configuration.GetSection("JwtBearer"),
            EncryptionOptions = configuration.GetSection("Encryption"),
            BlobOptions = configuration.GetSection("AzureBlob"),
            SmtpOptions = configuration.GetSection("Smtp"),
            UrlsOptions = configuration.GetSection("Urls"),
            FondyConfig = configuration.GetSection("Fondy")
        };
        return appConfiguration;
    }
}