using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using Checkers.Models.Interfaces;
using Checkers.Models.Network;
using Checkers.Models.Network.DBML;
using Checkers.Views;


namespace Checkers.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Сетевая игра, модель входа на сервер.
    /// </summary>
    public class GameNetLoginModel : IGameNetLoginModel
    {
        /// <inheritdoc />
        /// <summary>
        /// Визуальная часть. 
        /// </summary>
        public GameNetLoginView View { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Контекст данных базы данных CheckersDB. 
        /// </summary>
        public CheckersDBDataContext Db { get; }
        /// <inheritdoc />
        /// <summary>
        /// Данные для передачи в окно лобби, чтобы открыть 
        /// окно лобби и пытаться соедениться с сервером.
        /// </summary>
        public LoginData LoginData { get; set; }


        public GameNetLoginModel()
        {
            Db = new CheckersDBDataContext();
            LoginData = new LoginData();
        } // GameNetLoginModel ctor


        /// <inheritdoc />
        /// <summary>
        /// Инициализация визуальной части.
        /// </summary>
        public void InitializeView(GameNetLoginView view)
        {
            View = view;

            // Локальный IP-адрес:
            Dns.GetHostEntry(Dns.GetHostName())
               .AddressList
               .ToList()
               .ForEach(ip => {
                   // Отсеиваем IPv6-адреса
                   if (ip.AddressFamily == AddressFamily.InterNetwork) {
                       View.TextBoxHostName.Text = ip.ToString();
                   } // if
               });
        } // InitializeView


        /// <inheritdoc />
        /// <summary>
        /// Сверяет данные в БД с введёнными.
        /// </summary>
        public async void Login()
        {
            if (string.IsNullOrWhiteSpace(View.TextBoxUserName.Text) ||
                string.IsNullOrWhiteSpace(View.PasswordBox.Password))
                return;

            try {
                var tempUserName = View.TextBoxUserName.Text;
                var tempPassword = View.PasswordBox.Password;

                View.ButtonLogin.IsEnabled = false;
                View.ProgressRing.IsActive = true;

                // Проверяем введённые данные на авторизацию
                await Task.Factory.StartNew(() => {
                    var clients = Db.Clients
                                    .First(l => l.UserName == tempUserName && l.Password == tempPassword);
                });

                LoginData.HostName = View.TextBoxHostName.Text;
                LoginData.Port = View.TextBoxPort.Text;
                LoginData.UserName = View.TextBoxUserName.Text;
                View.DialogResult = true;
            } catch (SqlException e) {
                Console.WriteLine(e.Message);
                View.TextBlockWarning.Text = "Сервер базы данных недоступен!";
                View.TextBlockWarning.Visibility = Visibility.Visible;
                View.ButtonLogin.IsEnabled = true;
                View.ProgressRing.IsActive = false;
            } catch (Exception) {
                View.TextBlockWarning.Text = "Такого пользователя не существует!";
                View.TextBlockWarning.Visibility = Visibility.Visible;
                View.ButtonLogin.IsEnabled = true;
                View.ProgressRing.IsActive = false;
            } // try-catch
        } // Login
    } // GameNetLoginModel class
} // Checkers.Models