using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EduFun.Kinect;
using System.Windows;
using System.Drawing;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;


namespace EduFun.Kinect.Callibration
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private PointInt[] CalculedBoundaries = new PointInt[4];
        private Access kinect;
        private List<Ellipse> ellipses = new List<Ellipse>();
        private int touchId;
        private bool touchIsMaintained = false;

        Window Parent;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(Window parent)
        {
            InitializeComponent();
            Parent = parent;
        }

        /*
        void kinect_OnTouchMaintained(object sender, TouchPointEventArgs e)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    touchMaintained(e);
                });
            }
            else
            {
                touchMaintained(e);
            }


        }
        void touchMaintained(TouchPointEventArgs e)
        {

            if (e.Touch.id == touchId)
            {
                for (int i = 0; i < CalculedBoundaries.Length; i++)
                {
                    if (CalculedBoundaries[i].IsEmpty())
                    {
                        CalculedBoundaries[i] = e.Touch.KinectPosition;

                        SolidColorBrush solidColorBrush = new SolidColorBrush();
                        solidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 0, 255, 0);
                        ellipses[i].Fill = solidColorBrush;

                        if (i == 3)
                        {
                            kinect.SetScreen(CalculedBoundaries);
                            kinect.OnTouchMaintained -= kinect_OnTouchMaintained;
                            this.Close();

                        }

                        touchIsMaintained = true;

                        return;
                    }
                }
            }
        }

        void kinect_OnTouchMove(object sender, TouchPointEventArgs e)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    touchMove(e);
                });
            }
            else
            {
                touchMove(e);
            }

        }

        void touchMove(TouchPointEventArgs e)
        {

             if (e.Touch.id == touchId && !touchIsMaintained)
            {

                if (!e.Touch.isMaintainable)
                {
                    for (int i = 0; i < CalculedBoundaries.Length; i++)
                    {
                        if (CalculedBoundaries[i].IsEmpty())
                        {
                            SolidColorBrush solidColorBrush = new SolidColorBrush();
                            solidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 255, 0, 0);
                            ellipses[i].Fill = solidColorBrush;
                        }

                    }
                    return;
                }

                for (int i = 0; i < CalculedBoundaries.Length; i++)
                {
                    if (CalculedBoundaries[i].IsEmpty())
                    {


                        SolidColorBrush solidColorBrush = new SolidColorBrush();
                        solidColorBrush.Color = System.Windows.Media.Color.FromArgb((byte)(e.Touch.duration * 255 / 50), 0, 255, 0);
                        ellipses[i].Fill = solidColorBrush;

                        return;
                    }
                }
            }

        }

        void kinect_OnTouchDown(object sender, TouchPointEventArgs e)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    touchDown(e);
                });
            }
            else
            {
                touchDown(e);
            }

        }

        void touchDown(TouchPointEventArgs e)
        {
            if (touchId == 0)
            {
                touchId = e.Touch.id;

                touchIsMaintained = false;

            }

        }

        void kinect_OnTouchUp(object sender, TouchPointEventArgs e)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    touchUp(e);
                });
            }
            else
            {
                touchUp(e);
            }
        }

        void touchUp(  TouchPointEventArgs e)
        {

                       if (e.Touch.id == touchId)
            {
                touchId = 0;

                for (int i = 0; i < CalculedBoundaries.Length; i++)
                {
                    if (CalculedBoundaries[i].IsEmpty())
                    {
                        SolidColorBrush solidColorBrush = new SolidColorBrush();
                        solidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 255, 0, 0);
                        ellipses[i].Fill = solidColorBrush;
                    }

                }
            }
        }
        */
        System.Media.SoundPlayer SoundTouchDown;
        System.Media.SoundPlayer SoundTouchMaintained;
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            kinect = Access.GetInstance(this);

            kinect.ResetScreen();

            kinect.OnTouchMaintained += kinect_OnTouchMaintained;
            // kinect.OnTouchMove += kinect_OnTouchMove;
            //kinect.OnTouchUp += kinect_OnTouchUp;
            kinect.OnTouchDown += kinect_OnTouchDown;

            kinect.setSpeechRecognition(new[] { "kinect recommencer" });
            kinect.OnSpeechRecognized += kinect_OnSpeechRecognized;

            SolidColorBrush solidColorBrush = new SolidColorBrush();

            solidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 255, 0, 0);

            //TODO dessiner les points

            for (int i = 0; i < 4; i++)
            {

                Ellipse ellipse = new Ellipse();

                ellipse.Fill = solidColorBrush;


                if (i == 0 || i == 2)
                    Canvas.SetRight(ellipse, -12);
                else
                    Canvas.SetLeft(ellipse, -25);

                if (i == 2 || i == 3)
                    Canvas.SetBottom(ellipse, -12);
                else
                    Canvas.SetTop(ellipse, -25);

                ellipse.Height = 50;
                ellipse.Width = 50;


                ellipses.Add(ellipse);
            }
            canvas.Children.Add(ellipses[0]);


            //son
            SoundTouchDown = new System.Media.SoundPlayer();
            SoundTouchDown.SoundLocation = "C:\\Windows\\Media\\Windows Menu Command.wav";
            SoundTouchMaintained = new System.Media.SoundPlayer();
            SoundTouchMaintained.SoundLocation = "C:\\Windows\\Media\\Speech On.wav";
        }

        private void Restart()
        {
            this.CalculedBoundaries = new PointInt[4];

            canvas.Children.RemoveRange(0, 4);
            canvas.Children.Add(ellipses[0]);


        }


        private void kinect_OnTouchDown(object sender, TouchPointEventArgs e)
        {
            SoundTouchDown.Stop();
            SoundTouchDown.Play();
        }

        private void kinect_OnTouchMaintained(object sender, TouchPointEventArgs e)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    touchMaintained(e);
                });
            }
            else
            {
                touchMaintained(e);
            }


        }
        void touchMaintained(TouchPointEventArgs e)
        {
            SoundTouchMaintained.Play();
            for (int i = 0; i < CalculedBoundaries.Length; i++)
            {
                if (CalculedBoundaries[i].IsEmpty())
                {
                    CalculedBoundaries[i] = e.Touch.KinectPosition;

                    SolidColorBrush solidColorBrush = new SolidColorBrush();
                    solidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 0, 255, 0);
                    ellipses[i].Fill = solidColorBrush;

                    if (i == 3)
                    {
                        kinect.SetScreen(CalculedBoundaries);
                        kinect.OnTouchMaintained -= kinect_OnTouchMaintained;
                        this.Visibility = System.Windows.Visibility.Collapsed;

                        this.ClosingDelegate();
                        EduFun.Welcome.WelcomeScreen accueil = new Welcome.WelcomeScreen();
                        accueil.Show();
                    }

                    // touchIsMaintained = true;

                    if (i + 1 < ellipses.Count())
                    {
                        SolidColorBrush solidColorBrushRouge = new SolidColorBrush();
                        solidColorBrushRouge.Color = System.Windows.Media.Color.FromArgb(255, 255, 0, 0);
                        ellipses[i + 1].Fill = solidColorBrushRouge;
                        canvas.Children.Add(ellipses[i + 1]);
                    }
                    return;
                }
            }
        }


        void kinect_OnSpeechRecognized(object sender, EduFun.Library.Resources.SpeechRecognizedEventArgs e)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    SpeechRecognized(e);
                });
            }
            else
            {
                SpeechRecognized(e);
            }


        }

        void SpeechRecognized(EduFun.Library.Resources.SpeechRecognizedEventArgs e)
        {


            if (e.result == "kinect recommencer")
            {
                this.Restart();
            }

        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ClosingDelegate();
        }

        private void ClosingDelegate()
        {
            kinect.OnTouchDown -= kinect_OnTouchDown;
            kinect.OnTouchMaintained -= kinect_OnTouchMaintained;
        }







    }
}
