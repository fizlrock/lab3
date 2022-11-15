namespace ImageProcessor;

public struct Point
{
    /// <summary>
    /// Структура для хранения координат двумерной точки
    /// </summary>
    public int X;
    public int Y;
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
    public Point(double x, double y)
    {
        X = (int)x;
        Y = (int)y;
    }
    public override string ToString()
    {
        return String.Format("({0}:{1})", X, Y);
    }

    public Point[] Get4AdjancentPoint()
    {
        Point[] points = new Point[4];
        points[0] = new Point(X, Y + 1);
        points[1] = new Point(X, Y - 1);
        points[2] = new Point(X + 1, Y);
        points[3] = new Point(X - 1, Y);
        return points;
    }

    public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);

}
