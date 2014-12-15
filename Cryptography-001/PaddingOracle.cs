//Originally from Wayne C. Louie
using System;
using System.Net.Sockets;

namespace Program
{
    class Program
    {
        static readonly int BLOCKSIZE = 16;
        static TcpClient client;
        static NetworkStream stream;
        static bool Oracle_Connect()
        {
            try
            {
                client = new TcpClient("54.165.60.84", 6667);
                stream = client.GetStream();
                Console.WriteLine("Connection opened successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection open failed with error [{0}]", e.Message);
                return false;
            }
            return true;
        }
        static bool Oracle_Disconnect()
        {
            try
            {
                stream.Close();
                client.Close();
                Console.WriteLine("Connection closed successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection close failed with error [{0}]", e.Message);
                return false;
            }
            return true;
        }
        static bool Oracle_Send(byte[] ctext, int num_blocks)
        {
            int packet_size= num_blocks*BLOCKSIZE + 2;
            byte[] buffy = new byte[packet_size];
            buffy[0] = (byte)num_blocks;
            for (int i = 0; i < packet_size-2; i++) buffy[i + 1] = ctext[i];
            buffy[packet_size - 1] = 0;
            try
            {
                stream.Write(buffy, 0, num_blocks * BLOCKSIZE + 2);
                try
                {
                    int results= stream.ReadByte();
                    return (results == 0x31); // ASCII 1 HEX 31 is success
                }
                catch (Exception e)
                {
                    Console.WriteLine("Stream read failed with error [{0}]", e.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Stream write failed with error [{0}]", e.Message);
            }
            return false;
        }
        static byte[] data = 
        {
            0x9F, 0x0B, 0x13, 0x94, 0x48, 0x41, 0xA8, 0x32, 0xB2, 0x42, 0x1B, 0x9E, 0xAF, 0x6D, 0x98, 0x36,
            0x81, 0x3E, 0xC9, 0xD9, 0x44, 0xA5, 0xC8, 0x34, 0x7A, 0x7C, 0xA6, 0x9A, 0xA3, 0x4D, 0x8D, 0xC0,
            0xDF, 0x70, 0xE3, 0x43, 0xC4, 0x00, 0x0A, 0x2A, 0xE3, 0x58, 0x74, 0xCE, 0x75, 0xE6, 0x4C, 0x31
        };
        static void Main(string[] args)
        {
            if (!Oracle_Connect())
            {
                Console.WriteLine("Connect failed.");
            }
            else
            {
                // Do your crypto magic work here.
                if (Oracle_Send(data, 3))
                {
                    Console.WriteLine("Ciphertext decrypted successfully.");
                }
                else
                {
                    Console.WriteLine("Ciphertext failed to decrypt.");
                }
                if (!Oracle_Disconnect())
                {
                    Console.WriteLine("Disconnect failed.");
                }
            }
            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }
    }
}