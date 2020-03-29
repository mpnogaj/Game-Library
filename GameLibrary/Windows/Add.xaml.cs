using System;
using System.Windows;
using GameLibrary.Games;
using Microsoft.Win32;

namespace GameLibrary.Windows
{
    /// <summary>
    /// Interaction logic for AddClassicGame.xaml
    /// </summary>
    public partial class Add : Window
    {
        private readonly MainWindow _mw;
        public Add(ref MainWindow mainWindow)
        {
            InitializeComponent();
            steamRadio.IsChecked = true;
            _mw = mainWindow;
        }

        private void pickFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Filter = "Executeable file(.exe) | *.exe", Title = "Pick your game's .exe file"
            };
            bool? result = openFile.ShowDialog();
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
            bool? steamChecked = steamRadio.IsChecked;
            bool newBool = steamChecked.HasValue && steamChecked.Value;
            if (newBool)
            {
                if (String.IsNullOrEmpty(gameAppID.Text))
                {
                    MessageBox.Show("The Steam AppID cannot be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                game = new SteamGame(new[] { gameName.Text, gameAppID.Text });
            }
            else
            {
                if (string.IsNullOrEmpty(gamePath.Text))
                {
                    MessageBox.Show("Path to the .exe file cannot be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                game = new NonSteam(new[] { gameName.Text, gamePath.Text });
            }
            _mw.AddNewGame(game);
            Close();
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
            Close();
        }
    }
}
