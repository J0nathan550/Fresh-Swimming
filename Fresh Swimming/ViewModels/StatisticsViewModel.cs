using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fresh_Swimming.Helpers;
using Fresh_Swimming.Views;
using ScottPlot;
using ScottPlot.WPF;

namespace Fresh_Swimming.ViewModels;

public partial class StatisticsViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _totalUsageForTheDayInHoursString;
    [ObservableProperty]
    private string? _averageSkillsOfUsersAsString;
    [ObservableProperty]
    private DateTime _selectedDate;
    [ObservableProperty]
    private bool _showForAllDays = false;

    private WpfPlot? _laneUsageHistogram;
    private WpfPlot? _profitPerLaneHistogram;

    partial void OnSelectedDateChanged(DateTime value) => RecalculateStatistics();

    private async void RecalculateStatistics()
    {
        TotalUsageForTheDayInHoursString = await Database.CalculateUsageAsync(SelectedDate, ShowForAllDays);
        AverageSkillsOfUsersAsString = await Database.CalculateAverageSkillAsync(SelectedDate, ShowForAllDays);
        await RecalculateStatisticsLaneUsage();
        await RecalculateStatisticsLaneProfit();
    }

    public void RegisterHistogramGraph(WpfPlot laneUsageHistogram, WpfPlot profitPerLaneHistogram)
    {
        _laneUsageHistogram = laneUsageHistogram;
        _profitPerLaneHistogram = profitPerLaneHistogram;
        DateTime currentTime = DateTime.Now;
        SelectedDate = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0);
    }

    private async Task RecalculateStatisticsLaneUsage()
    {
        if (_laneUsageHistogram == null) return;
        List<Tuple<string, double>> data = await Database.CalculateLaneUsageAsync(SelectedDate, ShowForAllDays);
        int pos = 1;
        Tick[] ticks = new Tick[data.Count];

        _laneUsageHistogram.Plot.Clear();

        foreach (Tuple<string, double> item in data)
        {
            ticks[pos - 1] = new Tick(pos, item.Item1);
            _laneUsageHistogram.Plot.Add.Bar(position: pos++, value: item.Item2);
        }

        _laneUsageHistogram.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
        _laneUsageHistogram.Plot.Axes.Bottom.MajorTickStyle.Length = 0;

        _laneUsageHistogram.Plot.Axes.Margins(bottom: 0);
        _laneUsageHistogram.Refresh();
    }

    private async Task RecalculateStatisticsLaneProfit()
    {
        if (_profitPerLaneHistogram == null) return;
        List<Tuple<string, double>> data = await Database.CalculateRentabilityOfLanesAsync(SelectedDate, ShowForAllDays);
        int pos = 1;
        Tick[] ticks = new Tick[data.Count];

        _profitPerLaneHistogram.Plot.Clear();

        foreach (Tuple<string, double> item in data)
        {
            ticks[pos - 1] = new Tick(pos, item.Item1);
            _profitPerLaneHistogram.Plot.Add.Bar(position: pos++, value: item.Item2);
        }

        _profitPerLaneHistogram.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
        _profitPerLaneHistogram.Plot.Axes.Bottom.MajorTickStyle.Length = 0;

        _profitPerLaneHistogram.Plot.Axes.Margins(bottom: 0);
        _profitPerLaneHistogram.Refresh();
    }

    partial void OnShowForAllDaysChanged(bool value) => RecalculateStatistics();

    [RelayCommand]
    private static void GoBackToMainView() => MainWindowView.Instance!.ContentFrame.Navigate(new Uri("/Views/StartupView.xaml", UriKind.RelativeOrAbsolute));
}