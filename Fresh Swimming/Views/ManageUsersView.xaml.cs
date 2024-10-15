using System.Diagnostics;
using System.Windows.Controls;

namespace Fresh_Swimming.Views;

public partial class ManageUsersView : UserControl
{
    public static ManageUsersViewModel ViewModel { get; set; } = new ManageUsersViewModel();

    public ManageUsersView()
    {
        DataContext = ViewModel;
        InitializeComponent();
        ViewModel.RegisterDataGrid(DataGridUsers);
    }
}