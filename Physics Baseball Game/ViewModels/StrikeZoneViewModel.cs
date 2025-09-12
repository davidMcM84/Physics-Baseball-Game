using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Physics_Baseball_Game.Commands;
using Physics_Baseball_Game.Models;
using Physics_Baseball_Game.ViewModels;
using Physics_Baseball_Game.Views.Utilities;

namespace Physics_Baseball_Game.ViewModels
{
    internal sealed class StrikeZoneViewModel : ViewModelBase
    {
        private BallPlayer _batter;
        private BallPlayer _pitcher;

        public StrikeZoneViewModel(BallPlayer batter, BallPlayer pitcher)
        {
            _batter = batter ?? throw new ArgumentNullException(nameof(batter));
            _pitcher = pitcher ?? throw new ArgumentNullException(nameof(pitcher));

            PitchCommand = new RelayCommand(Pitch);
            NextBatterCommand = new RelayCommand(NextBatter);
        }

        public BallPlayer Batter
        {
            get => _batter;
            set
            {
                if (SetProperty(ref _batter, value))
                {
                    OnPropertyChanged(nameof(BatterName));
                }
            }
        }

        public BallPlayer Pitcher
        {
            get => _pitcher;
            set
            {
                if (SetProperty(ref _pitcher, value))
                {
                    OnPropertyChanged(nameof(PitcherName));
                }
            }
        }

        public string BatterName => $"{Batter.FirstName} {Batter.LastName}";
        public string PitcherName => $"{Pitcher.FirstName} {Pitcher.LastName}";

        // Collection used by StrikeZoneView to plot points
        public ObservableCollection<PitchPoint> Pitches { get; } = new();

        // Commands bound by StrikeZoneView buttons
        public ICommand PitchCommand { get; }
        public ICommand NextBatterCommand { get; }

        // Simple demo pitch generator (normalized coordinates -1..1)
        private readonly Random _rng = new();

        private void Pitch()
        {
            // Box-Muller normal sample around zone center (0,0)
            double x = Clamp(Normal(0, 0.35), -1, 1);
            double y = Clamp(Normal(0, 0.35), -1, 1);

            Pitches.Add(new PitchPoint(x, y, $"Pitch: ({x:0.00},{y:0.00})"));
        }

        private void NextBatter()
        {
            Pitches.Clear();
            NextBatterRequested?.Invoke();
        }

        public event Action? NextBatterRequested;

        private static double Clamp(double v, double min, double max) => v < min ? min : (v > max ? max : v);

        private double Normal(double mean, double stddev)
        {
            // Box-Muller
            double u1 = 1.0 - _rng.NextDouble();
            double u2 = 1.0 - _rng.NextDouble();
            double z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            return mean + z0 * stddev;
        }
    }
}