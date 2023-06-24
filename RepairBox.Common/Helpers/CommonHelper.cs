using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;

namespace RepairBox.Common.Helpers
{
    public class CommonHelper
    {
        private const int SaltSize = 16; // Choose an appropriate salt size
        private const int HashSize = 32; // Choose an appropriate hash size
        private const int Iterations = 10000; // Choose an appropriate number of iterations

        public static double TotalPagesforPagination(int total, int pageSize)
        {
            double.TryParse(pageSize.ToString(), out double newPageSize);
            double.TryParse(total.ToString(), out double newTotal);

            return Math.Ceiling(newTotal/newPageSize);
        }
        public static (string hash, string salt) GenerateHashAndSalt(string password)
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = GenerateHash(password, salt);

            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }
        private static byte[] GenerateHash(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
        public static bool VerifyPassword(string password, string hash, string salt)
        {
            byte[] hashBytes = Convert.FromBase64String(hash);
            byte[] saltBytes = Convert.FromBase64String(salt);

            byte[] computedHash = GenerateHash(password, saltBytes);

            return SlowEquals(hashBytes, computedHash);
        }

        public static string DecodeJwtToken(string token, dynamic JwtConfig)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = JwtConfig.Issuer,
                ValidAudience = JwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.SecretKey))
            };

            var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

            var claims = claimsPrincipal.Claims;

            var claimDictionary = new Dictionary<string, string>();

            foreach (var claim in claims)
            {
                claimDictionary[claim.Type] = claim.Value;
            }

            return JsonSerializer.Serialize(claimDictionary);
        }
    }
}
