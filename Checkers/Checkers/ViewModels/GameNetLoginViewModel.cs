using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Checkers.Models.Interfaces;
using Checkers.Models.Network.DBML;
using Checkers.Services;
using Checkers.Views;


namespace Checkers.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// View Model для окна входа. 
    /// <see cref="T:Checkers.Views.GameNetLoginView" />.
    /// </summary>
    public sealed class GameNetLoginViewModel : BaseViewModel
    {
        /// <summary>
        /// Модель входа в сетевую игру.
        /// </summary>
        private readonly IGameNetLoginModel _gameNetLoginModel;


        public GameNetLoginViewModel(IGameNetLoginModel gameNetLoginModel)
        {
            _gameNetLoginModel = gameNetLoginModel;

            //***** СОЗДАНИЕ КОМАНД *****//
            ExitCommand = new RelayCommand(Exit);
            LoginCommand = new RelayCommand(Login);
            RegistrationCommand = new RelayCommand(Registration);
        } // GameNetLoginViewModel ctor


        //********************************* КОМАНДЫ ***********************************//
        /// <summary>Выход</summary>
        public RelayCommand ExitCommand { get; set; }
        private void Exit(object obj)
        {
            var view = Application.Current.Windows
                                  .OfType<PreloaderView>()
                                  .FirstOrDefault();
            if (view != null) {
                view.ButtonOnlineGameServer.IsEnabled = true;
                view.ButtonOnlineGameClient.IsEnabled = true;
            } // if

            _gameNetLoginModel.View.Close();
        } // Exit


        /// <summary>Вход</summary>
        public RelayCommand LoginCommand { get; set; }
        private void Login(object obj)
        {
            _gameNetLoginModel.Login();
        } // Login


        /// <summary>Регистрация</summary>
        public RelayCommand RegistrationCommand { get; set; }
        private void Registration(object obj)
        {
            _gameNetLoginModel.View.Hide();
            _gameNetLoginModel.View.ButtonLogin.IsEnabled = false;
            _gameNetLoginModel.View.ProgressRing.IsActive = true;

            var view = new RegistrationView();
            var show = view.ShowDialog();

            // Если регистрация юзера прошла, добавляем его в базу.
            if (show != null && show.Value) {
                var cl = new Clients {
                    Email = view.TextBoxEmail.Text,
                    Id = 0,
                    Password = view.PasswordBox.Password,
                    UserName = view.TextBoxUserName.Text
                };

                try {
                    //TODO: Если сервер базы данных недоступен не прерывать основной поток на ожидание таймаута
                    //await Task.Factory.StartNew(() => {
                        _gameNetLoginModel.Db.Clients.InsertOnSubmit(cl);
                        _gameNetLoginModel.Db.SubmitChanges();
                    //});
                } catch (SqlException) {
                    _gameNetLoginModel.View.TextBlockWarning.Text = "Сервер базы данных недоступен!";
                    _gameNetLoginModel.View.TextBlockWarning.Visibility = Visibility.Visible;
                    _gameNetLoginModel.View.ButtonLogin.IsEnabled = true;
                    _gameNetLoginModel.View.ProgressRing.IsActive = false;
                } // try-catch
            } // if

            _gameNetLoginModel.View.ButtonLogin.IsEnabled = true;
            _gameNetLoginModel.View.ProgressRing.IsActive = false;

            var preloaderView = Application.Current.Windows
                                           .OfType<PreloaderView>()
                                           .FirstOrDefault();
            if (preloaderView != null) {
                preloaderView.ProgressRing.IsActive = false;
                preloaderView.TextBlockProgress.Visibility = Visibility.Collapsed;
                preloaderView.StackPanelMenu.Visibility = Visibility.Visible;
            } // if

            _gameNetLoginModel.View.ShowDialog();
        } // Registration
    } // GameNetLoginViewModel class
} // Checkers.ViewModels