using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Endogine.ColorEx
{
    [AttributeUsage(AttributeTargets.Property)] //AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor
    public class ColorInfoAttribute : System.Attribute
    {
        public string Name = "";
        //public string Description = "";
        public float MinValue = 0f;
        public float MaxValue = 1f;
        public float UIMinValue = 0f;
        public float UIMaxValue = 1f;
        
        public ColorInfoAttribute()
        {
        }

        public ColorInfoAttribute(string name)
        {
            this.Name = name;
        }
        public ColorInfoAttribute(string name, float min, float max, float uiMin, float uiMax)
        {
            this.Name = name;
            this.MinValue = min;
            this.MaxValue = max;
            this.UIMinValue = uiMin;
            this.UIMaxValue = uiMax;
        }
        public ColorInfoAttribute(string name, float max, float uiMax)
        {
            this.Name = name;
            this.MaxValue = max;
            this.UIMaxValue = uiMax;
        }
        public ColorInfoAttribute(string name, float uiMax)
        {
            this.Name = name;
            this.UIMaxValue = uiMax;
        }
    }
}
