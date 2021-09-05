using System;

namespace Endogine.AStar
{
	/// <summary>
	/// Based on code by John Kenedy
	/// www.innosia.com/download/walkai.zip (c) 2005
	/// </summary>
	public class AStarNode
	{
		protected EPoint _Pos;

		protected bool _Walkable=true;
		protected float _Cost;
		protected float _TotalCost=0;
		protected float _CostToStart=0;
		protected float _CostToGoal=0;
		protected AStarNode _Parent=null;
		protected bool _IsParentDiagonal;

		public AStarNode(int i, int j)
		{
			_Pos = new EPoint(i,j);
		}

		public void Dispose()
		{
			_Parent = null;
		}

		public AStarNode ParentNode
		{
			get {return _Parent; }
			set
			{
				_Parent = value;
				if (_Parent==null)
					return;
				EPoint pntDiff = this.Pos - this._Parent.Pos;
				this._IsParentDiagonal = (pntDiff.X != 0 && pntDiff.Y != 0)?true:false;
			}
		}

		public float TotalCost
		{
			get{return _TotalCost;}
			set{_TotalCost = value;}
		}

		public float Cost
		{
			get{	return _Cost;	}
			set{_Cost = value;	}
		}

		public float CostToStart
		{
			get{return _CostToStart;}
			set{_CostToStart = value;}
		}

		public float CostToGoal
		{
			get{return _CostToGoal;	}
			set{_CostToGoal = value;}
		}

		public EPoint Pos
		{
			get {return this._Pos;}
		}

		public bool Walkable
		{
			get{return _Walkable;}
			set{_Walkable = value;}
		}

		public bool ParentIsDiagonal
		{
			get{return this._IsParentDiagonal;}
		}

		public void Reset()
		{
			Cost		= 1;
			TotalCost	= 0;
			CostToStart = 0;
			CostToGoal	= 0;
			ParentNode	= null;
			this._IsParentDiagonal = false;
		}

	}
}
