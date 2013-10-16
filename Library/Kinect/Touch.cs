

using System;
using System.Collections.Generic;
using System.Windows;

namespace EduFun.Kinect
{
    public class Touch
    {


        private PointInt _kinectPosition;
        /// <summary>
        /// Contient la position du Touché dans le repère de la Kinect (distance par rapport au point en haut à gauche du champ de vision de la caméra)
        /// </summary>
        public PointInt KinectPosition
        {
            get
            {
                if (this._kinectPosition.IsEmpty())
                {
                    _kinectPosition = CenterFromListPoint();
                }
                return _kinectPosition;
            }
        }

        private PointFloat _screenPositionFloat;
        /// <summary>
        /// les coordonnées du touché en % de la projection sur la table
        /// calculé à partir de la callibration
        /// </summary>
        public PointFloat ScreenPositionFloat
        { 
            get
            {
                if (_screenPositionFloat.IsEmpty())
                {
                    //TODO : bonne méthode?
                    _screenPositionFloat = Access.GetInstance().Screen.getScreenCoordonate(KinectPosition);
                }
                return _screenPositionFloat;
            }
        }

        private Point _screenPositionPixels = new Point();

        public Point GetScreenPositionPixels(Size rect)
        {

            if (_screenPositionPixels == new Point())
            {
                //TODO : bonne méthode?
                _screenPositionPixels = new Point((int)Math.Round(ScreenPositionFloat.X * rect.Width), (int)Math.Round(ScreenPositionFloat.Y * rect.Height));
            }

            return _screenPositionPixels;
        }

        /// <summary>
        /// les coordonnées de ce point das l'image précédente.
        /// (null si ce point est nouveau)
        /// </summary>
        public PointFloat PreviousScreenPosition;

        private PointFloat _screenTranslation;
        /// <summary>
        /// Calcule la translation en X et Y de ce point relativement à l'image précédente.
        /// résultat en % sur l'écran, vide si pas de point précédent.
        /// </summary>
        public PointFloat ScreenTranslation
        {
            get
            {
                if (_screenTranslation.IsEmpty())
                {
                    if (!PreviousScreenPosition.IsEmpty())
                    {
                        _screenTranslation.X = ScreenPositionFloat.X - PreviousScreenPosition.X;
                        _screenTranslation.Y = ScreenPositionFloat.Y - PreviousScreenPosition.Y;

                    }
                }
                return _screenTranslation;
            }
        }

        /// <summary>
        /// les coordonnées de ce point dans l'image précédente.
        /// (null si ce point est nouveau)
        /// </summary>
        public PointInt PreviousKinectPosition;

        private PointInt _kinectTranslation;
        /// <summary>
        /// Calcule la translation en X et Y de ce point relativement à l'image précédente.
        /// résultat en % sur l'écran, vide si pas de point précédent.
        /// </summary>
        public PointInt KinectTranslation
        {
            get
            {
                if (_kinectTranslation.IsEmpty())
                {
                    if (!PreviousKinectPosition.IsEmpty())
                    {
                        _kinectTranslation.X = KinectPosition.X - PreviousKinectPosition.X;
                        _kinectTranslation.Y = KinectPosition.Y - PreviousKinectPosition.Y;

                    }
                }
                return _kinectTranslation;
            }
        }


        private int _superficie;
        /// <summary>
        /// Renvoie la surface en pixels²
        /// </summary>
        public int Superficie
        {
            get
            {
                if (_superficie == 0)
                {
                    _superficie = Convert.ToInt32(Math.Sqrt(collection.Count));
                }

                return _superficie;
            }
        }


        /// <summary>
        /// identifiant unique du touché à la frame donnée
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// les pixels composants le touché. (utilisé par l'API)
        /// </summary>
        internal List<PointInt> collection;

        /// <summary>
        /// position du touché lors du down
        /// </summary>
        public PointInt Origin;

        /// <summary>
        /// âge en frames du touché
        /// </summary>
        public int Duration;

        /// <summary>
        /// indique si le touché est potentiellement maintenable (durée < 50 && distance(position/origine) < seuil.
        /// </summary>
        public bool IsMaintainable = true;

        public Touch()
        {
            collection = new List<PointInt>();
        }

        private PointInt CenterFromListPoint()
        {
            PointInt temp = new PointInt();

            for (int i = 0; i < collection.Count; i++)
            {
                temp.X += collection[i].X;
                temp.Y += collection[i].Y;
            }

            temp.X = temp.X / collection.Count;
            temp.Y = temp.Y / collection.Count;

            return temp;
        }

        /// <summary>
        /// calcule la distance entre la position d'origine et la position actuelle
        /// </summary>
        /// <returns></returns>
        public int GetDistanceFromOrigin()
        {
            return Convert.ToInt32(PointInt.CalculDistance(Origin, KinectPosition));
        }

        /// <summary>
        /// renvois si les deux touchés sont proche l'un de l'autre
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static bool IsCloseTo(Touch A, Touch B, int distance)
        {

            if (PointInt.CalculDistance(A.KinectPosition, B.KinectPosition) < distance)
                return true;

            return false;
        }

        /// <summary>
        /// renvois si les deux touchés sont proche l'un de l'autre en fonction de l'élant de la dernière position
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static bool IsCloseTo(Touch lastPosition, Touch newPosition, int distance, PointInt lastPositionKinectTranslation)
        {
            if (!lastPositionKinectTranslation.IsEmpty())
            {
                Console.WriteLine(lastPositionKinectTranslation.X + " ; " + lastPositionKinectTranslation.Y);
            }
            if (PointInt.CalculDistance(lastPosition.KinectPosition + lastPositionKinectTranslation, newPosition.KinectPosition) < distance)
                return true;

            return false;
        }

    
    }
}
