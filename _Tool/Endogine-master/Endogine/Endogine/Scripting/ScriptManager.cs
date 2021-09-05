//using System;
//using System.Reflection;
//using System.IO;
//using System.Text;
//using System.Collections;
//using System.Collections.Specialized;
//using Microsoft.CSharp;
//using System.Threading;
//
//using PluginInterface;
//
//namespace Endogine.Scripting
//{
//	//TODO: look at http://www.codeproject.com/csharp/DynamicPluginManager.asp
//	//Ruby? http://www.saltypickle.com/RubyDotNet/39
//	//so we don't need interfaces
//
//	/// <summary>
//	/// Summary description for ScriptManager.
//	/// </summary>
//	public class ScriptManager
//	{
//		private ArrayList _paths;
//		private string  _createdAssemblyBaseName;
//		private string _createdAssemblyName;
//		private string _createdAssemblyFile;
//
//		private string _usingAssemblyFiles;
//
//		private AppDomain _secondaryDomain;
//		private PluginInterfaceFactory _factory;
//
//		private int _versionCounter = 0;
//		private string _outputPath;
//
//		public ScriptManager()
//		{
//			this._paths = new ArrayList();
//			this._paths.Add(@"C:\Documents and Settings\Jonas\Mina dokument\Visual Studio Projects\Endogine\YE\Media\test.cs");
//
//			this._outputPath = EH.Instance.ApplicationDirectory;
//
//			this._createdAssemblyBaseName = "Plugins";
//			this._usingAssemblyFiles = ""; //Endogine.dll YE.dll PluginInterface.dll
//			this._usingAssemblyFiles+="\""+this._outputPath+"Endogine.dll"+"\"";
//			this._usingAssemblyFiles+=",\""+this._outputPath+"YE.dll"+"\"";
//
//			string _codePrefix = @"
//using System;
//using PluginInterface;
//using Endogine;
//
//namespace PluginTest
//public class Plugin1 : MarshalByRefObject, IPluginInterface
//{
//<>
//}
//";
//			//this.RefreshAssembly();
//		}
//
//		public void Dispose()
//		{
//			if (this._secondaryDomain!=null)
//				AppDomain.Unload(this._secondaryDomain);
//		}
//
//		public void RefreshAssembly()
//		{
//			this._createdAssemblyName = this._createdAssemblyBaseName+this._versionCounter.ToString();
//			this._createdAssemblyFile = "\""+this._outputPath+this._createdAssemblyName+".dll"+"\"";
//
//			string sSources = "";
//			foreach (string path in this._paths)
//				sSources+= "\""+path+"\" ";
//
//			string sApp = "C:\\WINDOWS\\Microsoft.NET\\Framework\\v1.1.4322\\csc.exe";
//
//			string sOptions = "";
//			if (this._usingAssemblyFiles.Length > 0)
//				sOptions+= " /reference:"+this._usingAssemblyFiles;
//			sOptions+=
//				" /target:library"
//				+" /out:"+ this._createdAssemblyFile
//				+" "+sSources
//				+" /define:DEBUG"
//				+" /optimize"; 
//
//			//EH.Put(sOptions);
//
//			System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(sApp, sOptions);
//			startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
//
//			System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo); //sApp, sOptions);
//			while (!process.HasExited)
//			{
//			}
//
//			if (process.ExitCode != 0)
//			{
//				System.IO.StreamReader r = process.StandardError;
//				string err = r.ReadToEnd();
//				if (err != null)
//				{
//					err+="AA";
//				}
//			}
//			this.LoadAssembly();
//		}
//
//		public void LoadAssembly()
//		{
//			if (false)
//			{
//				if (this._secondaryDomain!=null)
//					AppDomain.Unload(this._secondaryDomain);
//				// create a secondary app-domain
//				this._secondaryDomain = AppDomain.CreateDomain("SecondaryDomain");
//
//				// create the factory class in the secondary app-domain
//				this._factory = (PluginInterfaceFactory)this._secondaryDomain.CreateInstance("PluginInterface", "PluginInterface.PluginInterfaceFactory").Unwrap();
//			}
//			else
//			{
//				//Microsoft.CSharp.CSharpCodeProvider cp;
//				//System.CodeDom.Compiler.ICodeCompiler cmp = cp.CreateCompiler();
//				//System.CodeDom.Compiler.CompilerParameters comp = new System.CodeDom.Compiler.CompilerParameters();
//				//System.CodeDom.Compiler.CompilerResults cr = cmp.CompileAssemblyFromSource(null, "");
//				//cr.CompiledAssembly.
//				
//				//AppDomain.CurrentDomain.Load(
////@"C:\Documents and Settings\Jonas\Mina dokument\Visual Studio Projects\Endogine\YEApp\bin\Debug\Plugins.dll");
////				AppDomain.CurrentDomain.Load(this._createdAssemblyFile);
//			}
//		}
//
//		public object CreatePlugin(string classname)
//		{
//			object[] constructArgs = null; //new object[] { "Blabla!" };
//			System.Runtime.Remoting.ObjectHandle handle =
//				System.Activator.CreateInstance(this._createdAssemblyName, "PluginTest."+classname, constructArgs);
//			return handle.Unwrap();
//		}
//
//		public IPlugin CreateIPlugin(string classname)
//		{
//			object[] constructArgs = null; //new object[] { "Blabla!" };
//			if (false)
//			{
//				// with the help of this factory, we can now create a real instance
//				IPlugin iPlugin = this._factory.Create(this._createdAssemblyBaseName, "PluginTest."+classname, constructArgs);
//				return iPlugin;
//			}
//			else
//			{
//				System.Runtime.Remoting.ObjectHandle handle = System.Activator.CreateInstance(this._createdAssemblyBaseName, "PluginTest."+classname, constructArgs);
//				return (IPlugin)handle.Unwrap();
//			}
//		}
//	}
//}
