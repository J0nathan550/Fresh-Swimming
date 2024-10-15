using ColorPickerWPF;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fresh_Swimming.Helpers;
using Fresh_Swimming.Views;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Fresh_Swimming.Models;

public partial class User : ObservableObject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string Color { get; set; } = "FFFFFF";

    public SolidColorBrush ColorAsBrush => ColorConverters.GetSolidColorBrush(Color);
    public SolidColorBrush TextForColor => ColorConverters.GetSaturationColorBrush(Color);

    [ObservableProperty]
    private byte _skill;

    private bool skipCheckSkillOnce = false;

    [RelayCommand]
    private static void AddReservation(int id)
    {
        ReservationView.ViewModel.Initialize(id);
        MainWindowView.Instance!.ContentFrame.Navigate(new Uri("/Views/ReservationView.xaml", UriKind.RelativeOrAbsolute));
    }

    [RelayCommand]
    private static void AddPayment(int id)
    {
        PaymentView.ViewModel.Initialize(id);
        MainWindowView.Instance!.ContentFrame.Navigate(new Uri("/Views/PaymentView.xaml", UriKind.RelativeOrAbsolute));
    }

    [RelayCommand]
    private static async Task ChangeColorOfUser(int id)
    {
        bool ok = ColorPickerWindow.ShowDialog(out Color color);
        if (!ok) return;
        SolidColorBrush colorBrush = new(color);
        string userColor = colorBrush.ToString()[3..9];
        User user = await Database.GetUserActualByIDAsync(id);
        await Database.UpdateUserAsync(user.Id, user.Name, user.Email!, user.PhoneNumber!, user.Skill, userColor);
        ManageUsersView.ViewModel.Users = await Database.GetUsersAsync();
    }

    [RelayCommand]
    private async Task DeleteUser(int id)
    {
        MessageBoxResult result = MessageBox.Show($"You sure that you want to delete {Name}?", "Deleting User", MessageBoxButton.YesNo, MessageBoxImage.Information);
        if (result == MessageBoxResult.Yes)
        {
            await Database.DeleteUserAsync(id);
            ManageUsersView.ViewModel.Users = await Database.GetUsersAsync();
        }
    }

    partial void OnSkillChanged(byte value)
    {
        if (!skipCheckSkillOnce)
        {
            skipCheckSkillOnce = true;
            return;
        }
        SkillChanged();
    }

    private async void SkillChanged()
    {
        User user = await Database.GetUserActualByIDAsync(Id);
        await Database.UpdateUserAsync(user.Id, user.Name, user.Email!, user.PhoneNumber!, user.Skill, user.Color);
    }
}