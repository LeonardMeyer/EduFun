using EduFun.Games;
using EduFun.Kinect;
using EduFun.Library.Resources;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace EduFun.Welcome
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class WelcomeScreen : Window, SpeechListener
    {
        private DispatcherTimer CurrentDate;

        public event EventHandler<SpeechRecognizedEventArgs> SpeechRecognized;

        private String[] BASEWORDS = {"Kinect pause", "kinect callibration", "kinect éteind toi"};

        public string CurrentDateString { get; set; }

        GameManager GameMgr;

        Access KinectLib = Access.GetInstance();
        
        public WelcomeScreen()
        {
            InitializeComponent();

            Window.DataContext = this;

            CurrentDate = new DispatcherTimer();
            CurrentDate.Start();
            CurrentDate.Tick += new EventHandler(delegate(object s, EventArgs e)
                {
                    UpdateTime(s, e);
                    TimeLbl.Content = CurrentDateString;
                });

            CurrentDate.Interval = TimeSpan.FromSeconds(1.0);

            GameMgr = GameManager.GetInstance();
   
            activitiesControl.ItemsSource = GameMgr.GameList;
            
            
            //Kinect.Callibration.MainWindow mw = new Kinect.Callibration.MainWindow();
            //mw.Show();
            
            KinectLib = Access.GetInstance(this);

            KinectLib.OnTouchDown += KinectLib_OnTouchDown;
            KinectLib.OnTouchUp += KinectLib_OnTouchUp;
            KinectLib.OnTouchMove += KinectLib_OnTouchMove;

            KinectLib.setSpeechRecognition(BASEWORDS);
            KinectLib.OnSpeechRecognized += kinect_OnSpeechRecognized;

            this.Visibility = System.Windows.Visibility.Collapsed;

            
            
        }

        void KinectLib_OnTouchMove(object sender, TouchPointEventArgs e)
        {
            ;
        }

        private void kinect_OnSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            EventHandler<Library.Resources.SpeechRecognizedEventArgs> handler = SpeechRecognized;

            if (handler != null)
            {
                handler(this, e);
            }


        }



        void KinectLib_OnTouchUp(object sender, TouchPointEventArgs e)
        {
            ;
        }

        void gameBtn_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            ContentPresenter cp = sender as ContentPresenter;
            if (cp != null)
            {
                Lazy<IGame> gameModule = cp.Content as Lazy<IGame>;
                if (gameModule != null)
                {
                    RegisterGame(gameModule.Value);
                    gameModule.Value.Start();
                    //Access.GetInstance(this);
                }
            }
        }

        void KinectLib_OnTouchDown(object sender, TouchPointEventArgs e)
        {
            ;
        }

        private void UpdateTime(object sender, EventArgs e)
        {
            CurrentDateString = String.Format("{0:dddd d MMMM yyyy HH:mm:ss}", DateTime.Now);
        }

        private void ShutDown_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ShutDown_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void RegisterGame(IGame game)
        {
            UIElement ui = game.Initialize(this);
            Access.GetInstance(ui);
            KinectLib.setSpeechRecognition(game.SpeechCommands);
        }

        private void UnregisterGame(IGame game)
        {
            Access.GetInstance(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (object btn in activitiesControl.Items)
            {
                Lazy<IGame> game = btn as Lazy<IGame>;
                if (game != null)
                {
                    ContentPresenter gameBtn = activitiesControl.ItemContainerGenerator.ContainerFromItem(game) as ContentPresenter;
                    if (gameBtn != null)
                        gameBtn.TouchUp += gameBtn_TouchUp;
                }
            }
        }

        
    }
}
