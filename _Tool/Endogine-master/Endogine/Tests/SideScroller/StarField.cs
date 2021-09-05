using System;
using System.Collections;

using Endogine;

namespace SideScroller
{
	/// <summary>
	/// Summary description for StarField.
	/// </summary>
	public class StarField
	{
		private ArrayList m_layers;
		public StarField()
		{
			m_layers = new ArrayList();

			Random rnd = new Random();
			for (int nLayer = -1; nLayer < 2; nLayer++)
			{

				//before, I started with nLayer = -2 to get a layer in front of the "play area", but it was a bit confusing.

				
				//create layer, name it, set how fast it should scroll
				//(actually, for -1, we could use the default layer, but this looks more clean.)
				ParallaxLayer layer = new ParallaxLayer();
				m_layers.Add(layer);

				layer.Name = "Scroll"+nLayer.ToString();
				float fScrollFact = (float)Math.Pow(0.5, nLayer+1);
				layer.ScrollFactor = new EPointF(fScrollFact,fScrollFact);
				layer.Parent = EndogineHub.Instance.Stage.Camera;
				layer.LocZ = -nLayer-1;

				//create some sprites in each parallax layer
				for (int nSprite = 0; nSprite < 10; nSprite++)
				{
					WrappingSprite star = new WrappingSprite();
					star.Loc = new EPointF(rnd.Next(640)-320, rnd.Next(480)-240);
					star.WrapRect = new ERectangleF(new EPointF(0,0), EndogineHub.Instance.Stage.Size.ToEPointF());
					star.Parent = layer;
					star.Ink = RasterOps.ROPs.AddPin;
					star.MemberName = "Star";
				}
			}
		}

		public void Dispose()
		{
			foreach (ParallaxLayer layer in m_layers)
				layer.Dispose();
		}
	}
}
