using System.Windows.Controls;

namespace Fresh_Swimming.Views;

public partial class ManageUsersView : UserControl
{
    public ManageUsersView()
    {
        DataContext = new ManageUsersViewModel();
        InitializeComponent();
    }
}