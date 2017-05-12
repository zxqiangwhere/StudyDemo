using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace UDPServerApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        HPSocketCS.UdpServer Server = new HPSocketCS.UdpServer();
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
            Server.OnReceive += Server_OnReceive;
            Server.OnClose += Server_OnClose;
            Server.OnShutdown += Server_OnShutdown;
            this.lsb_ClientLst.ItemsSource = Clients;
            this.lsb_ClientLst.DisplayMemberPath = "IpAddress";
        }

        private HPSocketCS.HandleResult Server_OnShutdown()
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Server_OnClose(IntPtr connId, HPSocketCS.SocketOperation enOperation, int errorCode)
        {
            string ip = string.Empty;
            ushort port = 0;
            Server.GetRemoteAddress(connId, ref ip, ref port);
            AddMsg(string.Format("Client {0} Disconnect,IP:{1}", connId.ToString(), ip));
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Server_OnReceive(IntPtr connId, byte[] bytes)
        {
            string ReceMsg = Encoding.Default.GetString(bytes);
            AddMsg(string.Format("Receive <<{0}>> From {1}", ReceMsg, connId.ToString()));
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Server_OnSend(IntPtr connId, byte[] bytes)
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Server_OnAccept(IntPtr connId, IntPtr pClient)
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
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Server_OnHandShake(IntPtr connId)
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Server_OnPrepareListen(IntPtr soListen)
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private void btn_disConnect_Click(object sender, RoutedEventArgs e)
        {
            if (this.lsb_ClientLst.SelectedItem != null)
            {
                ClientInfo Selected = (ClientInfo)this.lsb_ClientLst.SelectedItem;
                Server.Disconnect(Selected.ConnId);
            }
        }

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txt_Server.Text) && !string.IsNullOrEmpty(this.txt_Port.Text))
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
        private void AddMsg(string message)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.lsb_msg.Items.Add(message);
            }));
        }
    }
}
