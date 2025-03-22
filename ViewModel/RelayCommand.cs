using System;
using System.Windows.Input;

namespace ShoppingCart.ViewModel
{
    public class RelayCommand : ICommand
    {

        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        private bool _canExecuteState = true;
        public event EventHandler CanExecuteChanged;
 

        public RelayCommand(Action execute, Func<bool> canExecute = null)
            : this(param => execute(), canExecute == null ? (Predicate<object>)null : param => canExecute())
        { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        //public void ToggleCanExecute()
        //{
        //    _canExecuteState = !_canExecuteState;
        //    CommandManager.InvalidateRequerySuggested();  // Notify UI to refresh command states
        //}

        public void ToggleCanExecute()
        {
            _canExecuteState = !_canExecuteState;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        //private readonly Action<T> _execute;
        //private readonly Func<T, bool>? _canExecute;

        //public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
        //{
        //    _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        //    _canExecute = canExecute;
        //}

        //public bool CanExecute(object? parameter) => _canExecute?.Invoke((T)parameter) ?? true;
        //public void Execute(object? parameter) => _execute((T)parameter);
        //public event EventHandler? CanExecuteChanged;

    }
}

