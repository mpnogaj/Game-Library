using System;
using System.Windows;
using Microsoft.Win32;
using GameLibrary.Games;

namespace GameLibrary.Windows
{
    /// <summary>
    /// Interaction logic for AddClassicGame.xaml
    /// </summary>
    public partial class Add : Window
    {
        private MainWindow mw;
        public Add(ref MainWindow mainWindow)
        {
            InitializeComponent();
            steamRadio.IsChecked = true;
            mw = mainWindow;
        }

        private void pickFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Executeable file(.exe) | *.exe";
            openFile.Title = "Pick your game's .exe file";
            Nullable<bool> result = openFile.ShowDialog();
            if (result == true)
            {
                gamePath.Text = openFile.FileName;
            }
        }

        private void addGameBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(gameName.Text))
            {
                MessageBox.Show("Game name cannot be empty!","Error",MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Game game;
            Nullable<bool> steamChecked = steamRadio.IsChecked;
            bool newBool = steamChecked.HasValue ? steamChecked.Value : false;
            if (newBool)
            {
                if (String.IsNullOrEmpty(gameAppID.Text))
                {
                    MessageBox.Show("The Steam AppID cannot be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                game = new SteamGame(new string[] { gameName.Text, gameAppID.Text });
            }
            else
            {
                if (string.IsNullOrEmpty(gamePath.Text))
                {
                    MessageBox.Show("Path to the .exe file cannot be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                game = new NonSteam(new string[] { gameName.Text, gamePath.Text });
            }
            mw.addNewGame(game);
            this.Close();
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

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
