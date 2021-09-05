using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Endogine.Editors
{
    public class EditorFactory
    {
        static Assembly _assembly;
        static string _loadedAssemblyName;

        public static bool LoadDll(string filename)
        {
            if (filename == _loadedAssemblyName)
                return false;
            string userEnteredName = filename;

            if (!Endogine.Files.FileFinder.IsFullyQualified(filename))
                filename = System.IO.Directory.GetCurrentDirectory() + "\\" + filename; // EH.Instance.ApplicationDirectory

            if (!System.IO.File.Exists(filename))
                return false;

            _assembly = Assembly.LoadFile(filename);
            _loadedAssemblyName = userEnteredName;
            return true;
        }

        public static bool HasDll
        {
            get { return _assembly != null; }
        }

        public static IEditorForm CreateEditor(string typeName)
        {
            if (_assembly == null)
                LoadDll("Endogine.Editors.dll");
            string sFullname = _assembly.FullName.Substring(0, _assembly.FullName.IndexOf(","));
            Type type = _assembly.GetType(sFullname + "." + typeName);
            if (type == null)
                throw new Exception("Editor not found: " + typeName);

            ConstructorInfo cons = type.GetConstructor(new Type[] { });
            object o = cons.Invoke(new object[] { });

            return (IEditorForm)o;
        }
    }
}
