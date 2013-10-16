using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduFun.Kinect
{
    public struct PointFloat
    {
        public float X;
        public float Y;

        public bool IsEmpty()
        {
            if (X == 0 && Y == 0)
                return true;
            return false;
        }

        public PointFloat(float x, float y)
        {
            X = x;
            Y = y;
        }

        public PointInt ToInt()
        {

            return new PointInt((int)X, (int)Y);

        }

        public static float CalculDistance(PointFloat A, PointFloat B)
        {
            //théorème de Pythagore
            return (float)Math.Sqrt((A.X - B.X) * (A.X - B.X) + (A.Y - B.Y) * (A.Y - B.Y));
        }
    }
}
