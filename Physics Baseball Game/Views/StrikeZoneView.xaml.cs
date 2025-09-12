using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Physics_Baseball_Game.Views.Utilities;

namespace Physics_Baseball_Game.Views
{
    /// <summary>
    /// Strike zone view with a 5x5 grid. Center 3x3 is the strike zone.
    /// Plot coordinates are normalized to [-1, +1] on both axes.
    /// X: -1 left edge of outer ring, +1 right edge. Y: -1 bottom, +1 top.
    /// The strike zone occupies [-0.5, +0.5] on both axes.
    /// </summary>
    public partial class StrikeZoneView : UserControl
    {
        public StrikeZoneView()
        {
            InitializeComponent();
            if (Pitches is INotifyCollectionChanged incc)
                incc.CollectionChanged += (_, __) => Redraw();
            Loaded += (_, __) => Redraw();
            SizeChanged += (_, __) => Redraw();
        }

        // Commands for buttons (bind from parent ViewModel)
        public static readonly DependencyProperty PitchCommandProperty =
            DependencyProperty.Register(nameof(PitchCommand), typeof(ICommand), typeof(StrikeZoneView));

        public ICommand? PitchCommand
        {
            get => (ICommand?)GetValue(PitchCommandProperty);
            set => SetValue(PitchCommandProperty, value);
        }

        public static readonly DependencyProperty NextBatterCommandProperty =
            DependencyProperty.Register(nameof(NextBatterCommand), typeof(ICommand), typeof(StrikeZoneView));

        public ICommand? NextBatterCommand
        {
            get => (ICommand?)GetValue(NextBatterCommandProperty);
            set => SetValue(NextBatterCommandProperty, value);
        }

        // Routed events for consumers that prefer event handlers
        public static readonly RoutedEvent PitchRequestedEvent =
            EventManager.RegisterRoutedEvent(nameof(PitchRequested), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(StrikeZoneView));

        public event RoutedEventHandler PitchRequested
        {
            add => AddHandler(PitchRequestedEvent, value);
            remove => RemoveHandler(PitchRequestedEvent, value);
        }

        public static readonly RoutedEvent NextBatterRequestedEvent =
            EventManager.RegisterRoutedEvent(nameof(NextBatterRequested), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(StrikeZoneView));

        public event RoutedEventHandler NextBatterRequested
        {
            add => AddHandler(NextBatterRequestedEvent, value);
            remove => RemoveHandler(NextBatterRequestedEvent, value);
        }

        // Pitches collection to draw
        public static readonly DependencyProperty PitchesProperty =
            DependencyProperty.Register(
                nameof(Pitches),
                typeof(ObservableCollection<PitchPoint>),
                typeof(StrikeZoneView),
                new PropertyMetadata(new ObservableCollection<PitchPoint>(), OnPitchesChanged));

        public ObservableCollection<PitchPoint> Pitches
        {
            get => (ObservableCollection<PitchPoint>)GetValue(PitchesProperty);
            set => SetValue(PitchesProperty, value);
        }

        private static void OnPitchesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (StrikeZoneView)d;

            if (e.OldValue is INotifyCollectionChanged oldIncc)
                oldIncc.CollectionChanged -= view.Pitches_CollectionChanged;

            if (e.NewValue is INotifyCollectionChanged newIncc)
                newIncc.CollectionChanged += view.Pitches_CollectionChanged;

            view.Redraw();
        }

        private void Pitches_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Redraw();
        }

        /// <summary>
        /// Add a single pitch to the overlay (also adds to Pitches).
        /// </summary>
        public void AddPitch(PitchPoint pitch)
        {
            Pitches.Add(pitch);
        }

        /// <summary>
        /// Returns 5x5 zone index for the given normalized coordinates.
        /// row/col are 0..4 (row 0 is top, col 0 is left). Center 3x3 (rows 1..3, cols 1..3) is strike zone.
        /// </summary>
        public (int row, int col) GetZoneIndex(double x, double y)
        {
            // Clamp to [-1, 1]
            x = x < -1 ? -1 : (x > 1 ? 1 : x);
            y = y < -1 ? -1 : (y > 1 ? 1 : y);

            // Map [-1..1] to 0..5, then take int 0..4
            int col = (int)((x + 1.0) / 2.0 * 5.0);
            int row = (int)((1.0 - (y + 1.0) / 2.0) * 5.0);

            if (col == 5) col = 4;
            if (row == 5) row = 4;

            return (row, col);
        }

        private void Redraw()
        {
            if (Overlay == null) return;

            Overlay.Children.Clear();

            double size = 500; // Viewbox scales this uniformly
            foreach (var p in Pitches)
            {
                var (left, top) = MapToPixels(p.X, p.Y, size);
                var ellipse = new Ellipse
                {
                    Width = p.Diameter,
                    Height = p.Diameter,
                    Fill = p.Fill ?? Brushes.OrangeRed,
                    Stroke = p.Stroke ?? Brushes.Black,
                    StrokeThickness = p.StrokeThickness
                };

                // Center the ellipse on (left, top)
                Canvas.SetLeft(ellipse, left - ellipse.Width / 2);
                Canvas.SetTop(ellipse, top - ellipse.Height / 2);
                Overlay.Children.Add(ellipse);
                if (!string.IsNullOrWhiteSpace(p.Tooltip))
                    ToolTipService.SetToolTip(ellipse, p.Tooltip);
            }
        }

        private static (double left, double top) MapToPixels(double x, double y, double size)
        {
            // Normalize: x,y in [-1..1]. Convert to 0..1
            double ux = (x + 1.0) / 2.0;
            double uy = 1.0 - (y + 1.0) / 2.0; // invert y to top-down

            return (ux * size, uy * size);
        }

        // Button handlers: execute bound commands if available, also raise routed events for subscribers
        private void PitchButton_Click(object sender, RoutedEventArgs e)
        {
            if (PitchCommand?.CanExecute(null) == true)
                PitchCommand.Execute(null);

            RaiseEvent(new RoutedEventArgs(PitchRequestedEvent));
        }

        private void NextBatterButton_Click(object sender, RoutedEventArgs e)
        {
            if (NextBatterCommand?.CanExecute(null) == true)
                NextBatterCommand.Execute(null);

            RaiseEvent(new RoutedEventArgs(NextBatterRequestedEvent));
        }
    }
}