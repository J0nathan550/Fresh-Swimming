using System.Windows.Controls;

namespace Fresh_Swimming.Views;

public partial class ManageLanesView : UserControl
{
    public static ManageLanesViewModel ViewModel { get; set; } = new ManageLanesViewModel();

    public ManageLanesView()
    {
        DataContext = ViewModel;
        InitializeComponent();
        ViewModel.RegisterDataGrid(DataGridLane);
    }
}