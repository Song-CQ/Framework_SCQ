using System;

namespace Endogine.Scripting.EScript.Nodes
{
	/// <summary>
	/// Summary description for ForNode.
	/// </summary>
	public class ForNode : ChunkNode
	{
		public ForNode()
		{
		}

		public void SetConditions(string s)
		{
			string[] conditions = s.Split(";".ToCharArray());
			//TODO: 
		}

		public override object Execute(Executer exec)
		{
			//TODO:
			return null;
		}

	}
}
