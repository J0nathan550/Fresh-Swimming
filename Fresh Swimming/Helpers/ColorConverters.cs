using System.Windows.Media;

namespace Fresh_Swimming.Helpers;

public static class ColorConverters
{
    public static SolidColorBrush GetSolidColorBrush(string color)
    {
        if (string.IsNullOrEmpty(color)) return new SolidColorBrush(Colors.White);
        if (!byte.TryParse(color[0..2], out byte r)) { r = 0xFF; }
        if (!byte.TryParse(color[2..4], out byte g)) { g = 0xFF; }
        if (!byte.TryParse(color[4..6], out byte b)) { b = 0xFF; }
        return new SolidColorBrush(Color.FromRgb(r, g, b));
    }
    public static SolidColorBrush GetSaturationColorBrush(string color)
    {
        if (string.IsNullOrEmpty(color)) return new SolidColorBrush(Colors.Black);
        if (!int.TryParse(color[0..2], out int r)) { r = 0xFF; }
        if (!int.TryParse(color[2..4], out int g)) { g = 0xFF; }
        if (!int.TryParse(color[4..6], out int b)) { b = 0xFF; }
        float l = (0.2126f * r + 0.7152f * g + 0.0722f * b);
        if (l < 128)
        {
            return new SolidColorBrush(Colors.White);
        }
        return new SolidColorBrush(Colors.Black);
    }
}