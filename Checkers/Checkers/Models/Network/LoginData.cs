namespace Checkers.Models.Network
{
    /// <summary>
    /// Данные для входа.
    /// </summary>
    public class LoginData
    {
        /// <summary>
        /// Имя хоста или IP-адрес.
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// Порт к которому подключается пользователь.
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string UserName { get; set; }
    } // LoginData class
} // Checkers.Models.Network