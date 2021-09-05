using System;
using System.IO;
using System.Xml;

namespace Endogine.Serialization
{
	/// <summary>
	/// Summary description for EndogineXML.
	/// </summary>
	public class EndogineXML
	{
		public EndogineXML()
		{
		}


		//TODO: move to castlib class
		public static System.Type GetFileWouldBeMemberType(string a_sFilename)
		{
			FileInfo file = new FileInfo(a_sFilename);
			string sExtension = file.Extension;
			if (sExtension == ".bmp"
				|| sExtension == ".gif"
				|| sExtension == ".jpg"
				|| sExtension == ".png")
				return typeof(MemberSpriteBitmap);

			return typeof(System.Boolean);
		}

		public static void LoadMovie(string a_sFilename, Sprite a_sp)
		{
			FileInfo fileMovie = new FileInfo(a_sFilename);
			FileInfo[] files = fileMovie.Directory.GetFiles();
			foreach (FileInfo file in files)
			{
				System.Type type = GetFileWouldBeMemberType(file.FullName);
				if (type == typeof(MemberSpriteBitmap))
				{
					MemberSpriteBitmap mb = new MemberSpriteBitmap(file.FullName);
				}
			}
			Load(a_sFilename, a_sp);
		}

		public static void Load(string a_sFilename, Sprite a_sp)
		{
			XmlDocument doc = new XmlDocument();
			XmlTextReader r = new XmlTextReader(a_sFilename);
			doc.Load(r);
			r.Close();

			RecurseCreateSpritesFromXml(doc.FirstChild.ChildNodes, a_sp);
		}

		private static void RecurseCreateSpritesFromXml(XmlNodeList a_xmlNodes, Sprite a_spToAddTo)
		{
			foreach (XmlNode node in a_xmlNodes)
			{
				if (node.NodeType == XmlNodeType.Text)
					continue;

				if (node.Attributes.Count > 0)
				{
					XmlNode attrib;
					attrib = node.Attributes.GetNamedItem("Assembly");
					if (attrib == null)
						continue;
					string sAssembly = attrib.InnerText;

					attrib = node.Attributes.GetNamedItem("Type");
					if (attrib == null)
						continue;
					string sType = attrib.InnerText;

					System.Runtime.Remoting.ObjectHandle obj = System.Activator.CreateInstance(sAssembly, sType);
					object o = obj.Unwrap();
					Sprite sp = (Sprite)o;

					Endogine.Serialization.Serializer.Deserialize(sp, node);

					sp.Parent = a_spToAddTo;

					XmlNode childrenNode = Endogine.Serialization.XmlHelper.GetNthSubNodeByName(node, "ChildSprites", 0);
					if (childrenNode!=null)
						RecurseCreateSpritesFromXml(childrenNode.ChildNodes, sp);
				}
			}
		}




		public static void Save(string a_sFilename, Sprite a_sp, bool a_bOnlyChildren)
		{
			XmlDocument doc = GenerateXMLDoc(a_sp, a_bOnlyChildren);
			doc.Save(a_sFilename);
		}

		public static XmlDocument GenerateXMLDoc(Sprite a_sp, bool a_bOnlyChildren)
		{
			if (a_sp == null)
				a_sp = EH.Instance.Stage.RootSprite;
			XmlDocument doc = new System.Xml.XmlDocument();
			XmlElement elm = doc.CreateElement("root");
			doc.AppendChild(elm);
			if (a_bOnlyChildren)
			{
				if (a_sp.ChildCount > 0)
				{
					for (int i = 0; i < a_sp.ChildCount; i++)
					{
						RecurseSpritesToXML(elm, a_sp.GetChildByIndex(i));
					}
				}
			}
			else
				RecurseSpritesToXML(elm, a_sp);
			return doc;
		}

		private static void RecurseSpritesToXML(System.Xml.XmlNode a_node, Sprite a_sp)
		{
			System.Xml.XmlNode newNode = Serialization.Serializer.Serialize(a_sp, a_node, null);
			if (a_sp.ChildCount > 0)
			{
				System.Xml.XmlNode childNode = a_node.OwnerDocument.CreateElement("ChildSprites");
				newNode.AppendChild(childNode);
				for (int i = 0; i < a_sp.ChildCount; i++)
				{
					RecurseSpritesToXML(childNode, a_sp.GetChildByIndex(i));
				}
			}
		}
	}
}
