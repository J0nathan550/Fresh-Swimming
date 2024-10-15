namespace Fresh_Swimming.Models;

public class Lane
{
    public int ID { get; set; }
    public string? Name { get; set; }
    public float CostPerHour { get; set; }
    public float Length { get; set; }
    public float Depth { get; set; }
}