using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }
        static ASCIIEncoding encoding = new ASCIIEncoding();

        Socket server;
        IPEndPoint ipe;
        List<Socket> listClient = new List<Socket>();
        Thread clientThread;

        public void CreateServer()
        {
            ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7890);// server lang nge vs ip 127.0.0.1 và port = 7890
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        }

        public void ListenConnect()
        {
            server.Bind(ipe);
            server.Listen(2);

            while (true)
            {
                Socket temp = server.Accept();
                listClient.Add(temp);


                this.Dispatcher.Invoke((Action)(() =>
                    {
                        string str = ("Chap nhan ket noi tu " + temp.RemoteEndPoint.ToString() + "\r\n");
                        txtServer.AppendText(str);
                    }));

                Thread processclient = new Thread(newThreadClient);
                processclient.IsBackground = true;
                processclient.Start(temp);

            }
        }

        public void newThreadClient(object obj)
        {
            Socket clientsk = (Socket)obj;
            while (true)
            {
                byte[] buff = new byte[1024];
                int rec = clientsk.Receive(buff);
                string clientcommand = encoding.GetString(buff);
                string[] tokens = clientcommand.Split(new Char[] { '|' });

                if (tokens[0] == "Name")
                {
                    foreach (Socket sk in listClient)
                    {
                        if (sk != clientsk)
                        {
                            sk.Send(buff, buff.Length, SocketFlags.None);//gui ten dang ki
                        }
                    }

                }
                if (tokens[0] == "Message")
                {
                    foreach (Socket sk in listClient)
                    {
                        if (sk != clientsk)
                        {
                            sk.Send(buff, buff.Length, SocketFlags.None);//gui du lieu
                        }

                    }
                }
                if(tokens[0] == "Position")
                {
                    foreach (Socket sk in listClient)
                    {
                        if (sk != clientsk)
                        {
                            sk.Send(buff, buff.Length, SocketFlags.None);//gui du lieu
                        }

                    }

                }

            }//ket thuc while
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateServer();
            clientThread = new Thread(new ThreadStart(ListenConnect));
            clientThread.IsBackground = true;
            clientThread.Start();
        }

    }

}

