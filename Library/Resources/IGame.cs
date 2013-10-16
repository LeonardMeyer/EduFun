using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace EduFun.Library.Resources
{
    public interface IGame
    {
        string Name { get; set; }
        Bitmap GameIcon { get; set; }
        /// <summary>
        /// Tableau de Strings contenant les mots à reconnaitre par la Kinect
        /// </summary>
        string[] SpeechCommands{ get; set; }

        GameLevel Difficulty { get; set; }
        GameCategory Category { get; set; }
        SpeechListener EventManager { get; set; }

        /// <summary>
        /// Fenêtre d'exécution du jeu (UserControl)
        /// </summary>
        UIElement GameView { get; set; }
        

        /// <summary>
        /// Instancie les vues du jeu et toutes ses variables
        /// </summary>
        /// <param name="evtManager">Référence vers l'event manager</param>
        /// <returns>La vue principale qu'il faut enregistrer auprès d'Access</returns>
        UIElement Initialize(SpeechListener evtManager);

        /// <summary>
        /// Affiche la fenêtre du jeu
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// Création de la fenêtre du menu pause, doit être en plein écran
        /// </summary>
        /// <returns>La fenêtre du menu pause</returns>
        UIElement Pause();
        

        


        event EventHandler<GameResultEventArgs> GameEnded;

        

        event EventHandler<EventArgs> GameLoaded;
        System.Windows.Size GetSize();




    }


}
