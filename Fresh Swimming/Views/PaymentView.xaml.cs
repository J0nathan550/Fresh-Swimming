using Fresh_Swimming.ViewModels;
using System.Windows.Controls;

namespace Fresh_Swimming.Views;

public partial class PaymentView : UserControl
{
    private static PaymentViewModel viewModel = new();

    public PaymentView()
    {
        DataContext = viewModel;
        InitializeComponent();
    }

    public static PaymentViewModel ViewModel { get => viewModel; set => viewModel = value; }
}