using System.Security.Cryptography;

namespace Service_Center.Security
{
    public static class PasswordHasher
    {
        private const int Iterations = 100_000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA512, HashSize);
            return $"PBKDF2-SHA512${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string password, string storedValue)
        {
            if (string.IsNullOrWhiteSpace(storedValue))
            {
                return false;
            }

            string[] parts = storedValue.Split('$');
            if (parts.Length != 4 || !parts[0].Equals("PBKDF2-SHA512", StringComparison.Ordinal))
            {
                return string.Equals(password, storedValue, StringComparison.Ordinal);
            }

            if (!int.TryParse(parts[1], out int iterations))
            {
                return false;
            }

            try
            {
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] expectedHash = Convert.FromBase64String(parts[3]);
                byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA512, expectedHash.Length);
                return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool NeedsRehash(string storedValue)
        {
            return !storedValue.StartsWith("PBKDF2-SHA512$", StringComparison.Ordinal);
        }
    }
}
