using System;

namespace Endogine.Scripting.EScript.Nodes
{
	/// <summary>
	/// Summary description for MethodNode.
	/// </summary>
	public class MethodNode : ChunkNode
	{
		public MethodNode()
		{
			this.CloseNodeAfterNextLine = true; //an { is always expected
		}

		/// <summary>
		/// Define the arguments that can be used with this method (for parsing, not execution)
		/// </summary>
		/// <param name="sArgs"></param>
		public void DefineArguments(string sArgs)
		{
			//TODO: check argument definitions
		}

		/// <summary>
		/// Done just before execution of the node. 
		/// </summary>
		/// <param name="args"></param>
		public void SetArguments(object[] args)
		{
			//TODO: can't store the arguments here in members - another function may call this
			//WHILE it's executing (e.g. a recursive call)
		}

		//TODO: Arguments property which returns a list with info about each argument's type and name
	}
}
