using System;

namespace Endogine.Scripting.EScript.Nodes
{
	/// <summary>
	/// Summary description for ChunkNode.
	/// </summary>
	public class ChunkNode : BaseNode
	{
		public bool CloseNodeAfterNextLine; //For parsing only, not used during execution

		public ChunkNode()
		{
		}
	}
}
