namespace GameLibrary.Games
{
    
    class NonSteam : Game
    {
        public string pathToEXE;

        public NonSteam(string[] data)
        {
            tittle = data[0];
            pathToEXE = data[1];
        }
    }
}
