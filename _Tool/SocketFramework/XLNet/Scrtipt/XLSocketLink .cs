using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace XLNet
{
    public abstract class XLSocketLink<Msg> where Msg : XLNetMsg
    {
        private Socket linkSocket;

        private Action ClearCB;

        public void StartReceive(Socket _linkSocket, Action _close)
        {
            linkSocket = _linkSocket;
            ClearCB = _close;
            XLNetPkg netPkg = new XLNetPkg();
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

        #region 解析网络流
        /// <summary>
        /// 解析包头数据
        /// </summary>
        /// <param name="ar"></param>
        private void RcvHeadData(IAsyncResult ar)
        {
            try
            {
                XLNetPkg netPkg = (XLNetPkg)ar.AsyncState;
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
                        AnalysisHeadBuffData(netPkg);
                        //接收包体数据
                        linkSocket.BeginReceive(netPkg.bobyBuff,0,netPkg.bobyLen,SocketFlags.None,RcvBobyData,netPkg);

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
                XLNetTool.LogError("接收包头数据错误:" + e.Message);
                Close();
            }


        }
        /// <summary>
        /// 解析包体数据
        /// </summary>
        /// <param name="ar"></param>
        private void RcvBobyData(IAsyncResult ar)
        {
            try
            {
                XLNetPkg netPkg = (XLNetPkg)ar.AsyncState;
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
                        Msg msg = BytesToMsg(netPkg.bobyBuff);
                        if (msg != null)
                        {
                            ///触发消息接收
                            OnReciveMsg(msg);
                        }
                        else
                        {
                            //解密失败 接受到一条错误消息
                            ReceivedErrorMessage();
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
                XLNetTool.LogError("接收包体数据错误:" + e.Message);
                Close();
            }
        }
        #endregion

        public byte[] MsgToBytes(Msg msg)
        {
            //序列化
            byte[] val = Serialize(msg);     
            //加密
            val = EncryptBytes(val);
            //加包头
            val = PackLenInfo(val);
            return val;
        }
        private Msg BytesToMsg(byte[] bs)
        {
            //解密
            byte[] val = AnalysisBytes(bs);
            //反序列化
            Msg msg = DoSerialize(val);
            
            return msg;
        }

        protected virtual byte[] Serialize(Msg msg)
        {
            //创建数据流
            using (MemoryStream ms = new MemoryStream())
            {
                //创建序列化类
                BinaryFormatter bf = new BinaryFormatter();
                //序列化类 写进流中
                bf.Serialize(ms,msg);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }
        protected virtual Msg DoSerialize(byte[] val)
        {

            //创建数据流
            using (MemoryStream ms = new MemoryStream(val))
            {
                //创建序列化类
                BinaryFormatter bf = new BinaryFormatter();
                //将流中数据 反序列化
                try
                {
                    Msg msg = bf.Deserialize(ms) as Msg;
                    return msg;
                }
                catch (Exception e)
                {
                    XLNetTool.LogWarn(e.Message);
                    return null;
                }
            }

        }
        

        /// <summary>
        /// 给包添加包头
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        protected virtual byte[] PackLenInfo(byte[] val)
        {
            int len = val.Length;
            byte[] pkg = new byte[len + 4];

            byte[] head = BitConverter.GetBytes(len);
            head.CopyTo(pkg, 0);
            val.CopyTo(pkg, 4);
            return pkg;
        }

        /// <summary>
        /// 解析包头数据
        /// </summary>
        protected virtual void AnalysisHeadBuffData(XLNetPkg netPkg)
        {
            int bobyLen = BitConverter.ToInt32(netPkg.headBuff, 0);
         
            netPkg.InitBodyBuff(bobyLen);
        }


        #region Send


       

        public void Send(Msg msg)
        {
            byte[] data = MsgToBytes(msg);
            Send(data);
        }

        public void Send(byte[] data)
        {
            //创建网络流
            NetworkStream stream = new NetworkStream(linkSocket);  
            try
            {
                if (stream.CanWrite)
                {
                    //开始往流里写入数据
                    stream.BeginWrite(data, 0, data.Length,SendCB,stream);
                }
            }
            catch (Exception e)
            {
                XLNetTool.LogError("写入消息错误:"+e.Message);
            }
            
        }

        private void SendCB(IAsyncResult ar)
        {
            try
            {
                NetworkStream stream = ar.AsyncState as NetworkStream;
                //结束写入
                stream.EndWrite(ar);
                //发送 但是好像会自动发送 不用调用
                stream.Flush();
                //关闭流但是不关闭Socket
                stream.Close();
            }
            catch (Exception e)
            {
                XLNetTool.LogError("发送消息错误:" + e.Message); throw;
            }
            
        }
        #endregion

        

        private void Clear()
        { 
            ClearCB?.Invoke();
        }

        public void Close()
        {
            
            OnDisConnected();
            Clear();
        }

        #region 虚方法

        /// <summary>
        /// 加密byte[]数组
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        protected virtual byte[] EncryptBytes(byte[] val)
        {
            return val;
        }
        /// <summary>
        /// 解密byte[]数组
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        protected virtual byte[] AnalysisBytes(byte[] val)
        {
            return val;
        }

        /// <summary>
        /// 建立连接
        /// </summary>
        protected virtual void OnConnected()
        {
            XLNetTool.Log("New Seesion Connected.");
        }

        /// <summary>
        /// 收到消息
        /// </summary>
        protected virtual void OnReciveMsg(Msg msg)
        {
            XLNetTool.Log("Receive Network Message.");
        }

        /// <summary>
        /// 接收到一条解密失败或者错误消息
        /// </summary>
        protected virtual void ReceivedErrorMessage()
        {
            XLNetTool.LogWarn("ReceivedErrorMessage");
        }

        /// <summary>
        /// 连接断开
        /// </summary>
        protected virtual void OnDisConnected()
        {
            XLNetTool.Log("Session Disconnected.");
        }
        #endregion
    }
}
