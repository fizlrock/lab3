using SkiaSharp;

public static class Utils
{


	public static Pattern LoadPatternFromFile(string path)
	{
		SKBitmap bitmap;
		using(Stream stream = File.OpenRead(path))
		{
			bitmap = SKBitmap.Decode(stream);
		}

		Console.WriteLine($"Загрузка нового шаблона {bitmap.Width}*{bitmap.Height}");
		SKColor[,] colormap = new SKColor[bitmap.Width,bitmap.Height];
		
		for(int x = 0; x < bitmap.Width; x++) for(int y = 0; y < bitmap.Height; y++)
		{
			colormap[x,y] = bitmap.GetPixel(x,y);
		}
		return new Pattern(colormap);

	}

}







