using BackItUp.ViewModels.TaskManagement;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace BackItUp.ViewModels.Commands
{
    internal class ToggleRunOnStartupCommand : ICommand
    {
        private readonly BackupInfoViewModel _ViewModel;

        /// <summary>
        /// Initialize a new instance of the ToggleRunOnStartupCommand class.
        /// </summary>
        /// <param name="viewModel"></param>
        public ToggleRunOnStartupCommand(BackupInfoViewModel viewModel) {
            _ViewModel = viewModel;
        }

        #region ICommand members
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            // Call the static method to toggle addition or deletion of tash scheduler job.
            ProgramOptionsManager.ToggleRunOnStartup(_ViewModel.IsRunOnStartupEnabled);
        }
        #endregion
    }
}
