using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Metaballs.Demo
{
    public partial class MainWindow : Window
    {
        private static readonly int CELL_SIZE = 10;

        private readonly Path _contours = new()
        {
            StrokeThickness = 3,
            Stroke           = Brushes.Lime,
            Fill             = Brushes.Transparent
        };

        private DateTime _lastFrameTime = DateTime.Now;

        public MainWindow()
        {
            InitializeComponent();
            Scene.Children.Add(_contours);
            this.SizeChanged += Metaball.OnSizeChanged;
            CompositionTarget.Rendering += OnRenderFrame;
        }


        private void OnRenderFrame(object? _, EventArgs __)
        {
            var now = DateTime.Now;
            double deltaTime = (DateTime.Now - _lastFrameTime).TotalMilliseconds;
            _lastFrameTime = now;

            Metaball.MoveAll(Scene, deltaTime);

            RenderMetaballs();
        }

        private void RenderMetaballs()
        {
            int nx = Math.Max(2, (int)(Scene.ActualWidth / CELL_SIZE));
            int ny = Math.Max(2, (int)(Scene.ActualHeight / CELL_SIZE));

            var generatedSegments = MarchingSquares.Generate(Metaball.GeneralFunc, 0, 0, Scene.ActualWidth, Scene.ActualHeight, nx, ny);

            var geo = new StreamGeometry();
            using var ctx = geo.Open();
            foreach (var (a, b) in generatedSegments)
            {
                ctx.BeginFigure(a, false, false);
                ctx.LineTo(b, true, false);
            }
            geo.Freeze();
            _contours.Data = geo;
        }
    }
}