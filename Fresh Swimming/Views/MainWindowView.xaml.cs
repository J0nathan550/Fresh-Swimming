using System.Windows;

namespace Fresh_Swimming.Views;

public partial class MainWindowView : Window
{
    public static MainWindowView? Instance { get; private set; }

    public MainWindowView()
    {
        InitializeComponent();
        ContentFrame.Navigate(new Uri("/Views/StartupView.xaml", UriKind.RelativeOrAbsolute));
        Instance = this;
    }
}