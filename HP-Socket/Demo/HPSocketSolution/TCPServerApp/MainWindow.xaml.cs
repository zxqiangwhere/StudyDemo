using System;
using System.Collections.Generic;
using System.Linq;
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
using HPSocketCS;
using System.Collections.ObjectModel;

namespace TCPServerApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        HPSocketCS.TcpServer Server = new TcpServer();
        ObservableCollection<ClientInfo> _clients = new ObservableCollection<ClientInfo>();

        public ObservableCollection<ClientInfo> Clients
        {
            get
            {
                return _clients;
            }

            set
            {
                _clients = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Server.OnPrepareListen += Server_OnPrepareListen;
            Server.OnHandShake += Server_OnHandShake;
            Server.OnAccept += Server_OnAccept;
            Server.OnSend += Server_OnSend;
            //Server.OnPointerDataReceive += Server_OnPointerDataReceive;
            Server.OnReceive += Server_OnReceive;
            Server.OnClose += Server_OnClose;
            Server.OnShutdown += Server_OnShutdown;
            this.lsb_ClientLst.ItemsSource = Clients;
            this.lsb_ClientLst.DisplayMemberPath = "IpAddress";


        }

        private HandleResult Server_OnHandShake(IntPtr connId)
        {
            //throw new NotImplementedException();
            return HandleResult.Ok;
        }

        private HandleResult Server_OnShutdown()
        {
            //throw new NotImplementedException();
            return HandleResult.Ok;
        }

        private HandleResult Server_OnClose(IntPtr connId, SocketOperation enOperation, int errorCode)
        {
            //throw new NotImplementedException();
            AddMsg(string.Format("Client {0} Disconnect,IP:{1}", connId.ToString(), Clients.Where(p => p.ConnId == connId).First().IpAddress));
            return HandleResult.Ok;
        }

        private HandleResult Server_OnReceive(IntPtr connId, byte[] bytes)
        {
            //throw new NotImplementedException();
            AddMsg(string.Format("Receive Data From {0}", connId.ToString()));
            return HandleResult.Ok;
        }

        private HandleResult Server_OnPointerDataReceive(IntPtr connId, IntPtr pData, int length)
        {
            return HandleResult.Ok;
        }

        private HandleResult Server_OnSend(IntPtr connId, byte[] bytes)
        {
            throw new NotImplementedException();
        }

        private HandleResult Server_OnAccept(IntPtr connId, IntPtr pClient)
        {
            string ClientIp = string.Empty;
            ushort CLientPort = 0;
            Server.GetRemoteAddress(connId, ref ClientIp, ref CLientPort);
            ClientInfo client = new ClientInfo()
            {
                ConnId = connId,
                IpAddress = ClientIp,
                Port = CLientPort
            };
            this.Dispatcher.BeginInvoke(new Action(() => 
            {
                Clients.Add(client);
            }));
            AddMsg(string.Format("Receive Connect From {0},IP:{1},Port:{2}", connId.ToString(), ClientIp, CLientPort));
            return HandleResult.Ok;
        }

        private HandleResult Server_OnPrepareListen(IntPtr soListen)
        {
            return HandleResult.Ok;
        }

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txt_Server.Text)&&!string.IsNullOrEmpty(this.txt_Port.Text))
            {
                Server.IpAddress = this.txt_Server.Text;
                Server.Port = ushort.Parse(this.txt_Port.Text);
                if (Server.Start())
                {
                    AddMsg("Start Server Success");
                }
                else
                {
                    AddMsg("Start Server Failed");
                }
            }
        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (Server.Stop())
            {
                AddMsg("Stop Server Success");
            }
            else
            {
                AddMsg(string.Format("Start Server Failed,ErrrMsg:{0},ErrorCode:{1}", Server.ErrorMessage, Server.ErrorCode));
            }
        }
       

        private void btn_disConnect_Click(object sender, RoutedEventArgs e)
        {
            if (this.lsb_ClientLst.SelectedItem != null)
            {
                ClientInfo Selected = (ClientInfo)this.lsb_ClientLst.SelectedItem;
                Server.Disconnect(Selected.ConnId);
            }
        }
        private void AddMsg(string message)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.lsb_msg.Items.Add(message);
            }));
        }
    }
    public class ClientInfo
    {
        public IntPtr ConnId { get; set; }
        public string IpAddress { get; set; }
        public ushort Port { get; set; }
    }
}
