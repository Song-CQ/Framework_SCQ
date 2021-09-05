using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine.Tools
{
    public class RangeConverter
    {
        public float MinIn = 0;
        public float MaxIn = 1;
        public float MinOut = 0;
        public float MaxOut = 1;

        float _val = 0;

        public bool Limit;

        public RangeConverter()
        {
        }

        public float ConvertInToOut(float val)
        {
            float tmp = (val - MinIn) / (MaxIn - MinIn);
            return tmp * (MaxOut - MinOut) + MinOut;
        }
        public float ConvertOutToIn(float val)
        {
            float tmp = (val - MinOut) / (MaxOut - MinOut);
            return tmp * (MaxIn - MinIn) + MinIn;
        }

        public float ValueIn
        {
            get { return this._val; }
            set { this._val = value; }
        }
        public float ValueOut
        {
            get { return this.ConvertInToOut(this._val); }
            set { this._val = this.ConvertOutToIn(value); }
        }
    }
}
