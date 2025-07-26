using System.Windows;
using System.Windows.Media;

namespace Metaballs.Demo
{
    public partial class MainWindow : Window
    {
        private static readonly int CELL_SIZE = 15;
        private readonly DrawingVisual _drawingVisual;
        private readonly VisualHost _visualHost;

        private DateTime _lastFrameTime = DateTime.Now;

        public MainWindow()
        {
            InitializeComponent();

            _drawingVisual = new DrawingVisual();
            _visualHost = new VisualHost(_drawingVisual);
            Scene.Children.Add(_visualHost);

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


        private void RenderMetaballs()
        {
            int nx = Math.Max(2, (int)(Scene.ActualWidth / CELL_SIZE));
            int ny = Math.Max(2, (int)(Scene.ActualHeight / CELL_SIZE));

            var generatedSegments = MarchingSquares.Generate(Metaball.GeneralFunc, 0, 0, Scene.ActualWidth, Scene.ActualHeight, nx, ny);

            using var dc = _drawingVisual.RenderOpen();
            var pen = new Pen(Brushes.Lime, 3);

            foreach (var (a, b) in generatedSegments)
            {
                dc.DrawLine(pen, a, b);
            }
        }
    }
}