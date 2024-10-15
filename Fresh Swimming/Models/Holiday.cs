using CommunityToolkit.Mvvm.Input;
using Fresh_Swimming.Helpers;
using Fresh_Swimming.Views;
using System.Windows;

namespace Fresh_Swimming.Models;

public partial class Holiday
{
    public int ID { get; set; }
    public string? Name { get; set; }
    public DateTime Date { get; set; }
    public string DateAsText
    { 
        get
        {
            return Date.ToShortDateString();
        }
    }
    public bool AllowToEnter { get; set; }
    public float PriceForEntry { get; set; }

    [RelayCommand]
    private async Task DeleteHoliday(int id)
    {
        MessageBoxResult result = MessageBox.Show($"You sure that you want to delete {Name}?", "Deleting Holiday", MessageBoxButton.YesNo, MessageBoxImage.Information);
        if (result == MessageBoxResult.Yes)
        {
            await Database.DeleteHolidayAsync(id);
            ManageHolidaysView.ViewModel.Holidays = await Database.GetHolidaysAsync();
        }
    }
}