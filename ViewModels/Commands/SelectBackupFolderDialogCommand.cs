using System;
using System.Windows.Input;

namespace BackItUp.ViewModels.Commands
{
    internal class SelectBackupFolderDialogCommand : ICommand
    {
        private BackupInfoViewModel _ViewModel;

        /// <summary>
        /// Initialize a new instance of the SelectBackupFolderDialogCommand class.
        /// </summary>
        /// <param name="viewModel"></param>
        public SelectBackupFolderDialogCommand(BackupInfoViewModel viewModel) {
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
            _ViewModel.GetNewBackupFolderPath();
        }
        #endregion
    }
}
