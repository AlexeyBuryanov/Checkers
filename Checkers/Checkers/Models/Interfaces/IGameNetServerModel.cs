using Checkers.Views;


namespace Checkers.Models.Interfaces
{
    /// <summary>
    /// Интерфейс модели серверной части.
    /// </summary>
    public interface IGameNetServerModel
    {
        /// <summary>
        /// Визуальная часть. 
        /// </summary>
        GameNetServerView View { get; set; }

        /// <summary>
        /// Инициализация визуальной части.
        /// </summary>
        void InitializeView(GameNetServerView view);
        /// <summary>
        /// Асинхронно запускает сервер.
        /// </summary>
        bool StartServerAsync();
        /// <summary>
        /// Асинхронно останавливает сервер.
        /// </summary>
        bool StopServerAsync();
        /// <summary>
        /// Сворачивание в трей.
        /// </summary>
        void ToTray(string balloonTipText);
    } // IGameNetServerModel interface
} // Checkers.Models.Interfaces