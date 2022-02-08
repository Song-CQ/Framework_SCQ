using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using Fleck;

namespace XLNet
{
    public class XLSocket
    {
        private Socket skt = null;
        

        public XLSocket()
        {
            skt = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        }

        #region Server
        private int backlog = 10;
        public void StartAsServer(string ipStr, int port)
        {
            try
            {
                //转换ip
                IPAddress ip = IPAddress.Parse(ipStr);
                //创建ip和端口号
                IPEndPoint endPoint = new IPEndPoint(ip, port);
                //Socket绑定端口和ip
                skt.Bind(endPoint);
                //并设置可等待挂起的数量
                skt.Listen(backlog);
                //开始监听 第二个参数转换成了回调的 ar.AsyncState
                skt.BeginAccept(ClientConnectCB, skt);
            }
            catch (Exception e)
            {
                LogUtil.LogError("启动服务器错误:"+e.Message);
            }
            

        }

        private void ClientConnectCB(IAsyncResult ar)
        {
            try
            {
                //表达一个客户端连接 由参数转换获得
                //(ar.AsyncState as Socket).EndAccept(ar);
                //获得客户端Socket
                Socket clientSkt = skt.EndAccept(ar);
                //将Socket包装成网络链接

            }
            catch (Exception)
            {
                LogUtil.LogError("客户端连接失败");
            }

        }
        #endregion
    }
}
