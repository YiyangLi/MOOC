//Originally from Angelo Cresta, Magic shown by Yiyang LI

using System;
using System.Net.Sockets;
using System.Linq;

namespace Cryptography
{
    class Program
    {
        const string ATTACK_MSG = "I, the server, hereby agree that I will pay $100 to this student";
        const byte EXTRA_PADDING = 0;
        static TcpClient MAC_Client;
        static TcpClient VRFY_Clinet;
        static NetworkStream MAC_Stream;
        static NetworkStream VRFY_Stream;
        static void connect()
        {
            MAC_Client = new TcpClient("54.165.60.84", 6668);
            MAC_Stream = MAC_Client.GetStream();
            VRFY_Clinet = new TcpClient("54.165.60.84", 6669);
            VRFY_Stream = VRFY_Clinet.GetStream();
        }

        static void disconnect()
        {
            MAC_Stream.Close();
            MAC_Client.Close();
            VRFY_Stream.Close();
            VRFY_Clinet.Close();
        }
        static byte[] GetBytes(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }
        public static string GetString(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        static byte[] Mac(byte[] message, int mlength)
        {
            int packet_size = mlength + 2;
            byte[] buffy = new byte[packet_size];
            buffy[0] = (byte)mlength;
            for (int i = 0; i < mlength; i++)
                buffy[i + 1] = message[i];
            buffy[buffy.Length - 1] = EXTRA_PADDING;
            try
            {
                MAC_Stream.Write(buffy, 0, packet_size);
                byte[] data = new byte[16];
                int results = MAC_Stream.Read(data, 0, 16);
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine("Stream read failed with error [{0}]", e.Message);
                return null;
            }
        }
        static bool Verify(byte[] message, int mlength, byte[] tag)
        {
            int packet_size = message.Length + tag.Length + 2;
            byte[] buffy = new byte[packet_size];
            buffy[0] = (byte)mlength;
            for (int i = 0; i < message.Length; i++)
            {
                buffy[i + 1] = message[i];
            }
            for (int i = 0; i < tag.Length; i++)
            {
                buffy[i + message.Length + 1] = tag[i];
            }
            buffy[buffy.Length - 1] = EXTRA_PADDING;
            try
            {
                VRFY_Stream.Write(buffy, 0, packet_size);
                int results = VRFY_Stream.ReadByte();
                return results == (int)'1';
            }
            catch (Exception e)
            {
                Console.WriteLine("Stream read failed with error [{0}]", e.Message);
                return false;
            }
        }
        static void Main(string[] args)
        {
            connect();
            byte[] firstHalf = GetBytes(ATTACK_MSG.Substring(0, ATTACK_MSG.Length / 2));
            byte[] secondHalf = GetBytes(ATTACK_MSG.Substring(ATTACK_MSG.Length / 2));
            byte[] tag = Mac(firstHalf, firstHalf.Length);
            for (int i = 0; i < tag.Length; i++)
            {
                secondHalf[i] = (byte)(secondHalf[i] ^ tag[i]);
            }
            tag = Mac(secondHalf, secondHalf.Length);
            bool verify = Verify(GetBytes(ATTACK_MSG), ATTACK_MSG.Length, tag);
            //Bingo!
            disconnect();
            string answer = string.Concat(tag.Take(4).Select(i => i.ToString("X2")).ToArray());
            Console.WriteLine(answer);
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}