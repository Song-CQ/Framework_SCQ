using System;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace Endogine.Scripting
{
	/// <summary>
	/// Summary description for ScripterCSharp.
	/// </summary>
	public class ScripterCSharp : ScripterBase
	{
		public ScripterCSharp()
		{
			this._defaultContext = @"
class MyProgram
{
 static void Main()
 {
 ###
 }
}
";			
		}

		public override string GenerateReferenceString()
		{
			string refs = "";
			foreach (string ass in this._referencedAssemblies)
				refs+="using "+ass+";";
			return refs;
		}

		public override object Execute(string code)
		{
			ICodeCompiler compiler = new Microsoft.CSharp.CSharpCodeProvider().CreateCompiler();
			CompilerParameters cmpParams = new System.CodeDom.Compiler.CompilerParameters();
			cmpParams.GenerateInMemory = true;
			cmpParams.GenerateExecutable = true;
			//cmpParams.CompilerOptions = "/t:exe";

			foreach (string ass in this._referencedAssemblies)
				cmpParams.ReferencedAssemblies.Add(ass+".dll");

			string codeString = this.GenerateReferenceString();
			codeString = this.InsertCode(code);

			CompilerResults results = compiler.CompileAssemblyFromSource(cmpParams, codeString);

//			foreach (System.CodeDom.Compiler.CompilerError ce in results.Errors)
//				this.Put(ce.ErrorText);

			if (results.Errors.Count == 0 && results.CompiledAssembly!=null)
			{
				System.Reflection.MethodInfo methodInfo = results.CompiledAssembly.EntryPoint;
				return methodInfo.Invoke(null, null);
			}
			return null;
		}

	}
}
