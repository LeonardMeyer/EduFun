using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EduFun.Kinect
{
    public class ColorFrameEventArgs : EventArgs
    {
        /// <summary>
        /// Objet retourné par la Kinect
        /// </summary>
        public ColorImageFrame imageFrame;

        private WriteableBitmap outputImage;

        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;


        public ColorFrameEventArgs(ColorImageFrame imageFrame)
        {
            this.imageFrame = imageFrame;
        }

        /// <summary>
        /// permet de calculer l'image couleur à partir du retour de la Kinect
        /// </summary>
        /// <returns>Un objet Bitmap accepté par le control WPF Image.source</returns>
        public WriteableBitmap GetOutputImage()
        {
            

            Byte[] pixelData = new Byte[imageFrame.PixelDataLength];

            imageFrame.CopyPixelDataTo(pixelData);

            if (outputImage == null)
            {
                outputImage = new WriteableBitmap(imageFrame.Width, imageFrame.Height, 96, 96, PixelFormats.Bgr32, null);
                outputImage.WritePixels(new Int32Rect(0, 0, imageFrame.Width, imageFrame.Height), pixelData, imageFrame.Width * Bgr32BytesPerPixel, 0);

            }

            return outputImage;


        }


    }
}
