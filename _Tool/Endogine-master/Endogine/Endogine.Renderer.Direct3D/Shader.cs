using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;

namespace Endogine.Renderer.Direct3D
{
    public class Shader : Endogine.ResourceManagement.Shader
    {
        Effect _effect;

        public Shader(Device device, string filename)
        {
            string sErrors = null;
            try
            {
                //Effect.FromString(device, Text, null, 
                this._effect = Effect.FromFile(device, filename, null, null, ShaderFlags.Debug | ShaderFlags.PartialPrecision, null, out sErrors);
            }
            catch
            {
                throw new Exception("Shader load failure: " + filename + " Errors: " + sErrors);
            }
            if (sErrors != null && sErrors.Length > 0)
                throw new Exception("Shader load failure: " + filename + " Errors: " + sErrors);
        }

        public override void SetValue(string name, Endogine.BitmapHelpers.PixelDataProvider val)
        {
            this._effect.SetValue(name, ((PixelDataProvider)val).Texture);
        }
        public override void SetValue(string name, System.Drawing.Color val)
        {
            this._effect.SetValue(name, ColorValue.FromColor(val));
        }
        public override void SetValue(string name, System.Drawing.Color[] val)
        {
            throw new Exception("SetValue(string, Color[]) not implemented");
            //this._effect.SetValue(name, ColorValue.FromColor(val));
        }
        public override void SetValue(string name, bool val)
        {
            this._effect.SetValue(name, val);
        }
        public override void SetValue(string name, bool[] val)
        {
            this._effect.SetValue(name, val);
        }
        public override void SetValue(string name, int val)
        {
            this._effect.SetValue(name, val);
        }
        public override void SetValue(string name, int[] val)
        {
            this._effect.SetValue(name, val);
        }
        public override void SetValue(string name, float val)
        {
            this._effect.SetValue(name, val);
        }
        public override void SetValue(string name, float[] val)
        {
            this._effect.SetValue(name, val);
        }
        public override void SetValue(string name, string val)
        {
            this._effect.SetValue(name, val);
        }
        //public override void SetValue(string name, Vector3 val)        { }
        public override void SetValue(string name, Endogine.Vector4 val)
        {
            this._effect.SetValue(name, new Microsoft.DirectX.Vector4(val.X, val.Y, val.Z, val.W));
        }
        public override void SetValue(string name, Vector4[] val)
        {
            throw new Exception("SetValue(string, Vector4[]) not implemented");
            //this._effect.SetValue(name, ColorValue.FromColor(val));
        }
        public unsafe override void SetValue(string name, void* ptr, int length)
        {
            this._effect.SetValue(name, ptr, length);
        }

        public Effect Effect
        {
            get { return this._effect; }
        }
    }
}
