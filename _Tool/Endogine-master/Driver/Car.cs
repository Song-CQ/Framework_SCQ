//
//  Adapted from Marco Monster's car physics tutorial at http://home.planet.nl/~monstrous/
//  I've attempted to contact him for permission to reproduce, but no reply.
//  There's no license, and judging from comments on his site he doesn't mind replication.
//

using System;
using System.Collections.Generic;
using System.Text;
using Endogine;
using System.Drawing;

namespace Driver
{
    struct CarType
    {
        public float Wheelbase;        // wheelbase in m
        public float CenterOfMassToFront;                // in m, distance from center of mass to front axle
        public float CenterOfMassToRear;                // in m, idem to rear axle
        public float CenterOfMassToGround;                // in m, height of CM from ground
        public float Mass;            // in kg
        public float Inertia;        // in kg.m
        public float Length;
        public float Width;
        public float WheelLength, WheelWidth;
    }

    struct Environment
    {
        public float Drag;
        public float Resistance;
        public float CornerStiffnessR;
        public float CornerStiffnessF;
        public float MaxGrip;
    }

    public class Car : Sprite
    {
        CarType _carType;
        Environment _environment;

        EPointF _velocity = new EPointF();
        float _angularVelocity = 0;
        float _angle = 0;

        float _inputSteerAngle;
        float _inputThrottle;
        float _inputBrake;
        float _targetSpeed;

        bool _useFrontSlip = false;
        bool _useRearSlip = false;

        #region Calculated fields (stored for visualization)
        EPointF acceleration = new EPointF();
        EPointF localVel = new EPointF();
        EPointF force = new EPointF();
        EPointF localAcceleration = new EPointF();
        float slipanglefront;
        float slipanglerear;

        public float SlipAngleFront
        {
            get { return this.slipanglefront; }
        }
        public float SlipAngleRear
        {
            get { return this.slipanglerear; }
        }

        public float SteerAngle
        {
            get { return this._inputSteerAngle; }
        }
        public float Throttle
        {
            get { return this._inputThrottle; }
        }
        public float Brake
        {
            get { return this._inputBrake; }
        }
        public EPointF LocalVelocity
        {
            get { return this.localVel; }
        }
        public EPointF Force
        {
            get { return this.force; }
        }
        public EPointF LocalAcceleration
        {
            get { return this.localAcceleration; }
        }

        #endregion

        long _lastFrameTicks;

        Endogine.KeysSteering _keys;

        public Car()
        {
            this._carType = new CarType();
            this._carType.CenterOfMassToFront = 1; //2.5f;
            this._carType.CenterOfMassToRear = 1; // 1.5f;
            this._carType.CenterOfMassToGround = 1; // 0.5f;
            this._carType.Wheelbase = this._carType.CenterOfMassToFront + this._carType.CenterOfMassToRear; //2f;
            this._carType.Mass = 1500;
            this._carType.Inertia = 1500;
            this._carType.Width = 1.5f; // 2;
            this._carType.Length = 3; // 4.5f;
            this._carType.WheelLength = 0.7f;
            this._carType.WheelWidth = 0.3f;

            this._environment = new Environment();
            this._environment.Drag = 5;
            this._environment.Resistance = 30;
            this._environment.CornerStiffnessR = -5.2f;
            this._environment.CornerStiffnessF = -5.0f;
            this._environment.MaxGrip = 2;

            float size = 5;
            Bitmap bmp = new Bitmap((int)(this._carType.Width * size), (int)(this._carType.Length * size), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(new SolidBrush(Color.Red), new Rectangle(0, 0, bmp.Width, bmp.Height));
            g.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(0, 3 * bmp.Height / 4, bmp.Width, bmp.Height / 4));
            g.Dispose();
            MemberSpriteBitmap mb = new MemberSpriteBitmap(bmp);
            this.Member = mb;

            this._keys = new KeysSteering(Endogine.KeysSteering.KeyPresets.ArrowsSpace);
            this._keys.AddKeyPreset(KeysSteering.KeyPresets.awsdCtrlShift);
            this._keys.ReceiveEndogineKeys(null);
        }

        public override void EnterFrame()
        {
            float minStep = (float)Math.PI / 32;
            if (this._keys.GetKeyActive("right"))
                this._inputSteerAngle = Math.Max(this._inputSteerAngle - minStep, -(float)Math.PI / 4);
            if (this._keys.GetKeyActive("left"))
                this._inputSteerAngle = Math.Min(this._inputSteerAngle + minStep, (float)Math.PI / 4);

            if (this._inputSteerAngle > 0)
            {
                this._inputSteerAngle -= minStep / 4;
                if (this._inputSteerAngle < 0)
                    this._inputSteerAngle = 0;
            }
            else if (this._inputSteerAngle < 0)
            {
                this._inputSteerAngle += minStep / 4;
                if (this._inputSteerAngle > 0)
                    this._inputSteerAngle = 0;
            }

            if (this._keys.GetKeyActive("up"))
            {
                this._targetSpeed += 1;
                this._inputThrottle = Math.Min(this._inputThrottle + 10, 100f);
            }
            else if (this._keys.GetKeyActive("down"))
            {
                this._targetSpeed -= 1;
                this._inputThrottle = Math.Max(this._inputThrottle - 10, -60f);
            }
            else
            {
                if (localVel.X > 0)
                {
                    if (localVel.X > 0.5f)
                    {
                        this._inputThrottle = -25;
                    }
                    else if (this._inputThrottle == 0)
                        this._velocity = new EPointF();
                }
            }

            //if (this.Velocity.Length < this._targetSpeed)
            //    this._inputThrottle = 30;
            //else if (this.Velocity.Length > this._targetSpeed)
            //    this._inputThrottle = -30;

            if (this._inputThrottle > 0)
                this._inputThrottle -= 5;
            else if (this._inputThrottle < 0)
                this._inputThrottle += 5;

            if (this._keys.GetKeyActive("action"))
            {
                this._inputBrake = 100;
                this._inputThrottle = 0;
            }
            else
                this._inputBrake = 0;

            if (this._lastFrameTicks > 0)
            {
                int timeDelta = (int)(DateTime.Now.Ticks - this._lastFrameTicks);
                if (timeDelta > 0)
                    //this.DoPhysics(10);
                    this.DoPhysics(timeDelta / 10000);
            }
            this._lastFrameTicks = DateTime.Now.Ticks;
            this.Rotation = -this._angle;
            ERectangleF rct = new ERectangleF(new EPointF(), EH.Instance.Stage.ControlSize.ToEPointF());
            EPointF loc = this.Loc.Copy();
            rct.WrapPointInside(loc);
            this.Loc = loc;

            base.EnterFrame();
        }

        private void DoPhysics(float msecsPassed)
        {
            float timeDelta = msecsPassed / 1000;

            if (this._inputSteerAngle != 0)
            {
                int i = 0;
            }
            float sn = (float)Math.Sin(this._angle);
            float cs = (float)Math.Cos(this._angle);

            //SAE convention: x is to the front of the car, y is to the right, z is down

            //transform velocity in world reference frame to velocity in car reference frame
            localVel.X = cs * this._velocity.Y + sn * this._velocity.X;
            localVel.Y = -sn * this._velocity.Y + cs * this._velocity.X;

            //Lateral force on wheels
            //Resulting velocity of the wheels as result of the yaw rate of the car body
            //v = yawrate * r where r is distance of wheel to CG (approx. half wheel base)
            //yawrate (ang.velocity) must be in rad/s

            float yawspeed = this._carType.Wheelbase * 0.5f * this._angularVelocity;
            float rot_angle = 0;
            if (localVel.X == 0) //TODO: fix singularity
                rot_angle = 0;
            else
                rot_angle = (float)Math.Atan2(yawspeed, localVel.X); //localVel.X, yawspeed);

            //Calculate the side slip angle of the car (a.k.a. beta)
            float sideslip = 0;
            if (localVel.X == 0) //TODO: fix singularity
                sideslip = 0;
            else
                sideslip = (float)Math.Atan2(localVel.Y, localVel.X); //localVel.X, localVel.Y);

            //Calculate slip angles for front and rear wheels (a.k.a. alpha)
            slipanglefront = sideslip + rot_angle - this._inputSteerAngle * Math.Sign(localVel.X);
            slipanglerear = sideslip - rot_angle;

            //weight per axle = half car mass times 1G (=9.8m/s^2) 
            float weight = this._carType.Mass * 9.8f * 0.5f;

            //lateral force on front wheels = (Ca * slip angle) capped to friction circle * load
            EPointF flatf = new EPointF();
            flatf.Y = this._environment.CornerStiffnessF * slipanglefront;
            flatf.Y = Math.Min(this._environment.MaxGrip, flatf.Y);
            flatf.Y = Math.Max(-this._environment.MaxGrip, flatf.Y);
            flatf.Y *= weight;
            if (this._useFrontSlip)
                flatf.Y *= 0.5f;

            //lateral force on rear wheels
            EPointF flatr = new EPointF();
            flatr.Y = this._environment.CornerStiffnessR * slipanglerear;
            flatr.Y = Math.Min(this._environment.MaxGrip, flatr.Y);
            flatr.Y = Math.Max(-this._environment.MaxGrip, flatr.Y);
            flatr.Y *= weight;
            if (this._useRearSlip)
                flatr.Y *= 0.5f;

            //longitudinal force on rear wheels - very simple traction model
            EPointF ftraction = new EPointF();
            ftraction.X = 100f * (this._inputThrottle - this._inputBrake * Math.Sign(localVel.X));
            if (this._useRearSlip)
                ftraction.X *= 0.5f;


            //    Forces and torque on body

            //drag and rolling resistance
            EPointF pntAbs = new EPointF(Math.Abs(localVel.X), Math.Abs(localVel.Y));
            EPointF resistance = (localVel * this._environment.Resistance + localVel * pntAbs * this._environment.Drag) * -1;
            //resistance.x = -(RESISTANCE * velocity.x + DRAG * velocity.x * ABS(velocity.x));
            //resistance.y = -(RESISTANCE * velocity.y + DRAG * velocity.y * ABS(velocity.y));

            //sum forces
            force = new EPointF();
            force.X = ftraction.X + (float)Math.Sin(this._inputSteerAngle) * flatf.X + flatr.X + resistance.X;
            force.Y = ftraction.Y + (float)Math.Cos(this._inputSteerAngle) * flatf.Y + flatr.Y + resistance.Y;

            ////JB: mechanical friction and cylinders
            //force.X += -1000*localVel.X;

            //torque on body from lateral forces
            float torque = this._carType.CenterOfMassToFront * flatf.Y - this._carType.CenterOfMassToRear * flatr.Y;


            // Acceleration

            //Newton F = m.a, therefore a = F/m
            localAcceleration = force / this._carType.Mass;
            float angular_acceleration = torque / this._carType.Inertia;


            //  Velocity and position

            //transform acceleration from car reference frame to world reference frame
            acceleration = new EPointF(
                cs * localAcceleration.Y + sn * localAcceleration.X,
                -sn * localAcceleration.Y + cs * localAcceleration.X);

            //velocity is integrated acceleration
            this._velocity += acceleration * timeDelta;

            //position is integrated velocity
            this.Loc += this._velocity * timeDelta*20;


            //  Angular velocity and heading

            //integrate angular acceleration to get angular velocity
            this._angularVelocity += angular_acceleration * timeDelta;

            //integrate angular velocity to get angular orientation
            this._angle += this._angularVelocity * timeDelta;
        }
    }
}
