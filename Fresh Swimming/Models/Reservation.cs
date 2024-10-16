using System.Windows.Media;

namespace Fresh_Swimming.Models;

public class Reservation
{
    public string? LaneName { get; set; }
    public string? CostPerHour {  get; set; }
    public string? Length {  get; set; }
    public string? Depth {  get; set; }
    public string[] Hours { get; set; } = [];
    public bool IsHoliday { get; set; }
    public SolidColorBrush[] Colors { get; set; } = [];
    public SolidColorBrush[] ColorsForeground { get; set; } = [];
}