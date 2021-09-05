//TODO: any simpler way of performing arithmetics? http://www.codeproject.com/csharp/genericnumerics.asp

//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Endogine.Tools
//{
//    public class RangeConverter<T> where T: int, float, double 
//    {
//        public T Min;
//        public T Max;
//        public T MinOut;
//        public T MaxOut;

//        T _val;
//        public bool Limit;

//        public RangeConverter()
//        {
//            Min = 0;
//            Max = (T)1;
//            MinOut = (T)0;
//            MaxOut = (T)0;
//        }

//        public T ConvertInToOut(T val)
//        {
//            T tmp = (val - Min) / (Max - Min);
//            return tmp * (MaxOut - MinOut) + MinOut;
//        }
//        public T ConvertOutToIn(T val)
//        {
//            T tmp = (val - MinOut) / (MaxOut - MinOut);
//            return tmp * (Max - Min) + Min;
//        }

//        public T ValueIn
//        {
//            get { return this._val; }
//            set { this._val = value; }
//        }
//        public T ValueOut
//        {
//            get { return this._val; }
//            set { }
//        }
//    }
//}
