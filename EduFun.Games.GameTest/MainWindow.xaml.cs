
using EduFun.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Speech.Recognition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace EduFun.Games.GameTest
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        Kinect.Access kinect;


        public MainWindow()
        {
            InitializeComponent();

            try
            {
                kinect = Kinect.Access.GetInstance(this);
                txtSeuil.Text = kinect.Threshold.ToString();
                txtMin.Text = kinect.MinDepth.ToString();
                txtMax.Text = kinect.MaxDepth.ToString();

                //*
                kinect.OnTouchDown += kinect_OnTouchDown;
                kinect.OnTouchMove += kinect_OnTouchMove;
                kinect.OnTouchUp += kinect_OnTouchUp;
                // kinect.OnTouchMaintained += kinect_OnTouchMaintained;
                //*/
                // kinect.OnColorFrameReady += kinect_OnColorFrameReady;

                kinect.setSpeechRecognition(new[] { "Kinect pause", "kinect calibration", "kinect parasites", "kinect éteind toi"});
                kinect.OnSpeechRecognized += kinect_OnSpeechRecognized;
            }
            catch (KinectExceptions e)
            {
                MessageBox.Show(e.Message, "Erreur");
                Environment.Exit(0);
            }



        }

        void kinect_OnSpeechRecognized(object sender, EduFun.Library.Resources.SpeechRecognizedEventArgs e)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    speechRecognized(e);
                });
            }
            else
            {
                speechRecognized(e);
            }

        }
        void speechRecognized(EduFun.Library.Resources.SpeechRecognizedEventArgs e)
        {
            Console.WriteLine(e.result);

            if (e.result == "kinect calibration")
            {
                this.btnCalibrerProjection_Click(this, null);
            }
            else if (e.result == "kinect parasites")
            {
                this.btnCalibrer_Click(this, null);
            }
            else if (e.result == "kinect éteind toi")
            {
                this.Close();
            }
        }



        void kinect_OnColorFrameReady(object sender, ColorFrameEventArgs e)
        {

            ColorView.Source = e.GetOutputImage();
        }

        private void drawTouch(EduFun.Kinect.Touch touch)
        {
            Ellipse ellipse = new Ellipse();

            SolidColorBrush solidColorBrush = new SolidColorBrush();

            solidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 0, 255, 0);
            ellipse.Fill = solidColorBrush;


            SetPosition(ellipse, touch);

            ellipse.Tag = touch.Id;
            canvas.Children.Add(ellipse);

        }

        private void moveTouch(EduFun.Kinect.Touch touch)
        {
            foreach (UIElement uielement in canvas.Children)
            {
                if (((Ellipse)uielement).Tag.Equals((object)touch.Id))
                {
                    SetPosition(uielement, touch);

                    SolidColorBrush solidColorBrush = new SolidColorBrush();
                    solidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 0, 0, 255);
                    (uielement as Ellipse).Fill = solidColorBrush;

                    return;
                }
                else
                {
                    int i = 0;
                    i++;
                }
            }


        }

        private void eraseTouch(EduFun.Kinect.Touch touch)
        {
            foreach (UIElement uielement in canvas.Children)
            {
                if (((Ellipse)uielement).Tag.Equals((object)touch.Id))
                {

                    canvas.Children.Remove(uielement);

                    SolidColorBrush solidColorBrush = new SolidColorBrush();
                    solidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 255, 0, 0);
                    (uielement as Ellipse).Fill = solidColorBrush;

                    uielement.Visibility = Visibility.Collapsed;
                    return;
                }
            }
        }

        private void SetPosition(UIElement uiElement, EduFun.Kinect.Touch touch)
        {
            Canvas.SetLeft(uiElement, touch.ScreenPositionFloat.X * this.Width);
            Canvas.SetTop(uiElement, touch.ScreenPositionFloat.Y * this.Height);
            (uiElement as Ellipse).Height = touch.Superficie;
            (uiElement as Ellipse).Width = touch.Superficie;
        }

        private void setColor(UIElement uielement, System.Windows.Media.Color color)
        {

            SolidColorBrush solidColorBrush = new SolidColorBrush();

            solidColorBrush.Color = color;
            ((Ellipse)uielement).Fill = solidColorBrush;
        }

        void kinect_OnTouchUp(object sender, TouchPointEventArgs e)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    eraseTouch(e.Touch);
                });
            }
            else
            {
                eraseTouch(e.Touch);
            }
            
        }

        void kinect_OnTouchMove(object sender, TouchPointEventArgs e)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    moveTouch(e.Touch);
                });
            }
            else
            {
                moveTouch(e.Touch);
            }
            
        }

        void kinect_OnTouchMaintained(object sender, TouchPointEventArgs e)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    drawTouch(e.Touch);
                });
            }
            else
            {
                drawTouch(e.Touch);
            }
        }

        void kinect_OnTouchDown(object sender, TouchPointEventArgs e)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    drawTouch(e.Touch);
                });
            }
            else
            {
                drawTouch(e.Touch);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Kinect.Access.GetInstance().SensorStop();
        }

        private void btnAppliquerChangements_Click(object sender, RoutedEventArgs e)
        {
            int i, j, k;
            if (int.TryParse(txtSeuil.Text, out i) && int.TryParse(txtMin.Text, out j) && int.TryParse(txtMax.Text, out k))
            {
                if (j > k)
                {
                    MessageBox.Show("La valeur minimale ne peut être supérieure à la valeur maximale!", "Erreur de valeur", MessageBoxButton.OK);
                    return;
                }
                kinect.Threshold = i;
                kinect.MinDepth = j;
                kinect.MaxDepth = k;
            }
            else
            {
                MessageBox.Show("Au moins une des valeurs de calibrage n'est pas bonne", "Erreur de valeur", MessageBoxButton.OK);
            }
        }

        private void btnCalibrer_Click(object sender, RoutedEventArgs e)
        {
            kinect.baseDepthReset = true;
        }


        private void btnCalibrerProjection_Click(object sender, RoutedEventArgs e)
        {
            Kinect.Callibration.MainWindow mw = new Kinect.Callibration.MainWindow();
            mw.Show();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            Access.GetInstance().SetScreen(new[] { new PointInt(161, 41), new PointInt(597, 57), new PointInt(158, 364), new PointInt(604, 364) });
            //Access.Instance.SetScreen(new[] { new PointInt(0, 0), new PointInt(100, 0), new PointInt(0, 100), new PointInt(100, 100) });
        }



    }
}
