namespace GameLibrary.Games
{
    public class SteamGame : Game
    {
        public string appId;
        public SteamGame(string [] data)
        {
            tittle = data[0];
            appId = data[1];
        }
    }
}
