using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;
using System.ComponentModel;

namespace Endogine.ColorEx
{
    public class Palette : IEnumerable<KeyValuePair<string, ColorBase>>
    {
        PropList _colors = new PropList();
        //Dictionary<string, ColorBase> _colors = new Dictionary<string,ColorBase>();
        static Dictionary<string, List<Type>> _fileFormatExtensionsAndTypes;

        public Palette()
        {
        }

        public void Add(string name, ColorBase color)
        {
            this._colors.Add(name, color);
        }

        public void Insert(int index, string name, ColorBase color)
        {
            this._colors.Insert(index, name, color);
        }

        public void Remove(int index)
        {
            this._colors.RemoveAt(index);
        }
        public void Remove(string name)
        {
            this._colors.Remove(name);
        }
        public void Remove(ColorBase color)
        {
            this._colors.RemoveValue(color);
        }

        public void MoveRange(int startIndex, int count, int newStartIndex)
        {
            PropList temp = new PropList();
            for (int i = 0; i < count; i++)
            {
                temp.Add(this._colors.GetKey(startIndex), this._colors.GetByIndex(startIndex));
                this._colors.RemoveAt(startIndex);
            }
            if (newStartIndex > startIndex)
                newStartIndex -= count;
            for (int i = 0; i < count; i++)
            {
                this._colors.Insert(newStartIndex, temp.GetKey(i), temp.GetByIndex(i));
            }
        }

        public ColorBase this[int index]
        {
            get { return (ColorBase)this._colors.GetByIndex(index); }
            set { this._colors.SetByIndex(index, value); }
        }
        public ColorBase this[string name]
        {
            get { return (ColorBase)this._colors[name]; }
            set { this._colors[value] = value; }
        }

        public void Load(string filename)
        {
            this._colors.Clear();
            this.LoadMerge(filename);
        }
        public void LoadMerge(string filename)
        {
            Palette pal = Palette.CreateFromFile(filename);
            foreach (KeyValuePair<string, ColorBase> kv in pal)
                this.Add(kv.Key, kv.Value);
        }

        public static Palette CreateFromFile(string filename)
        {
            System.IO.FileInfo f = new System.IO.FileInfo(filename);
            if (!f.Exists)
                throw new System.IO.FileNotFoundException("Palette file doesn't exist: " + filename);
            Serialization.Palettes.Base filter = GetFilterForExtension(f.Extension);
            if (filter == null)
                throw new Exception("No palette file filters found that handles " + filename);
            return filter.Load(filename);
        }

        public void Save(string filename)
        {
            System.IO.FileInfo f = new System.IO.FileInfo(filename);
            Serialization.Palettes.Base filter = GetFilterForExtension(f.Extension);
            if (filter == null)
                throw new Exception("No palette file filters found that handles " + filename);
            filter.Save(filename, this);
        }

        public static Serialization.Palettes.Base GetFilterForExtension(string extension)
        {
            extension = extension.ToLower();
            if (extension.StartsWith("."))
                extension = extension.Remove(0, 1);
            Dictionary<string, List<Type>> extAndTypes = GetAvailableFileFormats();
            if (!extAndTypes.ContainsKey(extension))
                return null;
            Type type = extAndTypes[extension][0];
            ConstructorInfo ci = type.GetConstructor(new Type[] { });
            Serialization.Palettes.Base palFilter = (Serialization.Palettes.Base)ci.Invoke(new object[] { });
            return palFilter;
        }


        public static Dictionary<string, List<Type>> GetAvailableFileFormats()
        {
            if (_fileFormatExtensionsAndTypes == null)
            {
                Dictionary<string, List<Type>> extensionsAndTypes = new Dictionary<string, List<Type>>();
                Type[] types = typeof(Palette).Assembly.GetTypes();
                string search = "Serialization.Palettes.";
                Type x = typeof(Endogine.Serialization.Palettes.Aco);

                foreach (Type type in types)
                {
                    if (type.FullName.IndexOf(search) < 0)
                        continue;

                    AttributeCollection attribs = TypeDescriptor.GetAttributes(type);
                    CategoryAttribute catAtt = (CategoryAttribute)attribs[typeof(CategoryAttribute)];
                    string extension = "";
                    if (catAtt != null)
                        extension = catAtt.Category.ToLower();
                    List<Type> extTypes = null;
                    if (!extensionsAndTypes.ContainsKey(extension))
                    {
                        extTypes = new List<Type>();
                        extensionsAndTypes.Add(extension, extTypes);
                    }
                    else
                        extTypes = extensionsAndTypes[extension];

                    extTypes.Add(type);
                }
                _fileFormatExtensionsAndTypes = extensionsAndTypes;
            }
            return _fileFormatExtensionsAndTypes;
        }


        public int Count
        {
            get { return this._colors.Count; }
        }

        public IEnumerator<KeyValuePair<string, ColorBase>> GetEnumerator()
        {
            return new PaletteEnumerator(this._colors);
        }
        //Ugly "bug" in .NET 2.0...:
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        { return this.GetEnumerator(); }

        private class PaletteEnumerator : IEnumerator<KeyValuePair<string, ColorBase>>
        {
            private PropList _colors;
            private int _index;

            public PaletteEnumerator(PropList colors)
            {
                _colors = colors;
                _index = -1;
            }

            public void Reset()
            {
                _index = -1;
            }

            public KeyValuePair<string, ColorBase> Current
            {
                get { return new KeyValuePair<string, ColorBase>((string)_colors.GetKey(_index), (ColorBase)_colors.GetByIndex(_index)); }
            }
            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public void Dispose()
            { _colors = null; }

            public bool MoveNext()
            {
                _index++;
                if (_index >= _colors.Count)
                    return false;
                else
                    return true;
            }
        }
    }
}