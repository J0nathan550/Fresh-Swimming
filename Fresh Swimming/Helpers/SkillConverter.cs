namespace Fresh_Swimming.Helpers;

public static class SkillConverter
{
    public static string CalculateSkillAsString(int skill)
    {
        return skill switch
        {
            0 => "None",
            1 => "Beginner",
            2 => "Intermediate",
            3 => "Advanced",
            4 => "Pro",
            _ => "Unknown",
        };
    }
}