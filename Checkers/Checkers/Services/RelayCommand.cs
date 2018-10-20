using System;
using System.Windows.Input;


namespace Checkers.Services
{
    /// <inheritdoc />
    /// <summary>
    /// Кастомная команда. Класс реализует стандартный интерфейс ICommand описывающий команду.
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// Действие.
        /// </summary>
        private readonly Action<object> _execute;
        /// <summary>
        /// Может ли действие выполняться.
        /// </summary>
        private readonly Func<object, bool> _canExecute;

        /// <inheritdoc />
        /// <summary>
        /// Вызывается при изменении условий, указывающих, может ли команда выполняться.
        /// </summary>
        public event EventHandler CanExecuteChanged {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        } // CanExecuteChanged

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute    = execute;
            _canExecute = canExecute;
        } // RelayCommand ctor

        /// <inheritdoc />
        /// <summary>
        /// Определяет, может ли команда выполняться.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        } // CanExecute

        /// <inheritdoc />
        /// <summary>
        /// Выполняет логику команды.
        /// </summary>
        public void Execute(object parameter)
        {
            _execute(parameter);
        } // Execute
    } // class RelayCommand
} // Checkers.Model