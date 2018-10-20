namespace Checkers.Models.Interfaces
{
    /// <summary>
    /// Интерфейс модели окна предзагрузки.
    /// </summary>
    public interface IPreloaderModel
    {
        /// <summary>
        /// Начало игры с компьютером.
        /// </summary>
        void StartNewGameWithAi();
        /// <summary>
        /// Запускает окно сервера.
        /// </summary>
        void StartServer();
        /// <summary>
        /// Запускает клиент.
        /// </summary>
        void StartClientAsync();
    } // IGamePreloaderModel
} // Checkers.Model.Interfaces