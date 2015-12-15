using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
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


namespace Caro
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
        public Button[][] BtnChessList;
        int[][] isempty;// bang 0 là chua danh, 1 là player1, 2 là player 2
        int chedochoi = 0;// bắng 1 thì choioffline, 2 choi voi máy.
        int luocchoi = 1;
        bool canplay = false;
        MessageStr serverMsg = new MessageStr();

        string textchat = "";


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreatButtonChess();
            CreateArrayChess();
        }
        void CreateArrayChess()
        {
            isempty = new int[12][];
            for (int i = 0; i < 12; i++)
            {
                isempty[i] = new int[12];
                for (int j = 0; j < 12; j++)
                {
                    isempty[i][j] = 0;
                }
            }

        }
        string nguoichoi= "";



        public void CreatButtonChess()//tạo các button để tạo bàn cờ
        {
            BtnChessList = new Button[12][];
            for (int i = 0; i < 12; i++)
            {
                BtnChessList[i] = new Button[12];
                for (int j = 0; j < 12; j++)
                {
                    //Button btChess = new Button();
                    BtnChessList[i][j] = new Button();
                    //BtnChessList[i][j].Name = Convert.ToString(i * 10 + j);
                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0)
                        {
                            BtnChessList[i][j].Background = Brushes.White;
                        }
                        else
                        {
                            BtnChessList[i][j].Background = Brushes.AntiqueWhite;
                        }
                    }
                    else
                    {
                        if (j % 2 == 0)
                        {
                            BtnChessList[i][j].Background = Brushes.AntiqueWhite;
                        }
                        else
                        {
                            BtnChessList[i][j].Background = Brushes.White;
                        }

                    }
                    gridChess.Children.Add(BtnChessList[i][j]);
                    Grid.SetRow(BtnChessList[i][j], i);
                    Grid.SetColumn(BtnChessList[i][j], j);
                    BtnChessList[i][j].Click += btnChess_Click;//tao su kien click cho button

                }
            }
        }
        private void btnChess_Click(object sender, RoutedEventArgs e)//hiện thông báo vị trí của cờ
        {
            Button bt = sender as Button;
            int column = Grid.GetColumn(bt);
            int row = Grid.GetRow(bt);
            //add hình vao button

            //Them hinh X
            Image imgX = new Image();
            imgX.Source = new BitmapImage(new Uri(@"../Resources/X.png", UriKind.Relative));
        
           
            StackPanel stackpnX = new StackPanel();
            stackpnX.Orientation = Orientation.Horizontal;
            stackpnX.Margin = new Thickness();
            stackpnX.Children.Add(imgX);
           // bt.Content = stackpn1;

            //Them hinh 0
            Image imgO = new Image();
            imgO.Source = new BitmapImage(new Uri(@"../Resources/O.png", UriKind.Relative));


            StackPanel stackpnO = new StackPanel();
            stackpnO.Orientation = Orientation.Horizontal;
            stackpnO.Margin = new Thickness();
            stackpnO.Children.Add(imgO);
            //bt.Content = stackpn;
            
            switch (chedochoi)
            {
                    //che do choi 2 ng offline
                case 1:
                    {
                        if (isempty[row][column] == 0)
                        {
                            //ve bieu tuong XO cho ô cờ khi click
                            if (luocchoi == 1)
                            {
                                bt.Content =stackpnX;
                                luocchoi = 2;
                                isempty[row][column] = 1;
                            }
                            else
                            {
                                bt.Content = stackpnO;
                                luocchoi = 1;
                                isempty[row][column] = 2;
                            }
                            if (KTCot(row, column) != 0 || KTDong(row,column) != 0 || KTCheo(row,column) != 0)
                            {
                                string thongbaowin = "";
                                thongbaowin = "Player" + Convert.ToString(isempty[row][column]) + "  win";
                                MessageBox.Show(thongbaowin, "Thông báo");
                                CreatButtonChess();
                                CreateArrayChess();
                                chedochoi = 0;
                                
                            }
                        }
                        break;
                    }//ket thuc case 1 danh 2 người
                case 2:
                    {
                        if (isempty[row][column] == 0)
                        {
                            //nguoi danh chet
                            bt.Content = stackpnX;
                            luocchoi = 2;
                            isempty[row][column] = 1;
                            //Kt nguoi win
                            if (KTCot(row, column) == 1 || KTDong(row, column) == 1 || KTCheo(row, column) == 1)
                            {
                                string thongbaowin = "";
                                thongbaowin = "Player  win";
                                MessageBox.Show(thongbaowin, "Thông báo");
                                CreatButtonChess();
                                CreateArrayChess();
                                chedochoi = 0;

                            }
                            else
                            {


                                //den luoc may danh

                                Point Buoctiep = new Point();
                                Buoctiep = TimKiemNuocdi();
                                int dongmay = (int)Buoctiep.X;
                                int cotmay = (int)Buoctiep.Y;

                                BtnChessList[dongmay][cotmay].Content = stackpnO;

                                //gan 2 vào mang isempty
                                isempty[dongmay][cotmay] = 2;

                                //kt computer win

                                if (KTCot(dongmay, cotmay) == 2 || KTDong(dongmay, cotmay) == 2 || KTCheo(dongmay, cotmay) == 2)
                                {
                                    string thongbaowin = "";
                                    thongbaowin = "Computer  win";
                                    MessageBox.Show(thongbaowin, "Thông báo");
                                    CreatButtonChess();
                                    CreateArrayChess();
                                    chedochoi = 0;
                                }

                            }


                        }
                        break;                    
                    }//ket thuc case 2 danh vs may
                case 3:
                    {
                        if (isempty[row][column] == 0)
                        {
                            if (luocchoi == 1)
                            {
                                //nguoi danh chet
                                bt.Content = stackpnX;
                                isempty[row][column] = 1;
                                luocchoi = 2;
                                if (KTCot(row, column) == 1 || KTDong(row, column) == 1 || KTCheo(row, column) == 1)
                                {
                                    string thongbaowin = "";
                                    thongbaowin = "You win!!!";
                                    MessageBox.Show(thongbaowin, "Thông báo");
                                    CreatButtonChess();
                                    CreateArrayChess();
                                    chedochoi = 0;

                                }
                                else
                                {
                                    string str_position = "Position" + '|' + name.Text + '|' + row.ToString() + '|' + column.ToString() + '|';
                                    byte[] buff = new byte[1024];
                                    buff = Encoding.GetBytes(str_position);
                                    client.Send(buff, buff.Length, SocketFlags.None);
                                    //luocchoi = 2;
                                }

                            }
                        }
                        
                        
                        break;
                    }
            }

        }

        #region KT WIN
        //ham kt dòng
        public int KTDong(int row, int col)
        {
            int _row = row - 1;
            int count = 1 ;
            while (_row >= 0 && isempty[_row][col] == isempty[row][col])//kt hàng doc phia ben trai
            {
                count++;
                _row--;
                if (count == 5)
                    return isempty[row][col];
            }
            _row = row + 1;
            while (_row < 12 && isempty[_row][col] == isempty[row][col])//kt hàng doc phia ben trai
            {
                count++;
                _row++;
                if (count == 5)
                    return isempty[row][col];
            }
            return 0;
            
        }

        //ham kt dòng
        public int KTCot(int row, int col)
        {
            int _col = col - 1;
            int count = 1;
            while (_col >= 0 && isempty[row][_col] == isempty[row][col])//kt hàng doc phia ben trai
            {
                count++;
                _col--;
                if (count == 5)
                    return isempty[row][col];
            }
            _col = col + 1;
            while (_col < 12 && isempty[row][_col] == isempty[row][col])//kt hàng doc phia ben trai
            {
                count++;
                _col++;
                if (count == 5)
                    return isempty[row][col];
            }
            return 0;

        }

        public int KTCheo(int row, int col)
        {
            int cheotren = 1;
            int cheoduoi = 1;
            
            //TH1 kt duong cheo tren
            int _row = row - 1;
            int _col = col - 1;
            while (_row >= 0 && _col >= 0 && isempty[_row][_col] == isempty[row][col])
            {
                cheotren++;
                _row--;
                _col--;
                if (cheotren == 5)
                    return isempty[row][col];
            }
             _row = row + 1;
             _col = col + 1;
             while (_row <12 && _col < 12 && isempty[_row][_col] == isempty[row][col])
             {
                 cheotren++;
                 _row++;
                 _col++;
                 if (cheotren == 5)
                     return isempty[row][col];
             }
            
            //TH2 Kt duong cheo duoi
             _row = row + 1;
             _col = col - 1;
             while (_row < 12 && _col >= 0 && isempty[_row][_col] == isempty[row][col])
             {
                 cheoduoi++;
                 _row++;
                 _col--;
                 if (cheoduoi == 5)
                     return isempty[row][col];
             }
             _row = row - 1;
             _col = col + 1;
             while (_row >=0 && _col < 12 && isempty[_row][_col] == isempty[row][col])
             {
                 cheoduoi++;
                 _row--;
                 _col++;
                 if (cheoduoi == 5)
                     return isempty[row][col];
             }
            

            return 0;
        }

        #endregion

        #region Khung chat
        private void _exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }   
        private void _change_Click(object sender, RoutedEventArgs e)
        {
            nguoichoi += name.Text.ToString();

            byte[] buff = new byte[1024];
            string inform = "Guest change to " + nguoichoi + " \n";
            chatbox.AppendText(inform);
            string buffstr = "Name|" + name.Text + '\n' + '|';
            buff = Encoding.GetBytes(buffstr);
            client.Send(buff, buff.Length, SocketFlags.None);
        }

        private void _send_Click(object sender, RoutedEventArgs e)
        {
            nguoichoi += name.Text.ToString();
            
           

            byte[] buff = new byte[1024];
            string buffmess = "Message|" + name.Text + '|' + message.Text + '\n' + '|';
            if (isConnected == true)
            {
                buff = Encoding.GetBytes(buffmess);
                client.Send(buff, buff.Length, SocketFlags.None);
            }
            
            Dispatcher.Invoke(new Action(() =>
            {

                nguoichoi += name.Text.ToString();

                textchat += nguoichoi;
                textchat += " : ";
                textchat += message.Text.ToString();
                textchat += "\t(";
                textchat += DateTime.Now.ToString("hh:mm  dd/MM/yyyy)\n");
                chatbox.AppendText(textchat);
                message.Clear();
            }));


        }

        private void choi2nguoi_Click(object sender, RoutedEventArgs e)
        {
            chedochoi = 1;
            CreateArrayChess();
            Choivoimay.IsEnabled = false;
            Nguoichoionline.IsEnabled = false;
        }

        private void Choivoimay_Click(object sender, RoutedEventArgs e)
        {
            chedochoi = 2;
            CreateArrayChess();
            choi2nguoi.IsEnabled = false;
            Nguoichoionline.IsEnabled = false;

           
        }
        #endregion

        // xử lý code máy đánh 
        #region AI

       // private long[] Mangphongngu = new long[6] { 0, 1, 9, 81, 729, 6561 };
        //private long[] Mangtancong = new long[6] { 0, 2, 18, 162, 1458, 13112 };



        //private long[] Mangphongngu = new long[6] {   0, 9, 81, 729, 6561, 54049};
        private long[] Mangphongngu = new long[6] { 0, 3, 27, 243, 2187, 19683 };
        private long[] Mangtancong = new long[6] { 0, 9, 81, 729, 6561, 54049 };

        public Point TimKiemNuocdi()
        {
            Point nuocdi = new Point();
            long DiemMax = 0;
            for (int i = 0; i < 12; i++)
            {
                for(int j=0; j<12; j++)
                {
                    if (isempty[i][j] == 0)
                    {
                        long DiemTanCong = DiemTC_Doc(i, j) + DiemTC_Ngang(i, j) + DiemTC_CheoXuoi(i, j) + DiemTC_CheoNguoc(i, j);
                        long DiemPhongNgu = DiemPN_Doc(i, j) + DiemPN_Ngang(i, j) + DiemPN_CheoXuoi(i, j) + DiemPN_CheoNguoc(i, j);
                        long Diemtam = DiemPhongNgu > DiemTanCong ? DiemPhongNgu : DiemTanCong;
                        if (Diemtam > DiemMax)
                        {
                            DiemMax = Diemtam;
                            nuocdi.X = i;
                            nuocdi.Y = j;
                        }
                    }
                }
            }

            return nuocdi;
        }

        //xu ly tan cong
        public long DiemTC_Doc(int dong, int cot)
        {
            long DiemTong = 0;
            long Socomay = 0;
            long Soconguoi = 0;
            for (int Dem = 1; Dem < 5 && dong + Dem < 12; Dem++)
            {
                if (isempty[dong + Dem][cot] == 1)
                {
                    Soconguoi++;
                    break;
                }
                else if (isempty[dong + Dem][cot] == 2)
                {
                    Socomay++;
                }
                else
                    break;
            }
            for (int Dem = 1; Dem < 5 && dong - Dem >= 0; Dem++)
            {
                if (isempty[dong - Dem][cot] == 1)
                {
                    Soconguoi++;
                    break;
                }
                else if (isempty[dong - Dem][cot] == 2)
                {
                    Socomay++;
                }
                else
                    break;
            }


            DiemTong -= Mangphongngu[Soconguoi + 1];
            DiemTong += Mangtancong[Socomay];
            return DiemTong;

        }
  
        public long DiemTC_Ngang(int dong, int cot)
        {
            long DiemTong = 0;
            long Socomay = 0;
            long Soconguoi = 0;
            for (int Dem = 1; Dem < 5 && cot + Dem < 12; Dem++)
            {
                if (isempty[dong][cot + Dem] == 1)
                {
                    Soconguoi++;
                    break;
                }
                else if (isempty[dong][cot + Dem] == 2)
                {
                    Socomay++;
                }
                else
                    break;
            }
            for (int Dem = 1; Dem < 5 && cot - Dem >= 0; Dem++)
            {
                if (isempty[dong][cot -Dem] == 1)
                {
                    Soconguoi++;
                    break;
                }
                else if (isempty[dong][cot - Dem] == 2)
                {
                    Socomay++;
                }
                else
                    break;
            }



            DiemTong -= Mangphongngu[Soconguoi + 1];
            DiemTong += Mangtancong[Socomay];

            return DiemTong;

        }
        public long DiemTC_CheoXuoi(int cot, int dong)
        {
            long DiemTong = 0;
            long Socomay = 0;
            long Soconguoi = 0;
            for (int Dem = 1; Dem < 5 && cot -  Dem >= 0 && dong - Dem >=0; Dem++)
            {
                if (isempty[dong - Dem][cot - Dem] == 1)
                {
                    Soconguoi++;
                    break;
                }
                else if (isempty[dong - Dem][cot - Dem] == 2)
                {
                    Socomay++;
                }
                else
                    break;
            }
            for (int Dem = 1; Dem < 5 && cot + Dem <12 && dong + Dem < 12; Dem++)
            {
                if (isempty[dong + Dem][cot + Dem] == 1)
                {
                    Soconguoi++;
                    break;
                }
                else if (isempty[dong + Dem][cot + Dem] == 2)
                {
                    Socomay++;
                }
                else
                    break;
            }


           
            DiemTong -= Mangphongngu[Soconguoi + 1];
            DiemTong += Mangtancong[Socomay];



            return DiemTong;

        }
        public long DiemTC_CheoNguoc(int dong, int cot)
        {
            long DiemTong = 0;
            long Socomay = 0;
            long Soconguoi = 0;
            for (int Dem = 1; Dem < 5 && cot - Dem >= 0 && dong + Dem < 12; Dem++)
            {
                if (isempty[dong + Dem][cot - Dem] == 1)
                {
                    Soconguoi++;
                    break;
                }
                else if (isempty[dong + Dem][cot - Dem] == 2)
                {
                    Socomay++;
                }
                else
                    break;
            }
            for (int Dem = 1; Dem < 5 && cot + Dem < 12 && dong - Dem >= 0; Dem++)
            {
                if (isempty[dong - Dem][cot + Dem] == 1)
                {
                    Soconguoi++;
                    break;
                }
                else if (isempty[dong - Dem][cot + Dem] == 2)
                {
                    Socomay++;
                }
                else
                    break;
            }


            DiemTong -= Mangphongngu[Soconguoi + 1];
            DiemTong += Mangtancong[Socomay];



            return DiemTong;

        }

        //xu ly phong thu
        public long DiemPN_Doc(int dong, int cot)
        {
            long DiemTong = 0;
            long Socomay = 0;
            long Soconguoi = 0;
            for (int Dem = 1; Dem < 5 && dong + Dem < 12; Dem++)
            {
                if (isempty[dong + Dem][cot] == 1)
                {
                    Soconguoi++;
                    
                }
                else if (isempty[dong + Dem][cot] == 2)
                {
                    Socomay++;
                    break;
                }
                else
                    break;
            }
            for (int Dem = 1; Dem < 5 && dong - Dem >= 0; Dem++)
            {
                if (isempty[dong - Dem][cot] == 1)
                {
                    Soconguoi++;
                    
                }
                else if (isempty[dong - Dem][cot] == 2)
                {
                    Socomay++;
                    break;
                }
                else
                    break;
            }


            DiemTong += Mangphongngu[Soconguoi];
            return DiemTong;

        }

        public long DiemPN_Ngang(int dong, int cot)
        {
            long DiemTong = 0;
            long Socomay = 0;
            long Soconguoi = 0;
            for (int Dem = 1; Dem < 5 && cot + Dem < 12; Dem++)
            {
                if (isempty[dong][cot + Dem] == 1)
                {
                    Soconguoi++;
                    
                }
                else if (isempty[dong][cot + Dem] == 2)
                {
                    Socomay++;
                    break;
                }
                else
                    break;
            }
            for (int Dem = 1; Dem < 5 && cot - Dem >= 0; Dem++)
            {
                if (isempty[dong][cot - Dem] == 1)
                {
                    Soconguoi++;
                    
                }
                else if (isempty[dong][cot - Dem] == 2)
                {
                    Socomay++;
                    break;
                }
                else
                    break;
            }



            DiemTong += Mangphongngu[Soconguoi];
            


            return DiemTong;

        }
        public long DiemPN_CheoXuoi(int dong, int cot)
        {
            long DiemTong = 0;
            long Socomay = 0;
            long Soconguoi = 0;
            for (int Dem = 1; Dem < 5 && cot - Dem >= 0 && dong - Dem >= 0; Dem++)
            {
                if (isempty[dong - Dem][cot - Dem] == 1)
                {
                    Soconguoi++;
                    
                }
                else if (isempty[dong - Dem][cot - Dem] == 2)
                {
                    Socomay++;
                    break;
                }
                else
                    break;
            }
            for (int Dem = 1; Dem < 5 && cot + Dem < 12 && dong + Dem < 12; Dem++)
            {
                if (isempty[dong + Dem][cot + Dem] == 1)
                {
                    Soconguoi++;
                    
                }
                else if (isempty[dong + Dem][cot + Dem] == 2)
                {
                    Socomay++;
                    break;
                }
                else
                    break;
            }



            DiemTong += Mangphongngu[Soconguoi];
            



            return DiemTong;

        }
        public long DiemPN_CheoNguoc(int dong, int cot)
        {
            long DiemTong = 0;
            long Socomay = 0;
            long Soconguoi = 0;
            for (int Dem = 1; Dem < 5 && cot - Dem >= 0 && dong + Dem < 12; Dem++)
            {
                if (isempty[dong + Dem][cot - Dem] == 1)
                {
                    Soconguoi++;
                    
                }
                else if (isempty[dong + Dem][cot - Dem] == 2)
                {
                    Socomay++;
                    break;
                }
                else
                    break;
            }
            for (int Dem = 1; Dem < 5 && cot + Dem < 12 && dong - Dem >= 0; Dem++)
            {
                if (isempty[dong - Dem][cot + Dem] == 1)
                {
                    Soconguoi++;
                    
                }
                else if (isempty[dong - Dem][cot + Dem] == 2)
                {
                    Socomay++;
                    break;
                }
                else
                    break;
            }


            DiemTong += Mangphongngu[Soconguoi];
            
            return DiemTong;

        }

        #endregion

        #region Ket noi server
        Socket client;
        IPEndPoint ipe;
        Thread ketnoi;
        static ASCIIEncoding Encoding = new ASCIIEncoding();
        bool isConnected = false;


       
        //click nguoi choi online thu hien connect
        private void Nguoichoionline_Click(object sender, RoutedEventArgs e)
        {
            chedochoi = 3;
            Mayonline.IsEnabled = false;
            ketnoi = new Thread(Ketnoidenserver);
            ketnoi.IsBackground = true;
            ketnoi.Start();
            isConnected = true;
        }

        private void Ketnoidenserver(object obj)
        {
            ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7890);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            client.Connect(ipe);
            isConnected = true;
            Thread Langnghe = new Thread(LangngheDulieu);
            Langnghe.IsBackground = true;
            Langnghe.Start(client);
            //MessageBox.Show("Không thê kết nối");
        }


        private void LangngheDulieu(object obj)
        {
            Socket sk = obj as Socket;
            while (true)
            {
                byte[] buff = new byte[1024];
                int recv = client.Receive(buff);//nhận gói tin từ server lưu vào buff
                Mahoa(buff);
                
            }
        }

        private void Mahoa(byte[] buff)
        {
            string command = System.Text.Encoding.ASCII.GetString(buff);
            string[] tokens = command.Split(new Char[] { '|' });
            if(tokens[0]=="Name")
            {
                string messageinform ="Player "+tokens[1]+" is connected!!!\n";
                 Dispatcher.Invoke(new Action(() =>
               {
                   chatbox.AppendText(messageinform);
               }));
            }

            if (tokens[0] == "Message")//nếu gửi chuỗi tin nhắn
            {
                string Message = tokens[1] + ": ";
                Message += tokens[2] + "   (";
                Message += DateTime.Now.ToString("hh:mm  dd/MM/yyyy)");//ngày giờ hệ thống
                Message += '\n';
                Dispatcher.Invoke(new Action(() =>
               {
                    chatbox.AppendText(Message);
                }));

            }
            if (tokens[0] == "Position")
            {
                //int type = int.Parse(tokens[4]);
                string name = tokens[1];
                int row = int.Parse(tokens[2]);
                int column = int.Parse(tokens[3]);

                
                Dispatcher.Invoke(new Action(() =>
                {
                    Image imgO = new Image();
                    imgO.Source = new BitmapImage(new Uri(@"../Resources/O.png", UriKind.Relative));


                    StackPanel stackpnO = new StackPanel();
                    stackpnO.Orientation = Orientation.Horizontal;
                    stackpnO.Margin = new Thickness();
                    stackpnO.Children.Add(imgO);
                    //lay button tai vi tri row, column

                    BtnChessList[row][column].Content = stackpnO;
                    isempty[row][column] = 2;
                    luocchoi = 1;
                    if (KTCot(row, column) == 1 || KTDong(row, column) == 1 || KTCheo(row, column) == 1)
                    {
                        CreatButtonChess();
                        CreateArrayChess();
                        chedochoi = 0;
                        MessageBox.Show("You lost!!!");//đối thủ thắng
                        
                    }
                }));
                
            }
    
        }


        


        #endregion

        private void Maychoionline_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
