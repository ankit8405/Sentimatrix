using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Net.Http;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WpfSidebarApp
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly HubConnection _hubConnection;
        private readonly string _apiBaseUrl = "http://localhost:5000/api";
        private DispatcherTimer _refreshTimer;
        private SeriesCollection _sentimentSeries;
        private SeriesCollection _volumeSeries;

        public MainWindow()
        {
            InitializeComponent();
            
            _httpClient = new HttpClient();
            
            // Initialize SignalR connection
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/ticketHub")
                .Build();

            // Set up SignalR event handlers
            _hubConnection.On<Email>("ReceiveSeriousTicket", (email) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateDashboard();
                    ShowNotification($"New serious ticket from {email.Sender}");
                });
            });

            // Initialize charts
            InitializeCharts();

            // Set up refresh timer
            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            _refreshTimer.Tick += async (s, e) => await UpdateDashboard();
            _refreshTimer.Start();

            // Initial load
            Loaded += async (s, e) =>
            {
                await ConnectToHub();
                await UpdateDashboard();
            };
        }

        private void InitializeCharts()
        {
            // Sentiment Trend Chart
            _sentimentSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Average Sentiment",
                    Values = new ChartValues<double>(),
                    PointGeometry = null,
                    LineSmoothness = 0.5,
                    Stroke = new SolidColorBrush(Color.FromRgb(33, 150, 243))
                }
            };
            SentimentTrendChart.Series = _sentimentSeries;
            SentimentTrendChart.AxisX = new AxesCollection { new Axis { Labels = new List<string>() } };

            // Volume Chart
            _volumeSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Email Volume",
                    Values = new ChartValues<int>(),
                    Fill = new SolidColorBrush(Color.FromRgb(76, 175, 80))
                }
            };
            EmailVolumeChart.Series = _volumeSeries;
            EmailVolumeChart.AxisX = new AxesCollection { new Axis { Labels = new List<string>() } };
        }

        private async Task ConnectToHub()
        {
            try
            {
                await _hubConnection.StartAsync();
                ShowNotification("Connected to real-time updates");
            }
            catch (Exception ex)
            {
                ShowNotification($"Connection error: {ex.Message}");
            }
        }

        private async Task UpdateDashboard()
        {
            try
            {
                // Fetch all emails
                var response = await _httpClient.GetStringAsync($"{_apiBaseUrl}/Email");
                var emails = JsonConvert.DeserializeObject<List<Email>>(response);

                if (emails == null || !emails.Any())
                    return;

                // Update stats
                UpdateStats(emails);

                // Update charts
                UpdateCharts(emails);

                // Update recent emails grid
                RecentEmailsGrid.ItemsSource = emails.OrderByDescending(e => e.Time).Take(10);
            }
            catch (Exception ex)
            {
                ShowNotification($"Error updating dashboard: {ex.Message}");
            }
        }

        private void UpdateStats(List<Email> emails)
        {
            // Total emails
            TotalEmailsText.Text = emails.Count.ToString();

            // Positive ratio
            var positiveCount = emails.Count(e => e.Type == "positive");
            var positiveRatio = (double)positiveCount / emails.Count * 100;
            PositiveRatioText.Text = $"{positiveRatio:F1}%";

            // Average score
            var averageScore = emails.Average(e => e.Score);
            AverageScoreText.Text = $"{averageScore:F1}";

            // Calculate changes from last week
            var lastWeek = DateTime.UtcNow.AddDays(-7);
            var oldEmails = emails.Where(e => e.Time < lastWeek).ToList();
            
            if (oldEmails.Any())
            {
                var oldPositiveRatio = (double)oldEmails.Count(e => e.Type == "positive") / oldEmails.Count * 100;
                var oldAverageScore = oldEmails.Average(e => e.Score);

                var ratioChange = positiveRatio - oldPositiveRatio;
                var scoreChange = averageScore - oldAverageScore;

                PositiveRatioChange.Text = $"{(ratioChange >= 0 ? "+" : "")}{ratioChange:F1}% from last week";
                AverageScoreChange.Text = $"{(scoreChange >= 0 ? "+" : "")}{scoreChange:F1} from last week";
            }
        }

        private void UpdateCharts(List<Email> emails)
        {
            // Group by day for the last 7 days
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.UtcNow.Date.AddDays(-i))
                .Reverse()
                .ToList();

            var dailyStats = last7Days.Select(date =>
            {
                var dayEmails = emails.Where(e => e.Time.Date == date).ToList();
                return new
                {
                    Date = date,
                    AverageSentiment = dayEmails.Any() ? dayEmails.Average(e => e.Score) : 0,
                    Count = dayEmails.Count
                };
            }).ToList();

            // Update sentiment trend
            var sentimentValues = new ChartValues<double>(dailyStats.Select(s => s.AverageSentiment));
            _sentimentSeries[0].Values = sentimentValues;

            // Update volume chart
            var volumeValues = new ChartValues<int>(dailyStats.Select(s => s.Count));
            _volumeSeries[0].Values = volumeValues;

            // Update X-axis labels
            var labels = dailyStats.Select(s => s.Date.ToString("MM/dd")).ToList();
            SentimentTrendChart.AxisX[0].Labels = labels;
            EmailVolumeChart.AxisX[0].Labels = labels;
        }

        private void ShowNotification(string message)
        {
            MessageBox.Show(message, "Sentimatrix", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            // Already on dashboard
        }

        private async void AnalyticsButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement detailed analytics view
        }

        private async void EmailsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_apiBaseUrl}/Email");
                var emails = JsonConvert.DeserializeObject<List<Email>>(response);
                RecentEmailsGrid.ItemsSource = emails.OrderByDescending(e => e.Time);
            }
            catch (Exception ex)
            {
                ShowNotification($"Error loading emails: {ex.Message}");
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement settings view
        }

        private void AllEmailsButton_Click(object sender, RoutedEventArgs e)
        {
            AllEmailsWindow allEmailsWindow = new AllEmailsWindow();
            allEmailsWindow.Show();
            this.Close();
        }
    }

    public class Email
    {
        public string Id { get; set; }
        public string Body { get; set; }
        public int Score { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Type { get; set; }
        public DateTime Time { get; set; }
    }
}
