using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fresh_Swimming.Helpers;
using Fresh_Swimming.Views;

namespace Fresh_Swimming.ViewModels;

public partial class PaymentViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _userName;

    [ObservableProperty]
    private string? _amountToPay;

    private int userID = -1;

    public async void Initialize(int userID)
    {
        this.userID = userID;
        UserName = "User: " + await Database.GetUserByIDAsync(userID);
        AmountToPay = await Database.CalculateAmountToPayAsync(userID);
    }

    [RelayCommand]
    private async Task Pay()
    {
        await Database.ExecutePaymentAsync(userID);
        MainWindowView.Instance!.ContentFrame.Navigate(new Uri("/Views/ManageUsersView.xaml", UriKind.RelativeOrAbsolute));
    }

    [RelayCommand]
    private static void Cancel() => MainWindowView.Instance!.ContentFrame.Navigate(new Uri("/Views/ManageUsersView.xaml", UriKind.RelativeOrAbsolute));
}