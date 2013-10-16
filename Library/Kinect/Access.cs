using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using Microsoft.Kinect.Interop;
using System.Threading;
using System.Windows;
using EduFun.Library.Controls;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Threading;
//using System.Speech.Recognition;
//using System.Speech.AudioFormat;

namespace EduFun.Kinect
{
    public class Access
    {
        #region Construction
        //singleton
        private static Access instance;

        public static Access GetInstance(UIElement window)
        {
            _ref = window;
            return GetInstanceCommon();
        }

        public static Access GetInstance()
        {
            return GetInstanceCommon();
        }

        private static Access GetInstanceCommon()
        {
                        if (instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        //public static Access Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            instance = new Access();
        //        }
        //        return instance;
        //    }
        //}

        //constructeur privé

        private Access()
        {

            //TODO géré la présence de plusieurs Kinects
            if (KinectSensor.KinectSensors.Count > 0)
            {
                _sensor = KinectSensor.KinectSensors[0];

                if (_sensor.Status == KinectStatus.Connected)
                {
                    Thread t = new Thread(delegate()
                    {
                        //_sensor.ColorStream.Enable();
                        _sensor.DepthStream.Range = DepthRange.Near;

                        //résolution de Kinect
                        //_sensor.DepthStream.Enable(DepthImageFormat.Resolution80x60Fps30);
                        //_sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                        _sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                        //_sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                        frameSize.X = _sensor.DepthStream.FrameWidth;
                        frameSize.Y = _sensor.DepthStream.FrameHeight;

                        _sensor.DepthFrameReady += _sensor_DepthFrameReady;
                        //_sensor.ColorFrameReady += _sensor_ColorFrameReady;
                        _sensor.Start();

                        MinDepth = 10;
                        MaxDepth = 40;
                        Threshold = 35;

                        this.ResetScreen();

                        //reconaissance vocale
                        RecognizerInfo ri = GetKinectRecognizer();

                        if (null != ri)
                        {
                            this.SoundSpeechRecognized.SoundLocation = "C:\\Windows\\Media\\Speech On.wav";
                            this.speechEngine = new SpeechRecognitionEngine(ri.Id);


                            speechEngine.SpeechRecognized += speechEngine_SpeechRecognized;

                            setSpeechRecognition(new[] { "kinect parasite" });

                            speechEngine.SetInputToAudioStream(
                                _sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                            speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                        }
                        else
                        {
                            Console.WriteLine("PAS DE RECONAISSANCE VOCALE");
                        }

                    });

                    t.Start();
                    
                    t.Join();
                }
                else
                {
                    throw new KinectExceptions("Pas de Kinect de connectée");
                }
            }
        }





        #endregion

        #region Variables

        private KinectSensor _sensor;

        /// <summary>
        /// demande de réinitialisation de baseDepth
        /// </summary>
        public bool baseDepthReset = true;
        /// <summary>
        /// profondeur de base testée à chaque frame pour trouver les points de touché
        /// </summary>
        private int[] baseDepth;

        private int _threshold;
        /// <summary>
        /// Seuil de différence entre les parasites et un doigt
        /// </summary>
        public int Threshold { get { return _threshold; } set { _threshold = value; pixelJump = Convert.ToInt32(Math.Sqrt(value)) - 1; } }
        private int _minDepth;
        /// <summary>
        /// profondeur minium de repérage du touché
        /// </summary>
        public int MinDepth { get { return _minDepth; } set { if (value > 5 || value < MaxDepth) _minDepth = value; } }
        private int _maxDepth;
        /// <summary>
        /// profondeur maximum de repérage du touché
        /// </summary>
        public int MaxDepth { get { return _maxDepth; } set { if (value > MinDepth) _maxDepth = value; } }
        /// <summary>
        /// Défini le saut de pixel à ignorer sur la depthFrame
        /// </summary>
        private int pixelJump;

        public static UIElement _ref { get; set; }

        

        /// <summary>
        /// Représente la taille de l'image en x et y
        /// </summary>
        private PointInt frameSize;

        /// <summary>
        /// Tableau des données de profondeur reçut de la Kinect permettant de faire des comparaisons
        /// </summary>
        private short[] depthData;

        /// <summary>
        /// contient la position de la projection video dans le repère vidéo de la Kinect.
        /// </summary>
        public Screen Screen;

        /// <summary>
        /// Contient les derniers points touch détecté
        /// </summary>
        private TouchCollection lastTouchCollection = new TouchCollection();

        private SpeechRecognitionEngine speechEngine;

        private float _lastTime;
        private int _fps;
        public int FPS
        {
            get;
            private set;
        }


        #endregion

        #region Events
        /// <summary>
        /// Event se déclanchant quand un utilisateur pose son doigt sur la surface
        /// </summary>
        public event EventHandler<TouchPointEventArgs> OnTouchDown;
        /// <summary>
        /// Event se déclanchant quand un utilisateur laisse son doigt posé sur la surface (à partir de la deuxième image)
        /// </summary>
        public event EventHandler<TouchPointEventArgs> OnTouchMove;
        /// <summary>
        /// Event se déclanchant quand un utilisateur laisse son doigt posé sur la surface sans le bouger.
        /// </summary>
        public event EventHandler<TouchPointEventArgs> OnTouchMaintained;
        /// <summary>
        /// Event se déclancheant quand un utilisateur enlève son doigt de la surface
        /// </summary>
        public event EventHandler<TouchPointEventArgs> OnTouchUp;
        /// <summary>
        /// permet de récupérer le flux vidéo couleur de la Kinect. 
        /// </summary>
        public event EventHandler<ColorFrameEventArgs> OnColorFrameReady;
        /// <summary>
        /// permet de récupérer les mots reconnus 
        /// </summary>
        public event EventHandler<Library.Resources.SpeechRecognizedEventArgs> OnSpeechRecognized;
        /// <summary>
        /// son joué lors de la reconnaissance vocale
        /// </summary>
        private System.Media.SoundPlayer SoundSpeechRecognized = new System.Media.SoundPlayer(); 


        #endregion

        #region OnRaiseEvent
        protected void OnRaiseTouchDownEvent(Touch touch)
        {
            EventHandler<TouchPointEventArgs> handler = OnTouchDown;
            if (handler != null)
            {
                //si besoin de réaliser du traitement en plus pour compléter l'eventArg, c'est ici!
                handler(this, new TouchPointEventArgs(touch));

                if (_ref.Dispatcher.Thread != Thread.CurrentThread)
                {
                    _ref.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                    {
                        DelegateTargetTouchDown(touch);
                    });
                }
                else
                {
                    DelegateTargetTouchDown(touch);
                }

                

                
               // target.t
            }
        }

        private void DelegateTargetTouchDown(Touch touch)
        {
            UIElement target = _ref.InputHitTest(touch.GetScreenPositionPixels(_ref.RenderSize)) as UIElement;

            if (target != null)
            {
                TouchEventArgs args = new TouchEventArgs(new FakeDevice(touch.Id, touch, _ref.RenderSize, TouchAction.Down), 0);
                args.RoutedEvent = UIElement.TouchDownEvent;
                target.RaiseEvent(args);
            }
        }

        protected void OnRaiseTouchMoveEvent(Touch touch)
        {
            EventHandler<TouchPointEventArgs> handler = OnTouchMove;

            if (handler != null)
            {
                //si besoin de réaliser du traitement en plus pour compléter l'eventArg, c'est ici!
                handler(this, new TouchPointEventArgs(touch));
                //handler.BeginInvoke(this, new TouchPointEventArgs(touch), null, null);

                if (_ref.Dispatcher.Thread != Thread.CurrentThread)
                {
                    _ref.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                    {
                        DelegateTargetTouchDown(touch);
                    });
                }
                else
                {
                    DelegateTargetTouchDown(touch);
                }
            }
        }

        private void DelegateTargetTouchMove(Touch touch)
        {
            
               UIElement target = _ref.InputHitTest(touch.GetScreenPositionPixels(_ref.RenderSize)) as UIElement;

            if (target != null)
            {
                TouchEventArgs args = new TouchEventArgs(new FakeDevice(touch.Id, touch, _ref.RenderSize, TouchAction.Move), 0);
                args.RoutedEvent = UIElement.TouchMoveEvent;
                target.RaiseEvent(args);
            }
            
        }
        


        protected void OnRaiseTouchMaintainedEvent(Touch touch)
        {
            EventHandler<TouchPointEventArgs> handler = OnTouchMaintained;
            if (handler != null)
            {
                //si besoin de réaliser du traitement en plus pour compléter l'eventArg, c'est ici!
                handler(this, new TouchPointEventArgs(touch));
                //handler.BeginInvoke(this, new TouchPointEventArgs(touch), null, null);
            }
        }

        protected void OnRaiseTouchUpEvent(Touch touch)
        {
            EventHandler<TouchPointEventArgs> handler = OnTouchUp;

            if (handler != null)
            {
                //si besoin de réaliser du traitement en plus pour compléter l'eventArg, c'est ici!
                handler(this, new TouchPointEventArgs(touch));

                if (_ref.Dispatcher.Thread != Thread.CurrentThread)
                {
                    _ref.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                    {
                        DelegateTargetTouchUp(touch);
                    });
                }
                else
                {
                    DelegateTargetTouchUp(touch);
                }

                //handler.BeginInvoke(this, new TouchPointEventArgs(touch), null, null);
            }


        }

        private void DelegateTargetTouchUp(Touch touch)
        {
            System.Windows.Point pt = touch.GetScreenPositionPixels(_ref.RenderSize);
            UIElement target = _ref.InputHitTest(pt) as UIElement;

            if (target != null)
            {
                FakeDevice fkDevice = new FakeDevice(touch.Id, touch, _ref.RenderSize, TouchAction.Up);
                TouchPoint point = new TouchPoint(fkDevice, pt, new Rect(), TouchAction.Up);
                TouchEventArgs args = new TouchEventArgs(fkDevice, 0);
                args.RoutedEvent = UIElement.TouchUpEvent;
                target.RaiseEvent(args);
            }
        }

        protected void OnRaiseColorFrameReadyEvent(ColorImageFrame imageFrame)
        {
            EventHandler<ColorFrameEventArgs> handler = OnColorFrameReady;

            if (handler != null)
            {
                //si besoin de réaliser du traitement en plus pour compléter l'eventArg, c'est ici!

                handler(this, new ColorFrameEventArgs(imageFrame));
                //handler.BeginInvoke(this, new ColorFrameEventArgs(imageFrame), null, null);
            }
        }

        //tout le traitement lors de la réception d'une image couleur
        void _sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {



            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                    return;

                OnRaiseColorFrameReadyEvent(colorFrame);

            }
        }

        //tout le traitement lors de la réception d'une image de profondeur
        void _sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                ColorImageFormat cif = new ColorImageFormat();



                if (depthFrame == null)
                {
                    return;
                }

                //récupération des touchés de l'image actuelle
                TouchCollection actualTouchCollection = GetTouchsFromDepthFrame(depthFrame);
                //on garde une copie pour plus tard.
                TouchCollection tmpTouchCollection = new TouchCollection();



                //on parcours tous les points de touché de l'image précedente.
                foreach (Touch lastTouch in lastTouchCollection.collection.GetRange(0, lastTouchCollection.collection.Count))
                {
                    //on parcourt tous les touchés de l'image actuelle
                    foreach (Touch actualTouch in actualTouchCollection.collection.GetRange(0, actualTouchCollection.collection.Count))
                    {
                        //on vérifie si un touché actuel était déjà présent avant
                        if (Touch.IsCloseTo(lastTouch, actualTouch, Threshold / 2, lastTouch.KinectTranslation))
                        {


                            //on trace l'id, l'origine et l'age afin de garder le touché.
                            actualTouch.Id = lastTouch.Id;
                            actualTouch.Origin = lastTouch.Origin;
                            actualTouch.Duration = lastTouch.Duration + 1;
                            actualTouch.IsMaintainable = lastTouch.IsMaintainable;
                            actualTouch.PreviousScreenPosition = lastTouch.ScreenPositionFloat;
                            actualTouch.PreviousKinectPosition = lastTouch.KinectPosition;

                            //crade? on met a jour l'id dans les deux listes..
                            tmpTouchCollection.collection.Add(actualTouch);

                            //on raise l'event d'un touché qui reste appuié
                            OnRaiseTouchMoveEvent(actualTouch);


                            //on vérifie si c'est un touché maintenu.
                            //le point ne peut être maintenu qu'une seule fois
                            if (actualTouch.IsMaintainable)
                            {
                                //on regarde si le touché à bougé
                                if (actualTouch.GetDistanceFromOrigin() > Threshold / 2)
                                {
                                    actualTouch.IsMaintainable = false;
                                }
                                else
                                {
                                    //on lance l'event d'un touché maintenu
                                    if (actualTouch.Duration > 45)
                                    {
                                        actualTouch.IsMaintainable = false;
                                        OnRaiseTouchMaintainedEvent(actualTouch);
                                    }
                                }
                            }


                            //on supprime ce point des deux listes pour ne pas y retourner.
                            actualTouchCollection.collection.Remove(actualTouch);
                            lastTouchCollection.collection.Remove(lastTouch);

                            //on arrete de boucler sur les points de l'image actuelle, car on a trouvé une correspondance avec notre ancien point.
                            break;
                        }
                    }
                }
                foreach (Touch upTouch in lastTouchCollection.collection)
                {
                    OnRaiseTouchUpEvent(upTouch);
                }

                //on a bouclé sur tous les points en supprimant ceux qui étaient déjà dans l'image précedente.
                //on a donc un nouveau touché
                foreach (Touch newTouch in actualTouchCollection.collection)
                {
                    //c'est un nouveau point, on set alors son origine.
                    newTouch.Origin = newTouch.KinectPosition;

                    //on crée une ID non utilisée pour ce touché.
                    //on regarde donc les IDs des points actuels. possibilité d'erreur avec les points précedents supprimés?
                    newTouch.Id = actualTouchCollection.getNewId();
                    //crade?! on répercute sur la deuxième liste
                    tmpTouchCollection.collection.Add(newTouch);

                    OnRaiseTouchDownEvent(newTouch);
                }

                lastTouchCollection = tmpTouchCollection;
            }
        }


        protected TouchCollection GetTouchsFromDepthFrame(DepthImageFrame depthFrame)
        {
            TouchCollection result = new TouchCollection();
            /// Liste des pixels autour desquels on va chercher des touchs
            List<int> pixels = new List<int>();
            // Tableau des données de profondeur
            depthData = new short[depthFrame.PixelDataLength];

            // Image des résultats
            //int depthStride = depthFrame.Width * 4;
            //byte[] depthPixels = new byte[depthFrame.PixelDataLength * 4];


            // récupération des données de profondeur
            depthFrame.CopyPixelDataTo(depthData);

            // reset de l'image de référence
            if (baseDepthReset == true)
            {
                setBaseDepth(depthData);
                baseDepthReset = false;
                return result;
            }

            // récupère les points autour desquels on va devoir chercher des points "Touch"
            pixels = GetPixelForDetection(depthData);

            //recherche des points utilisables environnant
            for (int i = 0; i < pixels.Count; i++)
            {
                // Si le point est à zéro, alors on l'a déjà parcourut et il est utilisé
                if (depthData[pixels[i]] == 0)
                {
                    continue;
                }

                // Si utilisable, on le met à zéro
                depthData[pixels[i]] = 0;

                //on créé un touch potentiel avec le premier point
                Touch touch = new Touch();
                touch.collection.Add(GetCoordonateFromPosition(pixels[i]));

                //compteur
                int j = 0;

                // point à tester
                PointInt pointTest = new PointInt();
                PointInt pointActuel;

                bool completed = false;
                //vérification que le point n'est pas seul
                while (!completed)
                {

                    if (j >= touch.collection.Count)
                    {
                        completed = true;
                        continue;
                    }

                    pointActuel = touch.collection[j];

                    //on parcourt les 8 cases autour du point actuel.
                    //si une case est un point de touché, on la rajoute au touché complet.
                    //si on a tout parcouru, c'est que le touché est complet.
                    int portee = 3;
                    for (int x = -portee; x <= portee; x++)
                    {
                        for (int y = -portee; y <= portee; y++)
                        {
                            pointTest.X = pointActuel.X + x;
                            pointTest.Y = pointActuel.Y + y;
                            int position = GetPositionFromCoordonate(pointTest, frameSize);
                            if (position != -1 && IsTouch(depthData[position], position))
                            {
                                depthData[position] = 0;
                                touch.collection.Add(pointTest);
                            }
                        }
                    }

                    j++;
                }
                // On vérifie si la collection contient assez de point, si oui alors ce n'est pas un parasite
                if (touch.collection.Count > Threshold)
                {
                    result.collection.Add(touch);
                }
            }
            return result;
        }



        #endregion

        #region utils

        /// <summary>
        /// permet de réaliser le callibrage du flux vidéo par rapport au projecteur
        /// en créant une instance d'une nouvelle fenêtre.
        /// </summary>
        public void SetScreen(PointInt[] ScreenBoundaries)
        {
            // Screen = new Kinect.Screen();
            Screen.Boundaries = ScreenBoundaries;
        }


        /// <summary>
        /// permet de remettre à 0 le callibrage de l'écran
        /// </summary>
        public void ResetScreen()
        {
            //TODO: gérer les carrés (coefficient de 0 ou +infini)
            PointInt[] screenBoundaries = new PointInt[4];

            screenBoundaries[0] = new PointInt(0, 0);
            screenBoundaries[1] = new PointInt(_sensor.DepthStream.FrameWidth, 1);
            screenBoundaries[2] = new PointInt(1, _sensor.DepthStream.FrameHeight - 1);
            screenBoundaries[3] = new PointInt(_sensor.DepthStream.FrameWidth - 1, _sensor.DepthStream.FrameHeight);

            this.Screen = new Screen(screenBoundaries);
        }

        // la précision représente la taille du carré de précision désiré.
        protected List<int> GetPixelForDetection(short[] rawData)
        {
            int depart = 0;
            List<int> pixels = new List<int>();

            int width = _sensor.DepthStream.FrameWidth;
            int height = _sensor.DepthStream.FrameHeight;

            // set la première ligne à la moitié
            depart = width * Convert.ToInt16(pixelJump / 2) + Convert.ToInt16(pixelJump / 2);

            // pour chaque ligne
            for (int j = depart; j < height * width - 1; j += width * pixelJump)
            {
                // pour chaque ligne, on prend un pixel toute les valeurs de la précision
                for (int i = j; i < j + width; i += pixelJump)
                {
                    //si le point est sur la projection.
                    if (Screen.IsOver(GetCoordonateFromPosition(i)))
                    {
                        // si le point est utilisable, on l'ajoute à la liste
                        // note: à voir si ceux inutilisable si on prend plus tard ou pas les points voisins
                        if (IsTouch(rawData[i], i))
                        {
                            pixels.Add(i);
                        }
                    }

                }
            }

            return pixels;
        }



        private void setBaseDepth(short[] rawDepthData)
        {
            //création de la profondeur de base
            baseDepth = new int[_sensor.DepthStream.FramePixelDataLength];

            for (int i = 0; i < depthData.Length; i++)
            {
                baseDepth[i] = depthData[i] >> DepthImageFrame.PlayerIndexBitmaskWidth;
            }
        }

        /// <summary>
        /// Retourne les coordonnées x et y en fonction de la position du point dans le tableau et de la taille de la frame
        /// </summary>
        /// <returns></returVns>
        private PointInt GetCoordonateFromPosition(int i)
        {
            int y = i / _sensor.DepthStream.FrameWidth;
            int x = i - _sensor.DepthStream.FrameWidth * y;
            if (x < 0 || y < 0)
                Console.WriteLine("ALERTE Position x ou y < 0");
            return new PointInt(x, y);
        }

        /// <summary>
        /// Retourne la position dans le tableau des pixels de l'image en fonction de ses coordonnées
        /// </summary>
        /// <param name="p">Coordonnées du point</param>
        /// <param name="frame">Taille de la frame</param>
        /// <returns>retour -1 si la position n'est pas dans possible</returns>
        private int GetPositionFromCoordonate(PointInt p, PointInt frame)
        {
            int position = p.Y * frame.X + p.X;
            if (position > 0 && position < frame.X * frame.Y)
                return position;
            else
                return -1;
        }

        public void SensorStop()
        {
            if (_sensor != null)
            {
                _sensor.Stop();
                _sensor.AudioSource.Stop();
            }

            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= speechEngine_SpeechRecognized;
                this.speechEngine.RecognizeAsyncStop();
            }

        }





        int[] compteur = new int[6];
        int[] compteur2 = new int[6];

        /// <summary>
        /// vérifie un point, retourne si c'est un pixel de touché, set le point a 0 pour ne pas le vérifier deux fois.
        /// </summary>
        /// <param name="DepthData">valeur du pixel</param>
        /// <param name="i">emplacement du pixel</param>
        /// <returns></returns>
        private bool IsTouch(short DepthData, int i)
        {
            if (DepthData == 0)
                return false;

            int actualDepth = DepthData >> DepthImageFrame.PlayerIndexBitmaskWidth;

            //on supprime le point qui est vérifié pour ne pas y aller deux fois
            DepthData = 0;

            //si le point est exploitable

            if (baseDepth[i] != _sensor.DepthStream.TooNearDepth)
            {
                compteur[0] += 1;
            }
            if (baseDepth[i] != _sensor.DepthStream.TooFarDepth)
            {
                compteur[1] += 1;
            }
            if (baseDepth[i] != _sensor.DepthStream.UnknownDepth)
            {
                compteur[2] += 1;
            }
            if (actualDepth != _sensor.DepthStream.TooNearDepth)
            {
                compteur[3] += 1;
            }
            if (actualDepth != _sensor.DepthStream.TooNearDepth)
            {
                compteur[4] += 1;
            }
            if (actualDepth != _sensor.DepthStream.TooNearDepth)
            {
                compteur[5] += 1;
            }


            if (baseDepth[i] == _sensor.DepthStream.TooNearDepth)
            {
                compteur2[0] += 1;
            }
            if (baseDepth[i] == _sensor.DepthStream.TooFarDepth)
            {
                compteur2[1] += 1;
            }
            if (baseDepth[i] == _sensor.DepthStream.UnknownDepth)
            {
                compteur2[2] += 1;
            }
            if (actualDepth == _sensor.DepthStream.TooNearDepth)
            {
                compteur2[3] += 1;
            }
            if (actualDepth == _sensor.DepthStream.TooNearDepth)
            {
                compteur2[4] += 1;
            }
            if (actualDepth == _sensor.DepthStream.TooNearDepth)
            {
                compteur2[5] += 1;
            }


            if (baseDepth[i] != _sensor.DepthStream.TooNearDepth && baseDepth[i] != _sensor.DepthStream.TooFarDepth && baseDepth[i] != _sensor.DepthStream.UnknownDepth && actualDepth != _sensor.DepthStream.TooNearDepth && actualDepth != _sensor.DepthStream.TooFarDepth && actualDepth != _sensor.DepthStream.UnknownDepth)
            {
                //si le point est un point de touché
                if (actualDepth < baseDepth[i] - MinDepth && actualDepth > baseDepth[i] - MaxDepth)
                {
                    return true;
                }
            }

            return false;
        }


        #endregion

        #region speechrecognition

        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "fr-FR".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        /// <summary>
        /// Permet de rajouter les mots à reconnaitre
        /// </summary>
        /// <param name="values">tableau de string contenant les mots. il est conseillé de rajouter kinect devant les mots a reconnaitre pour ne pas faire de faux positifs</param>
        /// <returns>retourne false si la speech recognition FR n'est pas installée (par défault)</returns>
        public bool setSpeechRecognition(String[] values)
        {
            RecognizerInfo ri = GetKinectRecognizer();

            if (null != ri)
            {
                var actions = new Choices();

                foreach (String value in values)
                {
                    actions.Add(new SemanticResultValue(value, value));
                }

                var gb = new GrammarBuilder { Culture = ri.Culture };
                gb.AppendWildcard();
                gb.Append(actions);

                var g = new Grammar(gb);

                //speechEngine.UnloadAllGrammars();
                speechEngine.LoadGrammar(g);

                return true;
            }
            else
                return false;
        }

        //reconnaissance vocale
        void speechEngine_SpeechRecognized(object sender, Microsoft.Speech.Recognition.SpeechRecognizedEventArgs e)
        {

            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.5;


            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                //ici traitement de l'API lorsqu'un mot est détecté



                string result = e.Result.Semantics.Value.ToString();

                switch (result)
                {
                    case "kinect parasite":
                        baseDepthReset = true;
                        SoundSpeechRecognized.Play();
                        break;
                }

                EventHandler<Library.Resources.SpeechRecognizedEventArgs> handler = OnSpeechRecognized;

                if (handler != null)
                {
                    //si besoin de réaliser du traitement en plus pour compléter l'eventArg, c'est ici!
                   // handler(this, new Library.Resources.SpeechRecognizedEventArgs(result));

                    if (_ref.Dispatcher.Thread != Thread.CurrentThread)
                    {
                        _ref.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                        {
                            handler(this, new Library.Resources.SpeechRecognizedEventArgs(result));
                        });
                    }
                    else
                    {
                        handler(this, new Library.Resources.SpeechRecognizedEventArgs(result));
                    }

                }


            }
        }
        private void DelegateSpeechRecognized(Library.Resources.SpeechRecognizedEventArgs e)
        {

        }

        #endregion
    }
}
