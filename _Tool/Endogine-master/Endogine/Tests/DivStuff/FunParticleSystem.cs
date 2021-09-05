using System;
using System.Drawing;
using Endogine;

namespace Tests
{
	/// <summary>
	/// Summary description for ChangableParticleSystem.
	/// </summary>
	public class FunParticleSystem : Endogine.ParticleSystem.ParticleEmitter
	{
		protected int m_nWhichColors = 0;
		protected int m_nWhichSizes = 0;

		public FunParticleSystem()
		{
			m_nWhichColors = -1;
			ChangeColors();

			m_nWhichSizes = -1;
			ChangeSizes();

			MaxParticles = 100;
			NumNewParticlesPerFrame = 0.1f;
		}

		public override void EnterFrame()
		{
			Loc = m_endogine.MouseLoc.ToEPointF();
			base.EnterFrame();
		}

		public void ChangeColors()
		{
			m_nWhichColors = (m_nWhichColors+1)%3;
			System.Collections.SortedList aColors = new System.Collections.SortedList();
			switch (m_nWhichColors)
			{
				case 0:
					aColors.Add(0.0, Color.FromArgb(0,255,0));
					aColors.Add(0.5, Color.FromArgb(255,0,0));
					aColors.Add(0.7, Color.FromArgb(255,0,0));
					aColors.Add(0.75, Color.FromArgb(255,255,255));
					aColors.Add(0.8, Color.FromArgb(100,0,255));
					aColors.Add(1.0, Color.FromArgb(0,0,255));
					break;
				case 1:
					aColors.Add(0.0, Color.FromArgb(255,255,0));
					aColors.Add(0.5, Color.FromArgb(255,0,0));
					aColors.Add(1.0, Color.FromArgb(0,0,0));
					break;
				default:
					aColors.Add(0.0, Color.FromArgb(0,0,0));
					aColors.Add(0.3, Color.FromArgb(0,0,0));
					aColors.Add(0.7, Color.FromArgb(0,0,255));
					aColors.Add(0.85, Color.FromArgb(0,255,0));
					aColors.Add(1.0, Color.FromArgb(0,0,0));
					break;
			}
			this.SetColorList(aColors);
		}

		public void ChangeSizes()
		{
			m_nWhichSizes = (m_nWhichSizes+1)%3;
			System.Collections.SortedList aSizes = new System.Collections.SortedList();
			switch (m_nWhichSizes)
			{
				case 0:
					aSizes.Add(0.0, 1.0);
					aSizes.Add(0.6, 1.0);
					aSizes.Add(0.8, 0.1);
					aSizes.Add(1.0, 0.9);
					break;
				case 1:
					aSizes.Add(0.0, 1.0);
					aSizes.Add(1.0, 1.0);
					break;
				default:
					aSizes.Add(0.0, 0.3);
					aSizes.Add(1.0, 0.01);
					aSizes.Add(1.1, 0.01);
					break;
			}
			this.SetSizeList(aSizes);
		}
	}
}
