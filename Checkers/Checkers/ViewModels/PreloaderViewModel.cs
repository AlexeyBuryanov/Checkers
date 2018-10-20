using System.Linq;
using System.Windows;
using Checkers.Models.Interfaces;
using Checkers.Services;
using Checkers.Views;


namespace Checkers.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// View Model для окна предзагрузки 
    /// <see cref="T:Checkers.Views.PreloaderView" />.
    /// </summary>
    public sealed class PreloaderViewModel : BaseViewModel
    {
        /// <summary>
        /// Модель предзагрузчика.
        /// </summary>
        private readonly IPreloaderModel _gamePreloaderModel;

        
        public PreloaderViewModel(IPreloaderModel gamePreloaderModel)
        {
            _gamePreloaderModel = gamePreloaderModel;

            //***** СОЗДАНИЕ КОМАНД *****//
            ExitCommand = new RelayCommand(Exit);
            GameWithComputerCommand = new RelayCommand(GameWithComputer);
            OnlineGameClientCommand = new RelayCommand(OnlineGameClient);
            OnlineGameServerCommand = new RelayCommand(OnlineGameServer);
        } // PreloaderViewModel ctor


        //********************************* КОМАНДЫ ***********************************//
        /// <summary>Выход</summary>
        public RelayCommand ExitCommand { get; set; }
        private static void Exit(object obj)
        {
            Application.Current.Shutdown();
        } // Exit


        /// <summary>Игра с компьютером</summary>
        public RelayCommand GameWithComputerCommand { get; set; }
        private void GameWithComputer(object obj)
        {
            _gamePreloaderModel.StartNewGameWithAi();
        } // GameWithComputer


        /// <summary>Игра по сети - клиентская часть</summary>
        public RelayCommand OnlineGameClientCommand { get; set; }
        private void OnlineGameClient(object obj)
        {
            var view = Application.Current.Windows
                                  .OfType<PreloaderView>()
                                  .FirstOrDefault();
            if (view != null) {
                view.ButtonOnlineGameServer.IsEnabled = false;
                view.ButtonOnlineGameClient.IsEnabled = false;
            } // if

            _gamePreloaderModel.StartClientAsync();
        } // OnlineGameClientCommand


        /// <summary>Игра по сети - серверная часть</summary>
        public RelayCommand OnlineGameServerCommand { get; set; }
        private void OnlineGameServer(object obj)
        {
            var view = Application.Current.Windows
                                  .OfType<PreloaderView>()
                                  .FirstOrDefault();
            if (view != null) {
                view.ButtonOnlineGameServer.IsEnabled = false;
                view.ButtonOnlineGameClient.IsEnabled = false;
            } // if

            _gamePreloaderModel.StartServer();
        } // OnlineGameServer
    } // PreloaderViewModel
} // Checkers.ViewModel