using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Globalization;
using System.Net.Sockets;

namespace Cryptography
{
    class RSA_Sign
    {
        static void Main()
        {
            Oracle oracle = new Oracle();
            if (oracle.IsConnected)
            {
                var sign = oracle.Sign(new BigInteger(1));
                Console.WriteLine(sign);
            }
            oracle.Disconnect();
            Console.ReadKey();
        }
    }


    public class Helper
    {
        public static BigInteger modInverse(BigInteger a, BigInteger m)
        {
            return BigInteger.ModPow(a, m - 2, m);
        }
        public static BigInteger ASCII_TO_INT(string text)
        {
            string hexValue = string.Concat(Encoding.ASCII.GetBytes(text).Select(a => a.ToString("X2")).ToArray());
            return BigInteger.Parse(hexValue, NumberStyles.HexNumber);
        }

        public static byte[] GetBytes(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        public static BigInteger BinToBigInt(string value)
        {
            // BigInteger can be found in the System.Numerics dll
            BigInteger res = 0;
            BigInteger digit = 1;

            // I'm totally skipping error handling here
            for (int i = value.Length - 1; i >=0; i--)
            {
                res += value[i] == '1' ? digit : 0;
                digit <<= 1;
            }
            return res;
        }
    }

    public class Oracle
    {
        private readonly TcpClient Sign_Client;
        private readonly TcpClient VRFY_Clinet;
        private readonly NetworkStream Sign_Stream;
        private readonly NetworkStream VRFY_Stream;
        private const int MAX_PACKET_LEN = 8192;
        private const int NOT_BINARY_STR_ERR = -1;
        private const int MISSING_DELIMITER_ERR = -2;
        private const int ORIGINAL_MSG_ERR = -3;
        private readonly bool isConnected;
        public bool IsConnected
        {
            get { return isConnected; }
        }

        public Oracle()
        {
            try
            {
                Sign_Client = new TcpClient("54.165.60.84", 8080);
                Sign_Stream = Sign_Client.GetStream();
                VRFY_Clinet = new TcpClient("54.165.60.84", 8081);
                VRFY_Stream = VRFY_Clinet.GetStream();
                isConnected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                isConnected = false;
            }
        }

        public void Disconnect()
        {
            if (isConnected)
            {
                Sign_Stream.Close();
                Sign_Client.Close();
                VRFY_Stream.Close();
                VRFY_Clinet.Close();
            }
        }

        public BigInteger Sign(BigInteger bigInt)
        {
            var resp = new byte[MAX_PACKET_LEN];
            if (!isConnected)
            {
                Console.WriteLine("[WARNING]: You haven't connected to the server yet.");
                return BigInteger.Zero;
            }
            if (bigInt < 0)
            {
                Console.WriteLine("[ERROR]: Message cannot be negative!");
            }
            var bigIntBin = bigInt.ToBinaryString();
            var bigIntBytes = Helper.GetBytes(bigIntBin);
            Sign_Stream.Write(bigIntBytes, 0, bigIntBytes.Length);
            int results = Sign_Stream.Read(resp, 0, MAX_PACKET_LEN);
            var strResp = System.Text.Encoding.ASCII.GetString(resp);
            var respBigInt = Helper.BinToBigInt(strResp.Substring(0, strResp.IndexOf((char)0)));
            if (respBigInt == NOT_BINARY_STR_ERR)
                Console.WriteLine("[ERROR]: Message was not a valid binary string.");
            if (respBigInt == ORIGINAL_MSG_ERR)
                Console.WriteLine("[ERROR]: You cannot request a signature on the original message!");
            return respBigInt;
        }
    }
    public static class BigIntegerExtensions
    {
        /// <summary>
        /// Converts a <see cref="BigInteger"/> to a binary string.
        /// </summary>
        /// <param name="bigint">A <see cref="BigInteger"/>.</param>
        /// <returns>
        /// A <see cref="System.String"/> containing a binary
        /// representation of the supplied <see cref="BigInteger"/>.
        /// </returns>
        public static string ToBinaryString(this BigInteger bigint)
        {
            var bytes = bigint.ToByteArray();
            var idx = bytes.Length - 1;

            // Create a StringBuilder having appropriate capacity.
            var base2 = new StringBuilder(bytes.Length * 8);

            // Convert first byte to binary.
            var binary = Convert.ToString(bytes[idx], 2);

            // Ensure leading zero exists if value is positive.
            if (binary[0] != '0' && bigint.Sign == 1)
            {
                base2.Append('0');
            }

            // Append binary string to StringBuilder.
            base2.Append(binary);

            // Convert remaining bytes adding leading zeros.
            for (idx--; idx >= 0; idx--)
            {
                base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
            }

            return base2.Append("X").ToString();
        }

    }
    
}
