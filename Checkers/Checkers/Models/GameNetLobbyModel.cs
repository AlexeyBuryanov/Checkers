using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Checkers.Models.GameCore;
using Checkers.Models.Interfaces;
using Checkers.Models.Network;
using Checkers.Models.Network.DBML;
using Checkers.Models.Network.GameServer;
using Checkers.Services;
using Checkers.ViewModels;
using Checkers.Views;


namespace Checkers.Models
{
    /// <summary>
    /// Сетевая игра, модель лобби.
    /// </summary>
    public class GameNetLobbyModel : IGameNetLobbyModel, ICheckersSvcCallback
    {
        /// <inheritdoc />
        /// <summary>
        /// Визуальная часть. 
        /// </summary>
        public GameNetLobbyView View { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Данные для входа. Получаем данные из окна предзагрузчика, 
        /// если пройдём проверку на логин и пароль.
        /// </summary>
        public LoginData LoginData { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Клиент службы WCF.
        /// </summary>
        public ICheckersSvc Client { get; set; }

        /// <summary>
        /// Контекст БД.
        /// </summary>
        private CheckersDBDataContext Db { get; }
        /// <summary>
        /// Модель клиента.
        /// </summary>
        private IGameNetClientModel _clientModel;


        public GameNetLobbyModel()
        {
            Db = new CheckersDBDataContext();
            var timerCheckConnect = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timerCheckConnect.Tick += TimerCheckConnect_Tick;
        } // GameNetLobbyModel


        private async void TimerCheckConnect_Tick(object sender, EventArgs args)
        {
            try {
                Client.TestConnection();
            } catch (CommunicationException e) {
                Console.WriteLine(e.Message);
                await View.Dispatcher.InvokeAsync(() => {
                    View.GridWorkSpace.Visibility = Visibility.Collapsed;
                    View.StackPanelLoading.Visibility = Visibility.Visible;
                    View.ProgressRing.IsActive = false;
                    View.ProgressTextBlock.Text = "Не удаётся подключиться к серверу. Возможно сервер недоступен.";
                });
            } // try-catch
        } // TimerCheckConnect_Tick


        /// <inheritdoc />
        /// <summary>
        /// Инициализация визуальной части.
        /// </summary>
        public void InitializeView(GameNetLobbyView view)
        {
            View = view;
            View.Title += $" | {LoginData.UserName}";
            View.GridWorkSpace.Visibility = Visibility.Collapsed;
            View.ProgressRing.IsActive = true;
            // Настройка чек-боксов диалогового окна приглашения
            View.CbDialogInviteYouBlack.Click += (s, a) => {
                Debug.Assert(View.CbDialogInviteYouBlack.IsChecked != null, "View.CbDialogInviteYouBlack.IsChecked != null");
                View.CbDialogInviteYouWhite.IsEnabled = !View.CbDialogInviteYouBlack.IsChecked.Value;
                View.CbDialogInviteInvitedBlack.IsEnabled = !View.CbDialogInviteYouBlack.IsChecked.Value;
                View.CbDialogInviteInvitedWhite.IsChecked = View.CbDialogInviteYouBlack.IsChecked.Value;
            };
            View.CbDialogInviteYouWhite.Click += (s, a) => {
                Debug.Assert(View.CbDialogInviteYouWhite.IsChecked != null, "View.CbDialogInviteYouWhite.IsChecked != null");
                View.CbDialogInviteYouBlack.IsEnabled = !View.CbDialogInviteYouWhite.IsChecked.Value;
                View.CbDialogInviteInvitedWhite.IsEnabled = !View.CbDialogInviteYouWhite.IsChecked.Value;
                View.CbDialogInviteInvitedBlack.IsChecked = View.CbDialogInviteYouWhite.IsChecked.Value;
            };
            View.CbDialogInviteInvitedBlack.Click += (s, a) => {
                Debug.Assert(View.CbDialogInviteInvitedBlack.IsChecked != null, "View.CbDialogInviteInvitedBlack.IsChecked != null");
                View.CbDialogInviteInvitedWhite.IsEnabled = !View.CbDialogInviteInvitedBlack.IsChecked.Value;
                View.CbDialogInviteYouBlack.IsEnabled = !View.CbDialogInviteInvitedBlack.IsChecked.Value;
                View.CbDialogInviteYouWhite.IsChecked = View.CbDialogInviteInvitedBlack.IsChecked.Value;
            };
            View.CbDialogInviteInvitedWhite.Click += (s, a) => {
                Debug.Assert(View.CbDialogInviteInvitedWhite.IsChecked != null, "View.CbDialogInviteInvitedWhite.IsChecked != null");
                View.CbDialogInviteInvitedBlack.IsEnabled = !View.CbDialogInviteInvitedWhite.IsChecked.Value;
                View.CbDialogInviteYouWhite.IsEnabled = !View.CbDialogInviteInvitedWhite.IsChecked.Value;
                View.CbDialogInviteYouBlack.IsChecked = View.CbDialogInviteInvitedWhite.IsChecked.Value;
            };
            View.CbDialogInviteFirstMoveBlack.Click += (s, a) => {
                Debug.Assert(View.CbDialogInviteFirstMoveBlack.IsChecked != null, "View.CbDialogInviteFirstMoveBlack.IsChecked != null");
                View.CbDialogInviteFirstMoveWhite.IsEnabled = !View.CbDialogInviteFirstMoveBlack.IsChecked.Value;
            };
            View.CbDialogInviteFirstMoveWhite.Click += (s, a) => {
                Debug.Assert(View.CbDialogInviteFirstMoveWhite.IsChecked != null, "View.CbDialogInviteFirstMoveWhite.IsChecked != null");
                View.CbDialogInviteFirstMoveBlack.IsEnabled = !View.CbDialogInviteFirstMoveWhite.IsChecked.Value;
            };
            // Индивидуальный чат с помощью отдельных вкладок TabControl
            //View.ListBoxUsers.MouseDoubleClick += (s, a) => {

            //};
        } // InitializeView


        /// <inheritdoc />
        /// <summary>
        /// Соединение с сервером.
        /// </summary>
        public Task ConnectToServerAsync()
        {
            return Task.Factory.StartNew(async () => {
                // Для связи используем протокол TCP-IP
                var netTcpBinding = new NetTcpBinding {
                    CloseTimeout = new TimeSpan(0, 0, 2, 0),
                    OpenTimeout = new TimeSpan(0, 0, 2, 0),
                    ReceiveTimeout = new TimeSpan(0, 0, 10, 0),
                    SendTimeout = new TimeSpan(0, 0, 2, 0),
                    TransactionFlow = false,
                    TransferMode = TransferMode.Buffered,
                    TransactionProtocol = TransactionProtocol.OleTransactions,
                    HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                    // Максимум игроков в очереди на подключение
                    ListenBacklog = 10,
                    MaxBufferPoolSize = 1000000,
                    MaxBufferSize = 2055360000,
                    MaxReceivedMessageSize = 2055360000,
                    // Максимум игроков 20
                    MaxConnections = 20
                };

                // Формируем callback-context
                var context = new InstanceContext(this);

                // Создание фабрики
                var channelFactory =
                    new DuplexChannelFactory<ICheckersSvc>(context, netTcpBinding,
                        new EndpointAddress($@"net.tcp://{LoginData.HostName}:{LoginData.Port}/Checkers/Game"));

                try {
                    // Создание канала для связи с сервисом
                    Client = channelFactory.CreateChannel();

                    View.Closing += (s, a) => {
                        try {
                            Client.DeleteUserFromLobbyList(LoginData.UserName);
                        } catch (CommunicationObjectFaultedException e) {
                            Console.WriteLine(e.Message);
                        } // try-catch
                    };

                    // Добавляем вновь подключившегося пользователя в лобби
                    Client.AddUserToLobbyList(LoginData.UserName);
                    await View.Dispatcher.InvokeAsync(() => {
                        View.GridWorkSpace.Visibility = Visibility.Visible;
                        View.StackPanelLoading.Visibility = Visibility.Collapsed;
                        View.ProgressRing.IsActive = false;
                    });
                } catch (EndpointNotFoundException e) {
                    await View.Dispatcher.InvokeAsync(() => {
                        Console.WriteLine(e.Message);
                        View.ProgressRing.IsActive = false;
                        View.ProgressTextBlock.Text = "Не удаётся подключиться к серверу игры. Возможно сервер недоступен.";
                    });
                } catch (UriFormatException e) {
                    await View.Dispatcher.InvokeAsync(() => {
                        Console.WriteLine(e.Message);
                        View.ProgressRing.IsActive = false;
                        View.ProgressTextBlock.Text = "Недопустимый формат адреса сервера.";
                    });
                } // try-catch
            });
        } // ConnectToServerAsync


        //******************************** Callbacks **********************************//
        /// <summary>Добавляет сообщение в листбокс-чат</summary>
        private async void AddMsgToChatAsync(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
                return;
            await Task.Factory.StartNew(async () => {
                await View.Dispatcher.InvokeAsync(() => {
                    View.ListBoxChat.Items.Add(msg);
                });
            });
        } // AddMsgToChatAsync


        /// <summary>Изменяет список пользователей в лобби</summary>
        private async void SetLobbyUserListAsync(IEnumerable<string> userList)
        {
            // Очищаем список
            await View.Dispatcher.InvokeAsync(() => {
                View.ListBoxUsers.Items.Clear();
            });

            userList
                .ToList()
                .ForEach(async uname => {
                    try {
                        // Проверяем есть ли такой пользователь в базе истории
                        var gameHistory = Db.GameHistory.First(history => history.Who == uname);

                        // Если всё-таки есть, смотрим его историю
                        Db.GameHistory
                           .Join(Db.GameResult, g => g.IdResult, g => g.Id, (g, r) => new {
                               g.Who,
                               g.WithWhom,
                               g.GameTime,
                               r.State
                           })
                           .GroupBy(g => g.Who)
                           .Select(g => new {
                               Name = g.Key,
                               Wins = g.Count(w => w.State == "Victory"),
                               Defeats = g.Count(w => w.State == "Defeat")
                           })
                           .ToList()
                           .ForEach(async p => {
                              if (uname != p.Name)
                                  return;

                              // В зависимости от кол-ва побед (ранга) выбираем аватар.
                              // Бронза
                              var user = new UserTemplate(p.Name, "В сети", @"..\..\Images\Ranks\bronze.png");

                              // Серебро
                              if (p.Wins <= 20 && p.Wins >= 10) {
                                  user = new UserTemplate(p.Name, "В сети", @"..\..\Images\Ranks\silver.png");
                                  // Золото
                              } else if (p.Wins <= 30 && p.Wins >= 20) {
                                  user = new UserTemplate(p.Name, "В сети", @"..\..\Images\Ranks\gold.png");
                                  // Платина
                              } else if (p.Wins <= 40 && p.Wins >= 30) {
                                  user = new UserTemplate(p.Name, "В сети", @"..\..\Images\Ranks\platinum.png");
                                  // Алмаз
                              } else if (p.Wins <= 50 && p.Wins >= 40) {
                                  user = new UserTemplate(p.Name, "В сети", @"..\..\Images\Ranks\diamond.png");
                                  // Мейстер
                              } else if (p.Wins <= 60 && p.Wins >= 50) {
                                  user = new UserTemplate(p.Name, "В сети", @"..\..\Images\Ranks\master.png");
                                  // Гроссмейстер
                              } else if (p.Wins <= 70 || p.Wins > 70) {
                                  user = new UserTemplate(p.Name, "В сети", @"..\..\Images\Ranks\grandmaster.png");
                              }
                              await View.Dispatcher.InvokeAsync(() => {
                                  View.ListBoxUsers.Items.Add(user);
                              });
                           });
                    } catch (Exception) {
                        // Если нет, то
                        // создаём игрока по умолчанию
                        var user = new UserTemplate(uname, "В сети", @"..\..\Images\Ranks\bronze.png");

                        // Добавляем
                        await View.Dispatcher.InvokeAsync(() => {
                            View.ListBoxUsers.Items.Add(user);
                        });
                    } // try-catch
                });
        } // SetLobbyUserListAsync


        /// <summary>
        /// Ответ на приглашение в игру.
        /// </summary>
        /// <param name="from">От кого прислано приглашение</param>
        /// <param name="to">Кому</param>
        /// <param name="firstMove">Ходит первый</param>
        private async void ReplyForInvite((string, string) from, (string, string) to, string firstMove)
        {
            await View.Dispatcher.InvokeAsync(() => {
                View.DialogHostReplyForInvite.DialogClosing += (s, a) => {
                    if (!Equals(a.Parameter, true))
                        return;
                    Client.AcceptInvite(from, to, firstMove);
                };
                View.TbDialogHostReplyForInviteFrom.Text = $"{from.Item1} ";
                View.TbDialogHostReplyForInviteDescr.Text = $"{from.Item1} - {from.Item2}, Вы ({to.Item1}) - {to.Item2}, первые ходят {firstMove}.";
                View.DialogHostReplyForInvite.IsOpen = true;
            });
        } // ReplyForInvite


        /// <summary>
        /// Принятие приглашения в игру.
        /// </summary>
        private async void AcceptInvite((string, string) from, (string, string) to, string firstMove)
        {
            // TODO: Изменить статус пользователя из "в сети" на "в игре"
            //Client.DeleteUserFromLobbyList(from.Item1);
            //Client.AddUserToLobbyList();

            // Запускаем окно с игрой
            // TODO: Сделать GameNetClientView Singleton, чтобы повторно не запускалось куча окон
            await View.Dispatcher.InvokeAsync(() => {
                var view = new GameNetClientView();
                var model = new GameNetClientModel(from, to, firstMove, Client, LoginData);
                model.InitializeView(view);
                var viewModel = new GameNetClientViewModel(model);
                _clientModel = model;
                view.DataContext = viewModel;
                view.Show();
            });
        } // AcceptInvite


        /// <summary>
        /// Делает ход одновременно у двух игроков.
        /// </summary>
        private async void MakeMove(int row1, int col1, int row2, int col2)
        {
            await View.Dispatcher.InvokeAsync(() => {
                _clientModel.CurrentMove = new Move {
                    Piece1 = new Piece(row1, col1),
                    Piece2 = new Piece(row2, col2)
                };
                if (_clientModel.CheckMove())
                    _clientModel.MakeMove();
            });
        } // MakeMove


        /// <summary>
        /// Начинает новую игру у двух игроков.
        /// </summary>
        private async void StartNewGame()
        {
            await View.Dispatcher.InvokeAsync(() => {
                _clientModel.View.DialogHostWinner.IsOpen = false;
                _clientModel.StartNewGame();
            });
        } // StartNewGame


        public void SendMessageCallback(string msg) 
            => AddMsgToChatAsync(msg);

        public void SendUserListCallback(List<string> userList) 
            => SetLobbyUserListAsync(userList);

        public void SendInviteCallback((string, string) from, (string, string) to, string firstMove) 
            => ReplyForInvite(from, to, firstMove);

        public void AcceptInviteCallback((string, string) from, (string, string) to, string firstMove)
            => AcceptInvite(from, to, firstMove);

        public void MakeMoveCallback(int row1, int col1, int row2, int col2)
            => MakeMove(row1, col1, row2, col2);

        public void StartNewGameCallback()
            => StartNewGame();
    } // GameNetLobbyModel class
} // Checkers.Models