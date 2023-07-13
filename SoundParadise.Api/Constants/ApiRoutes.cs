namespace SoundParadise.Api.Constants;

public static class ApiRoutes
{
    public static class Category
    {
        public const string Base = "categories";

        public const string GetSubcategoriesOfCategory = "get-subcategories-of-category";

        public const string GetSubcategoryName = "get-subcategory-name";

        public const string GetCategoryName = "get-category-name";
    }

    public static class Cart
    {
        public const string Base = "carts";

        public const string GetCart = "get-cart";
        public const string GetCartAuth = Auth + "/get-cart";

        public const string AddToCart = "add-to-cart";
        public const string AddToCartAuth = Auth + "/add-to-cart";

        public const string UpdateCartItemQuantity = "update-cart-item-quantity";
        public const string UpdateCartItemQuantityAuth = Auth + "/update-cart-item-quantity";

        public const string RemoveFromCart = "remove-from-cart";
        public const string RemoveFromCartAuth = Auth + "/remove-from-cart";
    }

    public static class Order
    {
        public const string Base = "orders";

        public const string GetOrder = "get-order";

        public const string PostOrder = "post-order";

        public const string PostOrderAuth = Auth + "/post-order";

        public const string ConfirmOrder = "confirm-order";
    }

    public static class Product
    {
        public const string Base = "products";

        public const string SearchSuggestions = "search-suggestions";
        public const string SearchProducts = "search-products";
        public const string GetBySlug = "get-by-slug";
        public const string GetByCategoryName = "get-by-category-name";
        public const string GetByCategoryId = "get-by-category-id";
        public const string GetBySubcategoryName = "get-by-subcategory-name";
        public const string GetBySubcategoryId = "get-by-subcategory-id";

        public const string GetRecentBuys = "get-recent-buys";

        public const string GetPopular = "get-popular";

        public const string GetSpecialOffers = "get-special-offers";

        public const string UploadImage = "upload-image";

        public const string DeleteImage = "delete-image";
    }

    public static class User
    {
        public const string Base = "users";

        public const string SecureEndpoint = "secure-endpoint";

        public const string GetCurrentUser = "get-current-user";

        public const string ConfirmUser = "confirm-user";
        public const string RegisterUser = "register-user";
        public const string LoginUser = "login-user";
        public const string LogoutUser = "logout-user";
        public const string UpdateUser = "update-user";

        public const string UploadAvatar = "upload-avatar";

        public const string DeleteUser = "delete-user";
    }

    public static class Wishlist
    {
        public const string Base = "wishlists";

        public const string GetWishlist = "get-wishlist";

        public const string AddToWishlist = "add-product-to-wishlist";
        public const string RemoveFromWishlist = "remove-product-from-wishlist";
    }

    public static class Card
    {
        public const string Base = "cards";

        public const string GetCard = "get-card";

        public const string AddCard = "add-card";

        public const string UpdateCard = "update-card";

        public const string DeleteCard = "delete-card";
    }

    public static class Address
    {
        public const string Base = "addresses";

        public const string GetAddress = "get-address";

        public const string GetAddressByOption = GetAddress + "-" + "by-option";

        public const string AddAddress = "add-address";

        public const string UpdateAddress = "update-address";

        public const string DeleteAddress = "delete-address";
    }

    #region General

    public const string GetAll = "get-all";
    public const string GetById = "get-by-id";
    public const string Delete = "delete";
    private const string Auth = "auth";

    #endregion
}