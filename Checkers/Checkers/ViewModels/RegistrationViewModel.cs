using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Checkers.Models.Network.DBML;
using Checkers.Services;
using Checkers.Views;


namespace Checkers.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// View Model для окна регистрации. 
    /// <see cref="T:Checkers.Views.RegistrationView" />.
    /// </summary>
    public sealed class RegistrationViewModel : BaseViewModel
    {
        // Для проверки e-mail
        private const string Pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                       @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
        
        // Свойство через которое будем получать регистрационные данные.
        public Clients Client { get; set; }


        public RegistrationViewModel()
        {
            WarningVisibility = Visibility.Hidden;

            //***** СОЗДАНИЕ КОМАНД *****//
            DoRegistrCommand = new RelayCommand(Registration);
        } // RegistrationViewModel


        //********************************* КОМАНДЫ ***********************************//
        public RelayCommand DoRegistrCommand { get; set; }
        private void Registration(object obj)
        {
            // Получаем PasswordBox'ы, через параметр команды
            var passwords = obj as Tuple<PasswordBox, PasswordBox>;
            
            if (!string.IsNullOrWhiteSpace(UserName) &&
                !string.IsNullOrWhiteSpace(Email) &&
                !string.IsNullOrWhiteSpace(passwords?.Item1.Password) &&
                !string.IsNullOrWhiteSpace(passwords.Item2.Password)) {

                if (!Regex.IsMatch(Email, Pattern, RegexOptions.IgnoreCase)) {
                    Warning = "Некорректный e-mail";
                    WarningVisibility = Visibility.Visible;
                    return;
                } // if

                if (passwords.Item1.Password != passwords.Item2.Password) {
                    Warning = "Пароли не совпадают";
                    WarningVisibility = Visibility.Visible;
                    return;
                } // if

                var client = new Clients {
                    UserName = UserName,
                    Password = passwords.Item1.Password,
                    Email = Email
                };

                Client = client;

                Application.Current.Windows
                           .OfType<RegistrationView>()
                           .FirstOrDefault()
                           .DialogResult = true;

                var preloaderView = Application.Current.Windows
                                               .OfType<PreloaderView>()
                                               .FirstOrDefault();
                preloaderView?.Dispatcher.Invoke(() => {
                    preloaderView.StackPanelMenu.Visibility = Visibility.Collapsed;
                    preloaderView.ProgressRing.IsActive = true;
                    preloaderView.TextBlockProgress.Visibility = Visibility.Visible;
                });

                WarningVisibility = Visibility.Collapsed;
            } else {
                Warning = "Введите логин, пароль и электронную почту";
                WarningVisibility = Visibility.Visible;
            } // if-else
        } // Registration


        //******************** СВОЙСТВА СВЯЗАННЫЕ С ИНТЕРФЕЙСОМ ***********************//
        private string _userName;
        public string UserName {
            get => _userName;
            set {
                _userName = value;
                OnPropertyChanged();
            } // set
        } // UserName


        private string _email;
        public string Email {
            get => _email;
            set {
                _email = value;
                OnPropertyChanged();
            } // set
        } // Email


        private string _warning;
        public string Warning {
            get => _warning;
            set {
                _warning = value;
                OnPropertyChanged();
            } // set
        } // Warning


        private Visibility _warningVisibility;
        public Visibility WarningVisibility {
            get => _warningVisibility;
            set {
                _warningVisibility = value;
                OnPropertyChanged();
            } // set
        } // WarningVisibility
    } // RegistrationViewModel class
} // Checkers.ViewModels