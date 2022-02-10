using Fleck;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XLNet;

namespace XLNet
{
    internal class Program
    {
        static void Main(string[] args)
        {



            XLNetTool.SetLog((e) => Console.WriteLine(e), (e) => Console.WriteLine("警告  "+e), (e) => Console.WriteLine("错误  " + e));

            XLSocket<XLNetMsg, SocketLink> xLSocket = new XLSocket<XLNetMsg, SocketLink>();
            xLSocket.StartAsServer("127.0.0.1",8037);
            Console.WriteLine("启动完成");
            while (true)
            {
                Thread.Sleep(20);
            }
        }

        


    }

   

}

namespace XLNet
{
    public class SocketLink : XLSocketLink<XLNetMsg>
    {
        protected override void OnConnected()
        {
            base.OnConnected();
            XLNetMsg Val = new XLNetMsg();
            Val.Name = "客户端";
            Val.Val = "你好";
            Send(Val);
        }
        protected override void OnReciveMsg(XLNetMsg msg)
        {
            Console.WriteLine(msg.Name);
            Console.WriteLine(msg.Val);

        }
    }

}