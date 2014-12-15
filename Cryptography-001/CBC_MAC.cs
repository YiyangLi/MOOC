//Originally from Angelo Cresta

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
 
namespace PaddingOracle.Oracles
{
	public class Toolkit
	{
		public static byte[] Xor(byte[] A1, byte[] A2, int lenght)
        {
            byte[] result = new byte[lenght];
 
 
            for (int i = 0; i < lenght; i++)
            {
                result[i] = (byte)(A1[i] ^ A2[i]);
            }
 
            return result;
        }
	}
    public class OnlineCBC_MAC 
    {
 
        public static byte[] Mac(byte[] message, int mlength)
        {
 
            List<byte> RequestOracleMac = new List();
 
            for (int i = 0; i < message.Count(); i++)
            {
                RequestOracleMac.Add(message[i]);
            }
 
            //insert mlength at Index 0 before sending to the server
            RequestOracleMac.Insert(0, (byte)mlength);
 
            //insert null terminator
            RequestOracleMac.Insert(RequestOracleMac.Count, (byte)'\0');
 
            byte[] Tag = GetBytes(OnlineCBC_MAC.Connect("54.165.60.84", 6668, RequestOracleMac.ToArray()));
 
            return Tag;
 
        }
        public static int Verify(byte[] message, int mlength, byte[] tag)
        {
 
            List RequestOracleVerify = new List();
 
            for (int i = 0; i < message.Count(); i++)
            {
                RequestOracleVerify.Add(message[i]);
            }
 
            //insert mlength at Index 0 before sending to the server
            RequestOracleVerify.Insert(0, (byte)mlength);
 
            //insert tag 
            for (int i = 0; i < tag.Count(); i++)
            {
                RequestOracleVerify.Insert(RequestOracleVerify.Count, tag[i]);
            }
 
            //insert null terminator
            RequestOracleVerify.Insert(RequestOracleVerify.Count, (byte)'\0');
 
            int result =0;
            int.TryParse(OnlineCBC_MAC.Connect("54.165.60.84", 6669, RequestOracleVerify.ToArray()), out result);
 
            return result;
 
        }
 
        public static byte[] GetBytes(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }
 
        public static string GetString(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
 
 
        public static String Connect(String server, Int32 port, Byte[] data) //, String message)
        {
 
            string output = "";
            try
            {
                TcpClient client = new TcpClient(server, port);
 
                NetworkStream stream = client.GetStream();
 
                stream.Write(data, 0, data.Length);
 
                data = new Byte[256];
 
                // String to store the response ASCII representation.
                String responseData = String.Empty;
 
                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
 
                //Console.WriteLine("Received: {0}", responseData);
                output = responseData;
 
                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
 
            return output;
        }
    }
}