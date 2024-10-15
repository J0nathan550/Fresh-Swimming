using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fresh_Swimming.Helpers;
using Fresh_Swimming.Models;
using Fresh_Swimming.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Fresh_Swimming.ViewModels;

public partial class ManageHolidaysViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Holiday> _holidays = [];

    public ManageHolidaysViewModel()
    {
        Initialize();
    }

    private async void Initialize() => Holidays = await Database.GetHolidaysAsync();

    [ObservableProperty]
    private double _columnWidth = 0;

    [ObservableProperty]
    private string? _textBoxName = string.Empty;

    [ObservableProperty]
    private DateTime _selectedDateCalendar = DateTime.Now;

    [ObservableProperty]
    private bool? _allowToEnterCheckBox;

    [ObservableProperty]
    private string? _textBoxPricePerEntry;

    [RelayCommand]
    private async Task CreateHolidayAsync()
    {
        if (string.IsNullOrWhiteSpace(TextBoxName))
        {
            MessageBox.Show("Please specify lane name!");
            return;
        }
        if (!float.TryParse(TextBoxPricePerEntry, out float resultPrice))
        {
            MessageBox.Show("Please specify price!");
            return;
        }
        if (await Database.CheckHolidayAsync(TextBoxName))
        {
            MessageBox.Show("This holiday already exist!");
            return;
        }
        await Database.CreateHolidayAsync(TextBoxName, AllowToEnterCheckBox, SelectedDateCalendar, resultPrice);
        Holidays = await Database.GetHolidaysAsync();
        ColumnWidth = 0;
    }

    [RelayCommand]
    private static void GoBackToMainView() => MainWindowView.Instance!.ContentFrame.Navigate(new Uri("/Views/StartupView.xaml", UriKind.RelativeOrAbsolute));

    [RelayCommand]
    private void CreateHolidayShowPanel()
    {
        ColumnWidth = 200;
        TextBoxName = string.Empty;
        TextBoxPricePerEntry = "0";
        AllowToEnterCheckBox = false;
    }

    [RelayCommand]
    private void CancelCreation() => ColumnWidth = 0f;

    public void RegisterDataGrid(DataGrid dataGrid) => dataGrid.CellEditEnding += DataGrid_CellEditEnding;
    
    private async void DataGrid_CellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            if (e.Row.Item is Lane lane)
            {
                if (e.Column is DataGridTextColumn column)
                {
                    string newValue = ((TextBox)e.EditingElement).Text; // Get new value
                    switch (column.Header.ToString())
                    {
                        case "Name":
                            if (string.IsNullOrEmpty(newValue))
                            {
                                ((TextBox)e.EditingElement).Text = lane.Name;
                                return;
                            }
                            lane.Name = newValue;
                            break;
                        case "Date":
                            if (float.TryParse(newValue, out float costPerHour))
                            {
                                lane.CostPerHour = costPerHour;
                            }
                            break;
                        case "Allow to Enter":
                            if (float.TryParse(newValue, out float length))
                            {
                                lane.Length = length;
                            }
                            break;
                        case "Price for Entry":
                            if (float.TryParse(newValue, out float depth))
                            {
                                lane.Depth = depth;
                            }
                            break;
                    }

                    await Database.UpdateLaneAsync(lane.ID, lane.Name!, lane.CostPerHour, lane.Length, lane.Depth);
                }
            }
        }
    }
}