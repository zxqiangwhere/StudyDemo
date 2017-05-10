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

namespace TCPPullServerApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        HPSocketCS.TcpPullServer<Models.ClientInfo> Server = new HPSocketCS.TcpPullServer<Models.ClientInfo>();
        ObservableCollection<Models.ClientInfo> _clients = new ObservableCollection<Models.ClientInfo>();

        public ObservableCollection<Models.ClientInfo> Clients
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
        private int PkgHeaderSize = Marshal.SizeOf(new Models.PkgHeader());
        private bool IsIgnore = false;
        public MainWindow()
        {
            InitializeComponent();
            Server.OnPrepareListen += Server_OnPrepareListen;
            Server.OnAccept += Server_OnAccept;

            Server.OnSend += Server_OnSend;
            Server.OnReceive += Server_OnReceive;
            //Server.OnPointerDataReceive += Server_OnPointerDataReceive;
            Server.OnClose += Server_OnClose;

            Server.OnShutdown += Server_OnShutdown;
            this.lsb_ClientLst.ItemsSource = this.Clients;
            this.lsb_ClientLst.DisplayMemberPath = "IpAddress";
        }

        private HPSocketCS.HandleResult Server_OnShutdown()
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Server_OnClose(IntPtr connId, HPSocketCS.SocketOperation enOperation, int errorCode)
        {
            if (errorCode == 0)
            {
                AddMsg(string.Format("{0} Closed Connect", connId));
            }
            else
            {
                AddMsg(string.Format("{0} Connect Error,OP:{1},Code:{2}", connId, enOperation, errorCode));
            }
            this.Dispatcher.BeginInvoke(new Action(() => 
            {
                this.Clients.Remove(this.Clients.Where(p => p.ConnId == connId).FirstOrDefault());
            }));
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Server_OnPointerDataReceive(IntPtr connId, IntPtr pData, int length)
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Server_OnReceive(IntPtr connId, int length)
        {
            var client = Server.GetExtra(connId);
            if (client == null)
            {
                AddMsg(string.Format("Receive Msg From {0} , Error: {1}", connId, Server.ErrorMessage));
                return HPSocketCS.HandleResult.Error;
            }
            if (IsIgnore)
            {
                return HPSocketCS.HandleResult.Ignore;
            }
            Models.PkgInfo pkginfo = client.PkgInfo;
            int requiredLegth = pkginfo.Length;
            int remainLength = length;
            while (remainLength>=requiredLegth)
            {
                IntPtr bufferPtr = IntPtr.Zero;
                remainLength -= requiredLegth;
                bufferPtr = Marshal.AllocHGlobal(requiredLegth);
                if (Server.Fetch(connId,bufferPtr,requiredLegth) == HPSocketCS.FetchResult.Ok)
                {
                    if (pkginfo.IsHeader)
                    {
                        Models.PkgHeader head = (Models.PkgHeader)Marshal.PtrToStructure(bufferPtr, typeof(Models.PkgHeader));
                        AddMsg(string.Format("Receive Msg from {0},Buffer Total Size {1}, Head Id {2},BodySize {3}", connId,length, head.Id, head.BodySize));
                        requiredLegth = head.BodySize;
                    }
                    else
                    {
                        string ReceMsg = Marshal.PtrToStringAnsi(bufferPtr,requiredLegth);
                        AddMsg(string.Format("Receive Msg from {0},Buffer Total Size {1},Msg:{2}", connId, length, ReceMsg));
                        requiredLegth = PkgHeaderSize;
                    }
                    pkginfo.IsHeader = !pkginfo.IsHeader;

                }
                if (bufferPtr!=IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(bufferPtr);
                }

            }
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Server_OnSend(IntPtr connId, byte[] bytes)
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Server_OnAccept(IntPtr connId, IntPtr pClient)
        {
            string ip = string.Empty;
            ushort port = 0;
            if (Server.GetRemoteAddress(connId, ref ip, ref port))
            {
                AddMsg(string.Format("{0} OnAccept,IP {1},Port {2}", connId, ip, port));
            }
            else
            {
                AddMsg(string.Format("{0} OnAccept,Error", connId));
            }
            //设置附加信息
            Models.ClientInfo client = new Models.ClientInfo()
            {
                ConnId = connId,
                IpAddress = ip,
                Port = port,
                PkgInfo = new Models.PkgInfo()
                {
                    IsHeader = true,
                    Length = PkgHeaderSize
                },
            };
            if (Server.SetExtra(connId,client)==false)
            {
                AddMsg(string.Format("{0} Accept,SetConnectionExtra Fiale", connId));
                return HPSocketCS.HandleResult.Error;
            }
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                Clients.Add(client);
            }));
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult Server_OnPrepareListen(IntPtr soListen)
        {
            return HPSocketCS.HandleResult.Ok;
        }

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txt_Server.Text)&&!string.IsNullOrEmpty(this.txt_Port.Text))
            {
                string ip = this.txt_Server.Text;
                ushort port = ushort.Parse(this.txt_Port.Text);
                Server.IpAddress = ip;
                Server.Port = port;
                if (Server.Start())
                {
                    AddMsg("Server Start Success");
                }
                else
                {
                    AddMsg(string.Format("Server Start Failed,ErrorCode {0},ErrorMsg {1}", Server.ErrorCode, Server.ErrorMessage));
                }
            }
            
        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (Server.Stop())
            {
                AddMsg("Stop Server Success");
                this.Clients.Clear();
            }
            else
            {
                AddMsg(string.Format("Stop Server Failed,ErrorCode {0},ErrorMsg {1}", Server.ErrorCode, Server.ErrorMessage));
            }
        }

        private void btn_disConnect_Click(object sender, RoutedEventArgs e)
        {
            if (this.lsb_ClientLst.SelectedItem != null)
            {
                Models.ClientInfo Selected = (Models.ClientInfo)this.lsb_ClientLst.SelectedItem;
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

        private void ch_IgnoreMsg_Checked(object sender, RoutedEventArgs e)
        {
            IsIgnore = true;
        }

        private void ch_IgnoreMsg_Unchecked(object sender, RoutedEventArgs e)
        {
            IsIgnore = false;
        }

        private void btn_PullData_Click(object sender, RoutedEventArgs e)
        {
            if (!IsIgnore)
            {
                return;
            }
            bool isHead = true;
            int peekLength = PkgHeaderSize;
            IntPtr bufferPtr = IntPtr.Zero;
            bufferPtr = Marshal.AllocHGlobal(peekLength);

            while (Server.Fetch(this.Clients.First().ConnId,bufferPtr,peekLength) == HPSocketCS.FetchResult.Ok)
            {
                if (isHead)
                {
                    Models.PkgHeader head =(Models.PkgHeader)Marshal.PtrToStructure(bufferPtr, typeof(Models.PkgHeader));
                    peekLength = head.BodySize;
                }
                else
                {
                    string ReceMsg = Marshal.PtrToStringAnsi(bufferPtr, peekLength);
                    AddMsg(string.Format("Receive Msg from {0},Msg:{1}", this.Clients.First().ConnId, ReceMsg));
                    peekLength = PkgHeaderSize;
                }
                isHead = !isHead;
                if (bufferPtr!=IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(bufferPtr);
                }
                bufferPtr = Marshal.AllocHGlobal(peekLength);
            }
        }
    }
}
