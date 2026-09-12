using System.Security.Cryptography;
using System.Text;

namespace LegendsOfRus.CommercialRuntime
{
    internal static class CommercialRuntimeSourceHash
    {
        public static string Compute(string source)
        {
            string normalized = source ?? string.Empty;
            if (normalized.Length > 0 && normalized[0] == '\uFEFF')
                normalized = normalized.Substring(1);
            normalized = normalized.Replace("\r\n", "\n").Replace("\r", "\n");

            byte[] bytes = new UTF8Encoding(false).GetBytes(normalized);
            byte[] digest;
            using (SHA256 sha256 = SHA256.Create())
                digest = sha256.ComputeHash(bytes);

            var result = new StringBuilder(digest.Length * 2);
            for (int i = 0; i < digest.Length; i++)
                result.Append(digest[i].ToString("x2"));
            return result.ToString();
        }
    }
}
