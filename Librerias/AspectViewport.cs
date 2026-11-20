using System;

// Pure layout calculation; usable without initializing the Unity engine.
public struct AspectViewport
{
    public readonly float X;
    public readonly float Y;
    public readonly float Width;
    public readonly float Height;

    public AspectViewport(float x, float y, float width, float height)
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

    // Maps screen pixel coordinates into clamped [0..1] viewport space for smooth cursor/aim input.
    public void ScreenToViewportClamped(float screenX, float screenY, int screenWidth, int screenHeight, out float viewportX, out float viewportY)
    {
        if (screenWidth <= 0) throw new ArgumentOutOfRangeException("screenWidth");
        if (screenHeight <= 0) throw new ArgumentOutOfRangeException("screenHeight");

        float normX = screenX / screenWidth;
        float normY = screenY / screenHeight;
        float relX = Width > 0f ? (normX - X) / Width : 0f;
        float relY = Height > 0f ? (normY - Y) / Height : 0f;
        viewportX = Clamp01(relX);
        viewportY = Clamp01(relY);
    }

    // Smoothly interpolates between two camera viewports during dynamic aspect/window transitions.
    public static AspectViewport Lerp(AspectViewport from, AspectViewport to, float t)
    {
        float k = Clamp01(t);
        return new AspectViewport(
            from.X + (to.X - from.X) * k,
            from.Y + (to.Y - from.Y) * k,
            from.Width + (to.Width - from.Width) * k,
            from.Height + (to.Height - from.Height) * k
        );
    }

    private static float Clamp01(float v)
    {
        if (float.IsNaN(v) || v < 0f) return 0f;
        if (v > 1f) return 1f;
        return v;
    }
}
