using UnityEngine;

namespace  XL.Common
{
    public static class Vector3_Tool
    {

        public static EightDirection Front { get => front; }
        private static EightDirection front = new EightDirection
        {
            DirectionVector = new Vector2(0, 1),
            Type = DirectionType.Front
        };
        public static EightDirection RightFront { get => rightFront; }
        private static EightDirection rightFront = new EightDirection
        {
            DirectionVector = new Vector2(1, 1),
            Type = DirectionType.RightFront
        };
        public static EightDirection Right { get => Right; }
        private static EightDirection right = new EightDirection
        {
            DirectionVector = new Vector2(1, 0),
            Type = DirectionType.Right
        };
        public static EightDirection RightDown { get => rightDown; }
        private static EightDirection rightDown = new EightDirection
        {
            DirectionVector = new Vector2(1, -1),
            Type = DirectionType.RightDown
        };
        public static EightDirection Down { get => down; }
        private static EightDirection down = new EightDirection
        {
            DirectionVector = new Vector2(0, -1),
            Type = DirectionType.Down
        };
        public static EightDirection LeftDown { get => leftDown; }
        private static EightDirection leftDown = new EightDirection
        {
            DirectionVector = new Vector2(-1, -1),
            Type = DirectionType.LeftDown
        };
        public static EightDirection Left { get => left; }
        private static EightDirection left = new EightDirection
        {
            DirectionVector = new Vector2(-1, 0),
            Type = DirectionType.Left
        };
        public static EightDirection LeftFront { get => leftFront; }
        private static EightDirection leftFront = new EightDirection
        {
            DirectionVector = new Vector2(-1, 1),
            Type = DirectionType.LeftFront
        };


        /// <summary>
        /// 获取该向量接近的方向
        /// </summary>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static int GetVectorApproachingDirection(Vector2 vector2)
        {
            Vector2 val = new Vector2(0, 1);
            float min = Vector2.Angle(vector2, new Vector2(0,1));
            //小于就fu
            float cos =Vector2.Angle(vector2, new Vector2(1, 1));
            if (min > cos)
            {
                min = Vector2.Angle(vector2, new Vector2(1, 1));
                val = new Vector2(1, 1);
            }
            cos = Vector2.Angle(vector2, new Vector2(1, 0));
            if (min > cos)
            {
                min = Vector2.Angle(vector2, new Vector2(1, 0));
                val = new Vector2(1, 0);
            }
            cos = Vector2.Angle(vector2, new Vector2(1, -1));
            if (min > Vector2.Angle(vector2, new Vector2(1, -1)))
            {
                min = Vector2.Angle(vector2, new Vector2(1, -1));
                val = new Vector2(1, -1);
            }
            cos = Vector2.Angle(vector2, new Vector2(0, -1));
            if (min > Vector2.Angle(vector2, new Vector2(0, -1)))
            {
                min = Vector2.Angle(vector2, new Vector2(0, -1));
                val = new Vector2(0, -1);
            }
            cos = Vector2.Angle(vector2, new Vector2(-1, 0));
            if (min > cos)
            {
                min = cos;
                val = new Vector2(-1, 0);
            }
            cos = Vector2.Angle(vector2, new Vector2(-1,1));
            if (min > cos)
            {
                min = cos;
                val = new Vector2(-1, 1);
            }
            cos = Vector2.Angle(vector2, new Vector2(-1, -1));
            if (min > cos)
            {
                min = cos;
                val = new Vector2(-1, -1);
            }
            //if (Vector3.Cross(new Vector3(vector2.x, 0, vector2.y), val).y > 0)
            //{
            //    if (val.x != 0)
            //    {
            //        val.x = -val.x;
            //    }
            //}
            int id = 0;
            if (val == new Vector2(0, 1))
            {
                id = 1;
            } if (val == new Vector2(1, 1))
            {
                id = 2;
            } if (val == new Vector2(1, 0))
            {
                id = 3;
            } if (val == new Vector2(1, -1))
            {
                id = 4;
            } if (val == new Vector2(0, -1))
            {
                id = 5;
            } if (val == new Vector2(-1, -1))
            {
                id = 6;
            } if (val == new Vector2(-1, 0))
            {
                id = 7;
            } if (val == new Vector2(-1, 1))
            {
                id = 8;
            } 
            return id;




        }
        /// <summary>
        /// 获取向量接近的平行方向
        /// </summary>
        /// <param name="vector2"></param>
        /// <returns></returns>
        //public static EightDirection GetVectorApproachingDirection(Vector3 vector3)
        //{
        //    return GetVectorApproachingDirection(new Vector2(vector3.x,vector3.z));
        //}

        /// <summary>
        ///  将大于180度角进行以负数形式输出
        /// </summary>
        /// <returns></returns>
        public static float CheckAngle(this float value) 
        {
            float angle = value - 180;

            if (angle > 0)
            {
                return angle - 180;
            }

            if (value == 0)
            {
                return 0;
            }

            return angle + 180;
        }
    }

    public struct EightDirection
    {
        public Vector2 DirectionVector {  set; get; }
        public DirectionType Type {  set; get; }

       
    }
    public enum DirectionType
    {
        Front = 1,
        RightFront = 2,
        Right = 3,
        RightDown = 4,
        Down = 5,
        LeftDown = 6,
        Left = 7,
        LeftFront = 8,

    }

   


}
