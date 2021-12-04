using System;
using System.Windows.Input;

namespace RegexTester.Commands
{
    class RelayCommand : ICommand
    {
        private Action<object?> _execute;

        private Predicate<object?> _canExecute;

        private event EventHandler? CanExecuteChangedInternal;

        public RelayCommand(Action<object?> execute)
            : this(execute, DefaultCanExecute)
        {
        }

        public RelayCommand(Action<object?> execute, Predicate<object?> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public event EventHandler? CanExecuteChanged
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

        public bool CanExecute(object? parameter)
        {
            return this._canExecute != null && this._canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        public void OnCanExecuteChanged()
        {
            EventHandler? handler = CanExecuteChangedInternal;
            if (handler is null)
                return;
            
            //Application.Current.Dispatcher.BeginInvoke(new InvokeDelegate(() => { handler.Invoke(this, EventArgs.Empty); }));
            //DispatcherHelper.BeginInvokeOnUIThread(() => handler.Invoke(this, EventArgs.Empty));
            //handler.Invoke(this, EventArgs.Empty);
            handler(this, EventArgs.Empty);
        }

        public void Destroy()
        {
            _canExecute = _ => false;
            _execute = _ => { return; };
        }

        private static bool DefaultCanExecute(object? parameter) => true;
    }
}
