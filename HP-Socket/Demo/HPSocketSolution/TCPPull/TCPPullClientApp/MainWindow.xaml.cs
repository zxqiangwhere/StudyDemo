using System;
using System.Collections.Generic;
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

namespace TCPPullClientApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        HPSocketCS.TcpPullClient Client = new HPSocketCS.TcpPullClient();
        int Id = 0;
        private List<Models.PkgHeader> SendMessagHeadLst = new List<Models.PkgHeader>();
        public MainWindow()
        {
            InitializeComponent();
            Client.OnHandShake += Client_OnHandShake;
            Client.OnPrepareConnect += Client_OnPrepareConnect;
            Client.OnConnect += Client_OnConnect;
            Client.OnSend += Client_OnSend;
            Client.OnReceive += Client_OnReceive;
            Client.OnClose += Client_OnClose;
        }

        private HPSocketCS.HandleResult Client_OnClose(HPSocketCS.TcpClient sender, HPSocketCS.SocketOperation enOperation, int errorCode)
        {
            if (errorCode ==0)
            {
                AddMsg(string.Format("{0} Closed Connect", sender.ConnectionId));
            }
            else
            {
                AddMsg(string.Format("{0} Connect Error,OP:{1},Code:{2}", sender.ConnectionId, enOperation, errorCode));
            }
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Client_OnReceive(HPSocketCS.TcpPullClient sender, int length)
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Client_OnSend(HPSocketCS.TcpClient sender, byte[] bytes)
        {
            AddMsg(string.Format("Client {0} Send Message,MessageId {1} ,lenght:{1}", sender.ConnectionId, SendMessagHeadLst.Last().Id, bytes.Length));
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Client_OnConnect(HPSocketCS.TcpClient sender)
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Client_OnPrepareConnect(HPSocketCS.TcpClient sender, IntPtr socket)
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Client_OnHandShake(HPSocketCS.TcpClient sender)
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txt_SendContent.Text.Trim()))
            {
                string SendMsg = this.txt_SendContent.Text.Trim();
                IntPtr SendPtr = IntPtr.Zero;
                //Body
                byte[] bodyBytes = Encoding.Default.GetBytes(SendMsg);
                //Head
                Models.PkgHeader header = new Models.PkgHeader();
                header.Id = ++Id;
                header.BodySize = bodyBytes.Length;
                SendMessagHeadLst.Add(header);
                byte[] headBytes = Client.StructureToByte<Models.PkgHeader>(header);

                byte[] totalBuffer = GetSendBuffer(headBytes, bodyBytes);

                //Send IntPtr
                SendPtr = Marshal.AllocHGlobal(headBytes.Length + bodyBytes.Length);
                Marshal.Copy(headBytes, 0, SendPtr, headBytes.Length);
                Marshal.Copy(bodyBytes, 0, SendPtr + headBytes.Length, bodyBytes.Length);
                
                if (Client.Send(SendPtr, headBytes.Length + bodyBytes.Length))
                {
                    AddMsg(string.Format("Send Message,Length", totalBuffer.Length));
                }
                else
                {
                    AddMsg("Send Failed");
                }

                //Send Bytes
                //if (Client.Send(totalBuffer,totalBuffer.Length))
                //{
                //    AddMsg(string.Format("Send Message,Length",totalBuffer.Length));
                //}
                //else
                //{
                //    AddMsg("Send Failed");
                //}

                if (SendPtr!=IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(SendPtr);
                }
                //Marshal.StructureToPtr(header, bufferPtr,true);
                //byte[] headBytes = new byte[Marshal.SizeOf(header)];
                //Marshal.Copy(bufferPtr, headBytes, 0, Marshal.SizeOf(header));

            }
        }
        private byte[] GetSendBuffer(byte[] HeadBuffer,byte[] BodyBuffer)
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                int totalSize = HeadBuffer.Length + BodyBuffer.Length;
                ptr = Marshal.AllocHGlobal(totalSize);
                Marshal.Copy(HeadBuffer, 0, ptr, HeadBuffer.Length);
                Marshal.Copy(BodyBuffer, 0, ptr + HeadBuffer.Length, BodyBuffer.Length);

                byte[] TotalBuffer = new byte[totalSize];
                Marshal.Copy(ptr, TotalBuffer, 0, totalSize);
                return TotalBuffer;

            }
            finally
            {
                if (ptr!= IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptr);
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
    }
}
