using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine.ResourceManagement
{
    public abstract class Shaders
    {
        protected Dictionary<string, Shader> _aliasToEffect;
        protected Dictionary<string, Shader> _filenameToEffect;

        public Shaders()
        {
            this._aliasToEffect = new Dictionary<string, Shader>();
            this._filenameToEffect = new Dictionary<string, Shader>();
        }

        public abstract Shader Load(string filename, string alias);
        public Shader Load(string filename)
        {
            return this.Load(filename, filename);
        }
        public abstract Shader Create(string implementation, string alias);


        public void Reload(string alias)
        {
        }

        public abstract void Unload(string alias);


        protected bool CheckLoad(string filename, string alias)
        {
            //TODO: keep track of file date - we might want to reload the same file after changes.
            if (this._filenameToEffect.ContainsKey(filename))
                return false;

            if (!System.IO.File.Exists(filename))
                throw new System.IO.FileNotFoundException("D3D shader file not found: " + filename);

            return true;
        }
        protected void AddShader(Shader shader, string filename, string alias)
        {
            this._aliasToEffect.Add(alias, shader);
            this._filenameToEffect.Add(filename, shader);
        }

        public Shader this[string alias]
        {
            get
            {
                if (this._aliasToEffect.ContainsKey(alias))
                    return this._aliasToEffect[alias];

                if (this._filenameToEffect.ContainsKey(alias))
                    return this._filenameToEffect[alias];

                return null;
            }
        }
    }
}