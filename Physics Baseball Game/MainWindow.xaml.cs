using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Physics_Baseball_Game.Models;
using Physics_Baseball_Game.ViewModels;
using Physics_Baseball_Game.Views;

namespace Physics_Baseball_Game
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            // Temporary: create a sample player. Replace with repository lookup later.
            var player = new BasicPlayer(
                firstName: "Alex",
                lastName: "Carter",
                age: 24,
                position: PlayerPosition.CenterField,
                role: PlayerRole.Batter,
                batHand: BatHand.Right,
                throwHand: ThrowHand.Right,
                attributes: new PhysicalAttributes(
                    HeightInInches: 74,
                    WeightInPounds: 205,
                    SprintSpeed: 28.4,
                    Acceleration: 7.2,
                    BatSpeed: 72.5,
                    ArmStrength: 88.1,
                    ReactionTime: 0.18),
                pitches: new List<Pitch>());

            var pitcher = new BasicPlayer(
                firstName: "Jordan",
                lastName: "Reed",
                age: 27,
                position: PlayerPosition.Pitcher,
                role: PlayerRole.StartingPitcher,
                batHand: BatHand.Left,
                throwHand: ThrowHand.Left,
                attributes: new PhysicalAttributes(
                    HeightInInches: 76,
                    WeightInPounds: 220,
                    SprintSpeed: 24.0,
                    Acceleration: 5.5,
                    BatSpeed: 65.0,
                    ArmStrength: 95.0,
                    ReactionTime: 0.20),
                pitches: new List<Pitch>
                {
                    new Pitch("Fastball", 95, 2.5, 0.0),
                    new Pitch("Curveball", 78, 1.5, -5.0),
                    new Pitch("Slider", 85, 2.0, -2.5)
                });

            var vm = new PlayerProfileViewModel(player);
            var view = new PlayerProfileView { DataContext = vm };

            vm.CloseRequested += () => ShowStartMenu();
            vm.Saved += _ =>
            {
                // Persist later (repository call)
                ShowStartMenu();
            };

            ShowView(view);
        }

        private void ShowView(UserControl view)
        {
            StartScreenRoot.Visibility = Visibility.Collapsed;
            PageHost.Content = view;
            PageHost.Visibility = Visibility.Visible;
        }

        private void ShowStartMenu()
        {
            PageHost.Visibility = Visibility.Collapsed;
            PageHost.Content = null;
            StartScreenRoot.Visibility = Visibility.Visible;
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Options clicked (placeholder)");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (PageHost.Visibility == Visibility.Visible)
                {
                    // Treat Esc as cancel/back when in a page
                    ShowStartMenu();
                }
                else
                {
                    Close();
                }
            }
        }
    }
}