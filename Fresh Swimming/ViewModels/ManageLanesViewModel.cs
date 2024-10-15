using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fresh_Swimming.Helpers;
using Fresh_Swimming.Models;
using Fresh_Swimming.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace Fresh_Swimming;

public partial class ManageLanesViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Lane> lanes = [];

    public ManageLanesViewModel()
    {
        Initialize();
    }

    private async void Initialize() => Lanes = await Database.GetLanesAsync();

    [ObservableProperty]
    private double _columnWidth = 0;

    [ObservableProperty]
    private string? _textBoxName = string.Empty;

    [ObservableProperty]
    private string? _textBoxCostPerHour = "0";

    [ObservableProperty]
    private string? _textBoxLength = "0";
    
    [ObservableProperty]
    private string? _textBoxDepth = "0";


    [RelayCommand]
    private async Task CreateLaneAsync()
    {
        if (string.IsNullOrWhiteSpace(TextBoxName))
        {
            MessageBox.Show("Please specify lane name!");
            return;
        }
        if (!float.TryParse(TextBoxCostPerHour, out float resultCost))
        {
            MessageBox.Show("Please specify lane cost per hour!");
            return;
        }
        if (!float.TryParse(TextBoxLength, out float resultLength))
        {
            MessageBox.Show("Please specify lane length!");
            return;
        }
        if (!float.TryParse(TextBoxDepth, out float resultDepth))
        {
            MessageBox.Show("Please specify lane depth!");
            return;
        }
        if (await Database.CheckLaneAsync(TextBoxName))
        {
            MessageBox.Show("This lane already exist!");
            return;
        }
        await Database.CreateLaneAsync(TextBoxName, resultCost, resultLength, resultDepth);
        Lanes = await Database.GetLanesAsync();
        ColumnWidth = 0;
    }

    [RelayCommand]
    private static void GoBackToMainView() => MainWindowView.Instance!.ContentFrame.Navigate(new Uri("/Views/StartupView.xaml", UriKind.RelativeOrAbsolute));

    [RelayCommand]
    private void CreateLaneShowPanel()
    {
        ColumnWidth = 200;
        TextBoxName = string.Empty;
        TextBoxCostPerHour = "0";
        TextBoxLength = "0";
        TextBoxDepth = "0";
    }

    [RelayCommand]
    private void CancelCreation() => ColumnWidth = 0f;
}