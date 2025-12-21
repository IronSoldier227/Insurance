// PL/RelayCommand.cs
using System;
using System.Windows.Input;

namespace PL
{
    public class RelayCommand : ICommand
    {
        private readonly Func<object?, Task>? _executeAsync; // Добавим асинхронную версию
        private readonly Action<object?>? _execute;
        private readonly Predicate<object?>? _canExecute;
        private readonly Func<bool>? _canExecuteFunc; // Новый делегат

        // Конструктор для асинхронной команды
        public RelayCommand(Func<object?, Task> executeAsync, Predicate<object?>? canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        // Конструктор для синхронной команды (для обратной совместимости)
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object?> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecuteFunc = canExecute;
        }

        // НОВЫЙ Конструктор для асинхронной команды с Func<bool>
        public RelayCommand(Func<object?, Task> executeAsync, Func<bool>? canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecuteFunc = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public async void Execute(object? parameter)
        {
            if (_executeAsync != null)
            {
                await _executeAsync(parameter);
            }
            else if (_execute != null)
            {
                _execute(parameter);
            }
            // Обновляем CanExecute после выполнения, если есть canExecute
            if (_canExecute != null)
            {
                CommandManager.InvalidateRequerySuggested(); // Уведомляем WPF перепроверить CanExecute
            }
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}