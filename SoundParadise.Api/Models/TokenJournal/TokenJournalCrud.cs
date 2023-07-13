using SoundParadise.Api.Data;

namespace SoundParadise.Api.Models.TokenJournal;

/// <summary>
///     TokenJournal CRUD.
/// </summary>
public class TokenJournalCrud
{
    private readonly SoundParadiseDbContext _context;

    public TokenJournalCrud(SoundParadiseDbContext context)
    {
        _context = context;
    }
}