using System.Windows;
using System.Windows.Controls;

namespace Metaballs.Demo
{
    internal static class Metaball
    {
        private class Circle
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Dx { get; set; }
            public double Dy { get; set; }
            public double R { get; set;  }

            public void OutOfBoundsHandler(double sceneWidth, double sceneHeight)
            {
                double maxX = sceneWidth - R;
                double maxY = sceneHeight - R;

                if (X < R)
                    X = R;
                else if (X > maxX)
                    X = maxX;

                if (Y < R)
                    Y = R;
                else if (Y > maxY)
                    Y = maxY;
            }

            public double ApproximatedFunc(double x, double y)
            {
                var dx = x - X;
                var dy = y - Y;
                var r2 = dx * dx + dy * dy;
                return R * R / (r2 + R);
            }

            public void Move(Canvas scene, double deltaTime)
            {
                X += Dx * deltaTime / 75;
                Y += Dy * deltaTime / 75;
            }
        }

        private static class ReflectionsHandler
        {
            private static readonly HashSet<Circle> circlesToBounce = new HashSet<Circle>();

            internal static void ReflectLeftEdge(double height) =>
                ReflectEdge(height, 0, Axis.Vertical, c => c.Dx < 0, c => c.Dx *= -1);

            internal static void ReflectRightEdge(double height, double width) =>
                ReflectEdge(height, width, Axis.Vertical, c => c.Dx > 0, c => c.Dx *= -1);

            internal static void ReflectTopEdge(double width) =>
                ReflectEdge(width, 0, Axis.Horizontal, c => c.Dy < 0, c => c.Dy *= -1);

            internal static void ReflectBottomEdge(double height, double width) =>
                ReflectEdge(width, height, Axis.Horizontal, c => c.Dy > 0, c => c.Dy *= -1);

            private enum Axis { Horizontal, Vertical }

            private static void ReflectEdge(
                double length, double fixedCoord,
                Axis axis,
                Func<Circle, bool> velocityCondition,
                Action<Circle> reflectAction)
            {
                var minDiameter = GetMinDiameter();
                if (!minDiameter.HasValue) return;

                circlesToBounce.Clear();
                var step = length / (2 * minDiameter.Value);

                for (int i = 0; i < 2 * minDiameter.Value; i++)
                {
                    double x = axis == Axis.Vertical ? fixedCoord : i * step;
                    double y = axis == Axis.Vertical ? i * step : fixedCoord;

                    if (GeneralFunc(x, y) >= 0)
                    {
                        var circleToBounce = circles.Where(velocityCondition)
                                                    .MinBy(c => Math.Abs(c.X - x) + Math.Abs(c.Y - y));
                        if (circleToBounce != null)
                            circlesToBounce.Add(circleToBounce);
                    }
                }

                foreach (var circle in circlesToBounce)
                    reflectAction(circle);
            }
        }

        // I wished to make the app interactive
        // to have an ability to add/remove/change size of the circles.
        private static List<Circle> circles =
          [ new Circle { X = 300, Y = 300, Dx = -1.5, Dy = 2, R = 80 },
            new Circle { X = 400, Y = 300, Dx = 2.1, Dy = 2.9, R = 60 },
            new Circle { X = 200, Y = 200, Dx = 1.15, Dy = 3.1, R = 45 },
            new Circle { X = 600, Y = 120, Dx = -1.5, Dy = -5, R = 135 },
            new Circle { X = 500, Y = 400, Dx = -1.7, Dy = 1.25, R = 120 },
            new Circle { X = 400, Y = 300, Dx = 2.5, Dy = 2.15, R = 75 },
            new Circle { X = 800, Y = 300, Dx = -2.5, Dy = 4.15, R = 175 },
            new Circle { X = 800, Y = 300, Dx = 1.7, Dy = -2.3, R = 115 },
            new Circle { X = 800, Y = 300, Dx = -2.1, Dy = 4.3, R = 190 },
            new Circle { X = 800, Y = 300, Dx = 2.65, Dy = -1.3, R = 65 } ];

        // That's why some unneccessay methods are introduced:
        internal static double? GetMinDiameter() => circles.MinBy(c => c.R)?.R * 2;

        // Isoline value to draw a line:
        private static double ISO = 2.5;

        public static double GeneralFunc(double x, double y)
            => circles.Sum(circle => circle.ApproximatedFunc(x, y)) - ISO;

        public static void MoveAll(Canvas scene, double deltaTime)
        {
            foreach (var circle in circles) { circle.Move(scene, deltaTime); }

            ReflectionsHandler.ReflectLeftEdge(scene.ActualHeight);
            ReflectionsHandler.ReflectRightEdge(scene.ActualHeight, scene.ActualWidth);
            ReflectionsHandler.ReflectTopEdge(scene.ActualWidth);
            ReflectionsHandler.ReflectBottomEdge(scene.ActualHeight, scene.ActualWidth);
        }

        public static void OnSizeChanged (object sender, SizeChangedEventArgs _)
        {
            var window = sender as MainWindow;
            if (window is null) return;
            foreach (var circle in circles)
            {
                circle.OutOfBoundsHandler(window.Scene.ActualWidth, window.Scene.Height);
            }
        }
    }
}