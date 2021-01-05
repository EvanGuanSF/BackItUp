using System;
using System.Windows.Input;

namespace BackItUp.ViewModels.Commands
{
    internal class DeleteBackupItemCommand : ICommand
    {
        private readonly BackupInfoViewModel _ViewModel;

        /// <summary>
        /// Initialize a new instance of the DeleteBackupItemCommand class.
        /// </summary>
        /// <param name="viewModel"></param>
        public DeleteBackupItemCommand(BackupInfoViewModel viewModel) {
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
            return _ViewModel.CanDeleteItem;
        }

        public void Execute(object parameter)
        {
            _ViewModel.DeleteBackupItemBySelectedIndex();
        }
        #endregion
    }
}
