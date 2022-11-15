using SkiaSharp;

namespace ImageProcessor;


public class Image
{
    private int width, height;
    private SKBitmap bitmap;
    private SKColor[] pixmap;

    private static float t_step = 0.01f;

    public int Height() => height;
    public int Width() => width;

    public Image(int width, int height)
    {
        this.width = width;
        this.height = height;
        bitmap = new SKBitmap(width, height, SKColorType.Argb4444, SKAlphaType.Premul);

        using (SKCanvas bitmapCanvas = new SKCanvas(bitmap))
        {
            bitmapCanvas.Clear(new SKColor(255, 255, 255, 255));
        }

        pixmap = bitmap.Pixels;
    }

    private Image(SKBitmap somebitmap)
    {
        this.width = somebitmap.Width;
        this.height = somebitmap.Height;

        bitmap = somebitmap;
    }


    private void CheckAndSet(int x, int y, SKColor color)
    {
        if ((x >= 0) && (x < width) && (y >= 0) && (y < height)) pixmap[x + y * width] = color;
    }

    private SKColor GetPixel(int x, int y)
    {
        if (CheckPoint(new Point(x, y)))
        {
            return pixmap[x + y * width];
        }
        else return SKColor.Empty;
    }



    public void DrawCircle(Point point, int radius, SKColor color)
    {
        int x0, y0;
        x0 = point.X;
        y0 = point.Y;

        int x = 0;
        int y = radius;
        int radiusError = 3-2*radius;
        while (y >= x)
        {
            CheckAndSet(x + x0, y + y0, color);
            CheckAndSet(y + x0, x + y0, color);
            CheckAndSet(-x + x0, y + y0, color);
            CheckAndSet(-y + x0, x + y0, color);
            CheckAndSet(-x + x0, -y + y0, color);
            CheckAndSet(-y + x0, -x + y0, color);
            CheckAndSet(x + x0, -y + y0, color);
            CheckAndSet(y + x0, -x + y0, color);
            
            if (radiusError < 0)
            {
                radiusError +=4*x + 6 ;
            }
            else
            {
                radiusError += 4*(x-y)+10;
                y--;
            }

            x++;
        }

        bitmap.Pixels = pixmap;
    }

    public void BezierLine(Point a, Point b, Point c, Point d, SKColor color)
    {
        float t = t_step;
        double nx, ny, px, py;
        px = a.X;
        py = a.Y;

        while (t < 1)
        {
            nx = Math.Pow(1 - t, 3) * a.X + Math.Pow(1 - t, 2) * 3 * t * b.X + (1 - t) * 3 * t * t * c.X + t * t * t * d.X;
            ny = Math.Pow(1 - t, 3) * a.Y + Math.Pow(1 - t, 2) * 3 * t * b.Y + (1 - t) * 3 * t * t * c.Y + t * t * t * d.Y;
            DrawLine(new Point(px, py), new Point(nx, ny), color);
            px = nx;
            py = ny;
            t += t_step;
        }
        DrawLine(new Point(px, py), d, color);
    }

    public void BezierLine(Point a, Point b, Point c, SKColor color)
    {
        float t = t_step;

        double nx, ny, px, py;
        px = a.X;
        py = a.Y;

        while (t < 1)
        {
            nx = Math.Pow(1 - t, 2) * a.X + 2 * t * (1 - t) * b.X + t * t * c.X;
            ny = Math.Pow(1 - t, 2) * a.Y + 2 * t * (1 - t) * b.Y + t * t * c.Y;

            DrawLine(new Point(px, py), new Point(nx, ny), color);

            t += t_step;
            px = nx;
            py = ny;
        }
        DrawLine(new Point((int)px, (int)py), c, color);
    }


    public void DrawLine(Point first, Point second, SKColor color)
    {
        int x0, x1, y0, y1;
        x0 = first.X;
        y0 = first.Y;
        x1 = second.X;
        y1 = second.Y;
        var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0); // Проверяем рост отрезка по оси икс и по оси игрек
                                                           // Отражаем линию по диагонали, если угол наклона слишком большой
        if (steep)
        {
            Swap(ref x0, ref y0); // Перетасовка координат вынесена в отдельную функцию для красоты
            Swap(ref x1, ref y1);
        }
        // Если линия растёт не слева направо, то меняем начало и конец отрезка местами
        if (x0 > x1)
        {
            Swap(ref x0, ref x1);
            Swap(ref y0, ref y1);
        }
        int dx = x1 - x0;
        int dy = Math.Abs(y1 - y0);
        int error = dx / 2; // Здесь используется оптимизация с умножением на dx, чтобы избавиться от лишних дробей
        int ystep = (y0 < y1) ? 1 : -1; // Выбираем направление роста координаты y
        int y = y0;
        for (int x = x0; x <= x1; x++)
        {
            CheckAndSet(steep ? y : x, steep ? x : y, color);
            error -= dy;
            if (error < 0)
            {
                y += ystep;
                error += dx;
            }
        }

        bitmap.Pixels = pixmap;
    }

    private void Swap(ref int a, ref int b)
    {
        int c = a;
        a = b;
        b = c;
    }


    public void MarkDots(params Point[] list)
    {
        foreach (Point p in list) DrawCircle(p, 5, new SKColor(0, 50, 255));
    }

    public void SaveToJpeg(string path, string name, int quality)
    {
        using (var data = bitmap.Encode(SKEncodedImageFormat.Jpeg, quality))
        using (var stream = File.OpenWrite(Path.Combine(path, name + ".jpg")))
            data.SaveTo(stream);
    }

    public void FillCoroed(Point p, SKColor color)
    {
        Stack<Point> stack = new Stack<Point>();
        stack.Push(p);
        var base_color = bitmap.GetPixel(p.X, p.Y);

        while (stack.Count != 0)
        {
            p = stack.Pop();
            CheckAndSet(p.X, p.Y, color);
            var neighbours = p.Get4AdjancentPoint();
            foreach (Point n in neighbours)
            {
                if (CheckPoint(n) && GetPixel(n.X, n.Y).Equals(base_color))
                {
                    stack.Push(n);
                }
            }

        }
    }

    public bool CheckPoint(Point p) => (p.X >= 0) && (p.X < width) && (p.Y >= 0) && (p.Y < height);

    private void SetPixel(Point p, FillBrush brush)
    {
        CheckAndSet(p.X, p.Y, brush.GetColor(p.X, p.Y));
    }


    public void Fill(Point p, FillBrush brush)
    {

        Stack<Point> stack = new Stack<Point>();
        stack.Push(p);
        var base_color = GetPixel(p.X, p.Y);
        int x, y, rx, lx;

        while (stack.Count != 0)
        {
            p = stack.Pop();
            x = p.X;
            rx = x;
            lx = x;
            y = p.Y;
            SetPixel(p, brush);

            while (GetPixel(rx + 1, y).Equals(base_color) && (rx + 1 < width))
            {
                rx++;
                SetPixel(new Point(rx, y), brush);
            }
            while (GetPixel(lx - 1, y).Equals(base_color) && (lx - 1 >= 0))
            {
                lx--;
                SetPixel(new Point(lx, y), brush);
            }

            bool up_flag = true;
            bool down_flag = true;

            for (int i = rx; i > lx; i--)
            {
                bool up = GetPixel(i, y + 1).Equals(base_color);
                bool down = GetPixel(i, y - 1).Equals(base_color);

                if (up && (up_flag == true))
                {
                    up_flag = false;
                    stack.Push(new Point(i, y + 1));
                }
                else if (!up_flag && !up) up_flag = true;

                if (down && (down_flag == true))
                {
                    down_flag = false;
                    stack.Push(new Point(i, y - 1));
                }
                else if (!down_flag && !down) down_flag = true;

            }

        }

        bitmap.Pixels = pixmap;
    }

    public object Clone()
    {
        return new Image(bitmap.Copy());
    }

}
