using Checkers.Models.Interfaces;
using Checkers.Views;


namespace Checkers.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Модель предзагрузчика. Отсюда происходит запуск игр. 
    /// </summary>
    public sealed class PreloaderModel : IPreloaderModel
    {
        /// <summary>
        /// Модель игры с компьютером.
        /// </summary>
        private readonly IGameWithAiModel _gameWithAiModel;
        /// <summary>
        /// Модель сервера.
        /// </summary>
        private readonly IGameNetServerModel _gameNetServerModel;
        /// <summary>
        /// Модель входа на сервер.
        /// </summary>
        private readonly IGameNetLoginModel _gameNetLoginModel;
        /// <summary>
        /// Модель лобби.
        /// </summary>
        private readonly IGameNetLobbyModel _gameNetLobbyModel;


        public PreloaderModel(IGameWithAiModel gameWithAiModel, IGameNetServerModel gameNetServerModel, 
                              IGameNetLoginModel gameNetLoginModel, IGameNetLobbyModel gameNetLobbyModel)
        {
            // Получаем соответствующие модели через параметры.
            _gameWithAiModel = gameWithAiModel;
            _gameNetServerModel = gameNetServerModel;
            _gameNetLoginModel = gameNetLoginModel;
            _gameNetLobbyModel = gameNetLobbyModel;
        } // GamePreloaderModel


        /// <inheritdoc />
        /// <summary>
        /// Начинает игру с компьютером.
        /// </summary>
        public void StartNewGameWithAi()
        {
            _gameWithAiModel.InitializeView(new GameWithAiView());
            _gameWithAiModel.View.Show();
        } // StartNewGameWithAI


        /// <inheritdoc />
        /// <summary>
        /// Запускает окно сервера.
        /// </summary>
        public void StartServer()
        {
            _gameNetServerModel.InitializeView(new GameNetServerView());
            _gameNetServerModel.View.Show();
        } // StartServer


        /// <inheritdoc />
        /// <summary>
        /// Запускает клиент.
        /// </summary>
        public async void StartClientAsync()
        {
            _gameNetLoginModel.InitializeView(new GameNetLoginView());
            var result = _gameNetLoginModel.View.ShowDialog();
            
            if (result != null && result.Value) {
                _gameNetLobbyModel.LoginData = _gameNetLoginModel.LoginData;
                _gameNetLobbyModel.InitializeView(new GameNetLobbyView());
                _gameNetLobbyModel.View.Show();
                await _gameNetLobbyModel.ConnectToServerAsync();
            } // if
        } // StartClientAsync
    } // GamePreloaderModel class
} // Checkers.Model