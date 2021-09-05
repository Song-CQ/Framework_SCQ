using System;
using System.Collections;

namespace Endogine.Scripting.EScript.Types
{
	/// <summary>
	/// Summary description for Method.
	/// </summary>
	public class Method : Member
	{
		public ArrayList Arguments;

		public Method(string s)
		{
			this.Name = s;
			this.Arguments = new ArrayList();
		}

		public void AddArgument(Expression arg)
		{
			//TODO: this is where it goes wrong... Arguments will not be reset when re-executing
			this.Arguments.Add(arg);
		}

		public override Object PerformOperation(Executer exec, Operator op, Object otherTerm)
		{
			Object oNew = this.Evaluate(exec);
			if (op == null)
				return oNew;
			return oNew.PerformOperation(exec, op, otherTerm);
		}

		public override Object Evaluate(Executer exec)
		{
			object[] args = new object[this.Arguments.Count];
			System.Type[] argTypes = new Type[this.Arguments.Count];
			for (int i = 0; i < this.Arguments.Count; i++)
			{
				Expression expr = (Expression)this.Arguments[i];
				//terms for functions *always* have an Expression and no Value.
				args[i] = expr.Evaluate(exec).GetUnboxed(exec);
				argTypes[i] = args[i].GetType();
			}

			if (this.BelongsToObject == null)
				this.BelongsToObject = MemberSearch.FindMethodObject(this.Name, argTypes);

			if (this.BelongsToObject.GetType() == typeof(Nodes.ClassNode))
			{
				Nodes.MethodNode method = ((Nodes.ClassNode)this.BelongsToObject).GetMethod(this.Name);
				//TODO: arguments can't be set like this - another thread may call the same
				//method *while* this call is in execution, so arguments must be put on a stack!
				method.SetArguments(args);
				Executer exe = new Executer(method);
				object o = exe.Run(); //method.Execute();
				return Object.CreateType(o);
			}
			return Object.CreateType(Endogine.Serialization.Access.CallMethod(
				this.BelongsToObject, this.Name, args));
		}

		public override string ToString()
		{
			string sReturn = this.Name+"(";
			
			foreach (Expression expr in this.Arguments)
				sReturn+=expr.Print()+",";

			sReturn+=")";
			return sReturn;
		}

	}
}
