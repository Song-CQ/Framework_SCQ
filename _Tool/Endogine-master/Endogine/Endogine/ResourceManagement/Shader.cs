using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine.ResourceManagement
{
    public abstract class Shader
    {
        Dictionary<string, object> _parameters = new Dictionary<string,object>();
        string _technique = "standard";
        int _numPasses = 1;

        public object GetValue(string name)
        {
            return this._parameters[name];
        }
        //public void SetValue(string name, object val)
        //{
        //    
        //}
        protected void PostSetValue(string name, object val)
        {
            this._parameters[name] = val;
        }
        public abstract void SetValue(string name, Endogine.BitmapHelpers.PixelDataProvider val);
        public abstract void SetValue(string name, System.Drawing.Color val);
        public abstract void SetValue(string name, System.Drawing.Color[] val);
        public abstract void SetValue(string name, bool val);
        public abstract void SetValue(string name, bool[] val);
        public abstract void SetValue(string name, int val);
        public abstract void SetValue(string name, int[] val);
        public abstract void SetValue(string name, float val);
        public abstract void SetValue(string name, float[] val);
        public abstract void SetValue(string name, string val);
        //public abstract void SetValue(string name, Vector3 val);
        public abstract void SetValue(string name, Vector4 val);
        public abstract void SetValue(string name, Vector4[] val);
        public unsafe abstract void SetValue(string name, void* ptr, int length);
        
        public Dictionary<string, object> Parameters
        { get { return this._parameters; } }

        public string Technique
        {
            get { return this._technique; }
            set { this._technique = value; }
        }

        public int NumPassesToExecute
        {
            get { return this._numPasses; }
            set { this._numPasses = value; }
        }
    }
}
