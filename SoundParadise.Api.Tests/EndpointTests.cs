using System.Net;
using System.Text;
using System.Text.Json;
using SoundParadise.Api.Constants;

namespace SoundParadise.Api.Tests;

public class EndpointTests
{
    private readonly HttpClient _client;
    private readonly string ApiUrl;

    public EndpointTests()
    {
        _client = new HttpClient();
        ApiUrl = $"https://localhost:7010/api/v{ApiVersions.V1}/{{0}}/{{1}}";
    }

    #region PRODUCT_ENDPOINTS

    [Fact]
    public async Task GetByIdProduct_Endpoint_ReturnOk()
    {
        var registerUserRoute = string.Format(ApiUrl, ApiRoutes.Product.Base, ApiRoutes.GetById);
        var request = new HttpRequestMessage(HttpMethod.Get, registerUserRoute);
        request.Content = new StringContent(JsonSerializer.Serialize(new
        {
            productId = "4375125c-cbca-4dca-ad0f-01c5cc9811c5"
        }), Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region USER_ENDPOINTS

    [Fact]
    public async Task RegisterUser_Endpoint_ReturnsBadRequest()
    {
        var registerUserRoute = string.Format(ApiUrl, ApiRoutes.User.Base, ApiRoutes.User.RegisterUser);
        var request = new HttpRequestMessage(HttpMethod.Post, registerUserRoute);
        request.Content = new StringContent(JsonSerializer.Serialize(new
        {
            password = "string",
            id = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            name = "string",
            surname = "string",
            username = "string",
            email = "string",
            phoneNumber = "string",
            role = 0
        }), Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LoginUser_Endpoint_ReturnOk()
    {
        var registerUserRoute = string.Format(ApiUrl, ApiRoutes.User.Base, ApiRoutes.User.LoginUser);
        var request = new HttpRequestMessage(HttpMethod.Post, registerUserRoute);
        request.Content = new StringContent(JsonSerializer.Serialize(new
        {
            email = "VLXDTIMCHENKO@GMAIL.COM",
            password = "Hashed_password1"
        }), Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task LoqOutUser_Endpoint_ReturnOk()
    {
        var registerUserRoute = string.Format(ApiUrl, ApiRoutes.User.Base, ApiRoutes.User.LogoutUser);
        var request = new HttpRequestMessage(HttpMethod.Post, registerUserRoute);
        request.Content = new StringContent(JsonSerializer.Serialize(new
        {
        }), Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_Endpoint_ReturnBadRequest()
    {
        var registerUserRoute = string.Format(ApiUrl, ApiRoutes.User.Base, ApiRoutes.Delete);
        var request = new HttpRequestMessage(HttpMethod.Delete, registerUserRoute);
        request.Content = new StringContent(JsonSerializer.Serialize(new
        {
            userId = "04e7a301-05ef-4b09-858d-876536f63739"
        }), Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion
}