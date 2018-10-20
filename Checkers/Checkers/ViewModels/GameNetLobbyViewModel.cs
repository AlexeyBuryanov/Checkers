using System;
using System.Windows.Threading;
using Checkers.Models.Interfaces;
using Checkers.Services;
using MaterialDesignThemes.Wpf;


namespace Checkers.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// View Model для окна лобби. 
    /// <see cref="T:Checkers.Views.GameNetLobbyView" />.
    /// </summary>
    public sealed class GameNetLobbyViewModel : BaseViewModel
    {
        /// <summary>
        /// Модель лобби.
        /// </summary>
        private readonly IGameNetLobbyModel _gameNetLobbyModel;


        public GameNetLobbyViewModel(IGameNetLobbyModel gameNetLobbyModel)
        {
            _gameNetLobbyModel = gameNetLobbyModel;

            //***** СОЗДАНИЕ КОМАНД *****//
            SendMessageCommand = new RelayCommand(SendMessage);
            SendInviteCommand = new RelayCommand(SendInvite, o => 
                _gameNetLobbyModel.View.ListBoxUsers.SelectedItem != null && 
                (_gameNetLobbyModel.View.ListBoxUsers.SelectedItem as UserTemplate)?.Name != _gameNetLobbyModel.LoginData.UserName);
        } // GameNetLobbyViewModel


        /// <summary>Добавляет сообщение в листбокс-чат</summary>
        private async void AddMsgToChatAsync(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
                return;
            await _gameNetLobbyModel.View.Dispatcher.InvokeAsync(() => {
                _gameNetLobbyModel.View.ListBoxChat.Items.Add(msg);
            });
        } // AddMsgToChatAsync


        //********************************* КОМАНДЫ ***********************************//
        /// <summary>Отправить сообщение</summary>
        public RelayCommand SendMessageCommand { get; set; }
        private async void SendMessage(object obj)
        {
            // Отправка идёт всем
            await _gameNetLobbyModel.View.Dispatcher.InvokeAsync(() => {
                _gameNetLobbyModel.Client.SendMessageToAll($"{_gameNetLobbyModel.LoginData.UserName}: " +
                                                           $"{_gameNetLobbyModel.View.TextBoxMessage.Text}",
                                                           $"{_gameNetLobbyModel.LoginData.UserName}");
                AddMsgToChatAsync($"{_gameNetLobbyModel.LoginData.UserName}: {_gameNetLobbyModel.View.TextBoxMessage.Text}");
                _gameNetLobbyModel.View.TextBoxMessage.Text = "";
            });
        } // SendMessage


        /// <summary>Отправить приглашение в игру</summary>
        public RelayCommand SendInviteCommand { get; set; }
        private void SendInvite(object obj)
        {
            if (_gameNetLobbyModel.View.ListBoxUsers.SelectedItem == null)
                return;

            var user = _gameNetLobbyModel.View.ListBoxUsers.SelectedItem as UserTemplate;

            // Самому себе слать приглашения не разрешаем
            if (user?.Name == _gameNetLobbyModel.LoginData.UserName)
                return;

            // Запускаем окно-диалог с настройкой приглашения
            _gameNetLobbyModel.View.DialogHostInvite.DialogClosing += async (s, a) => {
                if (!Equals(a.Parameter, true))
                    return;

                await _gameNetLobbyModel.View.Dispatcher.InvokeAsync(() => {
                    // Уведомляем, что инвайт отправлен
                    var message = new SnackbarMessage {
                        Content = $"Приглашение отправлено пользователю {user?.Name}".ToUpper(),
                    };
                    _gameNetLobbyModel.View.Snackbar.Message = message;
                    _gameNetLobbyModel.View.Snackbar.IsActive = true;

                    var seconds = 0;
                    var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                    timer.Tick += (o, e) => {
                        seconds++;
                        if (seconds != 3)
                            return;
                        _gameNetLobbyModel.View.Snackbar.IsActive = false;
                        timer.Stop();
                    };
                    timer.Start();
                });

                (string, string) from;
                (string, string) to;
                string firstMove;

                if (_gameNetLobbyModel.View.CbDialogInviteYouBlack.IsChecked != null
                    && _gameNetLobbyModel.View.CbDialogInviteYouBlack.IsChecked.Value)
                    from = (_gameNetLobbyModel.LoginData.UserName, "Чёрные");
                else
                    from = (_gameNetLobbyModel.LoginData.UserName, "Белые");

                if (_gameNetLobbyModel.View.CbDialogInviteInvitedBlack.IsChecked != null
                    && _gameNetLobbyModel.View.CbDialogInviteInvitedBlack.IsChecked.Value)
                    to = (user?.Name, "Чёрные");
                else
                    to = (user?.Name, "Белые");

                if (_gameNetLobbyModel.View.CbDialogInviteFirstMoveBlack.IsChecked != null
                    && _gameNetLobbyModel.View.CbDialogInviteFirstMoveBlack.IsChecked.Value)
                    firstMove = "Чёрные";
                else
                    firstMove = "Белые";

                // Отправка инвайта
                _gameNetLobbyModel.Client.SendInviteTo(from, to, firstMove);
            };
            _gameNetLobbyModel.View.DialogHostInvite.IsOpen = true;
        } // SendMessage
    } // GameNetLobbyViewModel class
} // Checkers.ViewModels