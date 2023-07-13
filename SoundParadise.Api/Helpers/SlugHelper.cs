using System.Text;
using SoundParadise.Api.Data;

namespace SoundParadise.Api.Helpers;

/// <summary>
///     Slug helper.
/// </summary>
public static class SlugHelper
{
    private static readonly Slugify.SlugHelper _slugHelper = new();

    /// <summary>
    ///     Check slug is not token.
    /// </summary>
    /// <param name="slug">Slug.</param>
    /// <param name="context">Db context.></param>
    /// <returns>True if correct, false if not</returns>
    private static bool SlugIsNotTaken(string slug, SoundParadiseDbContext context)
    {
        return !context.Products.Any(u => u.Slug == slug);
    }

    /// <summary>
    ///     Trancate Guid.
    /// </summary>
    /// <param name="guid">Guid</param>
    /// <returns></returns>
    private static string TruncateGuid(Guid guid)
    {
        return guid.ToString().Substring(0, 8);
    }

    /// <summary>
    ///     Generate Slug.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="context">Db context.</param>
    /// <returns></returns>
    public static string GenerateSlug(string name, SoundParadiseDbContext context)
    {
        var slug = _slugHelper.GenerateSlug(name);

        if (SlugIsNotTaken(slug, context))
            return slug;

        var modifiedSlugBuilder = new StringBuilder(slug);
        do
        {
            modifiedSlugBuilder.Append('-').Append(TruncateGuid(Guid.NewGuid()));
        } while (!SlugIsNotTaken(modifiedSlugBuilder.ToString(), context));

        return modifiedSlugBuilder.ToString();
    }
}