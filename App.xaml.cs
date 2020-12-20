using BackItUp.Views;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;

namespace BackItUp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize main window and view model
            new MainWindow();
            // Initialize the tray icon.
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            notifyIcon.DataContext = new NotifyIconViewModel.NotifyIconViewModel();
        }
    }
}
