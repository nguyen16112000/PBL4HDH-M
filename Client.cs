using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Net;

namespace PBL4HDHM
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("192.168.43.124");
            int port = 9000;
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(ip, port);
                Console.WriteLine("Da ket noi!!!");
                NetworkStream ns = client.GetStream();
                Thread thread = new Thread(o => ReceiveData((TcpClient)o));
                thread.Start(client);
                string s;
                while (!string.IsNullOrEmpty((s = Console.ReadLine())))
                {
                    byte[] buffer = Encoding.ASCII.GetBytes(s);
                    ns.Write(buffer, 0, buffer.Length);
                }
                // ngat ket noi khi khong nhap gi
                client.Client.Shutdown(SocketShutdown.Send);
                thread.Join();
                ns.Close();
                client.Close();
                Console.WriteLine("Ngat ket noi tu server");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }
        static void ReceiveData(TcpClient client)
        {
            try
            {
                NetworkStream ns = client.GetStream();
                byte[] receivedBytes = new byte[1024];
                int byte_count;
                while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                {
                    Console.Write(Encoding.ASCII.GetString(receivedBytes, 0, byte_count));
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }
    }
}