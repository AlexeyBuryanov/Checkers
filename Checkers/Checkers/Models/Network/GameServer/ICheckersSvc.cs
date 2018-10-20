using System.ServiceModel;


namespace Checkers.Models.Network.GameServer
{
    /// <summary>
    /// Контракт службы WCF.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(ICheckersSvcCallback))]
    public interface ICheckersSvc
    {
        [OperationContract]
        void AddUserToLobbyList(string userName);

        [OperationContract]
        void DeleteUserFromLobbyList(string userName);

        [OperationContract]
        void SendMessageToAll(string msg, string sender);

        [OperationContract]
        void SendMessageTo(string msg, string from, string to);

        [OperationContract]
        void SendInviteTo((string, string) from, (string, string) to, string firstMove);

        [OperationContract]
        void AcceptInvite((string, string) from, (string, string) to, string firstMove);

        [OperationContract]
        void MakeMove(string player1, string player2, int row1, int col1, int row2, int col2);

        [OperationContract]
        void StartNewGame(string player1, string player2);

        [OperationContract]
        void TestConnection();
    } // ICheckersSvc interface
} // Checkers.Models.GameServer