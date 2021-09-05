using System;
using System.Collections;
using System.Reflection;
using Boo.Lang.Interpreter;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;

namespace Endogine.Scripting
{
	//http://boo.codehaus.org/Scripting+with+the+Boo.Lang.Compiler+API
	//http://boo.codehaus.org/Scripting+with+the+Boo.Lang.Interpreter+API
	//entity = interpreter.SuggestCodeCompletion("print 'zeng'.__codecomplete__")

	/// <summary>
	/// Summary description for Scripterboo.
	/// </summary>
	public class ScripterBoo : ScripterBase
	{
		protected Boo.Lang.Interpreter.InteractiveInterpreter _interpreter;
		protected Boo.Lang.Compiler.BooCompiler _compiler;

		protected Hashtable _stalledScripts;
		public ScripterBoo()
		{
			//Notes on threads: http://www.yoda.arachsys.com/csharp/threads/

            //IMPORTANT: runtime compiling of Boo code in this project. The following files must be referenced for this to work:
            //Boo.Lang.dll    Boo.Lang.Compiler.dll    Boo.Lang.Interpreter.dll    Boo.Lang.Parser.dll
			_interpreter = new InteractiveInterpreter();
            // Otherwise the line above will cause an exception!

			_defaultContext = "";

			this._compiler = new Boo.Lang.Compiler.BooCompiler();
			this._compiler.Parameters.Pipeline = new CompileToMemory(); //No need for an on-disk file.
			this._compiler.Parameters.Ducky = true; //By default, all objects will be ducked typed; no need for the user to "var as string" anywhere.

			this._instanceIdToObject = new Hashtable();
			this._instanceIdToClass = new Hashtable();
			this._stalledScripts = new Hashtable();
		}


		public override string GenerateReferenceString()
		{
			string refs = "";
			//TODO: "from" means from a specific .dll! 
			//Find out somehow which have separate dlls and which are included in other namespaces
			foreach (string ass in this._referencedAssemblies)
				refs+="import "+ass+" from " + ass + "\r\n";
			return refs;
		}

		public override void Load(string filename, string optionalNameID)
		{
			if (optionalNameID==null)
				optionalNameID = filename;
			this.Compile(Endogine.Files.FileReadWrite.Read(filename), optionalNameID);
		}

		public override System.Reflection.Assembly CompileMultiple(Hashtable scriptNamesAndCode)
		{
			bool bUseTemporaryFiles = true;
			ArrayList files = new ArrayList(); //for easier deletion after compile

			this._compiler.Parameters.Input.Clear();

			foreach (DictionaryEntry de in scriptNamesAndCode)
			{
				string nameID = (string)de.Key;
				string code = (string)de.Value;

				if (bUseTemporaryFiles)
				{
					string tempName = "__boo_"+nameID+".boo";
					Endogine.Files.FileReadWrite.Write(tempName, code);

					System.IO.FileInfo file = new System.IO.FileInfo(tempName);
					files.Add(file);

					this._compiler.Parameters.Input.Add(new FileInput(tempName));
				}
				else
				{
					//TODO: why is this StreamReader closed???
					StringInput inp = new StringInput(nameID, code);
					inp.Open();
					this._compiler.Parameters.Input.Add(inp);
				}
			}

			Boo.Lang.Compiler.CompilerContext context = this._compiler.Run();

			if (bUseTemporaryFiles)
			{
				//foreach (System.IO.FileInfo file in files)
				//	file.Delete();
			}


			//The main module name is always filename+Module in pascal case; 
			//this file is actually RunBooModule!
			//Using duck-typing, we can directly invoke static methods
			//Without having to do the typical System.Reflection voodoo.

			string[] errors;
			if (context.GeneratedAssembly == null)
			{
				errors = new string[context.Errors.Count];
				string sErrors = "Boo compiler errors\r\n"; // for "+nameID+":\r\n";
				for (int i=0; i<context.Errors.Count;i++)
				{
					Boo.Lang.Compiler.CompilerError err = context.Errors[i];
					string sError = err.LexicalInfo.FileName + "("+err.LexicalInfo.Line+","+err.LexicalInfo.Column+")"+ "\r\n" + err.Message;

					if (System.IO.File.Exists(err.LexicalInfo.FileName))
					{
						sError+="Source:\r\n";
						System.IO.StreamReader rd = new System.IO.StreamReader(err.LexicalInfo.FileName);
						string sFileContents = rd.ReadToEnd();
						string[] sLines = sFileContents.Split("\r\n".ToCharArray());
						if (err.LexicalInfo.Line>0)
							sError+=sLines[(err.LexicalInfo.Line-1)*2]+"\r\n";
						sError+=sLines[err.LexicalInfo.Line*2];
					}

					errors[i] = sError;
					sErrors+=sError+"\r\n";
				}
				throw new Exception(sErrors);
			}
			else
			{
				foreach (DictionaryEntry de in scriptNamesAndCode)
				{
					string nameID = (string)de.Key;
					this._classToAssembly[nameID] = context.GeneratedAssembly;
				}
			}
			return context.GeneratedAssembly;
		}

		public override void Compile(string code, string nameID)
		{
			Hashtable ht = new Hashtable();
			ht.Add(nameID, code);
			this.CompileMultiple(ht);
		}

		public override object Construct(string className, string instanceId)
		{
			//only allows one instance of each script (the whole scripter is designed that way...)
			if (instanceId == null)
				instanceId = className;

			Assembly ass = (Assembly)this._classToAssembly[className];
			if (ass==null)
				throw new Exception("No boo assembly named "+className);

			object o = this._instanceIdToObject[instanceId];
			if (o!=null)
				return o;

			Type scriptClass = ass.GetType(className);
			if (scriptClass==null)
				throw new Exception("Class not found: "+className);

			System.Reflection.ConstructorInfo cons = scriptClass.GetConstructor(new Type[]{});
			o = cons.Invoke(new object[]{});

			this._instanceIdToObject[instanceId] = o;
			this._instanceIdToClass[instanceId] = className;

			return o;
		}

		public override object Invoke(string instanceId, string methodName)
		{
//			methodInfo = ass.EntryPoint;
//			scriptClass = methodInfo.DeclaringType;
//			if (methodInfo.IsStatic)
//			{
//				string[] why = null;
//				object[] parms = new object[]{why};
//				return methodInfo.Invoke(null, parms);
//			}

			object o = this._instanceIdToObject[instanceId];
			if (o==null)
				o = this.Construct(instanceId);

			string className = (string)this._instanceIdToClass[instanceId];
			Assembly ass = (Assembly)this._classToAssembly[className];
			Type scriptClass = ass.GetType(className);
			MethodInfo methodInfo = scriptClass.GetMethod(methodName);

			if (methodInfo==null)
				return null;

			bool bIsGenerator = false;
			if (methodName == "Update")
				bIsGenerator = true;

			ParameterInfo[] pmInfos = methodInfo.GetParameters();
			try
			{
				if (!bIsGenerator)
				{
					return methodInfo.Invoke(o, null);
				}
				else
				{
					object result = null;
					if (this._stalledScripts.Contains(instanceId))
					{
						IEnumerator enumerator = (IEnumerator)this._stalledScripts[instanceId];
						bool bIsStalled = enumerator.MoveNext();  //proceeding with stalled
						result = enumerator.Current;
						if (!bIsStalled)
							this._stalledScripts.Remove(instanceId);
						else
							result = enumerator.Current;
					}
					else
					{
						IEnumerable gen = (IEnumerable)methodInfo.Invoke(o, null);
						if (gen!=null)
						{
							IEnumerator enumerator = gen.GetEnumerator();
							if (enumerator!=null)
							{
								this._stalledScripts[instanceId] = enumerator;
								if (enumerator.Current!=null)
									result = enumerator.Current.ToString() + " FIRST!";
							}
						}
					}
					if (result!=null)
						Console.WriteLine(result.ToString());
					return result;
				}
			}
			catch (Exception e)
			{
				throw new Exception("Boo instance "+instanceId+" ("+o.GetType().ToString()+") failed: "+e.Message + "\r\n" + e.StackTrace);
			}
		}
		

		
		public override object Execute(string code)
		{
			string codeString = this.InsertCode(code);
			codeString=this.GenerateReferenceString()+codeString;

			//interpreter.SetValue("Message", "ping");
			_interpreter.Eval(codeString);
			//Message = 'pong'
			//interpreter.GetValue("Message");

			return _interpreter.LastValue;
		}

		public override void Remove(string instanceId)
		{
			base.Remove (instanceId);
		}

	}
}
