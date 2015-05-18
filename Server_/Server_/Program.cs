using System;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Management.Instrumentation;
using System.Threading;


namespace Server_
{
    class Program
    {

        public static TcpListener server;

        public static int BUFFER_SIZE = 100;
        static int RequestsServed = 0;
       public static Socket client;
        public static void Main()
        {

            try
            {
                IPAddress ipAd = IPAddress.Parse("127.0.0.1");
               
                var threadList = new List<Thread>();
               
                server = new TcpListener(ipAd, 8002);
                Console.WriteLine("wait connection");
                server.Start();
                while (true)
                {
                    for (int x = 0;true; ++x)
                    {
                         client= server.AcceptSocket();
                        Console.WriteLine("Accepted ");
                       // threadList.Add( new Thread(TreatRequestThr));
                        System.Threading.ThreadPool.QueueUserWorkItem(
                         new System.Threading.WaitCallback(TreatRequestThr));
                        //threadList[1].Name = "Request " + ++RequestsServed;
                       // threadList[1].Start(client);

                    }
                   
                }

                Console.ReadKey();

                server.Stop();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }
        public static void TreatRequestThr(object tcp_client)
        {
            tcp_client = (Socket)client;
            TreatRequest((Socket)tcp_client);
        }
        public static void TreatRequest(Socket client)
        {

            RandomNumberGenerator rand = RandomNumberGenerator.Create();
            FileStream dataFile = File.OpenRead("data.txt");
            BufferedStream read = new BufferedStream(dataFile);
            int numOFBlock = 0;
            byte[] key = new byte[16];
            byte[] iv = new byte[16];
            rand.GetBytes(key);
            rand.GetBytes(iv);
          
           
           
           // int datalengh = byteData.Length;
           
            byte[] b = new byte[243];
            client.Receive(b);
            String publicKey = Encoding.ASCII.GetString(b);
            client.Send(AsymetryEncrypt(key, publicKey));
            //Console.WriteLine((AsymetryEncrypt(key, publicKey)).Length);
            client.Send(AsymetryEncrypt(iv, publicKey));
          //  Console.WriteLine(read.Length);
            if ((read.Length-1) < 64)
            { numOFBlock = 1; }
            else if ((read.Length-1) % 64 == 0)

            { numOFBlock = (int)((read.Length) / 64); }

            else
            { numOFBlock = (int)(((read.Length-1) / 64) + 1); }
          //  Console.WriteLine(numOFBlock);
            client.Send(BitConverter.GetBytes(numOFBlock));
            for (int n = 0; n < numOFBlock; n++)
            {
                byte[] byteData = new byte[64];
                read.Read(byteData, 0, byteData.Length);
                //Console.WriteLine(Encoding.ASCII.GetString(byteData));
               // Console.WriteLine(Encoding.ASCII.GetString(byteData).Length);
                //Console.WriteLine(Encoding.ASCII.GetString(symetryEncrypt(byteData, key, iv)));
             ///   Console.WriteLine(Encoding.ASCII.GetString(symetryEncrypt(byteData, key, iv)).Length);
                client.Send(symetryEncrypt(byteData, key, iv));
            }
            Console.WriteLine("Recieved...");
           
            //client.Send(Encoding.ASCII.GetString(symetryEncrypt(byteData, key, iv)).Length);
          
           
           // Console.WriteLine(Encoding.ASCII.GetString(symetryEncrypt(byteData, key, iv)));
           // Console.WriteLine(symetryEncrypt(byteData, key, iv).Length);
            
           //  Console.WriteLine((AsymetryEncrypt(key, publicKey)).Length);
            Console.WriteLine("finished");

        }
        public static byte[] symetryEncrypt(byte[] data, byte[] key, byte[] iv)
        {
            SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create("AES");
            algorithm.Key = key;
            algorithm.IV = iv;
            ICryptoTransform enc = algorithm.CreateEncryptor();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }
        public static byte[] AsymetryEncrypt(byte[] data, String publicKey)
        {
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
           // Console.WriteLine(publicKey);
            rsaProvider.FromXmlString(publicKey);
            return rsaProvider.Encrypt(data, false);
        }
    }
}
