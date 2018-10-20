using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;


namespace Checkers.Models.Network.GameServer
{
    /// <inheritdoc />
    /// <summary>
    /// Непосредственно сама служба ServiceHost.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    [CallbackBehavior(IncludeExceptionDetailInFaults = true)]
    public class CheckersSvc : ICheckersSvc
    {
        /// <summary>
        /// Экземпляр клиента вызывающий текущие операции.
        /// </summary>
        private ICheckersSvcCallback Callback { get; set; }
        /// <summary>
        /// Список пользователей (игроков).
        /// </summary>
        private List<User> UserList { get; set; }


        public CheckersSvc()
        {
            UserList = new List<User>();
        } // CheckersSvc ctor


        /// <summary>Добавляет нового юзера в список в сети</summary>
        /// <param name="userName">Ник игрока</param>
        public async void AddUserToLobbyList(string userName)
        {
            Callback = OperationContext.Current.GetCallbackChannel<ICheckersSvcCallback>();
            var user = new User {
                UserName = userName,
                Callback = Callback
            };
            UserList.Add(user);

            // Формируем список имён всех пользователей
            var namesLst = UserList.Select(usr => usr.UserName).ToList();

            // Перебираем пользователей и шлём им новый список имён
            await SendListToAll(namesLst);
        } // AddUserToLobbyList


        /// <summary>Удаляет юзера со списка в сети"</summary>
        /// <param name="userName">Ник игрока</param>
        public async void DeleteUserFromLobbyList(string userName)
        {
            // Формируем список без пользователя, что вышел
            UserList = UserList
                       .Where(u => u.UserName != userName)
                       .ToList();

            // Формируем новый список имён
            var namesLst = UserList
                           .Select(usr => usr.UserName)
                           .ToList();

            // Перебираем пользователей, что остались и шлём им новый список имён
            await SendListToAll(namesLst);
        } // DeleteUserFromLobbyList


        /// Служебный метод для AddUserToLobbyList и DeleteUserFromLobbyList
        /// <summary>Перебирает всех пользователей и по Callback-вызову обновляет список тех, кто в сети</summary>
        /// <param name="userList">Список, который отправляется</param>
        public Task SendListToAll(List<string> userList)
        {
            return Task.Factory.StartNew(() => {
                UserList
                    .ForEach(user => {
                        user.Callback?.SendUserListCallback(userList);
                    });
            });
        } // SendListToAll


        /// <summary>Отправляет сообщение всем юзерам (кроме отправителя)</summary>
        /// <param name="msg">Сообщение</param>
        /// <param name="sender">Отправитель</param>
        public void SendMessageToAll(string msg, string sender)
        {
            UserList.ForEach(user => {
                if (sender != user.UserName) {
                    user.Callback.SendMessageCallback(msg);
                } // if
            });
        } // SendMessageToAll


        /// <summary>Отправляет сообщение конкретно кому-то (кроме отправителя)</summary>
        /// <param name="msg">Сообщение</param>
        /// <param name="from">Отправитель</param>
        /// /// <param name="to">Кому отправляется</param>
        public void SendMessageTo(string msg, string from, string to)
        {
            UserList.ForEach(user => {
                if (from != user.UserName && to == user.UserName) {
                    user.Callback.SendMessageCallback(msg);
                } // if
            });
        } // SendMessageToAll


        /// <summary>Отправляет приглашение вступить в игру</summary>
        /// <param name="from">От кого</param>
        /// <param name="to">Кому</param>
        /// <param name="firstMove">Первый ходит</param>
        public async void SendInviteTo((string, string) from, (string, string) to, string firstMove)
        {
            await Task.Factory.StartNew(() => {
                foreach (var user in UserList.Where(u => u.UserName == to.Item1)) {
                    Callback = user.Callback;
                } // foreach
                Callback?.SendInviteCallback(from, to, firstMove);
            });
        } // SendInviteTo


        /// <summary>
        /// Принятие приглашения в игру.
        /// </summary>
        /// <param name="from">От кого и каким цветом играет</param>
        /// <param name="to">Кому и каким цветом играет</param>
        /// <param name="firstMove">Кто ходит первым</param>
        public async void AcceptInvite((string, string) from, (string, string) to, string firstMove)
        {
            await Task.Factory.StartNew(() => {
                var players = new User[2];

                UserList.ForEach(user => {
                    if (from.Item1 == user.UserName) {
                        players[0] = user;
                    } // if
                    if (to.Item1 == user.UserName) {
                        players[1] = user;
                    } // if
                });
                players[0].Callback.AcceptInviteCallback(from, to, firstMove);
                players[1].Callback.AcceptInviteCallback(from, to, firstMove);
            });
        } // AcceptInvite


        /// <summary>
        /// Делает ход у обоих игроков.
        /// </summary>
        /// <param name="player1">Игрок 1</param>
        /// <param name="player2">Игрок 2</param>
        /// <param name="row1">Фигура 1 строка</param>
        /// <param name="col1">Фигура 1 столбец</param>
        /// <param name="row2">Фигура 2 строка</param>
        /// <param name="col2">Фигура 2 столбец</param>
        public async void MakeMove(string player1, string player2, int row1, int col1, int row2, int col2)
        {
            await Task.Factory.StartNew(() => {
                var p1 = new User();
                var p2 = new User();

                foreach (var user in UserList) {
                    if (user.UserName == player1) {
                        p1 = user;
                    } // if
                    if (user.UserName == player2) {
                        p2 = user;
                    } // if
                } // foreach

                p1.Callback?.MakeMoveCallback(row1, col1, row2, col2);
                p2.Callback?.MakeMoveCallback(row1, col1, row2, col2);
            });
        } // MakeMove


        public async void StartNewGame(string player1, string player2)
        {
            await Task.Factory.StartNew(() => {
                var p1 = new User();
                var p2 = new User();

                foreach (var user in UserList) {
                    if (user.UserName == player1) {
                        p1 = user;
                    } // if
                    if (user.UserName == player2) {
                        p2 = user;
                    } // if
                } // foreach

                p1.Callback?.StartNewGameCallback();
                p2.Callback?.StartNewGameCallback();
            });
        } // StartNewGame


        public void TestConnection() {}
    } // CheckersSvc class
} // Checkers.Models.Network.GameServer