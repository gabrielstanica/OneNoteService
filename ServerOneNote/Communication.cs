using OneNoteOCRDll;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ServerOneNote
{
    public class Communication
    {
        public TcpClient Client { get; set; }
        public string TextToSearch { get; set; }

        public Communication(TcpClient client)
        {
            Client = client;
        }

        public Image ReceiveImage()
        {
            NetworkStream serverStream = Client.GetStream();
            Image img;
            BinaryReader reader = new BinaryReader(serverStream);
            // read how big the image buffer is
            int ctBytes = reader.ReadInt32();
            // read the image buffer into a MemoryStream
            MemoryStream ms = new MemoryStream(reader.ReadBytes(ctBytes));
            // get the image from the MemoryStream
            img = Image.FromStream(ms);
            serverStream.Flush();
            return img;
        }

        public string ReceiveText()
        {
            NetworkStream serverStream = Client.GetStream();
            byte[] inStream = new byte[9999999];
            Int32 bytes = serverStream.Read(inStream, 0, (int)Client.ReceiveBufferSize);
            string responseData = System.Text.Encoding.ASCII.GetString(inStream, 0, bytes);
            serverStream.Flush();
            return responseData;
        }

        public void SendText(string wantedText)
        {
            NetworkStream serverStream = Client.GetStream();
            ImageConverter converter = new ImageConverter();
            Byte[] sendData = Encoding.ASCII.GetBytes(wantedText);
            serverStream.Write(sendData, 0, sendData.Length);
            serverStream.Flush();
        }

        public void SendTextFound(TextFound textAreaToReturn)
        {
            NetworkStream serverStream = Client.GetStream();
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, textAreaToReturn);
                Byte[] sendData = ms.ToArray();
                serverStream.Write(sendData, 0, sendData.Length);
                serverStream.Flush();
            }
        }
    }
}
