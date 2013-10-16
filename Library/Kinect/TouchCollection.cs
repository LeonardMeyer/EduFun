using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace EduFun.Kinect
{
    public class TouchCollection : ICloneable
    {
        public List<Touch> collection;

        public TouchCollection()
        {
            collection = new List<Touch>();
        }

        public TouchCollection(List<Touch> collection)
        {
            this.collection = collection;
        }

        public BitmapImage getBitmapFromTouchs()
        {
            throw new NotImplementedException();

            //foreach (Touch touch in touchPointsEnter.touchCollection.collection.GetRange(0, touchPointsEnter.touchCollection.collection.Count))
            //{
            //    //suivant le seuil, on valide ou pas le point
            //    if (touch.collection.Count > threshold * threshold)
            //    {

            //        foreach (Point pdessin in touch.collection)
            //        {
            //            int pos = GetPositionFromCoordonate(pdessin, frame);
            //            depthPixels[pos * 4 + 0] = (byte)(255);
            //            depthPixels[pos * 4 + 1] = (byte)(255);
            //            depthPixels[pos * 4 + 2] = (byte)(255);
            //        }
            //    }
            //    // Si invalide on supprime le touch
            //    else
            //    {
            //        touchPointsEnter.touchCollection.collection.Remove(touch);
            //    }
            //}

            //touchPointsEnter.bitmapSource = BitmapSource.Create(depthFrame.Width, depthFrame.Height, 96, 96, PixelFormats.Bgr32, null, depthPixels, depthStride);
        }

        /// <summary>
        /// retourne un ID pas encore utilisé dans la liste
        /// </summary>
        /// <returns></returns>
        public int getNewId()
        {
            //TODO: mettre un hash? comme id
            Random random = new Random();
            
            int result = random.Next(0, 1000);

            bool completed = false;
            while(!completed)
            {
                completed = true;
                foreach (Touch touch in collection)
                  {
                    //TODO vérifier que l'id est unique
                      if (touch.Id == result)
                      {
                          completed = false;
                          result = random.Next(0, 1000);
                          break;
                      }
                }
            }


            return result;
        }

        public object Clone()
        {
            return new TouchCollection(this.collection);
        }
    }
}
