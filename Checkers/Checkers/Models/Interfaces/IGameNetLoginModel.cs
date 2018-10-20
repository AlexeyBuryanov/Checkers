using Checkers.Models.Network;
using Checkers.Models.Network.DBML;
using Checkers.Views;


namespace Checkers.Models.Interfaces
{
    /// <summary>
    /// Интерфейс модели входа.
    /// </summary>
    public interface IGameNetLoginModel
    {
        /// <summary>
        /// Визуальная часть. 
        /// </summary>
        GameNetLoginView View { get; set; }
        /// <summary>
        /// Контекст данных базы данных CheckersDB. 
        /// </summary>
        CheckersDBDataContext Db { get; }
        /// <summary>
        /// Данные для передачи в окно лобби, чтобы открыть 
        /// окно лобби и пытаться соедениться с сервером.
        /// </summary>
        LoginData LoginData { get; set; }

        /// <summary>
        /// Инициализация визуальной части.
        /// </summary>
        void InitializeView(GameNetLoginView view);
        /// <summary>
        /// Сверяет данные в БД с введёнными.
        /// </summary>
        void Login();
    } // IGameNetLoginModel interface
} // Checkers.Models.Interfaces