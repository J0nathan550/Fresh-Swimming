using Fresh_Swimming.ViewModels;
using System.Windows.Controls;

namespace Fresh_Swimming.Views;

public partial class ManageHolidaysView : UserControl
{
    public static ManageHolidaysViewModel ViewModel { get; set; } = new ManageHolidaysViewModel();

    public ManageHolidaysView()
    {
        DataContext = ViewModel;
        InitializeComponent();
        ViewModel.RegisterDataGrid(DataGridHolidays);
    }
}