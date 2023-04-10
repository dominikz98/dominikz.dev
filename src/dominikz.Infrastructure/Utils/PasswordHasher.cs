using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using dominikz.Domain.Options;
using Microsoft.Extensions.Options;

namespace dominikz.Infrastructure.Utils;

public class PasswordHasher
{
    private readonly IOptions<PasswordOptions> _options;
    private static readonly Random Rnd = new();

    public PasswordHasher(IOptions<PasswordOptions> options)
    {
        _options = options;
    }

    public static string Generate(int length = 24)
    {
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string number = "1234567890";
        const string special = "!@#$%^&*_-=+";

        var bytes = new byte[length];
        var res = new StringBuilder();

        foreach (var b in bytes)
        {
            switch (Rnd.Next(4))
            {
                case 0:
                    res.Append(lower[b % lower.Count()]);
                    break;
                case 1:
                    res.Append(upper[b % upper.Count()]);
                    break;
                case 2:
                    res.Append(number[b % number.Count()]);
                    break;
                case 3:
                    res.Append(special[b % special.Count()]);
                    break;
            }
        }

        return res.ToString();
    }

    public string GenerateHashedPassword(int passwordLength = 24)
    {
        var password = Generate(passwordLength);
        var salt = RandomNumberGenerator.GetBytes(_options.Value.SaltLength);
        var hash = GenerateHash(password, salt);
        return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
    }

    public string GenerateHash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(_options.Value.SaltLength);
        var hash = GenerateHash(password, salt);
        return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
    }

    private byte[] GenerateHash(string password, byte[] salt)
    {
        //create an hmac hash of the password using the pepper value as the key

        var pepper = ConvertSpacedHexToByteArray();
        using var hmac = new HMACSHA512(pepper);
        var initialHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        //generate a key value using pbkdf2 that will serve as the password hash
        using var pbkdf2 = new Rfc2898DeriveBytes(initialHash, salt, _options.Value.IterationCount, HashAlgorithmName.SHA1);
        return pbkdf2.GetBytes(_options.Value.KeyLength);
    }

    public bool Validate(string expected, string password)
    {
        var hashParts = expected.Split(':');
        var salt = Convert.FromBase64String(hashParts[0]);
        var hash = Convert.FromBase64String(hashParts[1]);
        var passwordHash = GenerateHash(password, salt);

        var differences = (uint)hash.Length ^ (uint)passwordHash.Length;
        for (var position = 0; position < Math.Min(hash.Length, passwordHash.Length); position++)
            differences |= (uint)(hash[position] ^ passwordHash[position]);

        return differences == 0;
    }

    private byte[] ConvertSpacedHexToByteArray()
    {
        var hexValuesSplit = _options.Value.Pepper.Split(' ');
        var data = new byte[hexValuesSplit.Length];

        for (var index = 0; index < hexValuesSplit.Length; index++)
            data[index] = byte.Parse(hexValuesSplit[index], NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        return data;
    }
}