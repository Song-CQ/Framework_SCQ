using System;
using Endogine;

namespace CaveHunter
{
	/// <summary>
	/// Summary description for BhReportWhenOutside.
	/// </summary>
	public class BhReportWhenOutside : Behavior
	{
		public delegate void OutsideDelegate(Sprite sp);
		public event OutsideDelegate Outside;

		public BhReportWhenOutside()
		{
		}

		protected override void EnterFrame()
		{
			base.EnterFrame();
			if (this.m_sp.ConvParentLocToRootLoc(this.m_sp.Loc).X < -40)
			{
				if (Outside!=null)
					Outside(m_sp);
			}
		}
	}
}
