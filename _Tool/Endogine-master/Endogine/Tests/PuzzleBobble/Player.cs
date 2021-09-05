using System;
using System.Drawing;
using Endogine;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PuzzleBobble
{
	/// <summary>
	/// Summary description for Player.
	/// </summary>
	public class Player : Sprite
	{
		private Sprite m_arrow;
		private PlayArea m_playArea;
		private int m_nAngle = 35; //0
		private int m_nAngleStep = 1;

		public Score m_score;
		private ArrayList m_aComingBalls = null;
		private Ball m_shootingBall;
		private Sprite m_spTarget;

		private KeysSteering m_keysSteering;

		public Player(PlayArea a_playArea, Hashtable a_htDefaultKeys)
		{
			m_playArea = a_playArea;
			MemberName = "Cross";

			m_arrow = new Sprite();
			m_arrow.Parent = this;
			m_arrow.Name = "Arrow";
			m_arrow.MemberName = "Cross";
			m_arrow.Member.CenterRegPoint();
			m_arrow.Member = m_arrow.Member;
			m_arrow.LocZ = 60;
			//m_arrow.Loc = m_playArea.Grid.GetGfxLocFromGridLoc(m_playArea.Grid.GetGridStartLoc());

			if (a_htDefaultKeys.Count == 0)
			{
				a_htDefaultKeys = new Hashtable();
				a_htDefaultKeys.Add("left", Keys.Left);
				a_htDefaultKeys.Add("right", Keys.Right);
				a_htDefaultKeys.Add("up", Keys.Up);
				a_htDefaultKeys.Add("shoot", Keys.Down);
			}
			m_keysSteering = new KeysSteering(a_htDefaultKeys);
            m_keysSteering.AddPair("left", "right");
            m_keysSteering.KeyEvent += new Endogine.KeyEventHandler(m_keysSteering_KeyEvent);

			m_score = new Score();

			m_spTarget = new Sprite();
			m_spTarget.MemberName = "Cross";
			m_spTarget.CenterRegPoint();
			m_spTarget.Blend = 50;
			m_spTarget.Parent = m_playArea;
			m_spTarget.LocZ = 50;
		}

        void m_keysSteering_KeyEvent(KeyEventArgs e, bool bDown)
        {
            if (!bDown)
                return;
            if (m_keysSteering.GetActionForKey(e.KeyCode) == "shoot")
                LaunchBall();
        }

		public override void EnterFrame()
		{
			base.EnterFrame();

			if (m_keysSteering.GetKeyActive("left"))
				Angle = Angle+m_nAngleStep;
			
			else if (m_keysSteering.GetKeyActive("right"))
				Angle = Angle-m_nAngleStep;
			
			else if (m_keysSteering.GetKeyActive("up"))
			{
				if (Angle < 0)
					Angle = Angle+m_nAngleStep;
				else if (Angle > 0)
					Angle = Angle-m_nAngleStep;
			}
		}

        //private void m_endogine_KeyEvent(System.Windows.Forms.KeyEventArgs e, bool bDown)
        //{
        //    if (!bDown)
        //        return;
        //    if (m_keysSteering.GetActionForKey(e.KeyCode) == "shoot")
        //        LaunchBall();
        //}

		public int Angle
		{
			set {
				m_nAngle = Math.Max(Math.Min(value,80),-80);
				double dAngle = -Math.PI*m_nAngle/180;
				float fSpeed = 30;
				EPointF pntVel = new EPointF(fSpeed*(float)Math.Sin(dAngle), -fSpeed*(float)Math.Cos(dAngle));
				m_arrow.Loc = pntVel;

				//m_arrow.Rotation = m_nAngle;
				ShowPath();
			}
			get {return m_nAngle;}
		}

		private void AddComingBall()
		{
			Random rnd = new Random();
			ArrayList aTypes = m_playArea.Grid.GetBallTypes();
			foreach (Ball ball in m_aComingBalls)
			{
				if (!aTypes.Contains(ball.BallType))
					aTypes.Add(ball.BallType);
			}

			int nType = 0;
			if (aTypes.Count>0)
				nType = (int)aTypes[rnd.Next(aTypes.Count)];
			Ball newball = new Ball(nType, m_playArea);
			m_aComingBalls.Add(newball);
		}

		private void PushNextBall()
		{
			Ball ball = (Ball)m_aComingBalls[0];
			ball.Loc = Loc.Copy(); //m_playArea.Grid.GetGfxLocFromGridLoc(m_playArea.Grid.GetGridStartLoc());
			for (int i = 1; i < m_aComingBalls.Count; i++)
			{
				Ball ballX = (Ball)m_aComingBalls[i];
				ballX.LocX = ball.LocX + 20;
				ballX.LocY = ball.LocY + 20;
			}
		}
		public void LaunchBall()
		{
			m_shootingBall = (Ball)m_aComingBalls[0];
			m_shootingBall.Shoot(Angle);

			m_aComingBalls.RemoveAt(0);
			AddComingBall();
			PushNextBall();
		}

		public void ShowPath()
		{
			m_shootingBall = (Ball)m_aComingBalls[0];
			EPointF loc = m_shootingBall.Loc;
			double dAngle = -Math.PI*Angle/180;
			float fSpeed = 1000;
			EPointF pntVel = EPointF.FromLengthAndAngle(fSpeed, (float)dAngle);

			EPoint pntGridStick = null;
			EPointF pntBounce;

			int nMaxTests = 40; //if more bounces than this is calculated, then something has gone wrong as it's outside the system
//			EH.Put("Start at "+loc.ToString());
			while (nMaxTests-->0)
			{
				if (m_playArea.m_pathCalc.GetFirstStickOrBounce(ref loc, ref pntVel, out pntGridStick,out pntBounce, true) == false)
				{
//					EH.Put("Test num:"+nMaxTests.ToString() + " loc"+loc.ToString());
//					EH.Put("Bounce:" + loc.ToString()+" vel:"+pntVel.ToString());
//					loc = pntBounce.Copy();
				}
				else
				{
//					if (m_spTarget!=null)
//						m_spTarget.Loc = m_playArea.Grid.GetGfxLocFromGridLoc(pntGridStick);
					break;
				}
			}
		}

		public void NextLevel()
		{
			if (m_aComingBalls!=null)
			{
				foreach (Ball ball in m_aComingBalls)
					ball.Dispose();
			}
			m_aComingBalls = new ArrayList();
			for (int i = 0; i < 2; i++)
				AddComingBall();
			PushNextBall();
		}
	}
}
