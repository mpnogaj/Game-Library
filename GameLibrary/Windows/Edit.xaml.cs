using System;
using System.Windows;
using Microsoft.Win32;
using GameLibrary.Games;

namespace GameLibrary.Windows
{
    /// <summary>
    /// Interaction logic for Edit.xaml
    /// </summary>
    public partial class Edit : Window
    {
        private MainWindow mw;
        private int index;
        public Edit(ref MainWindow mainWindow, Game game, int indexToSwap)
        {
            InitializeComponent();
            mw = mainWindow;
            index = indexToSwap;
            if(game is SteamGame)
            {
                SteamGame sg = (SteamGame)game;
                steamRadio.IsChecked = true;
                gameNewAppID.Text = sg.appId;
                gameNewName.Text = sg.tittle;
            }
            else
            {
                NonSteam ns = (NonSteam)game;
                classicRadio.IsChecked = true;
                gameNewPath.Text = ns.pathToEXE;
                gameNewName.Text = ns.tittle;
            }
        }

        private void pickFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Executeable file(.exe) | *.exe";
            openFile.Title = "Pick your game's .exe file";
            Nullable<bool> result = openFile.ShowDialog();
            if (result == true)
            {
                gameNewPath.Text = openFile.FileName;
            }
        }

        private void classicRadio_Checked(object sender, RoutedEventArgs e)
        {
            steamGrid.Visibility = Visibility.Collapsed;
            classicGrid.Visibility = Visibility.Visible;
        }

        private void steamRadio_Checked(object sender, RoutedEventArgs e)
        {
            steamGrid.Visibility = Visibility.Visible;
            classicGrid.Visibility = Visibility.Collapsed;
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            Nullable<bool> steamChecked = steamRadio.IsChecked;
            bool newBool = steamChecked.HasValue ? steamChecked.Value : false;
            if (newBool)
            {
                SteamGame sg = new SteamGame(new string[] { gameNewName.Text, gameNewAppID.Text });
                mw.SwapGameInList(sg, index);
            }
            else
            {
                NonSteam ns = new NonSteam(new string[] { gameNewName.Text, gameNewPath.Text });
                mw.SwapGameInList(ns, index);
            }
            this.Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
