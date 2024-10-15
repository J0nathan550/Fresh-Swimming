using CommunityToolkit.Mvvm.Input;
using Fresh_Swimming.Helpers;
using Fresh_Swimming.Views;
using System.Windows.Media;

namespace Fresh_Swimming.Models;

public partial class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string SkillAsText
    {
        get
        {
            return SkillConverter.CalculateSkillAsString(Skill);
        }
    }

    public byte Skill { get; set; }
    public string Color { get; set; } = "FFFFFF";

    public SolidColorBrush ColorAsBrush => ColorConverters.GetSolidColorBrush(Color);

    public SolidColorBrush TextForColor => ColorConverters.GetSaturationColorBrush(Color);

    [RelayCommand]
    private static void AddReservation(int id)
    {
        ReservationView.viewModel.Initialize(id);
        MainWindowView.Instance!.ContentFrame.Navigate(new Uri("/Views/ReservationView.xaml", UriKind.RelativeOrAbsolute));
    }

    [RelayCommand]
    private static void AddPayment(int id)
    {
        PaymentView.ViewModel.Initialize(id);
        MainWindowView.Instance!.ContentFrame.Navigate(new Uri("/Views/PaymentView.xaml", UriKind.RelativeOrAbsolute));
    }
}