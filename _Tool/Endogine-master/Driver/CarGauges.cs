using System;
using System.Collections.Generic;
using System.Text;
using Endogine;
using Endogine.Forms;

namespace Driver
{
    public class CarGauges
    {
        Car _car;
        public CarGauges(Car car)
        {
            this._car = car;

            this.Create1DMeter(new EPointF(0,0), "Throttle", 100);
            this.Create1DMeter(new EPointF(0,10), "Brake", 100);
            this.Create1DMeter(new EPointF(0,20), "SteerAngle", (float)Math.PI/4);
            this.Create1DMeter(new EPointF(0, 30), "SlipAngleFront", (float)Math.PI);
            this.Create1DMeter(new EPointF(0, 40), "SlipAngleRear", (float)Math.PI);

            this.Create2DMeter(new ERectangleF(100, 100, 200, 200), "LocalVelocity", 0.2f);
            this.Create2DMeter(new ERectangleF(200, 100, 200, 200), "Force", 300f);
            //LocalAcceleration
        }

        private MeterBar Create1DMeter(EPointF ptLoc, string prop, float max)
        {
            MeterBar bar = new MeterBar();
            bar.Rect = new ERectangleF(ptLoc.X, ptLoc.Y, 100, 8);
            bar.MaxValue = max;
            bar.SetAutoFetch(this._car, prop);
            return bar;
        }

        private MeterXY Create2DMeter(ERectangleF rct, string prop, float limit)
        {
            MeterXY xy = new MeterXY();
            xy.Rect = rct;
            xy.SetAutoFetch(this._car, prop);
            xy.RangeX.MinIn = -limit;
            xy.RangeX.MaxIn = limit;
            xy.RangeY.MinIn = -limit;
            xy.RangeY.MaxIn = limit;
            return xy;
        }
    }
}
