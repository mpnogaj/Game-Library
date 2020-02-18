using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Games
{
    public class SteamGame : Game
    {
        public string appId;
        public SteamGame(string [] data)
        {
            this.tittle = data[0];
            this.appId = data[1];
        }
    }
}
