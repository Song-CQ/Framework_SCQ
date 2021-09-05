using System;
using System.Reflection;
using System.ComponentModel;

namespace Endogine
{
	/// <summary>
	/// A ParallaxLayer manages it's child sprites so that they are moved according to the ScrollFactor when the camera is moved
	/// </summary>
	public class ParallaxLayer : Sprite
	{
		protected EPointF m_pntScrollFactor;

		public ParallaxLayer()
		{
			m_pntScrollFactor = new EPointF(1,1);
			this.SourceRect = new ERectangle(0,0,1,1);
			this.Rect = new ERectangleF(0,0,1,1);
		}

        [Browsable(false)]
		public EPointF ScrollFactor
		{
			get {return m_pntScrollFactor;}
			set {m_pntScrollFactor = value;}
		}

        //TODO: http://www.codeproject.com/cs/miscctrl/bending_property.asp
        /// <summary>
        /// Just so I don't have to implement a TypeConverter. WTF?!? It's still grayed out in propertygrid?!
        /// </summary>
        public System.Drawing.PointF ScrollFactor2
        {
            get { return m_pntScrollFactor.ToPointF(); }
            set { m_pntScrollFactor = new EPointF(value); }
        }


		public override EPointF Loc
		{
			get
			{
				return base.Loc/m_pntScrollFactor;
			}
			set
			{
				base.Loc = value*m_pntScrollFactor;
			}
		}

        public override Sprite Copy()
        {
            ParallaxLayer sp = new ParallaxLayer();
            this.CopyTo(sp);
            return sp;
        }

        public override void CopyTo(Sprite sp)
        {
            base.CopyTo(sp);
            ParallaxLayer layer = (ParallaxLayer)sp;
            layer.ScrollFactor = this.ScrollFactor;
        }
	}
}
