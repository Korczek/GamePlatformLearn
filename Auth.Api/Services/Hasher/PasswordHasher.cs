using System.Security.Cryptography;

namespace Auth.Services.Hasher;

public static class PasswordHasher
{
    public static void Create(string password, out byte[] hash, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(16);
        hash = Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: salt,
            100_000,
            HashAlgorithmName.SHA256,
            32
        );
    }

    public static bool Verify(string password, byte[] hash, byte[] salt)
    {
        var newHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32
        );

        return CryptographicOperations.FixedTimeEquals(hash, newHash);
    }
}