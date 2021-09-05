using System;

namespace Endogine
{
    /// <summary>
    /// Summary description for Vector4.
    /// </summary>
    public class Vector4
    {
        public float X, Y, Z, W;
        public Vector4(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }
        public Vector4(Vector3 v, float w)
        {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = v.Z;
            this.W = w;
        }
        public Vector4()
        {
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        public Vector4 Copy()
        {
            return new Vector4(X, Y, Z, W);
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
                    case 3:
                        return this.W;
                    default:
                        throw new Exception("Out of bounds in Vector4: " + position);
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
                    case 3:
                        this.W = value;
                        break;
                    default:
                        throw new Exception("Out of bounds in Vector4: " + position);
                }
            }
        }

        public static Vector4 operator -(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z, v1.W - v2.W);
        }
        public static Vector4 operator +(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z, v1.W + v2.W);
        }
        public static Vector4 operator *(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z, v1.W * v2.W);
        }
        public static Vector4 operator *(Vector4 v1, float f)
        {
            return new Vector4(v1.X * f, v1.Y * f, v1.Z * f, v1.W * f);
        }
        public static Vector4 operator /(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z, v1.W / v2.W);
        }
        public static Vector4 operator /(Vector4 v1, float f)
        {
            return new Vector4(v1.X / f, v1.Y / f, v1.Z / f, v1.W / f);
        }

        public static Vector4 Cross(Vector4 v1, Vector4 v2)
        {
            throw new Exception("Method not implemented: Vector4.Dot()");
            //return new Vector4(
            //    v1.Y * v2.Z - v2.Y * v1.Z,
            //    v1.Z * v2.X - v2.Z * v1.X,
            //    v1.X * v2.Y - v2.X * v1.Y);
        }

        public void Project(object viewport, Matrix4 projection, Matrix4 view, Matrix4 world)
        {
        }
        public void Unproject(object viewport, Matrix4 projection, Matrix4 view, Matrix4 world)
        {
        }

        public float Length
        {
            get { return (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W); }
        }
        public float LengthSquared
        {
            get { return (X * X + Y * Y + Z * Z + W * W); }
        }

        public void Normalize()
        {
            float len = this.Length;
            this.W /= len;
            this.X /= len;
            this.Y /= len;
            this.Z /= len;
        }
        public float Dot(Vector4 v1)
        {
            throw new Exception("Method not implemented: Vector4.Dot()");
            //return this.X * v1.X + this.Y * v1.Y + this.Z * v1.Z;
        }
        public Vector4 Cross(Vector4 v1)
        {
            return Vector4.Cross(this, v1);
        }

        public override string ToString()
        {
            return this.X.ToString() + ";" + this.Y + ";" + this.Z +";" + this.W;
        }
        public static Vector4 FromString(string val)
        {
            string[] vals = val.Split(';');
            Vector4 v = new Vector4();
            v.X = Convert.ToSingle(vals[0]);
            v.Y = Convert.ToSingle(vals[1]);
            v.Z = Convert.ToSingle(vals[2]);
            v.W = Convert.ToSingle(vals[3]);

            return v;
        }
    }
}
