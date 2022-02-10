using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLNet
{
    public class XLNetPkg
    {
        public byte[] headBuff = null;
        public int headIndex = 0;
        public int headLen = 4;

        public byte[] bobyBuff;
        public int bobyIndex;
        public int bobyLen;

        public XLNetPkg()
        {
            //在头部放入该消息的长度 一个int的长度是4个字节
            headBuff = new byte[4];
            headIndex = 0;
        }

        public void InitBodyBuff(int len)
        {
            bobyLen = len;
            bobyIndex = 0;
            bobyBuff = new byte[bobyLen];
        }

        public void ResetData()
        {
            headIndex = 0;
            bobyLen = 0;
            bobyBuff = null;
            bobyIndex = 0;
        }

    }
}
