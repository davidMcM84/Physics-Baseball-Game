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
    public partial class StrikeZoneView : UserControl
    {
        public StrikeZoneView()
        {
            InitializeComponent();
            Loaded += (_, __) =>
            {
                UpdateZoneSize();
                Redraw();
            };
            SizeChanged += (_, __) => Redraw();
        }

        // “Inch” dimensions for inner strike zone (center 3x3).
        public static readonly DependencyProperty PlateWidthInchesProperty =
            DependencyProperty.Register(nameof(PlateWidthInches), typeof(double), typeof(StrikeZoneView),
                new PropertyMetadata(17.0, OnZoneDimChanged));

        public static readonly DependencyProperty StrikeZoneHeightInchesProperty =
            DependencyProperty.Register(nameof(StrikeZoneHeightInches), typeof(double), typeof(StrikeZoneView),
                new PropertyMetadata(24.0, OnZoneDimChanged));

        public double PlateWidthInches
        {
            get => (double)GetValue(PlateWidthInchesProperty);
            set => SetValue(PlateWidthInchesProperty, value);
        }

        public double StrikeZoneHeightInches
        {
            get => (double)GetValue(StrikeZoneHeightInchesProperty);
            set => SetValue(StrikeZoneHeightInchesProperty, value);
        }

        private static void OnZoneDimChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = (StrikeZoneView)d;
            v.UpdateZoneSize();
            v.Redraw();
        }

        private void UpdateZoneSize()
        {
            if (ZoneRoot == null) return;

            const double dipsPerInch = 96.0;

            // Inner strike zone inches (defaults): 17" wide x 24" high
            double innerWidthInches = PlateWidthInches;          // 17 by default
            double innerHeightInches = StrikeZoneHeightInches;   // 24 by default

            // Each grid cell is 1/3 of the inner zone size; total grid is 5 cells per axis
            double totalWidthInches = innerWidthInches * (5.0 / 3.0);
            double totalHeightInches = innerHeightInches * (5.0 / 3.0);

            // Convert inches -> DIPs for layout; LayoutTransform scales DIPs to 2 px/inch
            ZoneRoot.Width = totalWidthInches * dipsPerInch;
            ZoneRoot.Height = totalHeightInches * dipsPerInch;
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

        // Routed events (unchanged)
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

        private void Pitches_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => Redraw();

        public void AddPitch(PitchPoint pitch) => Pitches.Add(pitch);

        public (int row, int col) GetZoneIndex(double x, double y)
        {
            // x,y normalized to [-1,1]. Inner zone is [-0.5,0.5].
            x = x < -1 ? -1 : (x > 1 ? 1 : x);
            y = y < -1 ? -1 : (y > 1 ? 1 : y);

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

            // Use actual zone size instead of a constant.
            double width = ZoneRoot?.ActualWidth > 0 ? ZoneRoot.ActualWidth : 0;
            double height = ZoneRoot?.ActualHeight > 0 ? ZoneRoot.ActualHeight : 0;
            if (width <= 0 || height <= 0) return;

            foreach (var p in Pitches)
            {
                var (left, top) = MapToPixels(p.X, p.Y, width, height);
                var ellipse = new Ellipse
                {
                    Width = p.Diameter,
                    Height = p.Diameter,
                    Fill = p.Fill ?? Brushes.OrangeRed,
                    Stroke = p.Stroke ?? Brushes.Black,
                    StrokeThickness = p.StrokeThickness
                };

                Canvas.SetLeft(ellipse, left - ellipse.Width / 2);
                Canvas.SetTop(ellipse, top - ellipse.Height / 2);
                Overlay.Children.Add(ellipse);
                if (!string.IsNullOrWhiteSpace(p.Tooltip))
                    ToolTipService.SetToolTip(ellipse, p.Tooltip);
            }
        }

        private static (double left, double top) MapToPixels(double x, double y, double width, double height)
        {
            // Normalize: x,y in [-1..1]. Convert to 0..1 per axis.
            double ux = (x + 1.0) / 2.0;
            double uy = 1.0 - (y + 1.0) / 2.0; // invert y to top-down
            return (ux * width, uy * height);
        }
    }
}