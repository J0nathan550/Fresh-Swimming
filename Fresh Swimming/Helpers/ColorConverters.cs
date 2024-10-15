using System.Windows.Media;

namespace Fresh_Swimming.Helpers;

public static class ColorConverters
{
    public static SolidColorBrush GetSolidColorBrush(string color)
    {
        if (string.IsNullOrEmpty(color) || color.Length < 6) return new SolidColorBrush(Colors.White);

        try
        {
            byte r = Convert.ToByte(color[0..2], 16);
            byte g = Convert.ToByte(color[2..4], 16);
            byte b = Convert.ToByte(color[4..6], 16);

            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }
        catch
        {
            return new SolidColorBrush(Colors.White);
        }
    }

    public static SolidColorBrush GetSaturationColorBrush(string color)
    {
        if (string.IsNullOrEmpty(color) || color.Length < 6)
            return new SolidColorBrush(Colors.Black);

        try
        {
            int r = Convert.ToInt32(color[0..2], 16);
            int g = Convert.ToInt32(color[2..4], 16);
            int b = Convert.ToInt32(color[4..6], 16);

            float luminance = (0.2126f * r + 0.7152f * g + 0.0722f * b);

            if (luminance < 128)
            {
                return new SolidColorBrush(Colors.White);  // Light text on dark background
            }
            else
            {
                return new SolidColorBrush(Colors.Black);  // Dark text on light background
            }
        }
        catch
        {
            return new SolidColorBrush(Colors.Black); // Return black if parsing fails
        }
    }
}