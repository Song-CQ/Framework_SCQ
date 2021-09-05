using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;

namespace Endogine.Renderer.Direct3D
{
    class Shaders : Endogine.ResourceManagement.Shaders
    {
        Device _device;

        public Shaders()
        {
        }

        public Device Device
        {
            set { this._device = value; }
        }

        public override Endogine.ResourceManagement.Shader Load(string filename, string alias)
        {
            //http://www.directx.com/shader/tutorial_debugging.htm
            //http://download.nvidia.com/developer/SDK/Individual_Samples/effects.html

            Shader sh = (Shader)this[alias];
            if (sh == null)
            {
                if (this.CheckLoad(filename, alias))
                {
                    sh = new Shader(this._device, filename);
                    this.AddShader(sh, filename, alias);
                }
            }
            return sh; 
        }

        public override Endogine.ResourceManagement.Shader Create(string implementation, string alias)
        {

            throw new Exception("The method or operation is not implemented.");
        }

        public override void Unload(string alias)
        {
        }
    }
}
