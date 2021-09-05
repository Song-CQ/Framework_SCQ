using System;
using Endogine;

namespace Tests.SpaceInvaders
{
	/// <summary>
	/// Summary description for Cover.
	/// </summary>
	public class Cover : Sprite
	{
		public Cover()
		{
			this.Color = GameMain.Instance.m_clrOffwhite;
			this.SetGraphics("Cover");
		}

		public bool CheckCollision(Sprite sp)
		{
			EPointF pntHit = this.GetCollisionPoint(sp);
			if (pntHit != null)
			{
				//TODO: per-pixel collision detection. Make part of cover disappear.
				return true;
			}
			return false;
		}

	}
}
