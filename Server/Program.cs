using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class Program
    {
        static Dictionary<string, TcpClient> users = new Dictionary<string, TcpClient>();

        static void Main(string[] args)
        {
            StartServer();
        }

        static void StartServer()
        {
            TcpListener server = new TcpListener(IPAddress.Parse("192.168.0.103"), 27001);
            server.Start();
            Console.WriteLine("Server is running...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }
        static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            bytesRead = stream.Read(buffer, 0, buffer.Length);
            string username = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            Console.WriteLine($"Istifadeci: {username}");

            lock (users)
            {
                users[username] = client;
            }

            while (true)
            {
                try
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Mesaj alındı: {message}");

                       
                        string[] parts = message.Split('|');
                        if (parts.Length >= 2)
                        {
                            string targetUser = parts[0];
                            string actualMessage = parts[1];

                            TcpClient targetClient;
                            lock (users)
                            {
                                users.TryGetValue(targetUser, out targetClient);
                            }

                            if (targetClient != null)
                            {
                                NetworkStream targetStream = targetClient.GetStream();
                                byte[] messageBytes = Encoding.ASCII.GetBytes($"{username}: {actualMessage}");
                                targetStream.Write(messageBytes, 0, messageBytes.Length);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    break;
                }
            }

            lock (users)
            {
                users.Remove(username);
            }
            client.Close();
        }
    }
}

