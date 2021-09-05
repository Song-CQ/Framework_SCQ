using System;
using System.Collections;
using System.Drawing;

namespace Endogine.AStar
{
	/// <summary>
	/// Based on code by John Kenedy
	/// www.innosia.com/download/walkai.zip (c) 2005
	/// </summary>
	public class AStarSearch
	{
		//these events will probably mostly be used for debugging and visualization
		public delegate void SearchDelegate(object sender, EPoint coordinate);
		public event SearchDelegate SearchedCoordinate;
		public event SearchDelegate ChangedStartCoordinate;
		public event SearchDelegate ChangedGoalCoordinate;
		public event SearchDelegate ChangedAcceptableGoals;
		public event SearchDelegate SearchStarted;
		public event SearchDelegate SearchFinished;

		private AStarNode m_nodeCurrent;
		private AStarNode m_nodeStart;
		private AStarNode m_nodeGoal;

		private ArrayList m_listClosed = new ArrayList();
		private ArrayList m_listOpen = new ArrayList();
		private ArrayList m_listSolution = new ArrayList();

		//public EPoint Size;
		private ERectangle m_rctBounds;

		private bool m_bPaused;
		private bool m_bFoundSolution;

		private AStarNode[,] _TheMap;
		private AStarNode[] SuccesorNode = new AStarNode[8];

		private Point[] point = new Point[8];

		private ArrayList _acceptableNodes;

		public AStarSearch(int[,] occupiedGrid)
		{
			this.m_rctBounds = new ERectangle(0,0,occupiedGrid.GetLength(0)-1, occupiedGrid.GetLength(1)-1);

			_TheMap = new AStarNode[this.m_rctBounds.Width+1,this.m_rctBounds.Height+1];

			for(int i=0; i<_TheMap.GetLength(0); i++)
			{
				for(int j=0; j<_TheMap.GetLength(1); j++)
				{
					AStarNode node = new AStarNode(i,j);
					_TheMap[i,j] = node;

					if (occupiedGrid[i,j]==0)
						node.Walkable = true;
					else
						node.Walkable = false;
				}
			}

			for (int i=0; i<8; i++) point[i] = new Point();
		}

		public void Dispose()
		{
			m_listClosed.Clear();
			m_listOpen.Clear();
			m_listSolution.Clear();

			for(int i=0; i<_TheMap.GetLength(0); i++)
			{
				for(int j=0; j<_TheMap.GetLength(1); j++)
				{
					_TheMap[i,j].Dispose();
				}
			}
		}

		/// <summary>
		///  We sometimes know in advance that a location cannot be reached -
		///  but we want to come as close as possible. In these cases, set a number of acceptable goal points
		/// </summary>
		public ArrayList AcceptableGoals
		{
			set
			{
				if (value!=null && value.Count > 0)
				{
					this._acceptableNodes = new ArrayList();
					foreach (Point pnt in value)
						this._acceptableNodes.Add(this._TheMap[pnt.X,pnt.Y]);
					if (ChangedAcceptableGoals!=null)
						ChangedAcceptableGoals(this, null);
				}
			}
			get
			{
				ArrayList lst = new ArrayList();
				foreach (AStarNode node in this._acceptableNodes)
					lst.Add(node.Pos);
				return lst;
			}
		}

		public ArrayList BeginSearch(EPoint start, EPoint goal)
		{
			this.StartPos = start;
			this.GoalPos = goal;
			return this.BeginSearch();
		}

		public ArrayList BeginSearch()
		{
			this.m_bFoundSolution = false;

			if (this.m_nodeStart==null || this.m_nodeGoal==null)
				throw new Exception("Need start and goal!");

			m_nodeStart.ParentNode = null;
			m_nodeCurrent = m_nodeStart;
			UpdateCost(m_nodeStart);
			
			ResetMap();

			m_listOpen.Clear();
			m_listClosed.Clear();
			m_listSolution.Clear();
			m_listOpen.Add(m_nodeStart);

			if (this.SearchStarted!=null)
				this.SearchStarted(this, this.StartPos);

			this.Search();

			this._acceptableNodes = null; //don't reuse this for next search

			return this.Solution;
		}

		/// <summary>
		/// Get an array of EPoints describing the solution path and null if there is no solution.
		/// </summary>
		public ArrayList Solution
		{
			get
			{
				if (this.m_listSolution==null)
					return null;
				if (!this.m_bFoundSolution)
					return null;

				ArrayList aPath = new ArrayList();
				foreach (AStarNode node in this.m_listSolution)
					aPath.Add(node.Pos);
				return aPath;
			}
		}

		protected void ResetMap()
		{
			for (int i=0; i<_TheMap.GetLength(0); i++)
			{
				for (int j=0; j<_TheMap.GetLength(1); j++)
					_TheMap[i,j].Reset();
			}
		}

		/// <summary>
		/// Used to pause searching (e.g. to get time to visualize or debug)
		/// </summary>
		public bool Paused
		{
			get {return this.m_bPaused;}
			set
			{
				this.m_bPaused = value;
				if (!value && this.m_listClosed.Count > 0)
					this.Search();
			}
		}
		protected void UpdateCost(AStarNode box)
		{
			if (box.ParentNode!= null)
			{
				box.CostToStart = box.Cost + box.ParentNode.CostToStart;
//				if (box.ParentIsDiagonal) //TODO: this doesn't seem to help getting rid of diagonal loss..
//					box.CostToStart+=0.4142f;
			}
			else
				box.CostToStart = 0;
			//Manhattan distance:
			box.CostToGoal =  Math.Abs(GoalNode.Pos.X - box.Pos.X);
			box.CostToGoal += Math.Abs(GoalNode.Pos.Y - box.Pos.Y);
			box.TotalCost = box.CostToStart + box.CostToGoal + box.Cost;
		}
		
		protected void Search()
		{
			while( m_listOpen.Count>0 && !this.m_bPaused)
			{
				m_nodeCurrent = PopCheapestNodeInOpenList();

				//JB:
				if (m_nodeCurrent == GoalNode || (this._acceptableNodes!=null && this._acceptableNodes.Contains(m_nodeCurrent)))
				{
					if (this._acceptableNodes!=null && this._acceptableNodes.Contains(m_nodeCurrent))
					{
						this.m_nodeGoal = this.m_nodeCurrent;
					}

					//FOUND THE SOLUTION!
					while(m_nodeCurrent != null) 
					{
						m_listSolution.Insert(0,m_nodeCurrent);
						m_nodeCurrent = m_nodeCurrent.ParentNode;
					}
					this.m_bFoundSolution = true;

					//TODO: optimization: check for shortcuts - straight lines, as long as possible, between points on the path
					//so the final path is a number of points (that are not necessarily adjacent in the grid)
					if (SearchFinished!=null)
						SearchFinished(this, this.GoalNode.Pos);
					break;
				}
				
				GetSuccessorOfCurrentNode();
				for (int i=0; i<SuccesorNode.Length; i++)
				{
					if (SuccesorNode[i]==null) continue;
					if (!m_listOpen.Contains(SuccesorNode[i]) && !m_listClosed.Contains(SuccesorNode[i]))
					{
						SuccesorNode[i].ParentNode = m_nodeCurrent;
						UpdateCost(SuccesorNode[i]);
						m_listOpen.Add(SuccesorNode[i]);
					}
				}
				if (m_nodeCurrent != StartNode && m_nodeCurrent != GoalNode)
				{
					if (SearchedCoordinate!=null)
						SearchedCoordinate(this, m_nodeCurrent.Pos);
				}
				m_listClosed.Add(m_nodeCurrent);
			}
		}

//		protected void IsDiagonalWalkAble(int x, int j)
//		{
//		}

		protected void GetSuccessorOfCurrentNode()
		{
			point[0].X = m_nodeCurrent.Pos.X-1;	point[0].Y = m_nodeCurrent.Pos.Y-1;
			point[1].X = m_nodeCurrent.Pos.X;		point[1].Y = m_nodeCurrent.Pos.Y-1;
			point[2].X = m_nodeCurrent.Pos.X+1;	point[2].Y = m_nodeCurrent.Pos.Y-1;
			point[3].X = m_nodeCurrent.Pos.X+1;	point[3].Y = m_nodeCurrent.Pos.Y;
			point[4].X = m_nodeCurrent.Pos.X+1;	point[4].Y = m_nodeCurrent.Pos.Y+1;
			point[5].X = m_nodeCurrent.Pos.X;		point[5].Y = m_nodeCurrent.Pos.Y+1;
			point[6].X = m_nodeCurrent.Pos.X-1;	point[6].Y = m_nodeCurrent.Pos.Y+1;
			point[7].X = m_nodeCurrent.Pos.X-1;	point[7].Y = m_nodeCurrent.Pos.Y;			

			for (int i=0; i<8; i++)
			{
				if ((!IsOutBoundOrNotPassable(point[i].X,point[i].Y)))
				{
					if (i % 2 == 0)
					{
						int err = 0;
						int front = i + 1; if (front==8) front = 0;
						int end = i - 1; if (end==-1) end = 7;

						if (!IsOutBound(point[front].X,point[front].Y))
							if (_TheMap[point[front].X,point[front].Y].Walkable == false)
								err += 1;
							
						if (!IsOutBound(point[end].X,point[end].Y))
							if (_TheMap[point[end].X,point[end].Y].Walkable == false)
								err += 1;

						if (err==0)
							SuccesorNode[i] = _TheMap[point[i].X,point[i].Y];
						else
							SuccesorNode[i] = null;
					}
					else
					{
						SuccesorNode[i] = _TheMap[point[i].X,point[i].Y];
					}
				}
				else
				{
					SuccesorNode[i] = null;
				}
			}
		}

		protected AStarNode PopCheapestNodeInOpenList()
		{
			float min=((AStarNode)m_listOpen[0]).TotalCost;
			m_nodeCurrent = (AStarNode)m_listOpen[0];
			for (int i=1; i<m_listOpen.Count; i++)
			{
				if (((AStarNode)m_listOpen[i]).TotalCost < min)
				{
					min =((AStarNode)m_listOpen[i]).TotalCost;
					m_nodeCurrent = (AStarNode)m_listOpen[i];
				}
			}
			m_listOpen.Remove(m_nodeCurrent);
			return m_nodeCurrent;
		}

		public bool IsOutBoundOrNotPassable(int x, int y)
		{
			if (IsOutBound(x,y)) return true;
			if (!_TheMap[x,y].Walkable) return true;
			return false;
		}

		protected bool IsOutBound(int x, int y)
		{
			return !this.m_rctBounds.Contains(new EPoint(x,y));
//			if (x<0) return true;
//			if (y<0) return true;
//			if (x>=this.Size.X) return true;
//			if (y>=this.Size.Y) return true;
//			return false;
		}

		private bool IsInClosedList(AStarNode sender)
		{
			return m_listClosed.Contains(sender);
		}

		private bool IsInOpenList(AStarNode sender)
		{
			return m_listOpen.Contains(sender);
		}

		#region Properties		
		public ArrayList SolutionNodes
		{
			get{return m_listSolution;}
		}
		
		public EPoint StartPos
		{
			get {return m_nodeStart.Pos;}
			set
			{
				this.m_rctBounds.MakePointInside(value);
				this.StartNode = this._TheMap[value.X,value.Y];
			}
		}
		public EPoint GoalPos
		{
			get {return this.m_nodeGoal.Pos;}
			set
			{
				this.m_rctBounds.MakePointInside(value);
				//Possible optimizations: when the goal node is completely surrounded by non-passable nodes, it can't be reached
				this.GoalNode = this._TheMap[value.X,value.Y];
			}
		}

		public AStarNode StartNode
		{
			get{return m_nodeStart;	}
			set
			{
				m_nodeStart = value;
				if (ChangedStartCoordinate!=null)
					ChangedStartCoordinate(this, value.Pos);
			}
		}

		public AStarNode GoalNode
		{
			get{return m_nodeGoal;	}
			set
			{
				m_nodeGoal = value;
				if (ChangedGoalCoordinate!=null)
					ChangedGoalCoordinate(this, value.Pos);
			}
		}

		public EPoint Size
		{
			get {return this.m_rctBounds.Size;}
		}
		#endregion
	}
}
