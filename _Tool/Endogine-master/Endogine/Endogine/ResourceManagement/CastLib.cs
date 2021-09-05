using System;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.IO;
using Endogine.ResourceManagement;

//TODO: should be part of Endogine.ResourceManagement, but this takes so long to write...
namespace Endogine
{
	/// <summary>
	/// Summary description for CastLib.
	/// </summary>
	public class CastLib
	{
		//private DataTable m_dtMembers;
		private Hashtable _nameToMember;
		private Hashtable m_hashToMember;
		private ArrayList m_aMembers;
		public bool UseDummiesForFilesNotFound = false;

		private EndogineHub m_endogine;
		private DirectoryInfo m_dirInfo;

		//public PicRefs Pictures;
		//public Animations Animations;
		public FrameSets FrameSets;

		public CastLib(EndogineHub a_endogine)
		{
			m_endogine = a_endogine;

			//this.Animations = new Animations();
			this.FrameSets = new FrameSets();

			//this.Pictures = new PicRefs();
		}

		public void Init(string defaultDirectory)
		{
			m_dirInfo = new DirectoryInfo(defaultDirectory);

			m_aMembers = new ArrayList();
			m_hashToMember = new Hashtable();
			this._nameToMember = new Hashtable();

//			m_dtMembers = new DataTable();
//			m_dtMembers.Columns.Add("Name", typeof(System.String));
//			m_dtMembers.Columns.Add("Hash", typeof(System.Int32));

			PicRef.ScanMediaDirectories();
		}

		public void Dispose()
		{
//			m_dtMembers.Dispose();
		}

		/// <summary>
		/// Gets the default directory for media
		/// </summary>
		public string DirectoryPath
		{
			set
            {
                this.m_dirInfo = new DirectoryInfo(value);
            }
			get {return m_dirInfo.FullName + "\\";}
		}

		/// <summary>
		/// Finds a file. If a fully qualified path is supplied, it does nothing.
		/// Looks first in default castlib folder, then in application folder.
		/// If no extension is provided, the first found file which matches will be returned.
		/// TODO: could add several alternative paths to look in
		/// </summary>
		/// <param name="a_sFile">File name or path</param>
		public string FindFile(string a_sFile)
		{
			return AppSettings.Instance.FindFile(a_sFile);
		}

		public MemberBase GetOrCreate(string a_sName)
		{
			MemberBase mb = this.GetByName(a_sName);
			if (mb != null)
				return mb;

			return ((MemberBase)new MemberSpriteBitmap(a_sName));
		}

		public void Add(MemberBase a_mb)
		{
			this._nameToMember[a_mb.Name] = a_mb;
//			DataRow row = m_dtMembers.NewRow();
//			m_dtMembers.Rows.Add(row);
//
//			row["Name"] = a_mb.Name;
//			row["Hash"] = a_mb.GetHashCode();

			m_hashToMember[a_mb.GetHashCode()] = a_mb;

			m_aMembers.Add(a_mb);
		}

		public void Remove(MemberBase a_mb)
		{
			if (a_mb.Name==null)
			{
				return;
			}
			this._nameToMember.Remove(a_mb.Name);
//			DataRow[] rows = m_dtMembers.Select("Hash = "+a_mb.GetHashCode());
//			foreach (DataRow row in rows)
//				m_dtMembers.Rows.Remove(row);
			m_hashToMember.Remove(a_mb.GetHashCode());
			m_aMembers.Remove(a_mb);
		}

		public MemberBase GetByName(string a_sName)
		{
			return (MemberBase)this._nameToMember[a_sName];
			//TODO: This is far too slow!!!!
//			DataRow[] rows = m_dtMembers.Select("Name = '"+a_sName+"'");
//			if (rows.GetLength(0) > 0)
//			{
//				return (MemberBase)m_hashToMember[(int)rows[0]["Hash"]];
//			}
//			return (MemberBase)null;
		}

		public MemberBase GetByIndex(int a_nIndex)
		{
			return (MemberBase)m_aMembers[a_nIndex];
		}

		public int GetMemberCount()
		{
			return m_aMembers.Count;
		}

		/// <summary>
		/// All textures are ripped to bitmap objects, so they can be reloaded
		/// when device is switched to another.
		/// </summary>
		public void RipBitmapsFromDevice()
		{
			foreach (MemberBase mb in this.m_aMembers)
			{
				if (mb.GetType() == typeof(MemberSpriteBitmap))
				{
					//((MemberSpriteBitmap)mb).makebmp();
				}
			}
		}
	}
}
