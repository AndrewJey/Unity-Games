using System;

internal static class Program
{
    private static int checks;
    private static void Near(double actual, double expected)
    {
        if (Math.Abs(actual - expected) > 0.00001) throw new Exception("Expected " + expected + "; got " + actual);
    }
    private static void Layout(string name, int w, int h, float ratio, float x, float y, float width, float height)
    {
        AspectViewport result = AspectViewport.Calculate(w, h, ratio);
        Near(result.X, x); Near(result.Y, y); Near(result.Width, width); Near(result.Height, height);
        Near(result.Width * w / (result.Height * h), ratio);
        Near(result.X * 2 + result.Width, 1); Near(result.Y * 2 + result.Height, 1);
        checks++; Console.WriteLine("PASS: " + name);
    }
    private static void Reject(int w, int h, float ratio)
    {
        try { AspectViewport.Calculate(w, h, ratio); }
        catch (ArgumentOutOfRangeException) { checks++; Console.WriteLine("PASS: invalid layout rejected"); return; }
        throw new Exception("Invalid input was accepted");
    }
    public static void Main()
    {
        Layout("4:3 full screen", 800, 600, 4f/3f, 0, 0, 1, 1);
        Layout("16:9 pillarbox for 4:3", 1920, 1080, 4f/3f, .125f, 0, .75f, 1);
        Layout("16:10 pillarbox for 4:3", 1920, 1200, 4f/3f, 1f/12f, 0, 5f/6f, 1);
        Layout("portrait letterbox for 4:3", 1080, 1920, 4f/3f, 0, .2890625f, 1, .421875f);
        Layout("square screen", 1000, 1000, 4f/3f, 0, .125f, 1, .75f);
        Layout("16:9 requested", 1920, 1080, 16f/9f, 0, 0, 1, 1);
        Layout("old two-decimal comparison must not stretch 1.334", 1334, 1000, 4f/3f, .000249875f, 0, .99950025f, 1);
        Reject(0, 1080, 4f/3f); Reject(1920, 0, 4f/3f); Reject(-1, 1080, 4f/3f);
        Reject(1920, 1080, 0); Reject(1920, 1080, -1);
        Reject(1920, 1080, float.NaN); Reject(1920, 1080, float.PositiveInfinity);
        Console.WriteLine(checks + " offline checks passed; no Unity engine was initialized.");
    }
}
