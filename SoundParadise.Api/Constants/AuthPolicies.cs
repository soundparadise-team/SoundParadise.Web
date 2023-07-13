namespace SoundParadise.Api.Constants;

/// <summary>
///     Authorization policies.
/// </summary>
public static class AuthPolicies
{
    /// <summary>
    ///     Admin policy. This is the most privileged policy.
    /// </summary>
    public const string AdminPolicy = "AdminPolicy";

    /// <summary>
    ///     Customer policy. This is the least privileged policy.
    /// </summary>
    public const string CustomerPolicy = "CustomerPolicy";

    /// <summary>
    ///     Seller policy. This is the second most privileged policy.
    /// </summary>
    public const string SellerPolicy = "SellerPolicy";

    public const string AdminOrSellerPolicy = "AdminOrSellerPolicy";

    public const string AdminOrCustomerPolicy = "AdminOrCustomerPolicy";

    public const string SellerOrCustomerPolicy = "SellerOrCustomerPolicy";
}