using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fresh_Swimming.Helpers;
using Fresh_Swimming.Views;
using System.Windows;

namespace Fresh_Swimming.Models;

public partial class Holiday : ObservableObject
{
    public int ID { get; set; }
    public string? Name { get; set; }
    public string DateAsText
    { 
        get
        {
            return Date.ToShortDateString();
        }
    }
    public float PricePerEntry { get; set; }
    public string PricePerEntryAsString 
    { 
        get 
        { 
            return $"{PricePerEntry}$"; 
        } 
        set 
        {
            if (float.TryParse(PricePerEntryAsString, out float actualPrice))
            {
                PricePerEntry = actualPrice;
            }
        }
    }

    [ObservableProperty]
    private byte _allowToEnter;
    [ObservableProperty]
    private DateTime _date;

    private DateTime _delayUpdate = DateTime.Now;

    partial void OnAllowToEnterChanged(byte value) => UpdateData();

    partial void OnDateChanged(DateTime value) => UpdateData();

    private async void UpdateData()
    {
        if ((DateTime.Now - _delayUpdate).TotalSeconds < 2)
        {
            return;
        }
        await Database.UpdateHolidayAsync(ID, Name, Date, AllowToEnter, PricePerEntry);
    }

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