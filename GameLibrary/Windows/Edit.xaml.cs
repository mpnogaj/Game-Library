using System.Windows;
using GameLibrary.Games;
using Microsoft.Win32;

namespace GameLibrary.Windows
{
    /// <summary>
    /// Interaction logic for Edit.xaml
    /// </summary>
    public partial class Edit
    {
        private readonly MainWindow _mw;
        private readonly int _index;
        public Edit(ref MainWindow mainWindow, Game game, int indexToSwap)
        {
            InitializeComponent();
            _mw = mainWindow;
            _index = indexToSwap;
            if(game is SteamGame sg)
            {
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
            OpenFileDialog openFile = new OpenFileDialog
            {
                Filter = "Executeable file(.exe) | *.exe", Title = "Pick your game's .exe file"
            };
            bool? result = openFile.ShowDialog();
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
            bool? steamChecked = steamRadio.IsChecked;
            bool newBool = steamChecked.HasValue && steamChecked.Value;
            if (newBool)
            {
                SteamGame sg = new SteamGame(new[] { gameNewName.Text, gameNewAppID.Text });
                _mw.SwapGameInList(sg, _index);
            }
            else
            {
                NonSteam ns = new NonSteam(new[] { gameNewName.Text, gameNewPath.Text });
                _mw.SwapGameInList(ns, _index);
            }
            Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
