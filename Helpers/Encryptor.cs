using System.Security.Cryptography;
using System.Text;

namespace Helpers.Helpers
{
    /// <summary>
    /// Basic Encryption/Decryption API created by Eng.AAM for Encrypting passwords and Decrypting them.
    /// </summary>
    public class Encryptor
    {
        

        /// <summary>
        /// Encrypt a given string using Rfc2898.
        /// </summary>
        /// <param name="encryptString">The password to be encrypted.</param>
        /// <returns>Encrypted string.</returns>
        public static string Encrypt(string encryptString, string encryptKey)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptKey, new byte[] {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        /// <summary>
        /// Decrypt a given string using Rfc2898.
        /// </summary>
        /// <param name="cipherText">Encrypted password.</param>
        /// <returns>Original string.</returns>
        public static string Decrypt(string cipherText, string decryptKey)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(decryptKey, new byte[] {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        /// <summary>
        /// Encrypt a given file contents in bytes using AES algorithnm, and convert it to a .enc file.
        /// </summary>
        /// <param name="file">The FileInfo object to be read and encrypted.</param>
        /// <param name="encryptKey">The encryption key to use in the encryption.</param>
        public void EncryptFile(FileInfo file, string encryptKey)
        {
            // Declare CspParmeters and RsaCryptoServiceProvider objects.
            CspParameters _cspp = new CspParameters();
            RSACryptoServiceProvider _rsa;

            // Stores a key pair in the key container.
            _cspp.KeyContainerName = encryptKey;
            _rsa = new RSACryptoServiceProvider(_cspp)
            {
                PersistKeyInCsp = true
            };

            // Create instance of Aes for
            // symmetric encryption of the data.
            Aes aes = Aes.Create();
            ICryptoTransform transform = aes.CreateEncryptor();

            // Use RSACryptoServiceProvider to
            // encrypt the AES key.
            // rsa is previously instantiated:
            //    rsa = new RSACryptoServiceProvider(cspp);
            byte[] keyEncrypted = _rsa.Encrypt(aes.Key, false);

            // Create byte arrays to contain
            // the length values of the key and IV.
            int lKey = keyEncrypted.Length;
            byte[] LenK = BitConverter.GetBytes(lKey);
            int lIV = aes.IV.Length;
            byte[] LenIV = BitConverter.GetBytes(lIV);


            // Write the following to the FileStream
            // - length of the key
            // - length of the IV
            // - ecrypted key
            // - the IV
            // - the encrypted cipher content

            // Output file will be on same directory with same name and old extension + enc extension.
            string outFile = file.FullName + ".enc";

            using (var outFs = new FileStream(outFile, FileMode.Create))
            {
                outFs.Write(LenK, 0, 4);
                outFs.Write(LenIV, 0, 4);
                outFs.Write(keyEncrypted, 0, lKey);
                outFs.Write(aes.IV, 0, lIV);

                // Now write the cipher text using
                // a CryptoStream for encrypting.
                using (var outStreamEncrypted =
                    new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                {
                    // By encrypting a chunk at
                    // a time, you can save memory
                    // and accommodate large files.
                    int count = 0;
                    int offset = 0;

                    // blockSizeBytes can be any arbitrary size.
                    int blockSizeBytes = aes.BlockSize / 8;
                    byte[] data = new byte[blockSizeBytes];
                    int bytesRead = 0;


                    using (var inFs = new FileStream(file.FullName, FileMode.Open))
                    {
                        do
                        {
                            count = inFs.Read(data, 0, blockSizeBytes);
                            offset += count;
                            outStreamEncrypted.Write(data, 0, count);
                            bytesRead += blockSizeBytes;
                        } while (count > 0);
                    }


                    outStreamEncrypted.FlushFinalBlock();
                }
            }
        }

        /// <summary>
        /// Decrypt a given file contents in bytes using AES algorithnm, and convert it to it's original file format.
        /// </summary>
        /// <param name="file">The FileInfo object to be read and decrypted.</param>
        /// <param name="decryptKey">The encryption key to use in the decryption.</param>
        /// <returns>Returns 1 if successful, -1 if key is wrong, Exception otherwise.</returns>
        public int DecryptFile(FileInfo file, string decryptKey)
        {
            // Create instance of Aes for
            // symmetric decryption of the data.
            Aes aes = Aes.Create();
            aes.Padding = PaddingMode.None;

            // Create byte arrays to get the length of
            // the encrypted key and IV.
            // These values were stored as 4 bytes each
            // at the beginning of the encrypted package.
            byte[] LenK = new byte[4];
            byte[] LenIV = new byte[4];


            // Use FileStream objects to read the encrypted
            // file (inFs) and save the decrypted file (outFs).
            using (var inFs = new FileStream(file.FullName, FileMode.Open))
            {


                inFs.Seek(0, SeekOrigin.Begin);
                inFs.Read(LenK, 0, 3);
                inFs.Seek(4, SeekOrigin.Begin);
                inFs.Read(LenIV, 0, 3);

                // Convert the lengths to integer values.
                int lenK = BitConverter.ToInt32(LenK, 0);
                int lenIV = BitConverter.ToInt32(LenIV, 0);

                // Determine the start postition of
                // the ciphter text (startC)
                // and its length(lenC).
                int startC = lenK + lenIV + 8;
                int lenC = (int)inFs.Length - startC;

                // Create the byte arrays for
                // the encrypted Aes key,
                // the IV, and the cipher text.
                byte[] KeyEncrypted = new byte[lenK];
                byte[] IV = new byte[lenIV];

                // Extract the key and IV
                // starting from index 8
                // after the length values.
                inFs.Seek(8, SeekOrigin.Begin);
                inFs.Read(KeyEncrypted, 0, lenK);
                inFs.Seek(8 + lenK, SeekOrigin.Begin);
                inFs.Read(IV, 0, lenIV);

                // Declare CspParmeters and RsaCryptoServiceProvider objects.
                CspParameters _cspp = new CspParameters();
                RSACryptoServiceProvider _rsa;

                // Stores a key pair in the key container.
                _cspp.KeyContainerName = decryptKey;
                _rsa = new RSACryptoServiceProvider(_cspp)
                {
                    PersistKeyInCsp = true
                };

                // Use RSACryptoServiceProvider
                // to decrypt the AES key.
                byte[] KeyDecrypted = new byte[0];
                try
                {
                    KeyDecrypted = _rsa.Decrypt(KeyEncrypted, false);
                }
                catch (Exception)
                {
                    // Provided key is incorrect
                    return -1;
                }

                // Decrypt the key.
                ICryptoTransform transform = aes.CreateDecryptor(KeyDecrypted, IV);


                // Decrypt the cipher text from
                // from the FileSteam of the encrypted
                // file (inFs) into the FileStream
                // for the decrypted file (outFs).
                using (var outFs = new FileStream(file.FullName.Replace(".dbem", ""), FileMode.Create))
                {
                    int count = 0;
                    int offset = 0;

                    // blockSizeBytes can be any arbitrary size.
                    int blockSizeBytes = aes.BlockSize / 8;
                    byte[] data = new byte[blockSizeBytes];

                    // By decrypting a chunk a time,
                    // you can save memory and
                    // accommodate large files.

                    // Start at the beginning
                    // of the cipher text.
                    inFs.Seek(startC, SeekOrigin.Begin);


                    using (var outStreamDecrypted =
                        new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                    {
                        do
                        {
                            count = inFs.Read(data, 0, blockSizeBytes);
                            outStreamDecrypted.Write(data, 0, count);
                            offset += count;
                        } while (count > 0);


                        outStreamDecrypted.FlushFinalBlock();
                    }
                }

            }

            // Success.
            return 1;
        }

    }
}
