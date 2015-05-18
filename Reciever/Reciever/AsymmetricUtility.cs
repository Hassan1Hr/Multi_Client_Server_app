using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace client
{
    class AsymmetricUtility
    {
        public void GenerateNewRSAParams(string publicprivatekey, string publicOnlyKey)
        {

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            StreamWriter writer = new StreamWriter(publicprivatekey);
            string publicprivatekeyXML = rsa.ToXmlString(true);
            writer.Write(publicprivatekeyXML);
            writer.Close();

            writer = new StreamWriter(publicOnlyKey);
            string publicOnlyKeyXML = rsa.ToXmlString(false);
            writer.Write(publicOnlyKeyXML);
            writer.Close();

            //return publicprivatekeyXML;

        }

        public byte[] Decrypt(byte[] data, String publicPrivatekey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            StreamReader reader = new StreamReader(publicPrivatekey);
            string publicOnlyKeyXML = reader.ReadToEnd();
            rsa.FromXmlString(publicOnlyKeyXML);
            reader.Close();
            
                return rsa.Decrypt(data, false);
           
        }
    }
}
