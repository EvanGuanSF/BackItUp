using BackItUp.ViewModels;
using BackItUp.Views;
using System.Windows;

namespace BackItUp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize main window and view model
            var mainWindow = new MainWindow();
            var viewModel = new BackupInfoViewModel();
            mainWindow.DataContext = viewModel;
            mainWindow.Show();
        }
    }
}
