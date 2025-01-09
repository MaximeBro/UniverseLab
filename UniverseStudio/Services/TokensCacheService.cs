using UniverseStudio.Models;
using Timer = System.Timers.Timer;

namespace UniverseStudio.Services;

public class TokensCacheService : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
    private readonly Timer _expireTimer = new Timer(TimeSpan.FromMinutes(5));
    public Dictionary<string, UserTokenModel> Tokens { get; private set; } = [];

    public TokensCacheService()
    {
        _expireTimer.Elapsed += async (_, _) => await CheckSessionsAsync();
    }

    public bool AddToken(string internalId, UserTokenModel token) => Tokens.TryAdd(internalId, token);
    public UserTokenModel? RemoveToken(string internalId)
    {
        if (Tokens.Remove(internalId, out var token))
        {
            return token;
        }

        return null;
    }


    private async Task CheckSessionsAsync()
    {
        await _semaphore.WaitAsync();
        
        List<string> oldTokens = [];
        Parallel.ForEach(Tokens, (token, _) =>
        {
            if (DateTime.UtcNow - token.Value.LastAuthenticatedAt >= token.Value.MaxSessionDuration)
            {
                oldTokens.Add(token.Key);
            }
        });
        
        foreach (var token in oldTokens)
        {
            Tokens.Remove(token);
        }

        _semaphore.Release();
    }

    public void Dispose()
    {
        _expireTimer.Dispose();
        _semaphore.Dispose();
        GC.SuppressFinalize(this);
    }
}