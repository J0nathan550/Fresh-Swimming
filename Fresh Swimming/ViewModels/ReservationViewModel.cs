using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fresh_Swimming.Helpers;
using Fresh_Swimming.Models;
using Fresh_Swimming.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Fresh_Swimming.ViewModels;

public partial class ReservationViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Reservation> _reservations = [];
    [ObservableProperty]
    private string _currentUserData = string.Empty;
    [ObservableProperty]
    private string _confirmation = string.Empty;
    [ObservableProperty]
    private bool _isCreateReservButtonEnabled = false;
    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Now;
    [ObservableProperty]
    private DataGridCellInfo _currentSelectedCell;
    [ObservableProperty]
    private Visibility _controlVisibilities = Visibility.Collapsed;

    private int _startHour = -1;
    private int _endHour = -1;
    private int _userID = -1;
    private string _laneName = string.Empty;

    public async void Initialize(int userID)
    {
        _userID = userID;
        ProcessingWindow processingWindow = new()
        {
            Owner = MainWindowView.Instance
        };
        processingWindow.Show();
        ControlVisibilities = userID == -1 ? Visibility.Collapsed : Visibility.Visible;
        if (ControlVisibilities == Visibility.Visible)
        {
            CurrentUserData = await Database.GetUserByIDAsync(_userID);
            DateTime currentTime = DateTime.Now;
            SelectedDate = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day);
            Confirmation = "Select start and the end hours of the reservation!";
            IsCreateReservButtonEnabled = false;
        }
        Reservations = await Database.GetReservationsAsync(SelectedDate);
        processingWindow.Close();
    }

    partial void OnSelectedDateChanged(DateTime value) => ProcessReservationsAsync(value);
    private async void ProcessReservationsAsync(DateTime value)
    {
        ProcessingWindow processingWindow = new()
        {
            Owner = MainWindowView.Instance
        };
        processingWindow.Show();
        Reservations = await Database.GetReservationsAsync(value);
        processingWindow.Close();
    }

    partial void OnCurrentSelectedCellChanging(DataGridCellInfo value)
    {
        if(_userID == -1) return;
        ProcessingWindow processingWindow = new();
        processingWindow.Owner = MainWindowView.Instance;
        processingWindow.Show();
        try
        {
            if (value.Column == null || value.Item == null)
            {
                return;
            }
            Reservation? reservation = value.Item as Reservation;
            int columnIndex = value.Column.DisplayIndex;
            if (reservation == null)
            {
                return;
            }
            if (columnIndex < 1 || columnIndex > 13)
            {
                IsCreateReservButtonEnabled = false;
                return;
            }
            if (reservation.Hours != null && reservation.Hours.Length != 0 && reservation.Hours[columnIndex - 1] != null)
            {
                IsCreateReservButtonEnabled = false;
                return;
            }
            if (reservation.LaneName == _laneName)
            {
                if (_startHour > columnIndex + 7)
                {
                    _endHour = _startHour;
                    _startHour = columnIndex + 7;
                }
                else
                {
                    _endHour = columnIndex + 7;
                }
                Confirmation = $"From {_startHour} to {_endHour} on {_laneName}";
                IsCreateReservButtonEnabled = true;
            }
            else
            {
                _laneName = reservation.LaneName!;
                _startHour = columnIndex + 7;
                _endHour = -1;
                Confirmation = $"Start hour is: {_startHour} on {_laneName}";
                IsCreateReservButtonEnabled = false;
            }
        }
        finally 
        {
            processingWindow.Close();
        }
    }

    [RelayCommand]
    private void GoBackToMainView()
    {
        if (_userID == -1)
        {
            MainWindowView.Instance!.ContentFrame.Navigate(new Uri("/Views/StartupView.xaml", UriKind.RelativeOrAbsolute));
            return;
        }
        MainWindowView.Instance!.ContentFrame.Navigate(new Uri("/Views/ManageUsersView.xaml", UriKind.RelativeOrAbsolute));
    }

    [RelayCommand]
    private async Task CreateReservation()
    {
        ProcessingWindow processingWindow = new()
        {
            Owner = MainWindowView.Instance
        };
        processingWindow.Show();
        if (await Database.CheckReservationAvailabilityAsync(SelectedDate, _startHour, _endHour, _laneName))
        {
            Confirmation = $"You cannot create reservation, lane is already occupied.";
            IsCreateReservButtonEnabled = false;
            processingWindow.Close();
            return;
        }
        await Database.CreateReservationAsync(_userID, SelectedDate, _laneName, _startHour, _endHour);
        Reservations = await Database.GetReservationsAsync(SelectedDate);
        Confirmation = "Reservation created, you can create a new one!";
        IsCreateReservButtonEnabled = false;
        processingWindow.Close();
    }
}