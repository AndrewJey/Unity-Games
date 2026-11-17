using System;

// Pure layout calculation; usable without initializing the Unity engine.
public struct AspectViewport
{
    public readonly float X;
    public readonly float Y;
    public readonly float Width;
    public readonly float Height;

    private AspectViewport(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public static AspectViewport Calculate(int screenWidth, int screenHeight, float desiredAspect)
    {
        if (screenWidth <= 0) throw new ArgumentOutOfRangeException("screenWidth");
        if (screenHeight <= 0) throw new ArgumentOutOfRangeException("screenHeight");
        if (desiredAspect <= 0 || float.IsNaN(desiredAspect) || float.IsInfinity(desiredAspect))
            throw new ArgumentOutOfRangeException("desiredAspect");

        double currentAspect = (double)screenWidth / screenHeight;
        if (Math.Abs(currentAspect - desiredAspect) <= desiredAspect * 0.00001)
            return new AspectViewport(0, 0, 1, 1);
        if (currentAspect > desiredAspect)
        {
            float width = (float)(desiredAspect / currentAspect);
            return new AspectViewport((1 - width) / 2, 0, width, 1);
        }
        float height = (float)(currentAspect / desiredAspect);
        return new AspectViewport(0, (1 - height) / 2, 1, height);
    }
}
