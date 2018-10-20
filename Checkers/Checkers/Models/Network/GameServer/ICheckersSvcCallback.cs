using System.Collections.Generic;
using System.ServiceModel;


namespace Checkers.Models.Network.GameServer
{
    /// <summary>
    /// Представляет контракт обратного вызова. 
    /// </summary>
    [ServiceContract]
    public interface ICheckersSvcCallback
    {
        [OperationContract]
        void SendMessageCallback(string msg);

        [OperationContract]
        void SendUserListCallback(List<string> userList);

        [OperationContract]
        void SendInviteCallback((string, string) from, (string, string) to, string firstMove);

        [OperationContract]
        void AcceptInviteCallback((string, string) from, (string, string) to, string firstMove);

        [OperationContract]
        void MakeMoveCallback(int row1, int col1, int row2, int col2);

        [OperationContract]
        void StartNewGameCallback();
    } // ICheckersSvcCallback interface
} // Checkers.Models.GameServer