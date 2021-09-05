using System;

namespace Endogine
{
	/// <summary>
	/// Summary description for Vector3.
	/// </summary>
	public class Vector3
	{
		public float X, Y, Z;
		public Vector3(float x, float y, float z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}
		public Vector3()
		{
		}

        public float this[int position]
        {
            get
            {
                switch (position)
                {
                    case 0:
                        return this.X;
                    case 1:
                        return this.Y;
                    case 2:
                        return this.Z;
                    default:
                        throw new Exception("Out of bounds in Vector3: " + position);
                }
            }
            set
            {
                switch (position)
                {
                    case 0:
                        this.X = value;
                        break;
                    case 1:
                        this.Y = value;
                        break;
                    case 2:
                        this.Z = value;
                        break;
                    default:
                        throw new Exception("Out of bounds in Vector3: " + position);
                }
            }
        }

    	public static Vector3 operator -(Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.X-v2.X, v1.Y-v2.Y, v1.Z-v2.Z);
		}
		public static Vector3 operator +(Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.X+v2.X, v1.Y+v2.Y, v1.Z+v2.Z);
		}
		public static Vector3 operator *(Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.X*v2.X, v1.Y*v2.Y, v1.Z*v2.Z);
		}
		public static Vector3 operator *(Vector3 v1, float f)
		{
			return new Vector3(v1.X*f, v1.Y*f, v1.Z*f);
		}
		public static Vector3 operator /(Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.X/v2.X, v1.Y/v2.Y, v1.Z/v2.Z);
		}
		public static Vector3 operator /(Vector3 v1, float f)
		{
			return new Vector3(v1.X/f, v1.Y/f, v1.Z/f);
		}

        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            return new Vector3(
                v1.Y * v2.Z - v2.Y * v1.Z,
                v1.Z * v2.X - v2.Z * v1.X,
                v1.X * v2.Y - v2.X * v1.Y);
        }

        public void Project(object viewport, Matrix4 projection, Matrix4 view, Matrix4 world)
        {
        }
        public void Unproject(object viewport, Matrix4 projection, Matrix4 view, Matrix4 world)
        {
        }

        public float Length
        {
            get { return (float)Math.Sqrt(X * X + Y * Y + Z * Z); }
        }
        public float LengthSquared
        {
            get { return (X * X + Y * Y + Z * Z); }
        }

        public void Normalize()
        {
            float len = this.Length;
            this.X /= len;
            this.Y /= len;
            this.Z /= len;
        }
        public float Dot(Vector3 v1)
        {
            return this.X*v1.X + this.Y*v1.Y + this.Z*v1.Z;
        }
        public Vector3 Cross(Vector3 v1)
        {
            return Vector3.Cross(this, v1);
        }

        public override string ToString()
        {
            return this.X.ToString() + ";" + this.Y + ";" + this.Z;
        }
	}
}
