using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using GameLibrary.Properties;
using GameLibrary.Games;
using GameLibrary.Windows;

namespace GameLibrary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Game> games = new List<Game>();  
        static readonly string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Game Library\";
        static readonly string dataFile = dir + "data.csv";
        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.Source);
                MessageBox.Show(ex.InnerException.Message);
                MessageBox.Show(ex.StackTrace);
            }
            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;
            if (Properties.Settings.Default.Maximized)
            {
                WindowState = WindowState.Maximized;
            }
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(dataFile))
                File.Create(dataFile);
            loadGames(dataFile);
            initializeGamesList();
            gamesList.SelectedIndex = Settings.Default.lastIndex;
        }

        private void loadGames(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                while (sr != null)
                {
                    string[] data = new string[4];
                    string line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        return;
                    data = line.Split(',');
                    Game game;
                    if(data[0] == "steam")
                    {
                        game = new SteamGame(data.Skip(1).ToArray());
                    }
                    else
                    {
                        game = new NonSteam(data.Skip(1).ToArray());
                    }
                    games.Add(game);
                }
            }
        }

        private void initializeGamesList()
        {
            gamesList.Items.Clear();
            foreach(Game game in games)
            {
                gamesList.Items.Add(new ListViewItem() { Content = game.tittle });
            }
        }

        private void launchButton_Click(object sender, RoutedEventArgs e)
        {
            if (games[gamesList.SelectedIndex] is SteamGame)
            {
                SteamGame sg = (SteamGame) games[gamesList.SelectedIndex];
                Process.Start("steam://rungameid/" + sg.appId);
            }
            else
            {
                NonSteam ns = (NonSteam) games[gamesList.SelectedIndex];
                Process.Start(ns.pathToEXE);
            }
            this.WindowState = WindowState.Minimized;
        }

        private void addClassicGame_click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = this;
            Add acg = new Add(ref mw);
            acg.Show();
        }
        private void AppClose(object sender, EventArgs e)
        {
            saveGamesToFile();
            Settings.Default.lastIndex = gamesList.SelectedIndex;
            Settings.Default.Save();
            Application.Current.Shutdown();
        }

        public void addNewGame(Game game)
        {
            games.Add(game);
            initializeGamesList();
        }

        private void saveGamesToFile()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(dataFile, false))
                {
                    foreach (Game game in games)
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
                        if (game is SteamGame)
                        {
                            SteamGame sg = (SteamGame)game;
                            lineToWrite += ',' + sg.appId;
                        }
                        else
                        {
                            NonSteam ns = (NonSteam)game;
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
            if(MessageBox.Show("Are you sure you want to delete " + games[gamesList.SelectedIndex].tittle + " from the Game Library", "Delete game", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                games.RemoveAt(gamesList.SelectedIndex);
                initializeGamesList();
            }
        }

        private void editGame_click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = this;
            Edit edit = new Edit(ref mw, games[gamesList.SelectedIndex], gamesList.SelectedIndex);
            edit.Show();
        }

        public void SwapGameInList(Game newGame, int indexToSwap)
        {
            games.RemoveAt(indexToSwap);
            games.Insert(indexToSwap, newGame);
            initializeGamesList();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Properties.Settings.Default.Top = RestoreBounds.Top;
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Height = RestoreBounds.Height;
                Properties.Settings.Default.Width = RestoreBounds.Width;
                Properties.Settings.Default.Maximized = true;
            }
            else
            {
                Properties.Settings.Default.Top = this.Top;
                Properties.Settings.Default.Left = this.Left;
                Properties.Settings.Default.Height = this.Height;
                Properties.Settings.Default.Width = this.Width;
                Properties.Settings.Default.Maximized = false;
            }

            Properties.Settings.Default.Save();
        }

        private void gamesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(gamesList.SelectedIndex == -1)
            {
                launchBtn.IsEnabled = false;
                editBtn.IsEnabled = false;
                deleteBtn.IsEnabled = false;
            }
            else
            {
                launchBtn.IsEnabled = true;
                editBtn.IsEnabled = true;
                deleteBtn.IsEnabled = true;
            }
        }
    }
}