using System.Windows.Media;

namespace Physics_Baseball_Game.Views.Utilities
{
    /// <summary>
    /// Represents a plotted pitch in normalized coordinates [-1..1] for both axes.
    /// The strike zone is the inner square [-0.5..0.5] on both axes.
    /// </summary>
    public sealed class PitchPoint
    {
        public PitchPoint(double x, double y, string? tooltip = null)
        {
            X = x;
            Y = y;
            Tooltip = tooltip ?? string.Empty;
        }

        public double X { get; set; } // -1..1 left->right
        public double Y { get; set; } // -1..1 bottom->top

        public Brush? Fill { get; set; }
        public Brush? Stroke { get; set; }
        public double StrokeThickness { get; set; } = 1.0;
        public double Diameter { get; set; } = 10.0;

        public string Tooltip { get; set; }
    }
}