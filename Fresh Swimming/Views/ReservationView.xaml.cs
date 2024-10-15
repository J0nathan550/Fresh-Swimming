using Fresh_Swimming.ViewModels;
using System.Windows.Controls;

namespace Fresh_Swimming.Views;

public partial class ReservationView : UserControl
{
    public static readonly ReservationViewModel viewModel = new();
    
    public ReservationView()
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}