using System;
using System.Windows.Input;
using System.Diagnostics; 

namespace PL
{
    public class RelayCommand : ICommand
    {
        private readonly Func<object?, Task>? _executeAsync;
        private readonly Action<object?>? _execute;
        private readonly Predicate<object?>? _canExecutePredicate;
        private readonly Func<bool>? _canExecuteFunc;

        public RelayCommand(Func<object?, Task> executeAsync, Predicate<object?>? canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecutePredicate = canExecute;
        }

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecutePredicate = canExecute;
        }

        public RelayCommand(Action<object?> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecuteFunc = canExecute;
        }

        public RelayCommand(Func<object?, Task> executeAsync, Func<bool>? canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecuteFunc = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (_canExecutePredicate != null)
            {
                return _canExecutePredicate(parameter);
            }
            if (_canExecuteFunc != null)
            {
                return _canExecuteFunc();
            }
            return true;
        }

        public async void Execute(object? parameter)
        {
            try
            {
                if (_executeAsync != null)
                {
                    await _executeAsync(parameter);
                }
                else if (_execute != null)
                {
                    _execute(parameter);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RelayCommand.ExecuteAsync исключение: {ex.Message}");
                Debug.WriteLine($"Стек вызова: {ex.StackTrace}");
            }

            if (_canExecutePredicate != null || _canExecuteFunc != null)
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}