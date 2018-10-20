using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Checkers.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// Все ViewModel должны наследоваться от данного класса, чтобы
    /// не переопределять в каждой ViewModel интерфейс INotifyPropertyChanged, 
    /// реализация которого нужна для отслеживания события, какие именно свойства 
    /// изменились и какие нужно перерисовать.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } // OnPropertyChanged
    } // BaseViewModel class
} // Checkers.ViewModel