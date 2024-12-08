using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

namespace WpfSidebarApp
{
    public partial class MainWindow : Window
    {
        private HubConnection? hubConnection;
        private ObservableCollection<ProcessedEmail> tickets;
        private readonly string hubUrl = "http://localhost:5000/ticketHub";
        private readonly DispatcherTimer reconnectionTimer;
        private readonly HttpClient _httpClient;

        public MainWindow()
        {
            InitializeComponent();
            tickets = new ObservableCollection<ProcessedEmail>();
            TicketsListView.ItemsSource = tickets;

            // Initialize reconnection timer
            reconnectionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            reconnectionTimer.Tick += ReconnectionTimer_Tick;

            // Initialize HttpClient
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/")
            };

            InitializeHubConnection();
            _ = ConnectToHub();

            // Load existing tickets
            _ = LoadExistingTickets();
        }

        private void InitializeHubConnection()
        {
            // Bypass SSL certificate validation for development
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.HttpMessageHandlerFactory = _ => handler;
                })
                .WithAutomaticReconnect()
                .Build();

            hubConnection.Closed += async (error) =>
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    UpdateConnectionStatus("Disconnected", false);
                    reconnectionTimer.Start();
                });
            };

            hubConnection.Reconnecting += (error) =>
            {
                UpdateConnectionStatus("Reconnecting...", false);
                return System.Threading.Tasks.Task.CompletedTask;
            };

            hubConnection.Reconnected += (connectionId) =>
            {
                UpdateConnectionStatus("Connected", true);
                return System.Threading.Tasks.Task.CompletedTask;
            };

            hubConnection.On<ProcessedEmail>("ReceiveSeriousTicket", (ticket) =>
            {
                Dispatcher.Invoke(() =>
                {
                    tickets.Insert(0, ticket);
                    StatusText.Text = $"New serious ticket received from {ticket.SenderEmail}";
                });
            });
        }

        private async Task ConnectToHub()
        {
            try
            {
                if (hubConnection != null)
                {
                    await hubConnection.StartAsync();
                    UpdateConnectionStatus("Connected", true);
                    StatusText.Text = "Connected to server, waiting for serious tickets...";
                }
            }
            catch (Exception ex)
            {
                UpdateConnectionStatus("Connection failed", false);
                StatusText.Text = $"Connection error: {ex.Message}";
                reconnectionTimer.Start();
            }
        }

        private void UpdateConnectionStatus(string status, bool isConnected)
        {
            ConnectionStatus.Text = status;
            ConnectionStatus.Foreground = isConnected ? 
                new SolidColorBrush(Color.FromRgb(40, 167, 69)) :  // Green for connected
                new SolidColorBrush(Color.FromRgb(220, 53, 69));   // Red for disconnected
        }

        private async void ReconnectionTimer_Tick(object? sender, EventArgs e)
        {
            if (hubConnection?.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await hubConnection.StartAsync();
                    reconnectionTimer.Stop();
                    UpdateConnectionStatus("Connected", true);
                }
                catch
                {
                    UpdateConnectionStatus("Reconnection failed", false);
                }
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (hubConnection?.State == HubConnectionState.Disconnected)
            {
                await ConnectToHub();
            }
            else
            {
                await LoadExistingTickets();
            }
        }

        private async Task LoadExistingTickets()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/serious-tickets");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tickets = JsonConvert.DeserializeObject<List<ProcessedEmail>>(content);
                    
                    if (tickets != null)
                    {
                        foreach (var ticket in tickets)
                        {
                            this.tickets.Add(ticket);
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Error loading tickets: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            reconnectionTimer.Stop();
            if (hubConnection != null)
            {
                _ = hubConnection.DisposeAsync();
            }
            base.OnClosed(e);
        }
    }

    public class ProcessedEmail
    {
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? SenderEmail { get; set; }
        public DateTime ProcessedAt { get; set; }
        public int SentimentScore { get; set; }
    }
}
