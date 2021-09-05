using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine.ColorEx
{
    public abstract class ColorBase
    {
        public struct AxisInfo
        {
            public float Min;
            public float Max;
            public string Name;
            public float UIMin;
            public float UIMax;
            public string UIName;
            public int Ordinal;
            public System.Reflection.PropertyInfo PropertyInfo;
        }

        private int _a = 255;

        /// <summary>
        /// Alpha, 0-255
        /// </summary>
        public virtual int A
        {
            get { return _a; }
            set { _a = value; }
        }
	
        public abstract System.Drawing.Color ColorRGBA
        { get; set; }
        public abstract Vector4 Vector
        { get; set; }
        public abstract ColorRgbFloat RgbFloat
        { get; set; }

        /// <summary>
        /// Vector where values = 0-255
        /// </summary>
        public virtual Vector4 VectorRGBA
        {
            get
            {
                System.Drawing.Color color = this.ColorRGBA;
                return new Vector4((float)color.R / 255, (float)color.G / 255, (float)color.B / 255, (float)color.A / 255);
            }
            set
            {
                this.ColorRGBA = System.Drawing.Color.FromArgb((int)(value.W * 255), (int)(value.X * 255), (int)(value.Y * 255), (int)(value.Z * 255));
            }
        }

        /// <summary>
        /// Vector where values = 0-1
        /// </summary>
        public virtual float[] Array
        {
            get { return null; }
            set { }
        }

        public AxisInfo[] GetAxisInfo()
        {
            Type type = this.GetType();
            System.Reflection.PropertyInfo[] pis = type.GetProperties();
            List<AxisInfo> infos = new List<AxisInfo>();

            string sComponentOrder = type.Name.ToUpper().Replace("COLOR", "");

            foreach (System.Reflection.PropertyInfo pi in pis)
            {
                object[] cust = pi.GetCustomAttributes(true);

                AxisInfo info = new AxisInfo();
                bool addIt = false;
                foreach (object attr in cust)
                {
                    if (attr is System.ComponentModel.CategoryAttribute)
                    {
                        if (((System.ComponentModel.CategoryAttribute)attr).Category == "Axis")
                        {
                            info.Ordinal = sComponentOrder.IndexOf(pi.Name.ToUpper());
                            info.Name = pi.Name;
                            info.PropertyInfo = pi;
                            addIt = true;
                        }
                    }
                    else if (attr is System.ComponentModel.DefaultValueAttribute)
                    {
                        info.Min = 0;
                        info.Max = Convert.ToSingle(((System.ComponentModel.DefaultValueAttribute)attr).Value);
                    }
                    else if (attr is ColorInfoAttribute)
                    {
                        ColorInfoAttribute ciattr = (ColorInfoAttribute)attr;
                        info.Min = ciattr.MinValue;
                        info.Max = ciattr.MaxValue;
                        info.UIMin = ciattr.UIMinValue;
                        info.UIMax = ciattr.UIMaxValue;
                        info.UIName = ciattr.Name;
                    }
                }

                if (addIt)
                    infos.Add(info);
            }

            AxisInfo[] result = new AxisInfo[infos.Count];
            int i = 0;
            foreach (AxisInfo info in infos)
            {
                result[info.Ordinal] = info;
                i++;
            }

            return result;
        }

        public virtual void Validate()
        {
        }

        public ColorBase Copy()
        {
            System.Reflection.ConstructorInfo ci = this.GetType().GetConstructor(new Type[] { });
            ColorBase obj = (ColorBase)ci.Invoke(new object[] { });
            obj.Vector = this.Vector.Copy();
            return obj;
        }
    }
}
