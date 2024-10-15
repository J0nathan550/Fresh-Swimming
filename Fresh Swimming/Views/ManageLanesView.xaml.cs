using System.Windows.Controls;

namespace Fresh_Swimming.Views;

public partial class ManageLanesView : UserControl
{
    public ManageLanesView()
    {
        DataContext = new ManageLanesViewModel();
        InitializeComponent();
    }
}