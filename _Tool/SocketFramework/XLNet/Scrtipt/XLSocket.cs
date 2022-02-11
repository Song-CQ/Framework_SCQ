using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace XLNet
{
    public class XLSocket<Msg,SocketLink> where Msg : XLNetMsg where SocketLink : XLSocketLink<Msg> , new()
    {
        private Socket skt = null;
        
        private List<SocketLink> xLSocketLinkLst = new List<SocketLink>();

        public SocketLink ClientSocketLink;

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
                XLNetTool.LogError("启动服务器错误:"+e.Message);
            }
            

        }
        /// <summary>
        /// 一个客户端链接
        /// </summary>
        /// <param name="ar"></param>
        private void ClientConnectCB(IAsyncResult ar)
        {
            try
            {
                //表达一个客户端连接 由参数转换获得
                //(ar.AsyncState as Socket).EndAccept(ar);
                //获得客户端Socket
                Socket clientSkt = skt.EndAccept(ar);
                //将Socket包装成网络链接
                SocketLink xLSocketLink = new SocketLink();
                //启动监听
                xLSocketLink.StartReceive(clientSkt,() => xLSocketLinkLst.Remove(xLSocketLink));

                xLSocketLinkLst.Add(xLSocketLink);
            }
            catch (Exception e)
            { 
                XLNetTool.LogError("客户端连接失败:"+e.Message);
            }
            //继续监听
            skt.BeginAccept(ClientConnectCB, skt);
        }
        #endregion

        #region Client

        public void StartAsClient(string ipStr, int port)
        {
            try
            {
                //转换ip
                IPAddress ip = IPAddress.Parse(ipStr);
                //创建ip和端口号
                IPEndPoint endPoint = new IPEndPoint(ip, port);
                skt.BeginConnect(endPoint, ServerConnectCB,skt);
                XLNetTool.Log("开始链接服务器");
            }
            catch (Exception e)
            {

                XLNetTool.LogError("启动客户端:" + e.Message);
            }
        }

        private void ServerConnectCB(IAsyncResult ar)
        {
            try
            {
                skt.EndConnect(ar);
                ClientSocketLink = new SocketLink();
                ClientSocketLink.StartReceive(skt,null);
            }
            catch (Exception e)
            {

                XLNetTool.LogError("链接服务器失败:" + e.Message);
            }

        }
        #endregion
    }
}
