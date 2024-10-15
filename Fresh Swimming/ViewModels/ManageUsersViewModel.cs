using ColorPickerWPF;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fresh_Swimming.Helpers;
using Fresh_Swimming.Models;
using Fresh_Swimming.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Fresh_Swimming;

public partial class ManageUsersViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<User> users = [];

    public ManageUsersViewModel()
    {
        Initialize();
    }

    private async void Initialize() => Users = await Database.GetUsersAsync();

    [ObservableProperty]
    private double _columnWidth = 0;

    [ObservableProperty]
    private string? _textBoxName = string.Empty;

    [ObservableProperty]
    private string? _textBoxEmail = string.Empty;

    [ObservableProperty]
    private string? _textBoxPhoneNumber = string.Empty;

    [ObservableProperty]
    private SolidColorBrush _userColor = new(Colors.White);

    [ObservableProperty]
    private byte _comboBoxSkill = 0;

    [RelayCommand]
    private async Task CreateUserAsync()
    {
        if (string.IsNullOrWhiteSpace(TextBoxName))
        {
            MessageBox.Show("Please specify username!");
            return;
        }
        if (await Database.CheckUserAsync(TextBoxName, TextBoxEmail))
        {
            MessageBox.Show("This user already exist!");
            return;
        }
        string userColor = UserColor.ToString()[3..9];
        await Database.CreateUserAsync(TextBoxName, TextBoxEmail, TextBoxPhoneNumber, ComboBoxSkill, userColor);
        Users = await Database.GetUsersAsync();
        ColumnWidth = 0;
    }

    [RelayCommand]
    private void ShowColorPicker()
    {
        ColorPickerWindow.ShowDialog(out Color color);
        UserColor = new SolidColorBrush(color);
    }

    [RelayCommand]
    private void CreateUserShowPanel()
    {
        ColumnWidth = 200;
        TextBoxName = string.Empty;
        TextBoxEmail = string.Empty;
        TextBoxPhoneNumber = string.Empty;
        UserColor = new SolidColorBrush(Colors.White);
        ComboBoxSkill = 0;
    }

    [RelayCommand]
    private static void GoBackToMainView() => MainWindowView.Instance!.ContentFrame.Navigate(new Uri("/Views/StartupView.xaml", UriKind.RelativeOrAbsolute));

    [RelayCommand]
    private void CancelCreation() => ColumnWidth = 0f;
}