using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            StartClient();
        }
        static void StartClient()
        {
            TcpClient client = new TcpClient();
            client.Connect("192.168.0.103", 27001);

            Console.Write("Adinizi daxil edin: ");
            string username = Console.ReadLine();

            NetworkStream stream = client.GetStream();
            byte[] usernameBytes = Encoding.ASCII.GetBytes(username);
            stream.Write(usernameBytes, 0, usernameBytes.Length);

            Console.WriteLine("Baglanti quruldu.");

            while (true)
            {
                try
                {
                    string message = Console.ReadLine();
                    string sendMessage = $"{message}";
                    byte[] messageBytes = Encoding.ASCII.GetBytes(sendMessage);
                    stream.Write(messageBytes, 0, messageBytes.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Hata: {ex.Message}");
                    break;
                }
            }

            client.Close();
        }
    }
}
