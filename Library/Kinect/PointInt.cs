using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EduFun.Kinect
{
    public struct PointInt
    {
        public int X;
        public int Y;

        public bool IsEmpty()
        {
            if (X == 0 && Y == 0)
                return true;
            return false;
        }

        public PointInt(int x, int y)
        {
            X = x;
            Y = y;
        }

        public PointFloat toFloat()
        {
            return new PointFloat((float)X, (float)Y);
        }

        /// <summary>
        /// calcule la distance entre deux points
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static float CalculDistance(PointInt A, PointInt B)
        {
            //théorème de Pythagore
            return (float)Math.Sqrt((A.X - B.X) * (A.X - B.X) + (A.Y - B.Y) * (A.Y - B.Y));
        }

        public static PointInt operator +(PointInt pointA, PointInt pointB)
        {
            pointA.X += pointB.X;
            pointA.Y += pointB.Y;

            return pointA;

        }
    }
}
