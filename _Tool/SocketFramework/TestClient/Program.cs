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

            XLSocket<Msg, SocketLink> xLSocket = new XLSocket<Msg, SocketLink>();
            xLSocket.StartAsClient("127.0.0.1", 8037);
            Console.WriteLine("启动完成");

            while (true)
            {
                Thread.Sleep(20);
            }
        }

    }
    [Serializable]
    public class Msg : XLNetMsg
    {
        public string Val;
    }

    public class SocketLink : XLSocketLink<Msg>
    {
        protected override void OnConnected()
        {
            base.OnConnected();
            Msg Val = new Msg();
            Val.Name = "客户端";
            Val.Val = "sdadasd";
            Send(Val);
        }
        protected override void OnReciveMsg(Msg msg)
        {
            base.OnReciveMsg(msg);
            Console.WriteLine(msg.Name);
            Console.WriteLine(msg.Val);
        }


    }
}
