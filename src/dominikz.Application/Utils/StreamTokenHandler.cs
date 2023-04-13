using dominikz.Domain.Enums.Movies;
using dominikz.Domain.ViewModels.Movies;
using dominikz.Infrastructure.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace dominikz.Application.Utils;

public class StreamTokenHandler
{
    private readonly IMemoryCache _cache;
    private readonly PasswordHasher _hasher;

    public StreamTokenHandler(IMemoryCache cache, PasswordHasher hasher)
    {
        _cache = cache;
        _hasher = hasher;
    }

    public async Task<StreamTokenVm?> Create(StreamTokenPrefix prefix, Guid id, string path, CancellationToken cancellationToken)
    {
        var stream = await _cache.GetOrCreateAsync($"{prefix}_{id}_TOKEN", (info) =>
        {
            info.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(4);
            var hash = _hasher.GenerateHashedPassword();
            var token = new StreamToken(id, Base64UrlEncoder.Encode(hash), path, info.AbsoluteExpiration.Value);
            return Task.FromResult(token);
        });

        if (stream == null)
            return null;

        return new StreamTokenVm()
        {
            Token = stream.Token,
            Expiration = stream.Expiration
        };
    }

    public StreamToken? Verify(StreamTokenPrefix prefix, Guid id, string token)
    {
        if (_cache.TryGetValue<StreamToken>($"{prefix}_{id}_TOKEN", out var result) == false)
            return null;

        if (result!.Token != token)
            return null;

        return result;
    }
}

public record StreamToken(Guid Id, string Token, string FilePath, DateTimeOffset Expiration);
