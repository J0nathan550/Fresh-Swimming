using Fresh_Swimming.ViewModels;
using System.Windows.Controls;

namespace Fresh_Swimming.Views;

public partial class StatisticsView : UserControl
{
    private readonly StatisticsViewModel _statisticsViewModel = new();

    public StatisticsView()
    {
        DataContext = _statisticsViewModel;
        InitializeComponent();
        _statisticsViewModel.RegisterHistogramGraph(laneUsageHistogram, profitPerLaneHistogram);
    }
}