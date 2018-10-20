using System.Windows;
using Checkers.Models.Interfaces;
using Checkers.Services;
using Checkers.Views;


namespace Checkers.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// View Model для окна 
    /// <see cref="T:Checkers.Views.GameWithAiView" />.
    /// </summary>
    public sealed class GameWithAiViewModel : BaseViewModel
    {
        /// <summary>
        /// Модель игры с ИИ.
        /// </summary>
        private readonly IGameWithAiModel _gameWithAiModel;


        public GameWithAiViewModel(IGameWithAiModel gameWithAiModel)
        {
            _gameWithAiModel = gameWithAiModel;
            InfoPanelVisibility = 0;
            IsCheckInfoPanel = true;

            StatusText = "";
            TimePassed = "00:00:00";

            //***** СОЗДАНИЕ КОМАНД *****//
            ExitCommand = new RelayCommand(Exit);
            ExitToMenuCommand = new RelayCommand(ExitToMenu);
            NewGameCommand = new RelayCommand(NewGame);
            InfoPanelCommand = new RelayCommand(InfoPanel);
            AboutCommand = new RelayCommand(About);
        } // GameWithAiViewModel


        //********************************* КОМАНДЫ ***********************************//
        /// <summary>Выход из игры</summary>
        public RelayCommand ExitCommand { get; set; }
        private static void Exit(object obj)
        {
            Application.Current.Shutdown();
        } // Exit


        /// <summary>Выход в меню</summary>
        public RelayCommand ExitToMenuCommand { get; set; }
        private void ExitToMenu(object obj)
        {
            _gameWithAiModel.View.Close();
        } // ExitToMenu


        /// <summary>Новая игра</summary>
        public RelayCommand NewGameCommand { get; set; }
        private void NewGame(object obj)
        {
            StatusText = "Сейчас ходят чёрные.";
            _gameWithAiModel.StartNewGame();
        } // NewGame


        /// <summary>Информационная панель (её видимость)</summary>
        public RelayCommand InfoPanelCommand { get; set; }
        private void InfoPanel(object obj)
        {
            InfoPanelVisibility = IsCheckInfoPanel ? Visibility.Visible : Visibility.Collapsed;
        } // InfoPanel


        /// <summary>Об игре</summary>
        public RelayCommand AboutCommand { get; set; }
        private static void About(object obj)
        {
            var win = new AboutGameWithAiView();
            win.PreviewMouseLeftButtonDown += (s, e) => { (s as Window)?.Close(); };
            win.ShowDialog();
        } // NewGame


        //******************** СВОЙСТВА СВЯЗАННЫЕ С ИНТЕРФЕЙСОМ ***********************//
        /// <summary>Видимость информационной панели</summary>
        private Visibility _infoPanelVisibility;
        public Visibility InfoPanelVisibility {
            get => _infoPanelVisibility;
            set {
                _infoPanelVisibility = value;
                OnPropertyChanged();
            } // set
        } // InfoPanelVisibility


        /// <summary>Состояние флажка в подпункте меню "Информационная панель"</summary>
        private bool _isCheckInfoPanel;
        public bool IsCheckInfoPanel {
            get => _isCheckInfoPanel;
            set {
                _isCheckInfoPanel = value;
                OnPropertyChanged();
            } // set
        } // IsCheckInfoPanel


        /// <summary>Текст в статус-баре</summary>
        private string _statusText;
        public string StatusText {
            get => _statusText;
            set {
                _statusText = value;
                OnPropertyChanged();
            } // set
        } // StatusText


        /// <summary>Времени прошло</summary>
        private string _timePassed;
        public string TimePassed {
            get => _timePassed;
            set {
                _timePassed = value;
                OnPropertyChanged();
            } // set
        } // TimePassed
    } // GameWithAiViewModel class
} // Checkers.ViewModel