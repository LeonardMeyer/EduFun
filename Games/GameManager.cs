using EduFun.Library;
using EduFun.Library.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduFun.Games
{
        [Export]
        public class GameManager
        {

            private static GameManager _instance;
            static readonly object instanceLock = new object();

            [ImportMany(AllowRecomposition = true)]
            public List<Lazy<IGame>> GameList { get; set; }


            private GameManager()
            {
                DirectoryCatalog catalog = new DirectoryCatalog(@"../../../Bibliothèque de jeux/");
                CompositionContainer container = new CompositionContainer(catalog);

                container.SatisfyImportsOnce(this);
            }

            public static GameManager GetInstance()
            {
                lock (instanceLock)
                {
                    if (_instance == null)
                        _instance = new GameManager();


                    return _instance;
                }
            }

            public void StartGame(IGame game)
            {
                game.GameEnded += this.SaveGameResultHandler;
                game.Start();   
            }

            private void SaveGameResultHandler(object sender, GameResultEventArgs e)
            {
                //Faire la save
            }

        }

}
