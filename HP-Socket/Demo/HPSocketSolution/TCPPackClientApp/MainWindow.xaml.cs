using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
using Models;

namespace TCPPackClientApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private HPSocketCS.TcpPackClient Client = new HPSocketCS.TcpPackClient();
        private ObservableCollection<Models.Person> _People = new ObservableCollection<Models.Person>();

        public ObservableCollection<Person> People
        {
            get
            {
                return _People;
            }

            set
            {
                _People = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Client.OnPrepareConnect += Client_OnPrepareConnect;
            Client.OnConnect += Client_OnConnect;
            Client.OnSend += Client_OnSend;
            Client.OnReceive += Client_OnReceive;
            Client.OnClose += Client_OnClose;
            Client.PackHeaderFlag = 0xff;
            Client.MaxPackSize = 0x1000;

            this.lsb_Contents.ItemsSource = People;
        }

        private HPSocketCS.HandleResult Client_OnClose(HPSocketCS.TcpClient sender, HPSocketCS.SocketOperation enOperation, int errorCode)
        {
            if (errorCode == 0)
            {
                AddMsg(sender.ConnectionId.ToString() + ",OnClose");
            }
            else
            {
                AddMsg(string.Format(" > [{0},OnError] -> OP:{1},CODE:{2},Msg:{3}", sender.ConnectionId, enOperation, errorCode, Client.ErrorMessage));
            }
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Client_OnReceive(HPSocketCS.TcpClient sender, byte[] bytes)
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Client_OnSend(HPSocketCS.TcpClient sender, byte[] bytes)
        {
            AddMsg(string.Format("Send Message,Length:{0}",bytes.Length));
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Client_OnConnect(HPSocketCS.TcpClient sender)
        {
            AddMsg("Connect Success,ConnectionID:" + sender.ConnectionId.ToString());
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Client_OnPrepareConnect(HPSocketCS.TcpClient sender, IntPtr socket)
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txt_SendContent.Text))
            {
                //HPSocketCS.WSABUF[] bufs = new HPSocketCS.WSABUF[5];
                //for (int i = 0; i < 5; i++)
                //{
                //    string SendMsg = "Hello Message " + i.ToString();
                //    byte[] bytes = Encoding.Default.GetBytes(SendMsg);
                //    IntPtr ptr = IntPtr.Zero;
                //    ptr = Marshal.AllocHGlobal(bytes.Length);
                //    Marshal.Copy(bytes, 0, ptr, bytes.Length);
                //    bufs[i].Length = bytes.Length;
                //    bufs[i].Buffer = ptr;
                //    if (ptr!=IntPtr.Zero)
                //    {
                //        Marshal.FreeHGlobal(ptr);
                //    }
                //}

                //if (Client.SendPackets(bufs,bufs.Length))
                //{
                //    AddMsg(string.Format("{0} Send Message Counts: {1}", Client.ConnectionId.ToString(), bufs.Length));
                //}
                //Client.SetExtra(new Models.ClientInfo() { ConnId = Client.ConnectionId, IpAddress="1111" });
                byte[] bytes = Encoding.Default.GetBytes(this.txt_SendContent.Text);
                if (Client.Send(bytes, bytes.Length))
                {
                    AddMsg(string.Format("{0} Send Message: {1}", Client.ConnectionId.ToString(), this.txt_SendContent.Text));
                }
            }
        }

        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txt_Server.Text.Trim()) || string.IsNullOrEmpty(this.txt_Port.Text.Trim()))
            {
                this.lsb_msg.Items.Add("Tip:Enter Server Ip &&Port Number");
                return;
            }
            string ServerIp = this.txt_Server.Text.Trim();
            ushort PortNum = ushort.Parse(this.txt_Port.Text.Trim());
            bool IsSuccess = Client.Connect(ServerIp, PortNum, (bool)this.cb_IsAsync.IsChecked);
            if (IsSuccess)
            {
                this.lsb_msg.Items.Add("Tip:Connect Server Success");
            }
            else
            {
                this.lsb_msg.Items.Add(string.Format("Tip:Connect Failed:ErrorMessage:{0},ErrorCode:{1}", Client.ErrorMessage, Client.ErrorCode));
            }
        }

        private void btn_DisConnect_Click(object sender, RoutedEventArgs e)
        {
            if (Client.Stop())
            {
                AddMsg("Stop Connection");
            }
            else
            {
                AddMsg("Stop Connection Failed");
            }
        }

        private void AddMsg(string message)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.lsb_msg.Items.Add(message);
            }));
        }

        private void btn_AddContent_Click(object sender, RoutedEventArgs e)
        {
            People.Add(new Person()
            {
                 Name = this.txt_Name.Text,
                 Age = int.Parse(this.txt_Age.Text),
                 Address = this.txt_Address.Text
            });
            this.lbl_Count.Content = this.People.Count;
        }

        private void btn_SendPackage_Click(object sender, RoutedEventArgs e)
        {
            HPSocketCS.WSABUF[] bufs = new HPSocketCS.WSABUF[this.People.Count];
            this.People.ToList().ForEach(p => 
            {
                int index = this.People.IndexOf(p);

                //byte[] bodybytes = Client.StructureToByte<Models.Person>(p);
                byte[] bodybytes = Client.ObjectToBytes(p);
                Models.PkgHeader header = new Models.PkgHeader();
                header.Id = index;
                header.BodySize = bodybytes.Length;
                byte[] headbytes = Client.StructureToByte<Models.PkgHeader>(header);
                
                IntPtr ptr = IntPtr.Zero;
                int totalsize = headbytes.Length + bodybytes.Length;
                ptr = Marshal.AllocHGlobal(totalsize);
                Marshal.Copy(headbytes, 0, ptr, headbytes.Length);
                Marshal.Copy(bodybytes, 0, ptr + headbytes.Length, bodybytes.Length);

                bufs[index].Length = totalsize;
                bufs[index].Buffer = ptr;
                if (ptr!=IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptr);
                }

            });
            if (Client.SendPackets(bufs,bufs.Length))
            {
                AddMsg(string.Format("{0} Send Message,Counts: {1}", Client.ConnectionId.ToString(), bufs.Length));
            }
            else
            {
                AddMsg(string.Format("{0} Send Error", Client.ConnectionId.ToString()));
            }
            this.People.Clear();

        }
    }
}
