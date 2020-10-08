using System;
using System.Windows.Input;

namespace SuppLocals
{
    public class CommandHandler : ICommand
    {
        private readonly Action _action;

        public event EventHandler CanExecuteChanged;

        public CommandHandler(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
