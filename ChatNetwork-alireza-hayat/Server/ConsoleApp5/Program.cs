using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net.Http;
using System.Net.Sockets;
using System.Net;
using System.Text;

 
namespace ConsoleApp5
{



    class Program
    {
        static TcpListener tcpServer;// ip bedi port bede listen kone
        static TcpClient tcpClient;// request mifreste ...
        static Thread th;// be sorat movazi mikhay kar anjam bedim , 
        static string input;
        public static void Main()
        {
            th = new Thread(new ThreadStart(StartListen));
            th.Start();
        }
        public static void StartListen()
        {
            NetworkStream stream;//jens dade stream , masalan zamani ke dari tV online nega mikoni , jens dade video chon peyvastas streame ...

            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                //IPAddress localAddr = IPAddress.Parse("192.168.1.122");

                tcpServer = new TcpListener(localAddr, 5000);
                tcpServer.Start();

                // Keep on accepting Client Connection
                while (true)
                {

                    // New Client connected, call Event to handle it.
                    Thread t = new Thread(new ParameterizedThreadStart(NewClient));
                    tcpClient = tcpServer.AcceptTcpClient();
                    t.Start(tcpClient);
                    NetworkStream ns = tcpClient.GetStream();

                    ReceiveFile(ns);//تابع جدید ادد شده

                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            stream = tcpClient.GetStream();
            int sendOffset = 0;
            int recOffset = 0;

            int dataLength;
            Byte[] receiveData = new Byte[256];
            dataLength = stream.Read(receiveData, recOffset, receiveData.Length);
            String recvdMessage = System.Text.Encoding.ASCII.GetString(receiveData, 0, dataLength);
            Console.WriteLine(recvdMessage.ToString());
            recOffset += dataLength;

            dataLength = stream.Read(receiveData, recOffset, receiveData.Length);
            String recvvdMessage = System.Text.Encoding.ASCII.GetString(receiveData, 0, dataLength);
            Console.WriteLine(recvvdMessage.ToString());
            recOffset += dataLength;
            while (!stream.DataAvailable)
            {
            }
            dataLength = stream.Read(receiveData, 0, receiveData.Length);
            recvdMessage = System.Text.Encoding.ASCII.GetString(receiveData, 0, dataLength);
            Console.WriteLine(recvdMessage.ToString());

        }
        public static void ReceiveFile(NetworkStream ns)
        {

            byte[] fileDataLengthBytes = new byte[4];
            ns.Read(fileDataLengthBytes, 0, 4);
            int fileDataLength = BitConverter.ToInt32(fileDataLengthBytes, 0);

            byte[] fileData = new byte[fileDataLength];
            int bytesRead = 0;
            int totalBytesRead = 0;

            while (totalBytesRead < fileDataLength)
            {
                bytesRead = ns.Read(fileData, totalBytesRead, fileDataLength - totalBytesRead);
                totalBytesRead += bytesRead;
            }

            File.WriteAllBytes("received_file", fileData);
        }
    
    public static void NewClient(Object obj)
        {
            Console.WriteLine("Client Added");
            var client = (TcpClient)obj;
            NetworkStream ns = client.GetStream();

            StateObject state = new StateObject();
            state.workSocket = client.Client;
            client.Client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback((new Program()).OnReceive), state);
            
            while (true)
            {
                input = Console.ReadLine();
                if (input == "exit")
                    break;
                ns.Write(Encoding.ASCII.GetBytes(input), 0, input.Length);
                ns.Flush();

                //data = new byte[1024];
                //recv = ns.Read(data, 0, data.Length);
                //stringData = Encoding.ASCII.GetString(data, 0, recv);
                //Console.WriteLine(stringData);
            }
            Console.ReadKey();
        }
        public void OnReceive(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            int bytesRead;

            if (handler.Connected)
            {

                // Read data from the client socket. 
                try
                {
                    bytesRead = handler.EndReceive(ar);
                    if (bytesRead > 0)
                    {
                        // There  might be more data, so store the data received so far.
                        state.sb.Remove(0, state.sb.Length);
                        state.sb.Append(Encoding.ASCII.GetString(
                                         state.buffer, 0, bytesRead));

                        // Display Text in Rich Text Box
                        content = state.sb.ToString();
                        Console.WriteLine(content);

                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(OnReceive), state);

                    }
                }

                catch (SocketException socketException)
                {
                    //WSAECONNRESET, the other side closed impolitely
                    if (socketException.ErrorCode == 10054 || ((socketException.ErrorCode != 10004) && (socketException.ErrorCode != 10053)))
                    {
                        handler.Close();
                    }
                }

                // Eat up exception....Hmmmm I'm loving eat!!!
                catch (Exception exception)
                {
                    //MessageBox.Show(exception.Message + "\n" + exception.StackTrace);

                }
            }
        }
    }
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
}
