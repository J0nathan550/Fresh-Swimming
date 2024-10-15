using CommunityToolkit.Mvvm.Input;
using Fresh_Swimming.Helpers;
using Fresh_Swimming.Views;
using System.Windows;

namespace Fresh_Swimming.Models;

public partial class Lane
{
    public int ID { get; set; }
    public string? Name { get; set; }
    public float CostPerHour { get; set; }
    public string CostPerHourAsString
    {
        get
        {
            return $"{CostPerHour}$";
        }
        set
        {
            if (float.TryParse(value, out float value2))
            {
                CostPerHour = value2;
            }
        }
    }
    public float Length { get; set; }
    public float Depth { get; set; }

    [RelayCommand]
    private async Task DeleteLane(int id)
    {
        MessageBoxResult result = MessageBox.Show($"You sure that you want to delete {Name}?", "Deleting Lane", MessageBoxButton.YesNo, MessageBoxImage.Information);
        if (result == MessageBoxResult.Yes)
        {
            await Database.DeleteLaneAsync(id);
            ManageLanesView.ViewModel.Lanes = await Database.GetLanesAsync();
        }
    }
}