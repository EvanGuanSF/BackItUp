using BackItUp.ViewModels.Serialization;
using System;
using System.Windows.Input;

namespace BackItUp.ViewModels.Commands
{
    internal class LoadConfigCommand : ICommand
    {
        private readonly BackupInfoViewModel _ViewModel;

        /// <summary>
        /// Initialize a new instance of the LoadConfigCommand class.
        /// </summary>
        /// <param name="viewModel"></param>
        public LoadConfigCommand(BackupInfoViewModel viewModel) {
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
            return Serializer.IsSerializerIdle;
        }

        public void Execute(object parameter)
        {
            _ViewModel.LoadConfig();
        }
        #endregion
    }
}
