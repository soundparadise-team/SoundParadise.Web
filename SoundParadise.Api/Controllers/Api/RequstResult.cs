using System.Net;

namespace SoundParadise.Api.Controllers.Api;

/// <summary>
///     Request result
/// </summary>
public class RequestResult
{
    protected RequestResult(bool isSuccess, string message, HttpStatusCode httpStatus)
    {
        IsSuccess = isSuccess;
        Message = message;
        HttpStatus = httpStatus;
    }

    public bool IsSuccess { get; set; }
    public string Message { get; set; }

    public HttpStatusCode HttpStatus { get; set; }

    public static RequestResult Success(string message = "", HttpStatusCode httpStatus = HttpStatusCode.OK)
    {
        return new RequestResult(true, message, httpStatus);
    }

    public static RequestResult Error(string message = "", HttpStatusCode httpStatus = HttpStatusCode.BadRequest)
    {
        return new RequestResult(false, message, httpStatus);
    }
}