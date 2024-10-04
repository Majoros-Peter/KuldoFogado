using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Pipes;
using System.IO;
using Microsoft.Win32;
using System.Reflection.PortableExecutable;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;

namespace KuldoFogado
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string fajl = "nincs beolvasva";

        public MainWindow()
        {
            InitializeComponent();
        }

        static void IDK()
        {
            using (var server = new NamedPipeServerStream("8===D"))
            {
                MessageBox.Show("Waiting for client connection...");
                server.WaitForConnection();
                MessageBox.Show("Client connected.");

                // Read and write data through the pipe

                string message = fajl;
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                // Write the message length followed by the message
                server.Write(BitConverter.GetBytes(messageBytes.Length), 0, sizeof(int));
                server.Write(messageBytes, 0, messageBytes.Length);
                MessageBox.Show("Data sent: " + message);

                //read Back
                byte[] lengthBuffer = new byte[sizeof(int)];
                server.Read(lengthBuffer, 0, lengthBuffer.Length);
                int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

                // Read the message
                byte[] messageBuffer = new byte[messageLength];
                server.Read(messageBuffer, 0, messageBuffer.Length);
                string messageBack = Encoding.UTF8.GetString(messageBuffer);

                MessageBox.Show("Data received: " + messageBack);

            }
        }

        static string ReadPdf(string path)
        {
            Regex reg = new(@"Synopsis\n(?<syn>.+)\nDescription\n(?<desc>(?>.|\n)+)See Also(?>.|\n)+Solution\n(?<sol>.+)\nRisk Factor\n(?<risk>.+)");

            StringBuilder sb = new();

            PdfReader reader = new PdfReader(path);
            for (int page = 1; page <= 1/*reader.NumberOfPages*/; page++)
            {
                Match match = reg.Match(PdfTextExtractor.GetTextFromPage(reader, page));

                var a = match.Groups[0];

                sb.Append(PdfTextExtractor.GetTextFromPage(reader, page));
            }
            reader.Close();
            return sb.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new();

            if(ofd.ShowDialog() is true)
            {
                fajl = ReadPdf(ofd.FileName);
                IDK();
            }
        }
    }
}