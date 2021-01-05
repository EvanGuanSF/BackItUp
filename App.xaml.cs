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
        private MainWindow mainWindow;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            bool isStartingHidden = false;

            // Initialize main window and view model
            mainWindow = new MainWindow();

            // Handle startup arguments.
            foreach(string arg in e.Args)
            {
                if(arg == "-hidden")
                {
                    isStartingHidden = true;
                }
            }

            // Do not show the main window on launch if option is set.
            if (!isStartingHidden)
                mainWindow.Show();

            // Initialize the tray icon.
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            notifyIcon.DataContext = new BackItUp.ViewModels.NotifyIconViewModel();
        }
    }
}
