using BackItUp.ViewModels.Serialization;
using System;
using System.Windows.Input;

namespace BackItUp.ViewModels.Commands
{
    internal class SaveConfigCommand : ICommand
    {
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
            BackupInfoViewModel.SaveConfig();
        }
        #endregion
    }
}
