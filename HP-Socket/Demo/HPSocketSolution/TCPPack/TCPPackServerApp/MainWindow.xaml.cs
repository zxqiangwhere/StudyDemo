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

namespace TCPPackServerApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private int PkgHeaderSize = Marshal.SizeOf(new Models.PkgHeader());
        HPSocketCS.TcpPackServer<Models.ClientInfo> Server = new HPSocketCS.TcpPackServer<Models.ClientInfo>();

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
        public MainWindow()
        {
            InitializeComponent();
            Server.OnPrepareListen += Server_OnPrepareListen;
            Server.OnAccept += Server_OnAccept;
            Server.OnSend += Server_OnSend;
            Server.OnReceive += Server_OnReceive;
            Server.OnClose += Server_OnClose;
            Server.OnShutdown += Server_OnShutdown;

            Server.PackHeaderFlag = 0xff;
            Server.MaxPackSize = 0x1000;




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

        private HPSocketCS.HandleResult Server_OnReceive(IntPtr connId, byte[] bytes)
        {
            var clientInfo = Server.GetExtra<Models.ClientInfo>(connId);
            if (clientInfo != null)
            {
                if (bytes.Length<=20)
                {
                    AddMsg(string.Format(" > [{0},OnReceive] -> {1}:{2} ({3} bytes,Content:{4})", clientInfo.ConnId, clientInfo.IpAddress, clientInfo.Port, bytes.Length, Encoding.Default.GetString(bytes)));
                    

                }
                else
                {
                    //解析Package
                    Models.PkgInfo pkginfo = clientInfo.PkgInfo;
                    int NeedLength = pkginfo.Length;
                    int RemainLength = bytes.Length;
                    int StartIndex = 0;
                    IntPtr ptr = IntPtr.Zero;
                    while (RemainLength>=NeedLength)
                    {
                        RemainLength -= NeedLength;
                        ptr = Marshal.AllocHGlobal(NeedLength);
                        Marshal.Copy(bytes, StartIndex, ptr, NeedLength);
                        StartIndex += NeedLength;
                        if (pkginfo.IsHeader)
                        {
                            Models.PkgHeader head = Marshal.PtrToStructure<Models.PkgHeader>(ptr);
                            NeedLength = head.BodySize;
                        }
                        else
                        {
                            byte[] bodybytes = new byte[NeedLength];
                            Marshal.Copy(ptr, bodybytes, 0, NeedLength);
                            Models.Person person = (Models.Person)Server.BytesToObject(bodybytes);
                            AddMsg(string.Format(" {0},OnReceive -> Name:{1},Age:{2},Address:{3}", clientInfo.ConnId, person.Name, person.Age, person.Address));
                            NeedLength = PkgHeaderSize;
                        }
                        pkginfo.IsHeader = !pkginfo.IsHeader;
                        if (ptr!=IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(ptr);
                        }
                    }
                }
                return HPSocketCS.HandleResult.Ok;
            }
            else
            {
                AddMsg(string.Format(" > [{0},OnReceive] -> ({1} bytes)", connId, bytes.Length));
                return HPSocketCS.HandleResult.Error;
            }
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
            ////设置附加信息
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
            if (Server.SetExtra(connId, client) == false)
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
            if (!string.IsNullOrEmpty(this.txt_Server.Text) && !string.IsNullOrEmpty(this.txt_Port.Text))
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
    }
}
