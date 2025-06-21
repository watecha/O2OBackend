using System.Security.Cryptography;
using System.Text;

namespace O2OBackend.Shared.Utilities
{
    public static class PasswordHasher
    {
        // 臨時的簡化雜湊方法，用於編譯通過
        // !!! DO NOT USE IN PRODUCTION !!!
        // Will be replaced by stronger hashing in Authentication module
        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // 臨時的驗證方法
        public static bool VerifyPassword(string password, string storedHash)
        {
            string hashOfInput = HashPassword(password);
            return hashOfInput == storedHash;
        }
    }
}