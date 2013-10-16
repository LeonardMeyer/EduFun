using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EduFun.Kinect
{
    /// <summary>
    /// Argument des évenements TouchUp, TouchDown et TouchMove contenant UN touché entrant, sortant ou appuyé (suivant l'évenement) 
    /// </summary>
    public class TouchPointEventArgs : EventArgs
    {
        public Touch Touch { get; set; }

        public TouchPointEventArgs(Touch touch)
        {
            this.Touch = touch;
        }

    }



    /// <summary>
    /// Argument de l'évenement TouchMove contenant une COLLECTION de touchés afin de récupérer tous les points posés sur la surface
    /// </summary>
    //public class TouchPointsMoveEventArgs : EventArgs
    //{
    //    public TouchCollection TouchCollection { get; set; }

    //    public TouchPointsMoveEventArgs(TouchCollection touchColl
    //}
}
