using System;
using System.Xml;
using System.ComponentModel;

namespace Endogine.Serialization
{
	/// <summary>
	/// Summary description for Serializer.
	/// </summary>
	public class Serializer
	{
		public Serializer()
		{
		}

		public static object CreateFromXMLNode(System.Xml.XmlNode a_node)
		{
			string sAssembly = a_node.Attributes.GetNamedItem("Assembly").InnerText;
			string sType = a_node.Attributes.GetNamedItem("Type").InnerText;

			System.Runtime.Remoting.ObjectHandle obj = System.Activator.CreateInstance(sAssembly, sType);
			object o = obj.Unwrap();
			Deserialize(o, a_node);
			return o;
		}

		public static void Deserialize(object a_obj, System.Xml.XmlNode a_node)
		{
			System.Type type = a_obj.GetType();
			System.Reflection.PropertyInfo[] propInfos = type.GetProperties();

			foreach (System.Reflection.PropertyInfo propInfo in propInfos)
			{
				System.Xml.XmlNode node = null;
				node = Endogine.Serialization.XmlHelper.GetNthSubNodeByName(a_node, propInfo.Name, 0);
				if (node == null)
					continue;

				if (node.ChildNodes.Count > 0)
				{
					XmlNode childNode = XmlHelper.GetNthSubNodeByName(node, "ChildNodes", 0);
					if (childNode != null)
					{
					}
					else
					{
						object o = CreateFromXMLNode(node);
						propInfo.SetValue(a_obj, o, null);
						continue;
					}
				}

				string sVal = node.Attributes.GetNamedItem("v").InnerText;

				SetPropertyFromString(a_obj, propInfo, sVal);
			}
		}

		public static void SetPropertyFromString(object a_obj, System.Reflection.PropertyInfo propInfo, string sVal)
		{
			if (propInfo.PropertyType == typeof(float))
				propInfo.SetValue(a_obj, Convert.ToSingle(sVal), null);
			else if (propInfo.PropertyType == typeof(int))
				propInfo.SetValue(a_obj, Convert.ToInt32(sVal), null);
			else if (propInfo.PropertyType == typeof(string))
			{
				try
				{
					propInfo.SetValue(a_obj, sVal, null);
				}
				catch
				{
					//TODO: try shouldn't be needed, CastLib should replace with a "NotFound" member.
				}
			}
			else if (propInfo.PropertyType == typeof(bool))
				propInfo.SetValue(a_obj, Convert.ToBoolean(sVal), null);
			else if (propInfo.PropertyType == typeof(System.Drawing.Color))
			{
				if (sVal.IndexOf("Color [") == 0)
				{
					int i = sVal.IndexOf("[");
					sVal = sVal.Remove(0, i+1);
					i = sVal.IndexOf("]");
					sVal = sVal.Substring(0,i);
				}
				propInfo.SetValue(a_obj, System.Drawing.Color.FromName(sVal), null);
			}
		}

		public static XmlNode Serialize(object a_obj, XmlNode a_node, string a_name)
		{
			System.Type type = a_obj.GetType();
			System.Reflection.PropertyInfo[] propInfos = type.GetProperties();

			if (a_name == null)
				a_name = "GNode";
			XmlElement elm = a_node.OwnerDocument.CreateElement(a_name); //type.Name);

			XmlAttribute attrib;
			attrib = a_node.OwnerDocument.CreateAttribute("Assembly", null);
			attrib.InnerText = "Endogine"; //TODO: type.Assembly.AssemblyQualifiedName (=GetName())
			elm.Attributes.Append(attrib);

			//TODO: we should optionally be able to specify which base class to serialize.
			//For example, we might want to serialize all different sprite-based objects as sprites,
			//not as Players, Balls, Ships etc...
			attrib = a_node.OwnerDocument.CreateAttribute("Type", null);
			attrib.InnerText = type.FullName;
			elm.Attributes.Append(attrib);

			a_node.AppendChild(elm);

			foreach (System.Reflection.PropertyInfo propInfo in propInfos)
			{
				if (propInfo.PropertyType.IsSerializable && //!propInfo.PropertyType.IsArray &&
					propInfo.CanRead && propInfo.CanWrite && propInfo.PropertyType.IsPublic)
				{
					object propValue = null;
					try
					{
						propValue = propInfo.GetValue(a_obj, null);
					}
					catch
					{
						EndogineHub.Put(propInfo.Name + " failed");
						continue;
					}

					if (propValue == null)
						continue;

					object[] attribs;

//					attribs = propInfo.GetCustomAttributes(typeof(BrowsableAttribute), true);
//					if (attribs.Length > 0)
//					{
//						BrowsableAttribute propAttrib = (BrowsableAttribute)attribs[0];
//						if (propAttrib.Browsable == false)
//							continue;
//					}

					attribs = propInfo.GetCustomAttributes(typeof(DesignerSerializationVisibilityAttribute), true);
					if (attribs.Length > 0)
					{
						DesignerSerializationVisibilityAttribute propAttrib = (DesignerSerializationVisibilityAttribute)attribs[0];
						if (propAttrib.Visibility == DesignerSerializationVisibility.Hidden)
							continue;
					}

//					//don't serialize if default value:
					attribs = propInfo.GetCustomAttributes(typeof(DefaultValueAttribute), true);
					if (attribs.Length > 0)
					{
						DefaultValueAttribute propAttrib = (DefaultValueAttribute)attribs[0];
						if (propAttrib.Value.Equals(propValue))
							continue;
					}

					if (propInfo.PropertyType.IsValueType == false
						&& propInfo.PropertyType.IsSerializable
						&& propInfo.PropertyType.UnderlyingSystemType != typeof(string))
					{
						Serialize(propValue, elm, propInfo.Name);
					}
					else
					{
							string sVal = Convert.ToString(propValue);
							System.Xml.XmlElement prop = a_node.OwnerDocument.CreateElement(propInfo.Name);
							prop.SetAttribute("v", null, sVal);
							elm.AppendChild(prop);
					}
				}
			}
			return elm;
		}
	}
}
