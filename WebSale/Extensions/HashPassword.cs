using System.Security.Cryptography;
using System.Text;

namespace WebSale.Extensions
{
    public static class HashPassword
    {
        public static string HashPass(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }
    }
}
