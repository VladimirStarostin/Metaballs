using System.Windows;

namespace Metaballs
{
    internal static class MarchingSquares
    {
        private static readonly int[][] Edges =
        {
            [],
            [0,3],
            [0,1],
            [1,3],
            [1,2],
            [],
            [0,2],
            [2,3],
            [2,3],
            [0,2],
            [],
            [1,2],
            [1,3],
            [0,1],
            [0,3],
            []
        };

        public static IEnumerable<(Point A, Point B)> Generate(
            Func<double, double, double> f,
            double xmin, double ymin, double xmax, double ymax,
            int nx, int ny)
        {
            double dx = (xmax - xmin) / nx;
            double dy = (ymax - ymin) / ny;

            var prev = new double[nx + 1];
            var curr = new double[nx + 1];

            for (int ix = 0; ix <= nx; ix++)
                prev[ix] = f(xmin + ix * dx, ymin);

            for (int iy = 0; iy < ny; iy++)
            {
                double y0 = ymin + iy * dy;
                double y1 = y0 + dy;

                for (int ix = 0; ix <= nx; ix++)
                    curr[ix] = f(xmin + ix * dx, y1);

                for (int ix = 0; ix < nx; ix++)
                {
                    double x0 = xmin + ix * dx;
                    double x1 = x0 + dx;

                    double v0 = prev[ix];
                    double v1 = prev[ix + 1];
                    double v2 = curr[ix + 1];
                    double v3 = curr[ix];

                    int mask =
                        (v0 > 0 ? 1 : 0) |
                        (v1 > 0 ? 2 : 0) |
                        (v2 > 0 ? 4 : 0) |
                        (v3 > 0 ? 8 : 0);

                    var getEdges = () =>
                    {
                        if (mask == 5)
                        {
                            bool centerPositive = f(x0 + dx / 2, y0 + dy / 2) > 0;
                            return centerPositive ? [0, 1] : [2, 3];
                        }
                        else if (mask == 10)
                        {
                            bool centerPositive = f(x0 + dx / 2, y0 + dy / 2) > 0;
                            return centerPositive ? [1, 2] : [0, 3];
                        }
                        else
                            return Edges[mask];
                    };

                    var edges = getEdges();
                    if (edges.Length.Equals(0))
                        continue;

                    var firstEdge = edges[0];
                    var secondEdge = edges[1];

                    var calculatePoint = (int edgeNumber) =>
                        {
                            switch (edgeNumber)
                            {
                                case 0:
                                    return Lerp(x0, y0, v0, x1, y0, v1);
                                case 1:
                                    return Lerp(x1, y0, v1, x1, y1, v2);
                                case 2:
                                    return Lerp(x1, y1, v2, x0, y1, v3);
                                case 3:
                                    return Lerp(x0, y0, v0, x0, y1, v3);
                                default: throw new InvalidOperationException();
                            }
                        };
                    var pointA = calculatePoint(firstEdge);
                    var pointB = calculatePoint(secondEdge);
                    yield return (pointA, pointB);
                }

                (prev, curr) = (curr, prev);
            }
        }

        private static Point Lerp(double x0, double y0, double v0, double x1, double y1, double v1)
        {
            double t = v0 / (v0 - v1);
            return new Point(x0 + t * (x1 - x0), y0 + t * (y1 - y0));
        }
    }
}
