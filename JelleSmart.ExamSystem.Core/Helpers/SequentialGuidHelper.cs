using System.Security.Cryptography;
using System.Text;

namespace JelleSmart.ExamSystem.Core.Helpers
{
    /// <summary>
    /// Sequential Guid Generator
    /// SQL Server performansı için sıralı Guid üretir
    /// </summary>
    public static class SequentialGuidHelper
    {
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        /// <summary>
        /// Sıralı Guid oluşturur (SQL Server optimizasyonu için)
        /// </summary>
        public static string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// SQL Server için sıralı Guid oluşturur (timestamp ile)
        /// </summary>
        public static string NewSequentialGuid()
        {
            // Byte array oluştur
            byte[] guidBytes = new byte[16];
            byte[] timestampBytes = BitConverter.GetBytes(DateTime.UtcNow.Ticks);

            // Timestamp'i en başa koy (sıralı olması için)
            Array.Copy(timestampBytes, 0, guidBytes, 0, 8);

            // Geriye kalan byte'ları rastgele doldur
            byte[] randomBytes = new byte[8];
            _rng.GetBytes(randomBytes);
            Array.Copy(randomBytes, 0, guidBytes, 8, 8);

            return new Guid(guidBytes).ToString();
        }

        /// <summary>
        /// Belirli bir prefix ile Guid oluşturur (debug için kolay okunabilirlik)
        /// </summary>
        public static string NewPrefixedGuid(string prefix)
        {
            return $"{prefix}{Guid.NewGuid():N}";
        }
    }
}
