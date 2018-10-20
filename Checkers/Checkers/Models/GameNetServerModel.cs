using System;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Threading;
using Checkers.Models.Interfaces;
using Checkers.Models.Network.GameServer;
using Checkers.Views;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;


namespace Checkers.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Сетевая игра, серверная модель.
    /// </summary>
    public class GameNetServerModel : IGameNetServerModel
    {
        /// <inheritdoc />
        /// <summary>
        /// Визуальная часть. Окно построенное средствами XAML.
        /// </summary>
        public GameNetServerView View { get; set; }
        /// <summary>
        /// Хостинг. Основное приложение службы.
        /// </summary>
        private ServiceHost Host { get; set; }

        /// <summary>
        /// Значок в области уведомлений.
        /// </summary>
        private NotifyIcon _notifyIcon;


        public GameNetServerModel()
        {
            SettingNotifyIcon();
        } // GameNetServerModel


        /// <summary>
        /// Настройка NotifyIcon.
        /// </summary>
        private void SettingNotifyIcon()
        {
            _notifyIcon = new NotifyIcon {
                Text = @"Шашки - сервер.",
                Icon = Icon.ExtractAssociatedIcon(@"..\..\Icons\favicon.ico"),
                BalloonTipIcon = ToolTipIcon.Info,
                BalloonTipTitle = @"Шашки - сервер."
            };
            var contextMenu = new ContextMenu();
            var menuItems = new Menu.MenuItemCollection(contextMenu) {
                new MenuItem("Открыть", (s, a) => {
                    View.Visibility = Visibility.Visible;
                    _notifyIcon.Visible = false;
                }),
                new MenuItem("Выход", (s, a) => {
                    StopServerAsync();
                    _notifyIcon.Visible = false;
                    System.Windows.Application.Current.Windows
                          .OfType<GameNetServerView>()
                          .FirstOrDefault()?.Close();
                })
            };
            _notifyIcon.ContextMenu = contextMenu;
            _notifyIcon.DoubleClick += (s, a) => {
                View.Visibility = Visibility.Visible;
                _notifyIcon.Visible = false;
            };
        } // SettingNotifyIcon


        /// <inheritdoc />
        /// <summary>
        /// Инициализация визуальной части.
        /// </summary>
        /// <param name="view">Окно с разметкой.</param>
        public void InitializeView(GameNetServerView view)
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

            // Интернет IP-адрес:
            //var wc = new WebClient();
            //View.TextBoxHostName.Text = await wc.DownloadStringTaskAsync(new Uri("https://api.ipify.org"));
        } // InitializeView


        /// <inheritdoc />
        /// <summary>
        /// Сворачивание в трей.
        /// </summary>
        /// <param name="balloonTipText">Всплывающая подсказка.</param>
        public void ToTray(string balloonTipText)
        {
            _notifyIcon.BalloonTipText = balloonTipText;
            _notifyIcon.Visible = true;
            View.Visibility = Visibility.Hidden;
            _notifyIcon.ShowBalloonTip(2000);
        } // ToTray


        /// <inheritdoc />
        /// <summary>
        /// Асинхронно запускает сервер.
        /// </summary>
        public bool StartServerAsync()
        {
            try {
                // Создание экземпляра класса-хоста, который публикует службу 
                // (указывается сервис-контракт и адрес службы).
                Host = new ServiceHost(typeof(CheckersSvc), new Uri($@"net.tcp://{View.TextBoxHostName.Text}:{View.TextBoxPort.Text}/Checkers"));

                // Для связи используем протокол TCP-IP.
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

                // Задание конечных точек для сервиса.
                Host.AddServiceEndpoint(typeof(ICheckersSvc), netTcpBinding, "Game");

                View.Dispatcher.Invoke(() => {
                    View.StatusText.Text = "Запуск...";
                }, DispatcherPriority.Normal);

                // Старт службы (асинхронно).
                Host.BeginOpen(OpenCallbackAsync, null);
                ToTray("Сервер сейчас запущен.");
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            } // try-catch
        } // StartServerAsync

        /// <summary>
        /// Завершает ассинхронную операцию открытия.
        /// </summary>
        private void OpenCallbackAsync(IAsyncResult ar)
        {
            Host.EndOpen(ar);
            View.Dispatcher.Invoke(() => {
                View.StatusText.Text = "Сервер ожидает подключений...";
                View.ButtonStartStop.Content = "СТОП";
                View.TextBoxHostName.IsEnabled = false;
                View.TextBoxPort.IsEnabled = false;
            }, DispatcherPriority.Normal);
        } // OpenCallbackAsync


        /// <inheritdoc />
        /// <summary>
        /// Асинхронно останавливает сервер.
        /// </summary>
        public bool StopServerAsync()
        {
            try {
                // Начинаем ассинхронное закрытие объекта связи.
                Host.BeginClose(StopCallbackAsync, null);
                View.Dispatcher.Invoke(() => {
                    View.StatusText.Text = "Остановка сервера...";
                }, DispatcherPriority.Normal);
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            } // try-catch
        } // StopServerAsync

        /// <summary>
        /// Завершает ассинхронную операцию закрытия.
        /// </summary>
        private void StopCallbackAsync(IAsyncResult ar)
        {
            Host.EndClose(ar);
            View.Dispatcher.Invoke(() => {
                View.StatusText.Text = "Сервер остановлен.";
                View.ButtonStartStop.Content = "ЗАПУСК";
                View.TextBoxHostName.IsEnabled = true;
                View.TextBoxPort.IsEnabled = true;
            }, DispatcherPriority.Normal);
        } // StopCallbackAsync
    } // GameNetServerModel class
} // Checkers.Models