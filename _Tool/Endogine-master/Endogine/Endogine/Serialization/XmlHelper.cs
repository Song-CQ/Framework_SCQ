using System;
using System.Xml;

namespace Endogine.Serialization
{
	/// <summary>
	/// Summary description for XmlHelper.
	/// </summary>
	public class XmlHelper
	{
		public XmlHelper()
		{
		}

		/// <summary>
		/// Don't get it... shouldn't this exist already?
		/// </summary>
		/// <param name="a_node"></param>
		/// <param name="a_sName"></param>
		/// <param name="a_nIndex"></param>
		/// <returns></returns>
		public static XmlNode GetNthSubNodeByName(XmlNode a_node, string a_sName, int a_nIndex)
		{
			a_sName = a_sName.ToLower();
			foreach (XmlNode node in a_node.ChildNodes)
			{
				if (node.Name.ToLower() == a_sName)
				{
					if (a_nIndex-- == 0)
						return node;
				}
			}
			return (XmlNode)null;
		}

		public static XmlNode RenameNode(XmlNode node, string sNewName)
		{
			//can't just change the name of an XML node, so we have to create new, copy stuff and replace.
			XmlNode newNode = node.OwnerDocument.CreateNode(node.NodeType, sNewName, null);
			newNode.InnerXml = node.InnerXml;
			foreach (XmlAttribute attrib in node.Attributes)
			{
				XmlAttribute newAttrib = (XmlAttribute)node.OwnerDocument.CreateNode(XmlNodeType.Attribute, attrib.Name, null);
				newAttrib.InnerText = attrib.InnerText;
				newNode.Attributes.Append(newAttrib);
			}
			XmlNode parentNode = node.ParentNode;
			XmlNode insertBeforeNode = node.NextSibling;
			parentNode.RemoveChild(node);
			if (insertBeforeNode == null)
				parentNode.AppendChild(newNode);
			else
				parentNode.InsertBefore(newNode, insertBeforeNode);
			//parentNode.ReplaceChild(newNode, node);
			return newNode;
		}

		public static XmlDocument HashtableToXml(System.Collections.Hashtable ht)
		{
			XmlDocument doc = new XmlDocument();
			System.Xml.XmlNode node, subnode;
			node = doc.CreateElement("root");
			doc.AppendChild(node);

			System.Collections.IDictionaryEnumerator en = ht.GetEnumerator();
			while(en.MoveNext())
			{
				subnode = node.AppendChild(node.OwnerDocument.CreateElement(Convert.ToString(en.Key)));
				subnode.InnerText = Convert.ToString(en.Value);
			}

			return doc;
		}

		public static string GetValueOrInnerText(XmlNode node)
		{
			XmlAttribute attr = node.Attributes["value"];
			if (attr!=null)
				return attr.InnerText;
			return node.InnerText;
		}


		/// <summary>
		/// One level deep only. Does NOT substitute existing nodes.
		/// </summary>
		/// <param name="nodeDst"></param>
		/// <param name="nodeSrc"></param>
		public static void Merge(XmlNode nodeDst, XmlNode nodeSrc)
		{
			//TODO: doesn't merge Attributes
			foreach (XmlNode node in nodeSrc.ChildNodes)
			{
				XmlNode newNode = nodeDst[node.Name];
				if (newNode==null)
				{
					newNode = nodeDst.OwnerDocument.CreateNode(node.NodeType, node.Name, null);
					nodeDst.AppendChild(newNode);
					newNode.InnerText = node.InnerText;
				}
				//Merge(newNode, node);
			}
		}
		public static void MergeOverwrite(XmlNode nodeDst, XmlNode nodeSrc)
		{
			foreach (XmlNode node in nodeSrc.ChildNodes)
			{
				XmlNode newNode = nodeDst[node.Name];
				if (newNode==null)
				{
					newNode = nodeDst.OwnerDocument.CreateNode(node.NodeType, node.Name, null);
					nodeDst.AppendChild(newNode);
				}
				newNode.InnerXml = node.InnerXml;
			}
		}

		public static XmlElement CreateAndAddElement(XmlNode parentNode, string childName)
		{
			XmlElement node;
			if (parentNode.GetType() == typeof(XmlDocument))
				node = ((XmlDocument)parentNode).CreateElement(childName);
			else
				node = parentNode.OwnerDocument.CreateElement(childName);
			parentNode.AppendChild(node);
			return node;
		}

		public static XmlElement CreateAndAddElementWithValue(XmlNode parentNode, string childName, string val)
		{
			XmlElement node;
			XmlDocument owner = null;
			if (parentNode.GetType() == typeof(XmlDocument))
				owner = (XmlDocument)parentNode;
			else
				owner = parentNode.OwnerDocument;
			node = owner.CreateElement(childName);

			parentNode.AppendChild(node);

			XmlAttribute attr = owner.CreateAttribute("value");
			attr.InnerText = val;
			node.Attributes.Append(attr);

			return node;
		}

        public static XmlAttribute CreateAndAddAttribute(XmlNode parentNode, string attributeName)
        {
            return CreateAndAddAttribute(parentNode, attributeName);
        }
        public static XmlAttribute CreateAndAddAttribute(XmlNode parentNode, string attributeName, string innerText)
        {
            XmlAttribute node;
            if (parentNode.GetType() == typeof(XmlDocument))
                node = ((XmlDocument)parentNode).CreateAttribute(attributeName);
            else
                node = parentNode.OwnerDocument.CreateAttribute(attributeName);
            parentNode.Attributes.Append(node);
            if (innerText != null)
                node.InnerText = innerText;
            return node;
        }

        /// <summary>
        /// Unlike XmlNode.AppendChild(), this one works regardless of if they have the same OwnerDocument or not. Unless there's a Scheme
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="childNode"></param>
        public static void AppendChild(XmlNode parentNode, XmlNode childNode)
        {
            if (parentNode.OwnerDocument == childNode.OwnerDocument)
                parentNode.AppendChild(childNode);
            else
            {
                XmlNode newChildNode = parentNode.OwnerDocument.CreateNode(childNode.NodeType, childNode.Name, null);
                parentNode.AppendChild(newChildNode);
                if (childNode.InnerText.Length > 0)
                    newChildNode.InnerText = childNode.InnerText;

                foreach (XmlAttribute attribute in childNode.Attributes)
                {
                    CreateAndAddAttribute(newChildNode, attribute.Name, attribute.InnerText);
                }

                foreach (XmlNode node in childNode.ChildNodes)
                {
                    AppendChild(newChildNode, node);
                }
            }
        }
	}
}