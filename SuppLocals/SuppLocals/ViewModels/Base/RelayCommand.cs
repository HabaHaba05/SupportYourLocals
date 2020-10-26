using SuppLocals.Utilities.Helpers;
using System;
using System.Windows.Input;

namespace SuppLocals{
    public class RelayCommand<T> : ICommand
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _execute;


        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute(parameter);
            }

            public void Execute(object parameter)
            {
                _execute(parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }
        }

        public class RelayCommand : ICommand
        {
            private readonly Func<object, bool> _canExecute;
            private readonly Action<object> _execute;

            public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute;
            }


            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute(parameter);
            }

            public void Execute(object parameter)
            {
                _execute(parameter);
            }

            // Ensures WPF commanding infrastructure asks all RelayCommand objects whether their
            // associated views should be enabled whenever a command is invoked 
            public event EventHandler CanExecuteChanged
            {
                add
                {
                    CommandManager.RequerySuggested += value;
                    CanExecuteChangedInternal += value;
                }
                remove
                {
                    CommandManager.RequerySuggested -= value;
                    CanExecuteChangedInternal -= value;
                }
            }

            private event EventHandler CanExecuteChangedInternal;

            public void RaiseCanExecuteChanged()
            {
                CanExecuteChangedInternal.Raise(this);
            }
        }
    }
