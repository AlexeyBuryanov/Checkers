using Checkers.Models.Interfaces;
using Checkers.Services;


namespace Checkers.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// View Model для окна сервера. 
    /// <see cref="T:Checkers.Views.GameNetServerView" />.
    /// </summary>
    public sealed class GameNetServerViewModel : BaseViewModel
    {
        /// <summary>
        /// Модель сервера.
        /// </summary>
        private readonly IGameNetServerModel _gameNetServerModel;
        /// <summary>
        /// Стартовал ли сервер.
        /// </summary>
        private bool _isStart;


        public GameNetServerViewModel(IGameNetServerModel gameNetServerModel)
        {
            _gameNetServerModel = gameNetServerModel;

            StatusText = "Готово.";
            _isStart = false;

            //***** СОЗДАНИЕ КОМАНД *****//
            ExitCommand = new RelayCommand(Exit);
            ToTrayCommand = new RelayCommand(ToTray);
            StartStopServerCommand = new RelayCommand(StartStopServer);
        } // GameNetServerViewModel ctor


        //********************************* КОМАНДЫ ***********************************//
        /// <summary>Запускает или останавливает сервер</summary>
        public RelayCommand StartStopServerCommand { get; set; }
        private void StartStopServer(object obj)
        {
            if (!_isStart) {
                _gameNetServerModel.StartServerAsync();
                _isStart = true;
            } else {
                _gameNetServerModel.StopServerAsync();
                _isStart = false;
            } // if-else
        } // StartStopServer


        /// <summary>В трей</summary>
        public RelayCommand ToTrayCommand { get; set; }
        private void ToTray(object obj)
        {
            _gameNetServerModel.ToTray(_isStart ? "Сервер сейчас запущен." : "Сервер неактивен.");
        } // ToTray


        /// <summary>Выход</summary>
        public RelayCommand ExitCommand { get; set; }
        private void Exit(object obj)
        {
            // Останавливаем сервер, если запущен
            if (_isStart) {
                _gameNetServerModel.StopServerAsync();
            } // if
            // Закрываем окно
            _gameNetServerModel.View.Close();
            //Application.Current.Windows.OfType<GameNetServerView>().FirstOrDefault()?.Close();
        } // Exit


        //******************** СВОЙСТВА СВЯЗАННЫЕ С ИНТЕРФЕЙСОМ ***********************//
        /// <summary>Текст в статус-баре</summary>
        private string _statusText;
        public string StatusText {
            get => _statusText;
            set {
                _statusText = value;
                OnPropertyChanged();
            } // set
        } // StatusText
    } // GameNetServerViewModel class
} // Checkers.ViewModels