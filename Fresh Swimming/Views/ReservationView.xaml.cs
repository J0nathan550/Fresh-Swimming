using Fresh_Swimming.ViewModels;
using System.Windows.Controls;

namespace Fresh_Swimming.Views;

public partial class ReservationView : UserControl
{
    public static ReservationViewModel ViewModel { get; set; } = new();
    
    public ReservationView()
    {
        DataContext = ViewModel;
        InitializeComponent();
    }
}