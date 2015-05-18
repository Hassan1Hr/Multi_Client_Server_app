using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Net.Sockets;
namespace client
{
    public partial class Form1 : Form
    {
        Stream stm;
        TcpClient tcpclnt = new TcpClient();
        //-----------
        private SymmetricUtility sym = null;
        private byte[] dataLength = new byte[4];
        private byte[] publicKey = null;
        private AsymmetricUtility asy = null;
        private byte[] data1;
        private byte[] data2;
        private byte[] key = new byte[128];
            
        private byte[] iv = new byte[128];
        private byte[] decryptedKey = null;
        private byte[] decryptedIv = null;
        private byte[] decryptedData1 = null;
        private byte[] decryptedData2 = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            asy = new AsymmetricUtility();
            asy.GenerateNewRSAParams(@"E://publicAndPrivate.xml",@"E://public.xml");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tcpclnt.Connect("127.0.0.1", 8002);
            stm = tcpclnt.GetStream();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            StreamReader reader = new StreamReader(@"E://public.xml");
            string publicOnlyKeyXML = reader.ReadToEnd();
           // MessageBox.Show(publicOnlyKeyXML.Length+"");
            rsa.FromXmlString(publicOnlyKeyXML);
            reader.Close();

            publicKey = Encoding.ASCII.GetBytes(publicOnlyKeyXML);
            stm.Write(publicKey, 0, publicKey.Length);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            sym = new SymmetricUtility();
            asy = new AsymmetricUtility();
            stm.Read(key, 0, 128);
            stm.Read(iv, 0, 128);
            decryptedKey = asy.Decrypt(key, @"E://publicAndPrivate.xml");
            decryptedIv = asy.Decrypt(iv, @"E://publicAndPrivate.xml");
            stm.Read(dataLength,0,4);
            int numOfBlock = dataLength[0];
          //  MessageBox.Show(numOfBlock+"");
            StringBuilder image = new StringBuilder();
            for (int n = 0; n < numOfBlock; n++)
            {
                data1 = new byte[80];
                stm.Read(data1, 0, data1.Length);
                //textBox1.Text = (Encoding.ASCII.GetString(data1));
               decryptedData1 = sym.Decrypt(data1, decryptedKey, decryptedIv);
                textBox1.AppendText( (Encoding.ASCII.GetString(decryptedData1)));
               
            }
           
           // byteArrayToImage(im);
            
           // data2 = new byte[64];
           // List<Byte> aa = new List<byte>();
            
           
           // stm.Read(data2, 0, data2.Length);
          
           // MessageBox.Show(Encoding.ASCII.GetString(data)+"");
           // MessageBox.Show(Encoding.ASCII.GetString(key) + "");
           // MessageBox.Show(Encoding.ASCII.GetString(iv) + "");
            
           
           
        //    decryptedData2 = sym.Decrypt(data2, decryptedKey, decryptedIv);
            
           
        }
        
          //  return returnImage;
        }
    }


