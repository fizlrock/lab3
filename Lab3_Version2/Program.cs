using System.Diagnostics;
using ImageProcessor;
using SkiaSharp;


public class Program
{
   public static void Main(string[] args)
   {
       string path = "/home/fizlrock/RiderProjects/Lab3_Version2/samples";
       
       
       Image image = new Image(400, 400);
       var basalt = Utils.LoadPatternFromFile("/home/fizlrock/RiderProjects/Lab4/samples/basalt_top.png");
       var wood = Utils.LoadPatternFromFile("/home/fizlrock/RiderProjects/Lab4/samples/acacia_planks.png");
       var coral = Utils.LoadPatternFromFile(
           "/home/fizlrock/Загрузки/lb-photo-realism-mc1.18.x/assets/minecraft/textures/block/bubble_coral_block.png");
       var criper =
           Utils.LoadPatternFromFile(
               "/home/fizlrock/Загрузки/lb-photo-realism-mc1.18.x/assets/minecraft/textures/block/cyan_glazed_terracotta.png");
       var honey = Utils.LoadPatternFromFile(
           "/home/fizlrock/Загрузки/lb-photo-realism-mc1.18.x/assets/minecraft/textures/block/honeycomb_block.png");
       

       Point a, b, c, d;
       a = new Point(80,180);
       b = new Point(40,60);
       c = new Point(320, 60);
       //d = new Point(220,180);
       d = new Point(380,380);
       image.BezierLine(a,b,c,d, SKColors.Aquamarine);
       image.MarkDots(a,b,c,d);


       Point c1, c2;
       c1 = new Point(120, 300);
       c2 = new Point(240, 300);
       image.DrawCircle(c1, 80, SKColors.Firebrick);
       image.DrawCircle(c2, 80, SKColors.Firebrick);


       Point f1;
       f1 = new Point(180, 300);
       image.Fill(f1, new FillColor(SKColors.Chartreuse));
       image.Fill(c1, wood);
       image.Fill(new Point(10,10), basalt);
       image.Fill(c2, honey);




       image.SaveToJpeg(path, "test_image", 99);
       
       Process proc = new Process();
       proc.StartInfo.FileName = "imv";
       proc.StartInfo.Arguments = "/home/fizlrock/RiderProjects/Lab3_Version2/samples/test_image.jpg";
       proc.StartInfo.RedirectStandardOutput = true; 
       proc.Start();
        
       while(!proc.HasExited) Thread.Sleep(10);

   }

}