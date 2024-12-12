using System;
using System.Windows;

namespace WpfSidebarApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Removed global exception handling to prevent pop-ups
        }
    }
}
