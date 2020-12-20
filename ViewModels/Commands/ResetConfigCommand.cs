using BackItUp.ViewModels.Serialization;
using System;
using System.Windows.Input;

namespace BackItUp.ViewModels.Commands
{
    internal class ResetConfigCommand : ICommand
    {
        private BackupInfoViewModel _ViewModel;

        /// <summary>
        /// Initialize a new instance of the DeleteBackupItemCommand class.
        /// </summary>
        /// <param name="viewModel"></param>
        public ResetConfigCommand(BackupInfoViewModel viewModel) {
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
            return Serializer.isSerializerIdle;
        }

        public void Execute(object parameter)
        {
            _ViewModel.ResetConfig();
        }
        #endregion
    }
}
