/*
 * In an attempt to avoid the attacks on the "plain RSA" signature scheme, J. Random Hacker has designed her own RSA-based signature scheme. 
 * The scheme works as follows: the public key is a standard RSA public key (N, e), and the private key is the usual (N, d), where N is a 128-byte (1024-bit) integer. 
 * To sign a message m of length exactly 63 bytes, set
 *                         M = 0x00 m 0x00 m
 * (note that M is exactly 128 bytes long) and then compute the signature M^d mod N. 
 * 
 * (If m is shorter than 63 bytes, 0-bytes are first preprended to make its length exactly 63 bytes. 
 * This means that the signature on any message m is the same as the signatures on 0x00 m and 0x00 00 m, etc., allowing easy forgery attacks. 
 * This is a known vulnerability that is not the point of this problem.) 

 * J. Random Hacker is so sure this scheme is secure, she is offering a bounty of 1 point to anyone who can forge a signature on the 63-byte message
 *                                            'Crypto is hard --- even schemes that look complex can be broken'
 *  with respect to the public key N =
                                      a99263f5cd9a6c3d93411fbf682859a07b5e41c38abade2a551798e6c8af5af0
                                      8dee5c7420c99f0f3372e8f2bfc4d0c85115b45a0abc540349bf08b251a80b85
                                      975214248dffe57095248d1c7e375125c1da25227926c99a5ba4432dfcfdae3
                                      00b795f1764af043e7c1a8e070f5229a4cbc6c5680ff2cd6fa1d62d39faf3d41d
      and e = 10001. (Both given in hex.) 
 * You will be given the ability to obtain signatures on messages of your choice -- except for the message above! 
 * You will also be given access to a verification routine that you can use to verify your solution. 

*/


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
        const string HexN = ("0A99263F5CD9A6C3D93411FBF682859A07B5E41C38ABADE2A551798E6C8AF5AF0" + 
                             "8DEE5C7420C99F0F3372E8F2BFC4D0C85115B45A0ABC540349BF08B251A80B85" + 
                             "975214248DFFE57095248D1C7E375125C1DA25227926C99A5BA4432DFCFDAE3" +
                             "00B795F1764AF043E7C1A8E070F5229A4CBC6C5680FF2CD6FA1D62D39FAF3D41D");
        const string HexE = "10001";
        const string Message = @"Crypto is hard --- even schemes that look complex can be broken";
        static readonly string HexM = string.Concat(Message.Select(i => ((ushort)i).ToString("X2")).ToArray());
        static readonly BigInteger N = BigInteger.Parse(HexN, NumberStyles.AllowHexSpecifier);
        static readonly BigInteger e = BigInteger.Parse(HexE, NumberStyles.AllowHexSpecifier); //Public RSA Key
        static BigInteger M = BigInteger.Parse(HexM, NumberStyles.AllowHexSpecifier);
        static void Main()
        {
            Oracle oracle = new Oracle();
            BigInteger m1 = new BigInteger(2);
            BigInteger m2 = BigInteger.Divide(M, m1);
            if (oracle.IsConnected)
            {
                //P(m) = µ*m + r, where µ is the shift-and-concatenation
                //M = 0x00|m(as 63 bytes)|0x00|m(as 63 bytes), r=0 and µ=2**(64*8)+1
                var mu = BigInteger.Pow(new BigInteger(2), 64 * 8) + BigInteger.One; //Mu = Shift 1
                var signMu = oracle.Sign(BigInteger.One);
                //Verify the Signing oracle
                if (!BigInteger.ModPow(signMu, e, N).Equals(mu))
                {
                    //sign(1) = [mu^d mod N]
                    Console.WriteLine("Please confirm the signature oracle again. ");
                    return;
                }
                Console.WriteLine("The signature oracle is fine, we are good to go!");

                //Magic begins: (sigma(m)^e = sigma(m1)^e*sigma(m2)^e/sigma(1)^e) = mu*m1*m2)
                var signM1 = oracle.Sign(m1);
                var signM2 = oracle.Sign(m2);
                var signM1_e = BigInteger.ModPow(signM1, e, N);
                var signM2_e = BigInteger.ModPow(signM2, e, N);
                var signMu_e = BigInteger.ModPow(signMu, e, N);
                if (!(signM1_e * signM2_e / signMu_e).Equals(mu * m1 * m2))
                {
                    //[signM1*signM2/Mu mod N]
                    Console.WriteLine("Please confirm the e-th root. ");
                }
                //Showtime, module division to get the sigma of M
                var signMuInv = Helper.modInverse(signMu, N);
                var signM = BigInteger.Remainder(signM1 * signM2 * signMuInv,  N);
                if (oracle.Verify(M, signM))
                {
                    Console.Write("RSA hacked!");
                    Console.WriteLine("Submit your solution: {0}", signM.ToString("X").Substring(0, 8).ToUpper());
                }
                else
                    Console.Write("Sorry, please try again. ");

            }
            oracle.Disconnect();
            Console.ReadKey();
        }
    }


    public class Helper
    {
        public static BigInteger modInverse(BigInteger a, BigInteger m)
        {
            BigInteger i = m, v = 0, d = 1;
            while (a > 0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= m;
            if (v < 0) 
                v = (v + m) % m;
            return v;
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
            for (int i = value.Length - 1; i >= 0; i--)
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
            var bigIntBin = bigInt.ToBinaryString() + "X";
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

        public bool Verify(BigInteger message, BigInteger sign)
        {
            var resp = new byte[MAX_PACKET_LEN];
            string binMessage = message.ToBinaryString().TrimStart('0');
            string binSign = sign.ToBinaryString().TrimStart('0');
            var package = Helper.GetBytes(string.Concat(binMessage, ":", binSign, "X"));
            VRFY_Stream.Write(package, 0, package.Length);
            int results = VRFY_Stream.Read(resp, 0, MAX_PACKET_LEN);
            var strResp = System.Text.Encoding.ASCII.GetString(resp);
            var respBigInt = Helper.BinToBigInt(strResp.Substring(0, strResp.IndexOf((char)0)));
            return respBigInt.Equals(BigInteger.One);
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

            return base2.ToString();
        }

    }

}
