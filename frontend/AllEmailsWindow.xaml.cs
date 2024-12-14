using System.Collections.ObjectModel;
using System.Windows;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using WpfSidebarApp.Models;

namespace WpfSidebarApp
{
    public partial class AllEmailsWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "http://localhost:5000/api";
        public ObservableCollection<Email> Emails { get; set; }

        public AllEmailsWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            Emails = new ObservableCollection<Email>();
            LoadEmailsAsync();
        }

        private async Task LoadEmailsAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_apiBaseUrl}/email");
                var emailList = JsonConvert.DeserializeObject<List<Email>>(response);
                
                Emails.Clear();
                foreach (var email in emailList.OrderByDescending(e => e.Time))
                {
                    Emails.Add(email);
                }
                
                EmailsGrid.ItemsSource = Emails;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading emails: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}