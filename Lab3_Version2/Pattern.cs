using SkiaSharp;

public interface FillBrush
{
    public SKColor GetColor(int x, int y);
}


public class FillColor : FillBrush
{
    private SKColor color;

		public FillColor(SKColor color) {this.color = color;}

    public FillColor(byte r, byte g, byte b)
    {
        color = new SKColor(r, g, b);
    }

    public SKColor GetColor(int x, int y)
    {
        return color;
    }

}


public class BasePattern : Pattern
{
    private static SKColor black = new SKColor(0, 0, 0);
    private static SKColor white = new SKColor(255, 255, 255);
    private static SKColor red = new SKColor(255, 0, 0);
    private static SKColor green = new SKColor(0, 255, 0);

    private static SKColor[,] pattern = new SKColor[3, 3]
    {
        {white,white,black},
        {white,black,white},
        {black,white,white}
    };

    public BasePattern() : base(pattern)
    { }
}

public class Pattern : FillBrush
{
    private SKColor[,] pattern;
    private int width, height;

    public Pattern(SKColor[,] pattern)
    {
        this.pattern = pattern;
        width = pattern.GetUpperBound(0) + 1;
        height = pattern.GetUpperBound(1) + 1;
        Console.WriteLine($"Создание нового шаблона {width}*{height}");
    }

    public SKColor GetColor(int x, int y)
    {
        return pattern[x % (width), y % (height)];
    }
}
