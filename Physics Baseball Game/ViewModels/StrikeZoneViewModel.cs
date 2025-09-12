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
        private Pitch? _selectedPitch;

        public StrikeZoneViewModel(BallPlayer batter, BallPlayer pitcher)
        {
            _batter = batter ?? throw new ArgumentNullException(nameof(batter));
            _pitcher = pitcher ?? throw new ArgumentNullException(nameof(pitcher));

            if(_pitcher.Repertoire.Count > 0)
            {
                _selectedPitch = _pitcher.Repertoire[0];
            }
            else
            {
                _selectedPitch = new Pitch("Fastball", 95, 2000, 0);
            }

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

        public Pitch? SelectedPitch
        {
            get => _selectedPitch;
            set => SetProperty(ref _selectedPitch, value);
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

        private void Pitch(object? parameter)
        {
            Pitch pitch;
            if (parameter is Pitch p)
            {
                pitch = p;
                SelectedPitch = p;
            }else if (SelectedPitch is Pitch sp)
            {
                pitch = sp;
            }
            else
            {
                return;
            }

            var point = ComputePitchPoint(pitch);
            Pitches.Add(point);
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

        // Basic trajectory model: break influenced by spin rate/axis and scaled by velocity (time in flight)
        // Returns normalized location (x,y) in [-1..1] where inner square [-0.5..0.5] is the strike zone.
        private PitchPoint ComputePitchPoint(Pitch pitch)
        {
            // Convert axis to radians; sign controls glove/arm side vs drop/rise
            double axisRad = pitch.SpinAxisInDegrees * Math.PI / 180.0;

            // Faster pitches have less time to break
            double timeScale = 90.0 / Math.Max(30.0, pitch.VelocityInMph);

            // Simple break model (toy): horizontal ~ sin(axis), vertical ~ cos(axis)
            double hBreak = pitch.SpinRateInRpm * Math.Sin(axisRad) * 0.25 * timeScale;
            double vBreak = -pitch.SpinRateInRpm * Math.Cos(axisRad) * 0.35 * timeScale;

            // Command variability (smaller sigma for stronger arms)
            double arm = Pitcher.Attributes.ArmStrength; // 0..100-ish
            double sigma = 0.12 * Math.Clamp(1.0 - (arm - 80.0) / 80.0, 0.6, 1.3);

            // Final location with noise, clamped to view range
            double x = Clamp(hBreak + Normal(0, sigma), -1, 1);
            double y = Clamp(vBreak + Normal(0, sigma), -1, 1);

            string tooltip = $"{pitch.Name} {pitch.VelocityInMph:0} mph  ({x:0.00},{y:0.00})";
            return new PitchPoint(x, y, tooltip);
        }
    }
}