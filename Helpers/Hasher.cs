using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace GLS_BlazorMVC_PoC.Helpers
{
    /// <summary>
    /// Basic hashing API created by Eng.AAM for hashing passwords and comparisions.
    /// </summary>
    public class Hasher
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        /// <summary>
        /// Hashes a given password using Rfc2898 method.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <returns>New hash from the password.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        /// <summary>
        /// Check if two passwords are equal, by hashing the given password with the old hashed password.
        /// </summary>
        /// <param name="hashedPassword">Old hashed password, stored in your Database.</param>
        /// <param name="password">The given password to compare after hashing with old one.</param>
        /// <returns>True if both old hash and new hash are equal.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return ByteArraysEqual(buffer3, buffer4);
        }

        /// <summary>
        /// Compare two bytes arrays for equality.
        /// </summary>
        /// <param name="b1">First array to compare.</param>
        /// <param name="b2">Second array to compare</param>
        /// <returns>True if both arrays are equal.</returns>
        static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            // Validate buffers are the same length.
            // This also ensures that the count does not exceed the length of either buffer.  
            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }

    }
}
