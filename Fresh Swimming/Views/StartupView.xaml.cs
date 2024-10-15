using System.Windows;
using System.Windows.Controls;

namespace Fresh_Swimming.Views;

public partial class StartupView : UserControl
{
    public StartupView() => InitializeComponent();

    private void ChangeFrameToManageLanes_Click(object sender, RoutedEventArgs e) => MainWindowView.Instance!.ContentFrame.Navigate(new Uri("/Views/ManageLanesView.xaml", UriKind.RelativeOrAbsolute));
    private void ChangeFrameToManageUsers_Click(object sender, RoutedEventArgs e) => MainWindowView.Instance!.ContentFrame.Navigate(new Uri("/Views/ManageUsersView.xaml", UriKind.RelativeOrAbsolute));
    private void ChangeFrameToViewStatistics_Click(object sender, RoutedEventArgs e) => MainWindowView.Instance!.ContentFrame.Navigate(new Uri("/Views/StatisticsView.xaml", UriKind.RelativeOrAbsolute));
    
    private void ChangeFrameToViewReservations_Click(object sender, RoutedEventArgs e)
    {
        ReservationView.viewModel.Initialize(-1);
        MainWindowView.Instance!.ContentFrame.Navigate(new Uri("/Views/ReservationView.xaml", UriKind.RelativeOrAbsolute));
    }

}