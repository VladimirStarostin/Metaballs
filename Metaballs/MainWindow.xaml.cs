using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Metaballs.Demo
{
    public partial class MainWindow : Window
    {
        private static readonly int CELL_SIZE = 10;
        private readonly DrawingVisual _drawingVisual;
        private readonly VisualHost _visualHost;

        private DateTime _lastFrameTime = DateTime.Now;

        public MainWindow()
        {
            InitializeComponent();
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.Default;
            RenderOptions.SetEdgeMode(Scene, EdgeMode.Aliased);
            RenderOptions.SetBitmapScalingMode(Scene, BitmapScalingMode.LowQuality);
            RenderOptions.SetClearTypeHint(Scene, ClearTypeHint.Auto);

            this.MouseDown += OnClick;
            _drawingVisual = new DrawingVisual();
            Scene.Children.Add(new VisualHost(_drawingVisual));

            this.SizeChanged += Metaball.OnSizeChanged;
            CompositionTarget.Rendering += OnRenderFrame;
        }

        private void OnRenderFrame(object? _, EventArgs __)
        {
            var now = DateTime.Now;
            double deltaTime = (now - _lastFrameTime).TotalMilliseconds;
            _lastFrameTime = now;

            Metaball.MoveAll(Scene, deltaTime);
            RenderMetaballs();

            Scene.InvalidateVisual();
        }

        private double _hue = 0.0;

        private static Color FromHsv(double h, double s, double v)
        {
            int hi = (int)(h / 60) % 6;
            double f = h / 60 - Math.Floor(h / 60);

            v = v * 255;
            byte p = (byte)(v * (1 - s));
            byte q = (byte)(v * (1 - f * s));
            byte t = (byte)(v * (1 - (1 - f) * s));
            byte vb = (byte)v;

            return hi switch
            {
                0 => Color.FromRgb(vb, t, p),
                1 => Color.FromRgb(q, vb, p),
                2 => Color.FromRgb(p, vb, t),
                3 => Color.FromRgb(p, q, vb),
                4 => Color.FromRgb(t, p, vb),
                5 => Color.FromRgb(vb, p, q),
                _ => Colors.White
            };
        }

        private bool IsHue = false;

        private void OnClick(object _, EventArgs __)
        {
            IsHue = !IsHue;
        }

        private Color rainbowColor => FromHsv(_hue, 1.0, 1.0);

        private void RenderMetaballs()
        {
            int nx = Math.Max(2, (int)(Scene.ActualWidth / CELL_SIZE));
            int ny = Math.Max(2, (int)(Scene.ActualHeight / CELL_SIZE));

            var generatedSegments = MarchingSquares.Generate(Metaball.GeneralFunc, 0, 0, Scene.ActualWidth, Scene.ActualHeight, nx, ny);

            using var dc = _drawingVisual.RenderOpen();

            if (IsHue)
            {
                _hue += 1.0;
                if (_hue > 360.0) _hue = 0.0;
            }

            var pen = new Pen(new SolidColorBrush(rainbowColor), 3); ;

            foreach (var (a, b) in generatedSegments)
            {
                dc.DrawLine(pen, a, b);
            }
        }
    }
}