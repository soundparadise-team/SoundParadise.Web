using Microsoft.AspNetCore.Mvc;

namespace SoundParadise.Api.Controllers.Api;

/// <summary>
///     Base Route Attribute
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class BaseRouteAttribute : RouteAttribute
{
    /// <summary>
    ///     Base Route Attribute constructor
    /// </summary>
    /// <param name="template"></param>
    public BaseRouteAttribute(string template)
        : base("api/v{version:apiVersion}/" + template)
    {
    }
}