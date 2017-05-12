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

namespace UDPClientApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private HPSocketCS.UdpClient Client;
        public MainWindow()
        {
            InitializeComponent();
            Client = new HPSocketCS.UdpClient();
            Client.OnPrepareConnect += Client_OnPrepareConnect;
            Client.OnConnect += Client_OnConnect;
            Client.OnSend += Client_OnSend;
            Client.OnReceive += Client_OnReceive;
            Client.OnClose += Client_OnClose;
        }

        private HPSocketCS.HandleResult Client_OnClose(HPSocketCS.UdpClient sender, HPSocketCS.SocketOperation enOperation, int errorCode)
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Client_OnReceive(HPSocketCS.UdpClient sender, byte[] bytes)
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Client_OnSend(HPSocketCS.UdpClient sender, byte[] bytes)
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Client_OnConnect(HPSocketCS.UdpClient sender)
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Client_OnPrepareConnect(HPSocketCS.UdpClient sender, IntPtr socket)
        {
            return HPSocketCS.HandleResult.Ok;
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

        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txt_SendContent.Text))
            {
                string SendMsg = this.txt_SendContent.Text;
                byte[] bytes = Encoding.Default.GetBytes(SendMsg);
                if (Client.Send(bytes, bytes.Length))
                {
                    AddMsg(string.Format("{0} Send Message: {1}", Client.ConnectionId.ToString(), SendMsg));
                }
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
