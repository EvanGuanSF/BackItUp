using BackItUp.ViewModels.Commands;
using BackItUp.ViewModels.TaskManagement;
using System.Windows;
using System.Windows.Input;

namespace BackItUp.ViewModels
{
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    internal class NotifyIconViewModel
    {
        /// <summary>
        /// Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => !Application.Current.MainWindow.IsVisible,

                    CommandAction = () => Application.Current.MainWindow.Visibility = Visibility.Visible
                };
            }
        }

        /// <summary>
        /// Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public ICommand HideWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => Application.Current.MainWindow.IsVisible,

                    CommandAction = () => Application.Current.MainWindow.Visibility = Visibility.Collapsed
                };
            }
        }


        /// <summary>
        /// Toggle the visibility of the window.
        /// </summary>
        public ICommand ToggleWindowVisibilityCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => Application.Current.MainWindow != null,

                    CommandAction = () =>
                    {
                        if(Application.Current.MainWindow.IsVisible)
                            Application.Current.MainWindow.Visibility = Visibility.Collapsed;
                        else if(!Application.Current.MainWindow.IsVisible)
                            Application.Current.MainWindow.Visibility = Visibility.Visible;
                    }
                };
            }
        }


        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        TaskManager.ShutDownScheduler();
                        Application.Current.Shutdown();
                    }
                };
            }
        }
    }
}
