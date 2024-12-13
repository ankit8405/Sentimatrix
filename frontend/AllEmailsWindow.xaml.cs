using System.Collections.ObjectModel;
using System.Windows;

namespace WpfSidebarApp
{
    public partial class AllEmailsWindow : Window
    {
        public ObservableCollection<Email> Emails { get; set; }

        public AllEmailsWindow()
        {
            InitializeComponent();
            LoadEmails();
            EmailsGrid.ItemsSource = Emails;
        }

        private void LoadEmails()
        {
            // Sample data for demonstration
            Emails = new ObservableCollection<Email>
            {
                new Email { Sender = "example1@example.com", Receiver = "example1@example.com", Score = 5, Body = "This is a sample email body 1." },
                new Email { Sender = "example2@example.com", Receiver = "example2@example.com", Score = 3, Body = "This is a sample email body 2." },
                new Email { Sender = "example3@example.com", Receiver = "example3@example.com", Score = 4, Body = "This is a sample email body 3." }
            };
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}