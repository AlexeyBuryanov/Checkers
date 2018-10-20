using System.Threading.Tasks;
using Checkers.Models.Network;
using Checkers.Models.Network.GameServer;
using Checkers.Views;


namespace Checkers.Models.Interfaces
{
    /// <summary>
    /// Интерфейс модели лобби.
    /// </summary>
    public interface IGameNetLobbyModel
    {
        /// <summary>
        /// Визуальная часть. 
        /// </summary>
        GameNetLobbyView View { get; set; }
        /// <summary>
        /// Данные для входа. Получаем данные из окна предзагрузчика, 
        /// если пройдём проверку на логин и пароль.
        /// </summary>
        LoginData LoginData { get; set; }
        /// <summary>
        /// Клиент службы WCF.
        /// </summary>
        ICheckersSvc Client { get; set; }

        /// <summary>
        /// Инициализация визуальной части.
        /// </summary>
        void InitializeView(GameNetLobbyView view);
        /// <summary>
        /// Соединение с сервером.
        /// </summary>
        Task ConnectToServerAsync();
    } // IGameNetLobbyModel interface
} // Checkers.Models.Interfaces