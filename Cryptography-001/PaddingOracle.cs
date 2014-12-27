//Originally TCP connection from Wayne C. Louie, Magic is shown by Yiyang LI
using System;
using System.Net.Sockets;
using System.Linq;

namespace Program
{
    class Program
    {
        const int BLOCKSIZE = 16;
        const byte EXTRA_PADDING = 0;
        const string cipherString = "9F0B13944841A832B2421B9EAF6D9836813EC9D944A5C8347A7CA69AA34D8DC0DF70E343C4000A2AE35874CE75E64C31";
        static readonly byte[] ciphertext = Enumerable.Range(0, cipherString.Length / 2)
                .Select(x => byte.Parse(cipherString.Substring(x * 2, 2), System.Globalization.NumberStyles.HexNumber))
                .ToArray();

        static TcpClient client;
        static NetworkStream stream;
        static void connect()
        {
            client = new TcpClient("54.165.60.84", 6667);
            stream = client.GetStream();
        }
        static void disconnect()
        {
            client.Close();
            stream.Close();
        }

        static bool Oracle_Send(byte[] ctext, int num_blocks)
        {
            int packet_size = num_blocks * BLOCKSIZE + 2;
            byte[] buffy = new byte[packet_size];
            buffy[0] = (byte)num_blocks;
            for (int i = 0; i < num_blocks * BLOCKSIZE; i++)
                buffy[i + 1] = ctext[i];
            buffy[buffy.Length - 1] = EXTRA_PADDING;
            try
            {
                stream.Write(buffy, 0, packet_size);
                try
                {
                    int results = stream.ReadByte();
                    if (results == 0)
                        results = stream.ReadByte();
                    return (results == (int)'1'); // ASCII 1 HEX 31 is success
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
        static void Main(string[] args)
        {
            connect();
            if (Oracle_Send(ciphertext, 3))
            {
                Console.WriteLine("Ciphertext decrypted successfully.");
            }
            // Do your crypto magic work here.
            int paddingSize = 0, msgLen = 0;
            var cloneCipher = (byte[])ciphertext.Clone();
            for (int k = 0; k < BLOCKSIZE; k++)
            {
                cloneCipher[16 + k] = 0;
                if (!Oracle_Send(cloneCipher, 3))
                {
                    Console.WriteLine("Ciphertext failed to decrypt. We got the message length. ");
                    msgLen = k;
                    break;
                }
            }
            char[] cleartext = Enumerable.Range(0, ciphertext.Length - 1).Select(i => i > msgLen ? ' ' : '?').ToArray();
            cloneCipher = (byte[])ciphertext.Clone();
            
            for (int block = 0; block < 2; block++)
            {
                cloneCipher = (byte[])ciphertext.Clone();
                for (paddingSize = BLOCKSIZE - msgLen + 1; paddingSize <= BLOCKSIZE; paddingSize++)
                {
                    int indexOffset = block == 0 ? 16 : 0;
                    int numBlock = block == 0 ? 3 : 2;
                    int index = BLOCKSIZE - paddingSize + indexOffset;
                    for (int i = BLOCKSIZE - 1; i > BLOCKSIZE - paddingSize; i--)
                    {
                        cloneCipher[i + indexOffset] = (byte)((cloneCipher[i + indexOffset] ^ paddingSize ^ (paddingSize - 1)));
                    }
                    for (byte i = 0; i < 0xFF; i++)
                    {
                        cloneCipher[index] = (byte)i;
                        if (Oracle_Send(cloneCipher, numBlock))
                        {
                            char c = (char)(ciphertext[index] ^ cloneCipher[index] ^ paddingSize);
                            Console.WriteLine(c);
                            cleartext[index] = c;
                            break;
                        }
                    }
                }
                msgLen = BLOCKSIZE;
            }
            disconnect();
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
        
    }
}