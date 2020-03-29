using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GameLibrary.Games;
using GameLibrary.Properties;
using GameLibrary.Windows;

namespace GameLibrary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly List<Game> _games = new List<Game>();  
        static readonly string Dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Game Library\";
        static readonly string DataFile = Dir + "data.csv";
        static private bool _emergency;
        public MainWindow()
        {
            _emergency = false;
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.Source);
                MessageBox.Show(ex.StackTrace);
            }
            Top = Settings.Default.Top;
            Left = Settings.Default.Left;
            Height = Settings.Default.Height;
            Width = Settings.Default.Width;
            if (Settings.Default.Maximized)
            {
                WindowState = WindowState.Maximized;
            }
            if (!Directory.Exists(Dir))
            {
                Directory.CreateDirectory(Dir);
            }
            if (!File.Exists(DataFile))
                File.Create(DataFile);
            LoadGames(DataFile);
            InitializeGamesList();
            GamesList.SelectedIndex = Settings.Default.lastIndex;
        }

        private void LoadGames(string path)
        {
            try
            { 
                using (StreamReader sr = new StreamReader(path))
                {
                    while (true)
                    {
                        string line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            return;
                        string[] data = line.Split(',');
                        Game game;
                        if (data[0] == "steam")
                        {
                            game = new SteamGame(data.Skip(1).ToArray());
                        }
                        else
                        { 
                            game = new NonSteam(data.Skip(1).ToArray());
                        }
                        _games.Add(game);
                    }
                }
            }
            catch (Exception)
            {
                MessageBoxResult result = MessageBox.Show(
                    "The save file is corrupted. Do you want to create a new one?", "Error",
                    MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (result == MessageBoxResult.Yes)
                {
                    Task.WaitAll(Task.Run(() => File.Delete(DataFile)));
                    Task.WaitAll(Task.Run(() => File.Create(DataFile)));
                }
                else
                {
                    _emergency = true;
                    this.AppClose(this, EventArgs.Empty);
                }
            }
        }

        private void InitializeGamesList()
        {
            GamesList.Items.Clear();
            foreach(Game game in _games)
            {
                GamesList.Items.Add(new ListViewItem { Content = game.tittle });
            }
        }

        private void launchButton_Click(object sender, RoutedEventArgs e)
        {
            if (_games[GamesList.SelectedIndex] is SteamGame sg)
            {
                Process.Start("steam://rungameid/" + sg.appId);
            }
            else
            {
                NonSteam ns = (NonSteam) _games[GamesList.SelectedIndex];
                Process.Start(ns.pathToEXE);
            }
            WindowState = WindowState.Minimized;
        }

        private void addClassicGame_click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = this;
            Add acg = new Add(ref mw);
            acg.Show();
        }
        private void AppClose(object sender, EventArgs e)
        {
            if (!_emergency)
            {
                SaveGamesToFile();
                Settings.Default.lastIndex = GamesList.SelectedIndex;
                Settings.Default.Save();
            }
            Application.Current.Shutdown();
        }

        public void AddNewGame(Game game)
        {
            _games.Add(game);
            InitializeGamesList();
        }

        private void SaveGamesToFile()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(DataFile, false))
                {
                    foreach (Game game in _games)
                    {
                        string lineToWrite = "";
                        if (game is SteamGame)
                        {
                            lineToWrite += "steam,";
                        }
                        else
                        {
                            lineToWrite += "classic,";
                        }

                        lineToWrite += game.tittle;
                        if (game is SteamGame sg)
                        {
                            lineToWrite += ',' + sg.appId;
                        }
                        else
                        {
                            NonSteam ns = (NonSteam) game;
                            lineToWrite += ',' + ns.pathToEXE;
                        }

                        sw.WriteLine(lineToWrite);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void deleteGame_click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to delete " + _games[GamesList.SelectedIndex].tittle + " from the Game Library", "Delete game", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _games.RemoveAt(GamesList.SelectedIndex);
                InitializeGamesList();
            }
        }

        private void editGame_click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = this;
            Edit edit = new Edit(ref mw, _games[GamesList.SelectedIndex], GamesList.SelectedIndex);
            edit.Show();
        }

        public void SwapGameInList(Game newGame, int indexToSwap)
        {
            _games.RemoveAt(indexToSwap);
            _games.Insert(indexToSwap, newGame);
            InitializeGamesList();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Settings.Default.Top = RestoreBounds.Top;
                Settings.Default.Left = RestoreBounds.Left;
                Settings.Default.Height = RestoreBounds.Height;
                Settings.Default.Width = RestoreBounds.Width;
                Settings.Default.Maximized = true;
            }
            else
            {
                Settings.Default.Top = Top;
                Settings.Default.Left = Left;
                Settings.Default.Height = Height;
                Settings.Default.Width = Width;
                Settings.Default.Maximized = false;
            }

            Settings.Default.Save();
        }

        private void gamesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(GamesList.SelectedIndex == -1)
            {
                LaunchBtn.IsEnabled = false;
                EditBtn.IsEnabled = false;
                DeleteBtn.IsEnabled = false;
            }
            else
            {
                LaunchBtn.IsEnabled = true;
                EditBtn.IsEnabled = true;
                DeleteBtn.IsEnabled = true;
            }
        }
    }
}