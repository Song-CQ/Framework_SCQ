using System;
using System.Drawing;
using System.Collections;

namespace Endogine.ResourceManagement
{
	/// <summary>
	/// Summary description for MemberBase.
	/// </summary>
	public class MemberBase
	{
		protected EndogineHub m_endogine;
		protected ArrayList m_aSpritesUsingMe;
		protected ArrayList m_aSpritesStrategyUsingMe;
		protected string m_sName;
		protected string m_sFileFullName;
		protected Bitmap m_bmpThumbnail;

		public MemberBase()
		{
			m_endogine = EndogineHub.Instance;
			m_aSpritesUsingMe = new ArrayList();
		}

		public virtual void Dispose()
		{
			m_endogine.CastLib.Remove(this);
		}

		public void AddSprite(Sprite a_sp)
		{
			m_aSpritesUsingMe.Add(a_sp);
		}

		public void RemoveSprite(Sprite a_sp)
		{
			m_aSpritesUsingMe.Remove(m_aSpritesUsingMe.IndexOf(a_sp));
		}

		public string FileFullName
		{
			get {return m_sFileFullName;}
			set {m_sFileFullName = value;}
		}
		public string Name
		{
			get {return m_sName;}
			set
			{
				m_sName = value;
				m_endogine.CastLib.Remove(this);
				m_endogine.CastLib.Add(this);
			}
		}

		public virtual System.Drawing.Bitmap Thumbnail
		{
			get
			{
				return this.m_bmpThumbnail;
			}
		}
	}
}
