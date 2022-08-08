using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XLNet;

namespace TestClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            XLNetTool.SetLog((e) => Console.WriteLine(e), (e) => Console.WriteLine(e), (e) => Console.WriteLine(e));

            XLSocket<XLNetMsg, SocketLink> xLSocket = new XLSocket<XLNetMsg, SocketLink>();
            xLSocket.StartAsClient("127.0.0.1", 8037);
            Console.WriteLine("启动完成");

            while (true)
            {
                Thread.Sleep(20);
            }
        }

    }

    public class SocketLink : XLSocketLink<XLNetMsg>
    {
        protected override void OnConnected()
        {
            base.OnConnected();
            XLNetMsg Val = new XLNetMsg();
            Console.WriteLine("连接成功");
            Val.msgLst.Add("发送给服务器");
            Send(Val);
        }
        protected override void OnReciveMsg(XLNetMsg msg)
        {
            base.OnReciveMsg(msg);
            Console.WriteLine(msg.msgLst[0]);
        }


    }
}
