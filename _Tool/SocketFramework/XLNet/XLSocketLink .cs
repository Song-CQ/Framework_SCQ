using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace XLNet
{
    public class XLSocketLink<Msg> where Msg : XLNetMsg
    {
        private Socket linkSocket;

        private Action ClearCB;

        public XLSocketLink(Socket _linkSocket)
        {
            linkSocket = _linkSocket;
        }

        private void StartReceive(Action _close)
        {
            ClearCB = _close;
            NetPkg netPkg = new NetPkg();
            OnConnected();

            //开始接收消息 （接收的数据缓存数组，从该缓存数组的哪个索引接送数据(偏移)标识,要接受的最大数据长度，回调函数，回调函数的参数）
            /* ps：
             * 在接收到的消息中会有分包粘问题（思路）
             * 
             * 
             * BeginReceive 就像是用一个勺子取水，传入BeginReceive的buffer参数相当于舀水勺（大小我们自己定义）。
             * 当我们调用 BeginReceive方法时(开启新的线程)，就用舀水勺去水池（socket接受数据的缓冲区）中舀水，没有水则等待。舀一次之后，就会调用回调函数（不管舀了多少），
             * 在回调函数中，（通过EndReceive，知道我们舀了多少水），我们自己决定对水勺中的水做什么（即对数据的处理），
             * 然后继续舀水（在回调函数中调用BeginReceive），若池中有数据，继续舀，没有就等待。其中，每次重新去舀水的时候，我们都是用的空的水勺，之前水勺中的水被清空了。
             * 
             * 而解决分包粘包就是先用4个字节大小的舀水勺 舀出包头  包头中包含着该消息的长度，好决定我们接下来用多大的 舀水勺去舀水；
             * 
             */
            linkSocket.BeginReceive(netPkg.headBuff,0,netPkg.headLen,SocketFlags.None, RcvHeadData, netPkg);

        }

        /// <summary>
        /// 解析包头数据
        /// </summary>
        /// <param name="ar"></param>
        private void RcvHeadData(IAsyncResult ar)
        {
            try
            {
                NetPkg netPkg = (NetPkg)ar.AsyncState;
                //获取接收到的字节数组
                int length =  linkSocket.EndReceive(ar);

                if (length > 0)
                {
                    //将数组索引增加
                    netPkg.headIndex += length;
                    //如果没有接收完包头数据 则继续接收包头数据
                    if (netPkg.headIndex < netPkg.headLen)
                    {
                        //这时要设置存放偏移和还要接收多少长度（总长度减去已经接送到的长度）
                        linkSocket.BeginReceive(netPkg.headBuff, netPkg.headIndex, netPkg.headLen - netPkg.headIndex, SocketFlags.None, RcvHeadData, netPkg);

                    }
                    else // 接收完包头数据 
                    {
                        //解析包头数据 初始化包体长度
                        netPkg.InitBodyBuff();
                        //接收包体数据
                        linkSocket.BeginReceive(netPkg.bobyBuff,netPkg.bobyIndex,netPkg.bobyLen,SocketFlags.None,RcvBobyData,netPkg);

                    }
                }
                else
                {
                    OnDisConnected();
                    Clear();
                }
            }
            catch (Exception e)
            {
                LogUtil.LogError("接收包数据错误"+e.Message);
                Close();
            }
           

        }

        private void RcvBobyData(IAsyncResult ar)
        {
            try
            {
                NetPkg netPkg = (NetPkg)ar.AsyncState;
                //获取接收到的字节数组
                int length = linkSocket.EndReceive(ar);

                if (length > 0)
                {
                    //将数组索引增加
                    netPkg.bobyIndex += length;
                    //如果没有接收完包体数据 则继续接收包体数据
                    if (netPkg.bobyIndex < netPkg.bobyLen)
                    {
                        //这时要设置存放偏移和还要接收多少长度（总长度减去已经接送到的长度）
                        linkSocket.BeginReceive(netPkg.bobyBuff, netPkg.bobyIndex, netPkg.bobyLen - netPkg.bobyIndex, SocketFlags.None, RcvBobyData, netPkg);
                    }
                    else // 接收完包体数据 
                    {
                        //解析包头数据 初始化包体长度
                        Msg msg = AnalysisToMsg(netPkg.bobyBuff);
                        if (msg != null)
                        {
                            
                        }
                        netPkg.ResetData();
                        //接收包头数据
                        linkSocket.BeginReceive(netPkg.headBuff, 0, netPkg.headLen, SocketFlags.None, RcvHeadData, netPkg);

                    }
                }
                else
                {
                    OnDisConnected();
                    Clear();
                }
            }
            catch (Exception e)
            {
                LogUtil.LogError("接收包数据错误" + e.Message);
                Close();
            }
        }


        protected virtual Msg AnalysisToMsg(byte[] vs)
        {
            return null;
        }

        protected virtual byte[] AnalysisToMsg(Msg msg)
        {
            return null;
        }

        public void Send(Msg msg)
        {
            byte[] data = null;
            Send(data);
        }

        private void Send(byte[] data)
        {
            linkSocket.

        }

        private void Clear()
        { 
            ClearCB?.Invoke();
        }

        public void Close()
        {
            
            OnDisConnected();
            Clear();
        }

        /// <summary>
        /// Connect network
        /// </summary>
        protected virtual void OnConnected()
        {
            LogUtil.Log("New Seesion Connected.");
        }

        /// <summary>
        /// Receive network message
        /// </summary>
        protected virtual void OnReciveMsg(Msg msg)
        {
            LogUtil.Log("Receive Network Message.");
        }

        /// <summary>
        /// Disconnect network
        /// </summary>
        protected virtual void OnDisConnected()
        {
            LogUtil.Log("Session Disconnected.");
        }
    }
}
