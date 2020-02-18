using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Games
{
    
    class NonSteam : Game
    {
        public string pathToEXE;

        public NonSteam(string[] data)
        {
            this.tittle = data[0];
            this.pathToEXE = data[1];
        }
    }
}
