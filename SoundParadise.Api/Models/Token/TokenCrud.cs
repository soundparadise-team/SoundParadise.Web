using SoundParadise.Api.Data;

namespace SoundParadise.Api.Models.Token;

/// <summary>
///     Token CRUD.
/// </summary>
public class TokenCrud
{
    private readonly SoundParadiseDbContext _context;

    public TokenCrud(SoundParadiseDbContext context)
    {
        _context = context;
    }
}