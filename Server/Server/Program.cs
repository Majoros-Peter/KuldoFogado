using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO.Pipes;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RunServer();
        }

        static void RunServer()
        {
            while (true)
            {
                //Client
                using (var client = new NamedPipeClientStream(".", "8===D", PipeDirection.InOut))
                {
                    Console.WriteLine("Server is online");
                    client.Connect();
                    Console.WriteLine("Someone connected!");

                    // Read and write data through the pipe

                    byte[] lengthBuffer = new byte[sizeof(int)];
                    client.Read(lengthBuffer, 0, lengthBuffer.Length);
                    int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

                    // Read the message
                    byte[] messageBuffer = new byte[messageLength];
                    client.Read(messageBuffer, 0, messageBuffer.Length);
                    string message = Encoding.UTF8.GetString(messageBuffer);
                    Console.WriteLine("Data received: " + message);

                    //Write back
                    string messageBack = "Küldöm vissza!";
                    byte[] messageBackBytes = Encoding.UTF8.GetBytes(messageBack);
                    client.Write(BitConverter.GetBytes(messageBackBytes.Length), 0, sizeof(int));
                    client.Write(messageBackBytes, 0, messageBackBytes.Length);
                    Console.WriteLine("Data Sent");

                    client.Close();
                }
            }
        }
    }
}