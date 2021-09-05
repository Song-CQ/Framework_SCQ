using System;
using System.Collections.Generic;

namespace Endogine
{
	/// <summary>
	/// Summary description for Node.
	/// </summary>
	public class Node
	{
		public Node ParentNode;
		public PropList ChildNodes;
		protected object _value;
		public object Tag;
		public string Text; //Value could be used, but casting is so tiresome...
		private bool _bAutoCreateChildIfNotExists;

		public Node()
		{
			this.ChildNodes = new PropList();
		}
		public Node(bool bAutoCreateChildIfNotExists)
		{
			this._bAutoCreateChildIfNotExists = bAutoCreateChildIfNotExists;
			this.ChildNodes = new PropList();
		}

		public object Value
		{
			get {return this._value;}
			set {this._value = value;}
		}

		public virtual Node CreateChild(string sName)
		{
			Node node = null;
			if (this._bAutoCreateChildIfNotExists)
				node = new Node(true);
			else
				node = new Node();

			this.AppendChild(sName, node);
			return node;
		}

		public Node RootNode
		{
			get
			{
				if (this.ParentNode != null)
					return this.ParentNode.RootNode;
				return this;
			}
		}
		public string Name
		{
			get
			{
				if (this.ParentNode!=null)
					return (string)this.ParentNode.ChildNodes.GetKeyOfValue(this);
				return "root";
			}
			set
			{
				if (this.ParentNode!=null)
					this.ParentNode.ChildNodes.SetKeyByIndex(this.ParentNode.ChildNodes.IndexOfValue(this), value);
			}
		}

		public Node GetNthNodeAbove(int nSteps)
		{
			if (nSteps == 0)
				return this;
			return this.ParentNode.GetNthNodeAbove(nSteps-1);
		}

		/// <summary>
		/// Nodes depth in tree; root node has depth 0.
		/// </summary>
		public int Depth
		{
			get
			{
				if (this.ParentNode!=null)
					return 1+this.ParentNode.Depth;
				return 0;
			}
		}

		public Node this [int index]
		{
			get
			{
				return (Node)this.ChildNodes.GetByIndex(index);
			}
		}
		public Node this [string XPath]
		{
			get
			{
				return this.Get(XPath, this._bAutoCreateChildIfNotExists);
			}
		}
		public Node GetOrCreate(string XPath)
		{
			return this.Get(XPath, true);
		}
		private Node Get(string XPath, bool bCreateIfNotFound)
		{
			string[] names = XPath.Split(".".ToCharArray());
//			if (bCreateIfNotFound)
//				Console.WriteLine(XPath + " createmaybe " + names.Length.ToString());
			Node node = this;
			foreach (string s in names)
			{
				Node newNode = (Node)node.ChildNodes[s];
				if (newNode == null)
				{
					if (bCreateIfNotFound)
					{
						newNode = node.AppendChild(s); //CreateChild(s);
//						Console.WriteLine("Create "+s);
					}
					else
						return null;
				}
//				else if (bCreateIfNotFound)
//					Console.WriteLine("Already there "+s);
				node = newNode;
			}
//			if (bCreateIfNotFound)
//				Console.WriteLine("returning "+node.Name);
			return node;
		}

		public bool HasChildNodes
		{
			get {return this.ChildNodes.Count > 0;}
		}

		public Node FirstChild
		{
			get {return (Node)this.ChildNodes.GetByIndex(0);}
		}
		public Node LastChild
		{
			get {return (Node)this.ChildNodes.GetByIndex(this.ChildNodes.Count-1);}
		}

		/// <summary>
		/// if has child nodes, returns first child. If not, returns next sibling. If no siblings, returns parent's next
		/// </summary>
		public Node NextSpecial
		{
			get
			{
				if (this.HasChildNodes)
					return this.FirstChild;
				Node n = this.NextSibling;
				if (n!=null)
					return n;

				n = this;
				while (n!=null)
				{
					n = n.ParentNode;
					if (n!=null)
					{
						if (n.NextSibling!=null)
							return n.NextSibling;
					}
				}
				return null;
			}
		}

        /// <summary>
        /// Returns null if the current node is the last in parent's ChildNodes list
        /// </summary>
		public Node NextSibling
		{
			get
			{
				if (this.ParentNode==null)
					return null;
				int nIndex = this.ParentNode.ChildNodes.IndexOfValue(this);
				if (nIndex < this.ParentNode.ChildNodes.Count-1)
					return (Node)this.ParentNode.ChildNodes.GetByIndex(nIndex+1);
				return null;
			}
		}
        /// <summary>
        /// Returns null if the current node is the first in parent's ChildNodes list
        /// </summary>
        public Node PreviousSibling
		{
			get
			{
				int nIndex = this.ParentNode.ChildNodes.IndexOfValue(this);
				if (nIndex > 0)
					return (Node)this.ParentNode.ChildNodes.GetByIndex(nIndex-1);
				return null;
			}
		}

//		public void ReplaceChild(Node newChild, Node oldChild)
//		{
//			//TODO:
//		}
//		public void InsertAfter(Node newChild, Node refChild)
//		{
//		}
//		public void InsertBefore(Node newChild, Node refChild)
//		{
//		}
//		public void PrependChild(string sName, Node oNode)
//		{
//			//TODO: add as first child
//		}

		public void AppendChild(string sName, Node oNode)
		{
			oNode.ParentNode = this;
			this.ChildNodes.Add(sName, oNode);
		}
		public Node AppendChild(string sName)
		{
			//TODO: why same functionality!?
			return this.CreateChild(sName);
//			Node oNode = new Node();
//			oNode.ParentNode = this;
//			this.ChildNodes.Add(sName, oNode);
//			return oNode;
		}
		public void RemoveChild(Node oNode)
		{
			this.ChildNodes.RemoveValue(oNode);
		}

        public Node FindFirstNode(string name)
        {
            if (this.ChildNodes.ContainsKey(name))
                return (Node)this.ChildNodes[name];
            foreach (Node node in this.ChildNodes.Values)
            {
                Node found = node.FindFirstNode(name);
                if (found != null)
                    return found;
            }
            return null;
        }

        /// <summary>
        /// Returns all children (recursively) as a flat List. The internal strucure (parent, children) is preserved.
        /// </summary>
        /// <returns></returns>
        public List<Node> GetFlattened()
        {
            List<Node> flat = new List<Node>();
            foreach (Node node in this.ChildNodes.Values)
                node.AddToFlattened(flat);
            return flat;
        }
        private void AddToFlattened(List<Node> flat)
        {
            flat.Add(this);
            foreach (Node node in this.ChildNodes.Values)
                node.AddToFlattened(flat);
        }

		public System.Xml.XmlDocument CreateXmlDocument()
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
			System.Xml.XmlNode node = doc.CreateElement("root");
			doc.AppendChild(node);
			this.AddToXml(node);
			return doc;
		}
		public void AddToXml(System.Xml.XmlNode xmlNode)
		{
//			System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(
			System.Xml.XmlDocument doc;
			if (xmlNode.GetType() == typeof(System.Xml.XmlDocument))
				doc = (System.Xml.XmlDocument)xmlNode;
			else
				doc = xmlNode.OwnerDocument;

			for (int i = 0; i < this.ChildNodes.Count; i++)
			{
				System.Xml.XmlNode xmlNew;
				Node child = (Node)this.ChildNodes.GetByIndex(i);
				try
				{
					xmlNew = doc.CreateElement(child.Name);
				}
				catch
				{
					xmlNew = doc.CreateElement("IncorrectXmlName");
				}
				if (child.Value!=null)
				{
					System.Xml.XmlAttribute attrib = doc.CreateAttribute("value");
					attrib.InnerText = child.Value.ToString();
					xmlNew.Attributes.Append(attrib);
				}
				xmlNode.AppendChild(xmlNew);
				child.AddToXml(xmlNew);
			}
		}

		public string ToTabbed()
		{
			return this.ToTabbed("\t", "");
		}
		private string ToTabbed(string indent, string currentIndent)
		{
			string sAll = "";
			string sSubIndent = currentIndent + indent;
			for (int i = 0; i < this.ChildNodes.Count; i++)
			{
				Node child = (Node)this.ChildNodes.GetByIndex(i);
				sAll+=currentIndent + child.Name;

				//if (child.Value!=null && child.Value.GetType() == typeof(string))
				//	sAll+="\t"+(string)child.Value;
				if (child.Text!=null)
					sAll+="\t"+(string)child.Text;

				sAll+="\n";
				if (child.HasChildNodes)
					sAll+=child.ToTabbed(indent, sSubIndent);
			}
			return sAll;
		}

		public static Node FromTabbed(string tabbed)
		{
			Node node = new Node();

			tabbed = tabbed.Replace("\r", "");
			string[] lines = tabbed.Split("\n".ToCharArray());

			System.Text.RegularExpressions.Match m;
			int nLastIndentation = -1;
			foreach (string sLine in lines)
			{
				if (sLine.Length == 0)
					continue;
				m = System.Text.RegularExpressions.Regex.Match(sLine, @"[^\t]"); // "\S" non-whitespace chars
				if (m.Index == -1)
					continue;
				int nIndentation = m.Index;

				if (nIndentation > nLastIndentation && nLastIndentation > -1)
				{
					//one level down!
					node = node.LastChild;
				}
				else if (nIndentation < nLastIndentation)
				{
					//level up!
					node = node.GetNthNodeAbove(nLastIndentation-nIndentation);
				}
				string sNode = sLine.Remove(0,m.Index).Trim();

				//check if there is a tab left (eg: name	item)
				//if so, the text after the tab is entered as the node.Value
				m = System.Text.RegularExpressions.Regex.Match(sNode, @"[\t]");
				string sValue = null;
				if (m.Success)
				{
					sValue = sNode.Remove(0,m.Index+1);
					sNode = sNode.Substring(0,m.Index);
				}

				Node subNode = node.CreateChild(sNode);
				subNode.Text = sValue;

				nLastIndentation = nIndentation;
			}

			return node.RootNode;
		}

		public void Merge(Node nodeToMergeWith)
		{
			for (int i = 0; i<nodeToMergeWith.ChildNodes.Count; i++)
			{
				Node node = (Node)nodeToMergeWith.ChildNodes.GetByIndex(i);

				//does it exist here already?
				Node alreadyHereNode = this[node.Name];
				if (alreadyHereNode == null)
					alreadyHereNode = this.CreateChild(node.Name);

				if (node.HasChildNodes)
					alreadyHereNode.Merge(node);
			}
		
		}
	}
}
