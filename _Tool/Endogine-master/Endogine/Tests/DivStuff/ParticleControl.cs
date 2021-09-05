using System;
using Endogine;

namespace Tests
{
	/// <summary>
	/// Summary description for ParticleControl.
	/// </summary>
	public class ParticleControl : Endogine.Forms.Form
	{
		private FunParticleSystem m_psys = null;

		public ParticleControl(FunParticleSystem a_psys)
		{
			m_psys = a_psys;

			this.Rect = new ERectangleF(400,100,200,200);
			this.LocZ = 50;

			Endogine.Forms.Button btnChangeColors = new Endogine.Forms.Button();
			btnChangeColors.Parent = this;
			btnChangeColors.Rect = new ERectangleF(10,60,100,30);
			btnChangeColors.LocZ = 1;
			btnChangeColors.MouseEvent+=new Endogine.Sprite.MouseEventDelegate(btnChangeColors_MouseEvent);

			Endogine.Forms.Button btnChangeSize = new Endogine.Forms.Button();
			btnChangeSize.Parent = this;
			btnChangeSize.Rect = new ERectangleF(10,100,50,50);
			btnChangeSize.LocZ = 1;
			btnChangeSize.MouseEvent+=new Endogine.Sprite.MouseEventDelegate(btnChangeSize_MouseEvent);

			Endogine.Forms.Slider slider = new Endogine.Forms.Slider();
			slider.Parent = this;
			slider.Rect = new ERectangleF(70,100,100,30);
			slider.LocZ = 1;
			slider.SliderEvent+=new Endogine.Forms.Slider.SliderEventDelegate(slider_SliderEvent);

			Endogine.Forms.Slider sliderSize = new Endogine.Forms.Slider();
			sliderSize.Parent = this;
			sliderSize.Rect = new ERectangleF(70,140,100,30);
			sliderSize.LocZ = 1;
			sliderSize.SliderEvent+=new Endogine.Forms.Slider.SliderEventDelegate(sliderSize_SliderEvent);
		}
		private void btnChangeColors_MouseEvent(Sprite sender, System.Windows.Forms.MouseEventArgs e, Endogine.Sprite.MouseEventType t)
		{
			//Mouse down on color change button
			if (t == Endogine.Sprite.MouseEventType.Down)
			{
				if (m_psys==null)
					return;
				m_psys.ChangeColors();
			}
		}
		private void btnChangeSize_MouseEvent(Sprite sender, System.Windows.Forms.MouseEventArgs e, Endogine.Sprite.MouseEventType t)
		{
			//Mouse down on size change button
			if (t == Endogine.Sprite.MouseEventType.Down)
			{
				if (m_psys==null)
					return;
				m_psys.ChangeSizes();
			}
		}

		private void slider_SliderEvent(float fPosition, Endogine.Sprite.MouseEventType t)
		{
			//Dragged the numparticles slider
			if (m_psys==null)
				return;
			m_psys.NumNewParticlesPerFrame = fPosition*2;
		}

		private void sliderSize_SliderEvent(float fPosition, Endogine.Sprite.MouseEventType t)
		{
			//Dragged the particle size slider
			if (m_psys==null)
				return;
			m_psys.SizeFact = fPosition*5;
		}
	}
}
