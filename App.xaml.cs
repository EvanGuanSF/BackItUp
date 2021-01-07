using BackItUp.Views;
using Hardcodet.Wpf.TaskbarNotification;
using System.Threading;
using System.Windows;
using System;
using System.Diagnostics;

namespace BackItUp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;
        private MainWindow mainWindow;
        // Mutex for single-instance program operation.
        static readonly Mutex mutex = new Mutex(true, "BackItUpInstance");

        protected override void OnStartup(StartupEventArgs e)
        {
            if(mutex.WaitOne(TimeSpan.Zero, true))
            {
                base.OnStartup(e);
                bool isStartingHidden = false;

                // Initialize main window and view model
                mainWindow = new MainWindow();

                // Handle startup arguments.
                foreach (string arg in e.Args)
                {
                    if (arg == "-hidden")
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
            else
            {
                Debug.WriteLine("Another instance of the program is already running.");
                Current.Shutdown();
            }
        }
    }
}
