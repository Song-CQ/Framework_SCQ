using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace XLNet
{
    public class XLSocketLink
    {
        private Socket linkSocket;

        private Action Close;

        public XLSocketLink(Socket _linkSocket)
        {
            linkSocket = _linkSocket;

        }

    }
}
