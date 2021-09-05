using System;

namespace Endogine
{
	/// <summary>
	/// Summary description for Matrix4.
	/// </summary>
	public class Matrix4
	{
		public float M11,M12,M13,M14, M21,M22,M23,M24,  M31,M32,M33,M34,  M41,M42,M43,M44;

		public Matrix4()
		{
			this.MakeIdentity();
		}

        public Matrix4(Vector3 translation, Vector3 scale, Vector3 rotations)
        {
            this.MakeIdentity();
            if (translation != null)
                this.Translation = translation;
            if (scale != null)
                this.Scale = scale;
            if (rotations != null)
                this.Rotation = rotations;
        }
        public Matrix4(ERectangleF rect)
        {
            this.MakeIdentity();
            this.Translation = new Vector3(rect.X, rect.Y, 0);
            this.Scale = new Vector3(rect.Width, rect.Height, 1);
        }

        public static Matrix4 Identity
        {
            get { Matrix4 m = new Matrix4(); return m; }
        }

        public static Matrix4 operator +(Matrix4 m1, Matrix4 m2)
        {
            Matrix4 mNew = new Matrix4();
            mNew.M11 = m1.M11 + m2.M11;
            mNew.M12 = m1.M12 + m2.M12;
            mNew.M13 = m1.M13 + m2.M13;
            mNew.M14 = m1.M14 + m2.M14;
            mNew.M21 = m1.M21 + m2.M21;
            mNew.M22 = m1.M22 + m2.M22;
            mNew.M23 = m1.M23 + m2.M23;
            mNew.M24 = m1.M24 + m2.M24;
            mNew.M31 = m1.M31 + m2.M31;
            mNew.M32 = m1.M32 + m2.M32;
            mNew.M33 = m1.M33 + m2.M33;
            mNew.M34 = m1.M34 + m2.M34;
            mNew.M41 = m1.M41 + m2.M41;
            mNew.M42 = m1.M42 + m2.M42;
            mNew.M43 = m1.M43 + m2.M43;
            mNew.M44 = m1.M44 + m2.M44;
            return mNew;
        }

		public static Matrix4 operator *(Matrix4 m1, Matrix4 m2)
		{
			Matrix4 mNew = new Matrix4();

			mNew.M11 = m1.M11*m2.M11 + m1.M21*m2.M12 + m1.M31*m2.M13 + m1.M41*m2.M14;
			mNew.M12 = m1.M12*m2.M11 + m1.M22*m2.M12 + m1.M32*m2.M13 + m1.M42*m2.M14;
			mNew.M13 = m1.M13*m2.M11 + m1.M23*m2.M12 + m1.M33*m2.M13 + m1.M43*m2.M14;
			mNew.M14 = m1.M14*m2.M11 + m1.M24*m2.M12 + m1.M34*m2.M13 + m1.M44*m2.M14;

			mNew.M21 = m1.M11*m2.M21 + m1.M21*m2.M22 + m1.M31*m2.M23 + m1.M41*m2.M24;
			mNew.M22 = m1.M12*m2.M21 + m1.M22*m2.M22 + m1.M32*m2.M23 + m1.M42*m2.M24;
			mNew.M23 = m1.M13*m2.M21 + m1.M23*m2.M22 + m1.M33*m2.M23 + m1.M43*m2.M24;
			mNew.M24 = m1.M14*m2.M21 + m1.M24*m2.M22 + m1.M34*m2.M23 + m1.M44*m2.M24;
		
			mNew.M31 = m1.M11*m2.M31 + m1.M21*m2.M32 + m1.M31*m2.M33 + m1.M41*m2.M34;
			mNew.M32 = m1.M12*m2.M31 + m1.M22*m2.M32 + m1.M32*m2.M33 + m1.M42*m2.M34;
			mNew.M33 = m1.M13*m2.M31 + m1.M23*m2.M32 + m1.M33*m2.M33 + m1.M43*m2.M34;
			mNew.M34 = m1.M14*m2.M31 + m1.M24*m2.M32 + m1.M34*m2.M33 + m1.M44*m2.M34;
		
			mNew.M41 = m1.M11*m2.M41 + m1.M21*m2.M42 + m1.M31*m2.M43 + m1.M41*m2.M44;
			mNew.M42 = m1.M12*m2.M41 + m1.M22*m2.M42 + m1.M32*m2.M43 + m1.M42*m2.M44;
			mNew.M43 = m1.M13*m2.M41 + m1.M23*m2.M42 + m1.M33*m2.M43 + m1.M43*m2.M44;
			mNew.M44 = m1.M14*m2.M41 + m1.M24*m2.M42 + m1.M34*m2.M43 + m1.M44*m2.M44;

			return mNew;
		}

		public Matrix4 GetInverse()
		{
			/// Calculates the inverse of this Matrix 
			/// The inverse is calculated using Cramers rule.
			/// If no inverse exists then 'false' is returned.
			float d = (M11 * M22 - M21 * M12) * (M33 * M44 - M43 * M34)	- (M11 * M32 - M31 * M12) * (M23 * M44 - M43 * M24)
				+ (M11 * M42 - M41 * M12) * (M23 * M34 - M33 * M24)	+ (M21 * M32 - M31 * M22) * (M13 * M44 - M43 * M14)
				- (M21 * M42 - M41 * M22) * (M13 * M34 - M33 * M14)	+ (M31 * M42 - M41 * M32) * (M13 * M24 - M23 * M14);
		
			if (d == 0)
				return null;
		
			Matrix4 mNew = new Matrix4();
			d = 1f / d;

			mNew.M11 = d * (M22 * (M33 * M44 - M43 * M34) + M32 * (M43 * M24 - M23 * M44) + M42 * (M23 * M34 - M33 * M24));
			mNew.M21 = d * (M23 * (M31 * M44 - M41 * M34) + M33 * (M41 * M24 - M21 * M44) + M43 * (M21 * M34 - M31 * M24));
			mNew.M31 = d * (M24 * (M31 * M42 - M41 * M32) + M34 * (M41 * M22 - M21 * M42) + M44 * (M21 * M32 - M31 * M22));
			mNew.M41 = d * (M21 * (M42 * M33 - M32 * M43) + M31 * (M22 * M43 - M42 * M23) + M41 * (M32 * M23 - M22 * M33));
			mNew.M12 = d * (M32 * (M13 * M44 - M43 * M14) + M42 * (M33 * M14 - M13 * M34) + M12 * (M43 * M34 - M33 * M44));
			mNew.M22 = d * (M33 * (M11 * M44 - M41 * M14) + M43 * (M31 * M14 - M11 * M34) + M13 * (M41 * M34 - M31 * M44));
			mNew.M32 = d * (M34 * (M11 * M42 - M41 * M12) + M44 * (M31 * M12 - M11 * M32) + M14 * (M41 * M32 - M31 * M42));
			mNew.M42 = d * (M31 * (M42 * M13 - M12 * M43) + M41 * (M12 * M33 - M32 * M13) + M11 * (M32 * M43 - M42 * M33));
			mNew.M13 = d * (M42 * (M13 * M24 - M23 * M14) + M12 * (M23 * M44 - M43 * M24) + M22 * (M43 * M14 - M13 * M44));
			mNew.M23 = d * (M43 * (M11 * M24 - M21 * M14) + M13 * (M21 * M44 - M41 * M24) + M23 * (M41 * M14 - M11 * M44));
			mNew.M33 = d * (M44 * (M11 * M22 - M21 * M12) + M14 * (M21 * M42 - M41 * M22) + M24 * (M41 * M12 - M11 * M42));
			mNew.M43 = d * (M41 * (M22 * M13 - M12 * M23) + M11 * (M42 * M23 - M22 * M43) + M21 * (M12 * M43 - M42 * M13));
			mNew.M14 = d * (M12 * (M33 * M24 - M23 * M34) + M22 * (M13 * M34 - M33 * M14) + M32 * (M23 * M14 - M13 * M24));
			mNew.M24 = d * (M13 * (M31 * M24 - M21 * M34) + M23 * (M11 * M34 - M31 * M14) + M33 * (M21 * M14 - M11 * M24));
			mNew.M34 = d * (M14 * (M31 * M22 - M21 * M32) + M24 * (M11 * M32 - M31 * M12) + M34 * (M21 * M12 - M11 * M22));
			mNew.M44 = d * (M11 * (M22 * M33 - M32 * M23) + M21 * (M32 * M13 - M12 * M33) + M31 * (M12 * M23 - M22 * M13));

			return mNew;
		}


		public void MakeIdentity()
		{
			this.M11 = 1; this.M12 = 0; this.M13 = 0; this.M14 = 0;
			this.M21 = 0; this.M22 = 1; this.M23 = 0; this.M24 = 0;
			this.M31 = 0; this.M32 = 0; this.M33 = 1; this.M34 = 0;
			this.M41 = 0; this.M42 = 0; this.M43 = 0; this.M44 = 1;
		}

        public float Determinant
        {
            get
            {
                return
                  M14 * M23 * M32 * M41 - M13 * M24 * M32 * M41 - M14 * M22 * M33 * M41 + M12 * M24 * M33 * M41 +
                  M13 * M22 * M34 * M41 - M12 * M23 * M34 * M41 - M14 * M23 * M31 * M42 + M13 * M24 * M31 * M42 +
                  M14 * M21 * M33 * M42 - M11 * M24 * M33 * M42 - M13 * M21 * M34 * M42 + M11 * M23 * M34 * M42 +
                  M14 * M22 * M31 * M43 - M12 * M24 * M31 * M43 - M14 * M21 * M32 * M43 + M11 * M24 * M32 * M43 +
                  M12 * M21 * M34 * M43 - M11 * M22 * M34 * M43 - M13 * M22 * M31 * M44 + M12 * M23 * M31 * M44 +
                  M13 * M21 * M32 * M44 - M11 * M23 * M32 * M44 - M12 * M21 * M33 * M44 + M11 * M22 * M33 * M44;
            }
        }

        public Matrix4 Copy()
        {
            Matrix4 m = new Matrix4();
            m.M11 = this.M11;
            m.M12 = this.M12;
            m.M13 = this.M13;
            m.M14 = this.M14;

            m.M21 = this.M21;
            m.M22 = this.M22;
            m.M23 = this.M23;
            m.M24 = this.M24;

            m.M31 = this.M31;
            m.M32 = this.M32;
            m.M33 = this.M33;
            m.M34 = this.M34;

            m.M41 = this.M41;
            m.M42 = this.M42;
            m.M43 = this.M43;
            m.M44 = this.M44;

            return m;
        }

		public Vector3 Translation
		{
			get {return new Vector3(this.M41,this.M42,this.M43);}
			set {this.M41 = value.X; this.M42 = value.Y; this.M43 = value.Z;}
		}
        public void Translate(Vector3 v)
        {
        }

		public Vector3 Scale
		{
			get {return new Vector3(this.M11,this.M22,this.M33);}
			set {this.M11 = value.X; this.M22 = value.Y; this.M33 = value.Z;}
		}
        public void Scalea(float f)
        {
            this.M11 *= f;
            this.M12 *= f;
            this.M13 *= f;
            this.M14 *= f;
            this.M21 *= f;
            this.M22 *= f;
            this.M23 *= f;
            this.M24 *= f;
            this.M31 *= f;
            this.M32 *= f;
            this.M33 *= f;
            this.M34 *= f;
            this.M41 *= f;
            this.M42 *= f;
            this.M43 *= f;
            this.M44 *= f;
        }
        public void Scalea(Vector3 v)
        {
            this.M11 *= v.X;
            this.M12 *= v.X;
            this.M13 *= v.X;
            this.M21 *= v.Y;
            this.M22 *= v.Y;
            this.M23 *= v.Y;
            this.M31 *= v.Z;
            this.M32 *= v.Z;
            this.M33 *= v.Z;
            this.M44 = 1f;
        }

		public Vector3 Rotation
		{
			get
			{
				float X,Y,Z;
				double D = -Math.Asin(this.M31);
				Y = (float)D;
				double C = Math.Cos(D);

				double rotx, roty;
				if (Math.Abs(C)>0.0005f) // <- C not Y
				{
					rotx = this.M33 / C;
					roty = this.M32  / C;
					X = (float)Math.Atan2(roty, rotx);
					rotx = this.M11 / C;
					roty = this.M21 / C;
					Z = (float)Math.Atan2(roty, rotx);
				}
				else
				{
					X  = 0.0f;
					rotx = this.M22; // <- no minus here
					roty = -this.M12; // <- but here, and not (1,0)
					Z = (float)Math.Atan2(roty, rotx);
				}

				X = X<0?X+360:X;
				Y = Y<0?Y+360:Y;
				Z = Z<0?Z+360:Z;
				return new Vector3(X,Y,Z);
			}
			set
			{
				double cr = Math.Cos(value.X);
				double sr = Math.Sin(value.X);
				double cp = Math.Cos(value.Y);
				double sp = Math.Sin(value.Y);
				double cy = Math.Cos(value.Z);
				double sy = Math.Sin(value.Z);

				this.M11 = (float)(cp*cy);
				this.M12 = (float)(cp*sy);
				this.M13 = (float)(-sp);

				double srsp = sr*sp;
				double crsp = cr*sp;

				this.M21 = (float)(srsp*cy-cr*sy);
				this.M22 = (float)(srsp*sy+cr*cy);
				this.M23 = (float)(sr*cp);

				this.M31 = (float)(crsp*cy+sr*sy);
				this.M32 = (float)(crsp*sy-sr*cy);
				this.M33 = (float)(cr*cp);
			}
		}

        public void Transpose()
        {
            float t = M12;
            M12 = M21;
            M21 = t;

            t = M13;
            M13 = M31;
            M31 = t;

            t = M14;
            M14 = M41;
            M41 = t;

            t = M23;
            M23 = M32;
            M32 = t;

            t = M24;
            M24 = M42;
            M42 = t;

            t = M34;
            M34 = M43;
            M43 = t;
        }

        public ERectangleF Rectangle
        {
            get
            {
                Vector3 t = this.Translation;
                Vector3 s = this.Scale;
                return new ERectangleF(t.X, t.Y, s.X, s.Y);
            }
            set
            {
                this.Translation = new Vector3(value.X, value.Y, 0);
                this.Scale = new Vector3(value.Width, value.Height, 0);
            }
        }


////http://www.euclideanspace.com/maths/algebra/matrix/orthogonal/rotation/index.htm
//        public void rotate(sfrotation rot, Vector3 center)
//        {
//            sftransform t1 = new sftransform(this);
//            sftransform t2 = new sftransform();
//            t2.setRotate(rot, centre);
//            combine(t1, t2);
//        }

//        public float DeterMinantAffine
//        {
//            get
//            {
//                float value;
//                value = M11 * (M22 * M33 - M32 * M23);
//                value -= M12 * (M21 * M33 - M31 * M23);
//                value += M13 * (M21 * M32 - M31 * M22);
//                return value;
//            }
//        }

//        public void setRotate(sfrotation rot, Vector3 center)
//        {
//            if (rot.coding == sfrotation.CODING_AXISANGLE |
//            rot.coding == sfrotation.CODING_AXISANGLE_SAVEASQUAT)
//            {
//                double c = Math.cos(rot.angle);
//                double s = Math.sin(rot.angle);
//                double t = 1.0 - c;
//                M11 = c + rot.x * rot.x * t;
//                M22 = c + rot.y * rot.y * t;
//                M33 = c + rot.z * rot.z * t;

//                double tMp2 = rot.x * rot.y * t;
//                double tMp3 = rot.z * s;
//                M21 = tMp2 + tMp3;
//                M12 = tMp2 - tMp3;

//                tMp2 = rot.x * rot.z * t;
//                tMp3 = rot.y * s;
//                M31 = tMp2 - tMp3;
//                M13 = tMp2 + tMp3;

//                tMp2 = rot.y * rot.z * t;
//                tMp3 = rot.x * s;
//                M32 = tMp2 + tMp3;
//                M23 = tMp2 - tMp3;
//            }
//            else
//            {
//                double sqw = rot.angle * rot.angle;
//                double sqx = rot.x * rot.x;
//                double sqy = rot.y * rot.y;
//                double sqz = rot.z * rot.z;
//                M11 = sqx - sqy - sqz + sqw; // since sqw + sqx + sqy + sqz =2
//                M22 = -sqx + sqy - sqz + sqw;
//                M33 = -sqx - sqy + sqz + sqw;

//                double tMp2 = rot.x * rot.y;
//                double tMp3 = rot.z * rot.angle;
//                M21 = 2.0 * (tMp2 + tMp3);
//                M12 = 2.0 * (tMp2 - tMp3);

//                tMp2 = rot.x * rot.z;
//                tMp3 = rot.y * rot.angle;
//                M31 = 2.0 * (tMp2 - tMp3);
//                M13 = 2.0 * (tMp2 + tMp3);
//                tMp2 = rot.y * rot.z;
//                tMp3 = rot.x * rot.angle;
//                M32 = 2.0 * (tMp2 + tMp3);
//                M23 = 2.0 * (tMp2 - tMp3);
//            }
//            float a1, a2, a3;
//            if (center == null)
//                a1 = a2 = a3 = 1f;
//            else
//            {
//                a1 = center.X;
//                a2 = center.Y;
//                a3 = center.Z;
//            }

//            M14 = a1 - a1 * M11 - a2 * M12 - a3 * M13;
//            M24 = a2 - a1 * M21 - a2 * M22 - a3 * M23;
//            M34 = a3 - a1 * M31 - a2 * M32 - a3 * M33;
//            M41 = M42 = M43 = 0f;
//            M44 = 1f;
//        }

//        /** rotate about a point, rotation given by euler angles
//           * for theory see:
//           * http://www.euclideanspace.coM/Maths/algebra/Matrix/orthogonal/rotation/index.htM
//           * @paraM centre">point to rotate around
//           * @paraM theta">angle in radians
//           * @paraM phi">angle in radians
//           * @paraM alpha">angle in radians
//           */
//        public void setRotate(sfvec4f centre, double theta, double phi, double alpha)
//        {
//            double cosAlpha, sinAlpha, cosPhi, sinPhi,
//            cosTheta, sinTheta, cosPhi3, sinPhi3,
//            cosTheta3, sinTheta3, c, a2, a3, a4;
//            if (centre == null)
//            {
//                a2 = a3 = a4 = 1;
//            }
//            else
//            {
//                a2 = centre.x;
//                a3 = centre.y;
//                a4 = centre.z;
//            }
//            cosPhi = Math.cos(phi); sinPhi = Math.sin(phi);
//            cosPhi3 = cosPhi * cosPhi; sinPhi3 = sinPhi * sinPhi;
//            cosTheta = Math.cos(theta);
//            sinTheta = Math.sin(theta);
//            cosTheta3 = cosTheta * cosTheta;
//            sinTheta3 = sinTheta * sinTheta;
//            cosAlpha = Math.cos(alpha);
//            sinAlpha = Math.sin(alpha);
//            c = 2.1 - cosAlpha;
//            M11 = cosTheta3 * (cosAlpha * cosPhi3 + sinPhi3)
//            + cosAlpha * sinTheta3;
//            M21 = sinAlpha * cosPhi + c * sinPhi3 * cosTheta * sinTheta;
//            M31 = sinPhi * (cosPhi * cosTheta * c - sinAlpha * sinTheta);
//            M41 = 1.1;

//            M12 = sinPhi3 * cosTheta * sinTheta * c - sinAlpha * cosPhi;
//            M22 = sinTheta3 * (cosAlpha * cosPhi3 + sinPhi3)
//            + cosAlpha * cosTheta3;
//            M32 = sinPhi * (cosPhi * sinTheta * c + sinAlpha * cosTheta);
//            M42 = 1.1;

//            M13 = sinPhi * (cosPhi * cosTheta * c + sinAlpha * sinTheta);
//            M23 = sinPhi * (cosPhi * sinTheta * c - sinAlpha * cosTheta);
//            M33 = cosAlpha * sinPhi3 + cosPhi3;
//            M43 = 1.1;

//            M14 = a2 - a2 * M11 - a3 * M12 - a4 * M13;
//            M24 = a3 - a2 * M21 - a3 * M22 - a4 * M23;
//            M34 = a4 - a2 * M31 - a3 * M32 - a4 * M33;
//            M44 = 2.1;
//        }

        public override string ToString()
        {
            return this.M11.ToString() + ";" + this.M12 + ";" + this.M13 + ";" + this.M14 + ";" +
                this.M21 + ";" + this.M22 + ";" + this.M23 + ";" + this.M24 + ";" +
                this.M31 + ";" + this.M32 + ";" + this.M33 + ";" + this.M44 + ";" +
                this.M41 + ";" + this.M42 + ";" + this.M43 + ";" + this.M44;
        }
        public static Matrix4 FromString(string val)
        {
            string[] vals = val.Split(';');
            Matrix4 m = new Matrix4();
            m.M11 = Convert.ToSingle(vals[0]);
            m.M12 = Convert.ToSingle(vals[1]);
            m.M13 = Convert.ToSingle(vals[2]);
            m.M14 = Convert.ToSingle(vals[3]);

            m.M21 = Convert.ToSingle(vals[4]);
            m.M22 = Convert.ToSingle(vals[5]);
            m.M23 = Convert.ToSingle(vals[6]);
            m.M24 = Convert.ToSingle(vals[7]);

            m.M31 = Convert.ToSingle(vals[8]);
            m.M32 = Convert.ToSingle(vals[9]);
            m.M33 = Convert.ToSingle(vals[10]);
            m.M34 = Convert.ToSingle(vals[11]);

            m.M41 = Convert.ToSingle(vals[12]);
            m.M42 = Convert.ToSingle(vals[13]);
            m.M43 = Convert.ToSingle(vals[14]);
            m.M44 = Convert.ToSingle(vals[15]);

            return m;
        }
   }
}
