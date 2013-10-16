using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace EduFun.Kinect
{
    public class Screen
    {
        private PointInt[] _boundaries;
        public bool _mirror = true;

        public PointInt[] Boundaries
        {
            get
            {
                return _boundaries;
            }

            set
            {
                _boundaries = value;

                _topWidth = PointInt.CalculDistance(_boundaries[0], _boundaries[1]);
                _topEquation = Screen.getEquationFromPoints(_boundaries[0], _boundaries[1]);

                _bottomWidth = PointInt.CalculDistance(_boundaries[2], _boundaries[3]);
                _bottomEquation = Screen.getEquationFromPoints(_boundaries[2], _boundaries[3]);

                _leftHeight = PointInt.CalculDistance(_boundaries[0], _boundaries[2]);
                _leftEquation = Screen.getEquationFromPoints(_boundaries[0], _boundaries[2]);

                _rightHeight = PointInt.CalculDistance(_boundaries[1], _boundaries[3]);
                _rightEquation = Screen.getEquationFromPoints(_boundaries[1], _boundaries[3]);

                _horizontalIntersection = Screen.getIntersectionOfAffine(_topEquation, _bottomEquation);
                _verticalIntersection = Screen.getIntersectionOfAffine(_rightEquation, _leftEquation);


                //_bottomLeftTotopRight = Touch.CalculDistance(_boundaries[2], _boundaries[1]);
                //_bottomrightTotopleft = Touch.CalculDistance(_boundaries[3], Boundaries[0]);

                //Produit scalaire:
                //cos(CAB) = ((vecteur)AB . (veceur)AC) / (AB * AC)

                //ou théorème d'al Kashi:
                //CAB = arcos((AC² + AB² - CB²) / (2 * AC * AB))

                //_topLeftCorner = (float)Math.Acos((float)((_leftHeight * _leftHeight) + (_topWidth * _topWidth) - (_bottomLeftTotopRight * _bottomLeftTotopRight)) / (float)(2 * _leftHeight * _topWidth));
                //_topLeftCorner = Screen.radianToDegree(_topLeftCorner);
            }
        }


        //distances
        private float _topWidth;
        private float _bottomWidth;
        private float _leftHeight;
        private float _rightHeight;

        //équations
        private PointFloat _leftEquation;
        private PointFloat _rightEquation;
        private PointFloat _topEquation;
        private PointFloat _bottomEquation;

        //points d'intersection
        private PointFloat _horizontalIntersection;
        private PointFloat _verticalIntersection;

        public Screen()
        {

        }

        public Screen(PointInt[] boundaries)
        {
            this.Boundaries = boundaries;
        }

        /// <summary>
        /// Retourne les coordonnées du repère de la Kinect en repère de l'écran de projection.
        /// </summary>
        /// <param name="coordonate"></param>
        /// <returns></returns>
        public PointFloat getScreenCoordonate(PointInt coordonate)
        {
            //TODO : faire le calcul en prenant en compte plus que un carré (le screen sera un trapèze
            /*
            PointFloat temp = new PointFloat();

            temp.X = (float)(coordonate.X - Boundaries[0].X) / (float)_topWidth;
            temp.Y = (float)(coordonate.Y - Boundaries[0].Y) / (float)_leftHeight;

            return temp;
            
            //*/

            //*
            PointFloat result = new PointFloat();
            PointFloat intersection;
            //si top et bottom sont parrallèle
            if (_horizontalIntersection.IsEmpty())
            {

                intersection = Screen.getIntersectionOfAffine(new PointFloat(_topEquation.X, coordonate.X - coordonate.Y * _topEquation.X), _leftEquation);

            }
            else
            {
                intersection = Screen.getIntersectionOfAffine(Screen.getEquationFromPoints(_horizontalIntersection, coordonate.toFloat()), _leftEquation);
            }

            result.Y = PointFloat.CalculDistance(intersection, _boundaries[0].toFloat()) / _leftHeight;

            //pareil avec left et right
            if (_verticalIntersection.IsEmpty())
            {

                intersection = Screen.getIntersectionOfAffine(new PointFloat(_leftEquation.X, coordonate.X - coordonate.Y * _leftEquation.X), _topEquation);

            }
            else
            {
                intersection = Screen.getIntersectionOfAffine(Screen.getEquationFromPoints(_verticalIntersection, coordonate.toFloat()), _topEquation);
                
            }

            result.X = PointFloat.CalculDistance(intersection, _boundaries[0].toFloat()) / _topWidth;

            if(_mirror)
            {
                result = mirror(result);
            }
            return result;
            //*/
        }

        /// <summary>
        /// vérifie si un pixel est au dessus de lécran de projection qui est identifié lors du callibrage
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool IsOver(PointInt coordonate)
        {

            //TODO : rendre compatible avec un trapèze
            if (coordonate.X > Boundaries[0].X && coordonate.X < Boundaries[3].X && coordonate.Y < Boundaries[3].Y && coordonate.Y > Boundaries[0].Y)
                return true;
            return false;
        }

        private static float radianToDegree(float value)
        {
            return (float)(180 * value / Math.PI);
        }

        /// <summary>
        /// Calcule l'équation de la droite affine passant par les points A et B
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns>L'équation est de type f(x) = ax + b où a, le coefficiant est: result.X et b, la valeur à l'origine est result.Y. C'est pour cela que je retourne un point</returns>
        private static PointFloat getEquationFromPoints(PointInt A, PointInt B)
        {
            return getEquationFromPoints(A.toFloat(), B.toFloat());
        }

        /// <summary>
        /// Calcule l'équation de la droite affine passant par les points A et B
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns>L'équation est de type f(x) = ax + b où a, le coefficiant est: result.X et b, la valeur à l'origine est result.Y. C'est pour cela que je retourne un point. si le coefficient est infinit, X= infinit, Y = abscisse de la droite</returns>
        private static PointFloat getEquationFromPoints(PointFloat A, PointFloat B)
        {

            PointFloat result = new PointFloat();

            //en prenant le vecteur AB
            //coefficient = AB.Y / AB.X
            if (A.X - B.X == 0)
            {
                result.X = float.PositiveInfinity;
                result.Y = A.X;
                return result;
            }
            else
            {
                result.X = (A.Y - B.Y) / (A.X - B.X);
            }

            //on sait que f(x) = ax + b
            //f(A) = A.y 
            //ax + b = coefficiant * A.X + b
            //b = A.y - coefficiant * a.X
            //if ((A.Y - result.X * A.X).Equals(B.Y - result.X * B.X))
            //{
            //    Console.WriteLine("ERREUR SCREEN.GETEQUATIONFROMPOINTS");
            //}

            result.Y = A.Y - result.X * A.X;
           // result.Y = B.Y - result.X * B.X;

            return result;
        }

        /// <summary>
        /// retourne un point représentant l'intersection des deux droites d1 et d2. Le point est empty si les deux droites sont parallèles
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        private static PointFloat getIntersectionOfAffine(PointFloat d1, PointFloat d2)
        {
            PointFloat result = new PointFloat();

            if(d1.X == d2.X)
            {
                //les droites sont parallèles car les coefficients sont identiques
                return result;
            }

            if (float.IsInfinity(d1.X))
            {
                result.X = d1.Y;
                result.Y = d2.X * result.X + d2.Y; 
                return result;
            }
            if (float.IsInfinity(d2.X))
            {
                result.X = d2.Y;
                result.Y = d1.X * result.X + d1.Y; 
                return result;
            }

            // on a deux équations: (x et y la position du point d'intersection)
            // y = ax + b donc:

            //y = d1.X * x + d1.Y
            //y = d2.X * x + d2.Y

            //d1.X * x + d1.Y = d2.X * x + d2.Y
            result.X = (d2.Y - d1.Y) / (d1.X - d2.X);

            result.Y = d1.X * result.X + d1.Y;
            //result.Y = d2.X * result.X + d2.Y;

            //if ((d1.X * result.X + d1.Y).Equals(d2.X * result.X + d2.Y))
            //{
            //    Console.WriteLine("ERREUR Screen.getIntersectionOfAffine");
            //}


            return result;
        }

        /// <summary>
        /// inverse les coordonnées d'un point selon une transformation mirroire
        /// </summary>
        /// <param name="pf"></param>
        /// <returns></returns>
        private PointFloat mirror(PointFloat pf)
        {
            pf.X = 1-pf.X;
           // pf.Y = 1-pf.Y;
            return pf;
        }  
    }
}
