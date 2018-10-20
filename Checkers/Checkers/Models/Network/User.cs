using System.Runtime.Serialization;
using Checkers.Models.Network.GameServer;


namespace Checkers.Models.Network
{
    /// <summary>
    /// Пользователь для работы с Callback.
    /// </summary>
    [DataContract]
    public class User
    {
        /// <summary>
        /// Для функций обратного вызова.
        /// </summary>
        [DataMember]
        public ICheckersSvcCallback Callback;

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        [DataMember]
        public string UserName;
    } // User class
} // Checkers.Models.Network