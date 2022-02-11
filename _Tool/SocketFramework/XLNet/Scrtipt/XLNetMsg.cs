using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLNet
{
    [Serializable]
    public class XLNetMsg
    {
        public int seq;
        /// <summary>
        /// 消息编号
        /// </summary>
        public List<int> cmdLst = new List<int>();
        /// <summary>
        /// 消息对应值 object 可替换成自定义类 
        /// </summary>
        public List<object> msgLst = new List<object>();
        public int err;
    }
}
