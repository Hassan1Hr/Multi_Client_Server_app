using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace client
{
    class SymmetricUtility
    {

        public byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (Aes algorithm = Aes.Create())
            {
                using (ICryptoTransform decryptor = algorithm.CreateDecryptor(key, iv))
                {
                    return Crypt(data, decryptor);
                }
            }
        }

        private byte[] Crypt(byte[] data, ICryptoTransform cryptor)
        {
            MemoryStream m = new MemoryStream();
            using (CryptoStream c = new CryptoStream(m, cryptor, CryptoStreamMode.Write))
            {
                c.Write(data, 0, data.Length);
                c.FlushFinalBlock();
            }
            return m.ToArray();
        }
    }
}
