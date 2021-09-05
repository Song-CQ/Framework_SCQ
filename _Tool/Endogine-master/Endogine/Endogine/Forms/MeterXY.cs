using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine.Forms
{
    public class MeterXY : Sprite
    {
        Endogine.Tools.Relay _relay;
        Sprite _indicator;
        public Endogine.Tools.RangeConverter RangeX;
        public Endogine.Tools.RangeConverter RangeY;

        public MeterXY()
        {
            this.SourceRect = new ERectangle(0, 0, 1, 1);

            this.RangeX = new Endogine.Tools.RangeConverter();
            this.RangeY = new Endogine.Tools.RangeConverter();

            this._indicator = new Sprite();
            this._indicator.Parent = this;
            this._indicator.Member = new MemberSpriteBitmap(
                Endogine.BitmapHelpers.BitmapHelper.CreateFilledBitmap(new EPoint(4, 1),
                System.Drawing.Color.FromArgb(255, 255, 255)));
        }

        public override void EnterFrame()
        {
            this._relay.Update();
        }
        public override ERectangleF Rect
        {
            get
            {
                return base.Rect;
            }
            set
            {
                base.Rect = value;
                this.RangeX.MaxOut = 1f/value.Width;
                this.RangeY.MaxOut = 1f/value.Height;
                this._indicator.Scaling = new EPointF(1f / value.Width, 1f / value.Height);
            }
        }

        public void SetAutoFetch(object o, string property)
        {
            this._relay = new Endogine.Tools.Relay(o, property, this, "Value");
        }

        public EPointF Value
        {
            get { return new EPointF(); }
            set
            {
                EPointF loc = new EPointF(
                    this.RangeX.ConvertInToOut(value.X),
                    this.RangeY.ConvertInToOut(value.Y));

                EPointF mid = new EPointF(
                    this.RangeX.ConvertInToOut(0),
                    this.RangeY.ConvertInToOut(0));

                EPointF diff = loc - mid;
                this._indicator.ScaleX = diff.Length;
                this._indicator.Rotation = diff.Angle;
                this._indicator.Loc = mid;
            }
        }
    }
}
