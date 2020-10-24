using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Net;
using System.Collections.Generic;
namespace PBL4HDHM
{
    class Server
    {
        static readonly object _lock = new object();
        static readonly Dictionary<int, TcpClient> list_clients = new Dictionary<int, TcpClient>();

        static void Main(string[] args)
        {
            int count = 1;

            try
            {
                IPAddress IP = IPAddress.Parse("192.168.1.3");
                TcpListener ServerSocket = new TcpListener(IP, 9000);
                ServerSocket.Start();

                while (true)
                {
                    TcpClient client = ServerSocket.AcceptTcpClient();
                    lock (_lock) list_clients.Add(count, client);
                    Console.WriteLine("Co ket noi!");

                    Thread t = new Thread(Handle_clients);
                    t.Start(count);
                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Handle_clients(object o)
        {
            int id = (int)o;
            TcpClient client;

            lock (_lock) client = list_clients[id];

            try
            {
                while (true)
                {
                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[1024];
                    int byte_count = stream.Read(buffer, 0, buffer.Length);

                    if (byte_count == 0)
                    {
                        break;
                    }

                    string data = Encoding.ASCII.GetString(buffer, 0, byte_count);
                    Broadcast(data, client);
                    Console.WriteLine(data);
                }

                lock (_lock) list_clients.Remove(id);
                client.Client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch (Exception)
            {
            }
        }
        public static string TinhLich(string data)
        {
            int N = int.Parse(data);
            int a = (N - 1) / 4, b = (N - 1) / 100, c = (N - 1) / 400;
            int thutu = 1;
            int ans = (N - 1 + a - b + c + thutu) % 7;
            string thu = "";
            switch (ans)
            {
                case 0: thu = "SU"; break;
                case 1: thu = "MO"; break;
                case 2: thu = "TU"; break;
                case 3: thu = "WE"; break;
                case 4: thu = "TH"; break;
                case 5: thu = "FR"; break;
                case 6: thu = "SA"; break;
            }
            return thu;
        }
        public static string Handle(string data)
        {
            return TinhLich(data);
        }

        public static void Broadcast(string data, TcpClient sender)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(Handle(data) + Environment.NewLine);

                lock (_lock)
                {
                    NetworkStream stream = sender.GetStream();
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}