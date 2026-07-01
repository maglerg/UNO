using System;
using System.Windows.Input;

namespace ClientUNO
{
    public class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool>? canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            execute();
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }


    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Func<T, bool>? canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter is T value)
                return canExecute?.Invoke(value) ?? true;

            return false;
        }

        public void Execute(object? parameter)
        {
            if (parameter is T value)
                execute(value);
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}