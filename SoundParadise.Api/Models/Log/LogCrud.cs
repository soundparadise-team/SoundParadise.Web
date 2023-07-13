using SoundParadise.Api.Data;

namespace SoundParadise.Api.Models.Log;

/// <summary>
///     LogModel CRUD.
/// </summary>
public class LogCrud
{
    private readonly SoundParadiseDbContext _context;

    public LogCrud(SoundParadiseDbContext context)
    {
        _context = context;
    }
}