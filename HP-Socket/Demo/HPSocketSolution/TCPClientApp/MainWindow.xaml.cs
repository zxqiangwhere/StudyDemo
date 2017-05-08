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

namespace TCPClientApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private HPSocketCS.TcpClient client;
        private IntPtr ClientId;
        public MainWindow()
        {
            InitializeComponent();
            client = new HPSocketCS.TcpClient();
            client.OnPrepareConnect += Client_OnPrepareConnect;
            client.OnConnect += Client_OnConnect;
            client.OnSend += Client_OnSend;
            client.OnReceive += Client_OnReceive;
            client.OnClose += Client_OnClose;
        }

        private HPSocketCS.HandleResult Client_OnClose(HPSocketCS.TcpClient sender, HPSocketCS.SocketOperation enOperation, int errorCode)
        {
            //throw new NotImplementedException();
            if (errorCode == 0)
            {
                AddMsg(sender.ConnectionId.ToString() + ",OnClose");
            }
            else
            {
                AddMsg(string.Format(" > [{0},OnError] -> OP:{1},CODE:{2}", sender.ConnectionId, enOperation, errorCode));
            }
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Client_OnReceive(HPSocketCS.TcpClient sender, byte[] bytes)
        {
            throw new NotImplementedException();
        }

        private HPSocketCS.HandleResult Client_OnSend(HPSocketCS.TcpClient sender, byte[] bytes)
        {
            throw new NotImplementedException();
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

        private void AddMsg(string message)
        {
            this.Dispatcher.BeginInvoke(new Action(() => 
            {
                this.lsb_msg.Items.Add(message);
            }));
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
            bool IsSuccess = client.Connect(ServerIp, PortNum, (bool)this.cb_IsAsync.IsChecked);
            if (IsSuccess)
            {
                this.lsb_msg.Items.Add("Tip:Connect Server Success");
            }
            else
            {
                this.lsb_msg.Items.Add(string.Format("Tip:Connect Failed:ErrorMessage:{0},ErrorCode:{1}", client.ErrorMessage, client.ErrorCode));
            }
            //if (!(bool)this.cb_IsAsync.IsChecked)
            //{

            //}

        }

        private void btn_DisConnect_Click(object sender, RoutedEventArgs e)
        {
            if (client.Stop())
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
                if (client.Send(bytes, bytes.Length))
                {
                    AddMsg(string.Format("{0} Send Message: {1}", client.ConnectionId.ToString(), SendMsg));
                }
            }

        }

        private void btn_SendFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_SendSerializableObject_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
