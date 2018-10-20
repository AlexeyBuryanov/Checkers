using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Checkers.Models.GameCore;
using Checkers.Models.Interfaces;
using Checkers.Models.Network;
using Checkers.Models.Network.DBML;
using Checkers.Models.Network.GameServer;
using Checkers.Properties;
using Checkers.Services;
using Checkers.Views;
using MaterialDesignThemes.Wpf;


namespace Checkers.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Здесь происходит всё, что связано с игрой клиента по сети.
    /// </summary>
    public class GameNetClientModel : IGameNetClientModel
    {
        /// <inheritdoc />
        /// <summary>
        /// Визуальная часть. Окно построенное средствами XAML.
        /// </summary>
        public GameNetClientView View { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Текущий ход.
        /// </summary>
        public Move CurrentMove { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Победитель.
        /// </summary>
        public string Winner { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Очередь.
        /// </summary>
        public string Turn { get; set; }
        /// <summary>
        /// История ходов для мониторинга в игре.
        /// </summary>
        private readonly ObservableCollection<HistoryMove> _observableHistoryMove;

        /// <summary>
        /// Номер хода.
        /// </summary>
        private int _numberMove;
        /// <summary>
        /// Таймер хода. Для истории, кто сколько времени думал прежде, чем сделать ход.
        /// </summary>
        private readonly DispatcherTimer _timerMove;
        /// <summary>
        /// Время хода.
        /// </summary>
        private DateTime _timeMove;
        /// <summary>
        /// Строка для отображения времени.
        /// </summary>
        private string _timePassed;
        /// <summary>
        /// Общий таймер времени.
        /// </summary>
        private readonly DispatcherTimer _commonTimer;
        /// <summary>
        /// Общее время.
        /// </summary>
        private DateTime _commonTime;
        /// <summary>
        /// От кого приглашение. 
        /// item1 - кто, item2 - кем играет
        /// </summary>
        private readonly (string, string) _from;
        /// <summary>
        /// Кому приглашение. 
        /// item1 - кто, item2 - кем играет
        /// </summary>
        private readonly (string, string) _to;
        /// <summary>
        /// Кто ходит первым
        /// </summary>
        private readonly string _firstMove;
        /// <summary>
        /// Клиент.
        /// </summary>
        private readonly ICheckersSvc _client;
        /// <summary>
        /// Данные для входа.
        /// </summary>
        private readonly LoginData _loginData;
        /// <summary>
        /// Контекст БД.
        /// </summary>
        private CheckersDBDataContext Db { get; }


        public GameNetClientModel()
        {
            CurrentMove = null;
            Winner = null;
            _numberMove = 1;
            _observableHistoryMove = new ObservableCollection<HistoryMove>();

            _timerMove = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _commonTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timerMove.Tick += TimerMove_Tick;
            _commonTimer.Tick += CommonTimerTick;
            _timeMove = new DateTime();
            _commonTime = new DateTime();
            _timePassed = "00:00:00";
            Db = new CheckersDBDataContext();
        } // GameWithAiModel ctor

        public GameNetClientModel((string, string) from, (string, string) to, string firstMove, 
                                  ICheckersSvc client, LoginData loginData)
        {
            _from = from;
            _to = to;
            _firstMove = firstMove;
            _client = client;
            _loginData = loginData;

            CurrentMove = null;
            Winner = null;
            _numberMove = 1;
            _observableHistoryMove = new ObservableCollection<HistoryMove>();

            _timerMove = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _commonTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timerMove.Tick += TimerMove_Tick;
            _commonTimer.Tick += CommonTimerTick;
            _timeMove = new DateTime();
            _commonTime = new DateTime();
            _timePassed = "00:00:00";
            Db = new CheckersDBDataContext();
        } // GameNetClientModel ctor


        private void CommonTimerTick(object sender, EventArgs e)
        {
            _commonTime = _commonTime.AddSeconds(1);
            View.TextBlockTimePassed.Text = _commonTime.ToString("HH:mm:ss");
        } // CommonTimerTick


        private void TimerMove_Tick(object sender, EventArgs e)
        {
            _timeMove = _timeMove.AddSeconds(1);
            _timePassed = _timeMove.ToString("HH:mm:ss");
        } // TimerMove_Tick


        /// <inheritdoc />
        /// <summary>
        /// Инициализация визуальной части.
        /// </summary>
        /// <param name="view">Окно с разметкой.</param>
        public void InitializeView(GameNetClientView view)
        {
            View = view;
            View.Title += $" | {_loginData.UserName}";
            View.GridMain.Visibility = Visibility.Hidden;
            Turn = _firstMove == "Чёрные" ? "Black" : "White";
            if (Turn == "Black") {
                View.TextBlockPlayerWhiteMove.Visibility = Visibility.Collapsed;
                View.TextBlockPlayerBlackMove.Visibility = Visibility.Visible;
            } else {
                View.TextBlockPlayerWhiteMove.Visibility = Visibility.Visible;
                View.TextBlockPlayerBlackMove.Visibility = Visibility.Collapsed;
            } // if-else
            if (_from.Item2 == "Чёрные") {
                View.TextBlockPlayer1.Text = _from.Item1;
                View.TextBlockPlayer2.Text = _to.Item1;
            } else {
                View.TextBlockPlayer2.Text = _from.Item1;
                View.TextBlockPlayer1.Text = _to.Item1;
            } // if-else
            MakeBoard();
            View.GridMain.Visibility = Visibility.Visible;
            _commonTimer.Start();
            _timerMove.Start();
            View.MenuItemView.IsEnabled = true;
            View.StatusText.Text = _firstMove == "Чёрные" ? "Сейчас ходят чёрные." : "Сейчас ходят белые.";
            View.DataGridHistoryMove.ItemsSource = _observableHistoryMove;
            View.ScoreBlacksLeft.Text = "12";
            View.ScoreBlacksTaked.Text = "0";
            View.ScoreWhitesLeft.Text = "12";
            View.ScoreWhitesTaked.Text = "0";
        } // CreateView


        /// <inheritdoc />
        /// <summary>
        /// Очистить доску.
        /// </summary>
        public void ClearBoard()
        {
            for (var r = 1; r < 9; r++) {
                for (var c = 0; c < 8; c++) {
                    var stackPanel = (StackPanel)GetGridElement(View.CheckersGrid, r, c);
                    View.CheckersGrid.Children.Remove(stackPanel);
                } // for c
            } // for r
        } // ClearBoard


        /// <inheritdoc />
        /// <summary>
        /// "Сделать" доску.
        /// </summary>
        public void MakeBoard()
        {
            for (var r = 1; r < 9; r++) {
                for (var c = 0; c < 8; c++) {
                    var stackPanel = new StackPanel();

                    if (r % 2 == 1) {
                        stackPanel.Background = c % 2 == 0 ? Brushes.White : Brushes.Black;
                    } else {
                        stackPanel.Background = c % 2 == 0 ? Brushes.Black : Brushes.White;
                    } // if-else

                    Grid.SetRow(stackPanel, r);
                    Grid.SetColumn(stackPanel, c);
                    View.CheckersGrid.Children.Add(stackPanel);
                } // for c
            } // for r

            MakeButtons();
        } // MakeBoard


        /// <inheritdoc />
        /// <summary>
        /// Сделать кнопки.
        /// </summary>
        public void MakeButtons()
        {
            for (var r = 1; r < 9; r++) {
                for (var c = 0; c < 8; c++) {
                    var stackPanel = (StackPanel)GetGridElement(View.CheckersGrid, r, c);
                    var button = new Button();
                    button.Click += Button_Click;
                    button.Height = 190;
                    button.Width = 190;
                    button.HorizontalAlignment = HorizontalAlignment.Stretch;
                    button.VerticalAlignment = VerticalAlignment.Stretch;

                    var whiteBrush = new ImageBrush {
                        ImageSource = new BitmapImage(new Uri(@"..\..\Images\Figures\checkerWhite.png", UriKind.Relative))
                    };
                    var blackBrush = new ImageBrush {
                        ImageSource = new BitmapImage(new Uri(@"..\..\Images\Figures\checkerBlack.png", UriKind.Relative))
                    };
                    switch (r) {
                        case 1:
                            if (c % 2 == 1) {
                                button.Background = whiteBrush;
                                button.Name = "buttonWhite" + r + c;
                                stackPanel.Children.Add(button);
                            } // if
                            break;
                        case 2:
                            if (c % 2 == 0) {
                                button.Background = whiteBrush;
                                button.Name = "buttonWhite" + r + c;
                                stackPanel.Children.Add(button);
                            } // if
                            break;
                        case 3:
                            if (c % 2 == 1) {
                                button.Background = whiteBrush;
                                button.Name = "buttonWhite" + r + c;
                                stackPanel.Children.Add(button);
                            } // if
                            break;
                        case 4:
                            if (c % 2 == 0) {
                                button.Background = Brushes.Black;
                                button.Name = "button" + r + c;
                                stackPanel.Children.Add(button);
                            } // if
                            break;
                        case 5:
                            if (c % 2 == 1) {
                                button.Background = Brushes.Black;
                                button.Name = "button" + r + c;
                                stackPanel.Children.Add(button);
                            } // if
                            break;
                        case 6:
                            if (c % 2 == 0) {
                                button.Background = blackBrush;
                                button.Name = "buttonBlack" + r + c;
                                stackPanel.Children.Add(button);
                            } // if
                            break;
                        case 7:
                            if (c % 2 == 1) {
                                button.Background = blackBrush;
                                button.Name = "buttonBlack" + r + c;
                                stackPanel.Children.Add(button);
                            } // if
                            break;
                        case 8:
                            if (c % 2 == 0) {
                                button.Background = blackBrush;
                                button.Name = "buttonBlack" + r + c;
                                stackPanel.Children.Add(button);
                            } // if
                            break;
                    } // switch r
                } // for c
            } // for r
        } // MakeButtons


        /// <inheritdoc />
        /// <summary>
        /// Получить элемент макета Grid.
        /// </summary>
        /// <param name="g">Макет.</param>
        /// <param name="r">Строка.</param>
        /// <param name="c">Столбец.</param>
        /// <returns>Дочерний элемент панели Grid.</returns>
        public UIElement GetGridElement(Panel g, int r, int c)
        {
            for (var i = 0; i < g.Children.Count; i++) {
                var e = g.Children[i];
                if (Grid.GetRow(e) == r && Grid.GetColumn(e) == c)
                    return e;
            } // for i
            return null;
        } // GetGridElement


        /// <inheritdoc />
        /// <summary>
        /// Обработчик нажатия.
        /// </summary>
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            // Не даём ходить шашкой оппонента.
            // Даём ходить только тому, чей пришёл черёд.
            if (_loginData.UserName == _from.Item1 && _from.Item2 == "Чёрные" ||
                _loginData.UserName == _to.Item1 && _to.Item2 == "Чёрные") {
                if (button.Name.Contains("White")) {
                    DisplayMessage("Это шашка оппонента!");
                    return;
                } // if

                if (Turn != "Black") {
                    DisplayMessage("Сейчас не ваша очередь ходить!");
                    return;
                } // if
            } // if
            if (_loginData.UserName == _from.Item1 && _from.Item2 == "Белые" ||
                _loginData.UserName == _to.Item1 && _to.Item2 == "Белые") {
                if (button.Name.Contains("Black")) {
                    DisplayMessage("Это шашка оппонента!");
                    return;
                } // if

                if (Turn != "White") {
                    DisplayMessage("Сейчас не ваша очередь ходить!");
                    return;
                } // if
            } // if

            var stackPanel = (StackPanel)button.Parent;
            var row = Grid.GetRow(stackPanel);
            var col = Grid.GetColumn(stackPanel);
            Console.WriteLine(Resources.GameWithAiViewModel_Button_Click__Row + row
                              + Resources.GameWithAiViewModel_Button_Click__Column + col);
            if (CurrentMove == null)
                CurrentMove = new Move();
            if (CurrentMove.Piece1 == null) {
                CurrentMove.Piece1 = new Piece(row, col);
                stackPanel.Background = Brushes.SlateBlue;
            } else {
                CurrentMove.Piece2 = new Piece(row, col);
                stackPanel.Background = Brushes.SlateBlue;
            } // if-else
            if (CurrentMove.Piece1 != null && CurrentMove.Piece2 != null)
                _client.MakeMove(_from.Item1, _to.Item1, CurrentMove.Piece1.Row, CurrentMove.Piece1.Column, 
                                 CurrentMove.Piece2.Row, CurrentMove.Piece2.Column);
        } // Button_Click


        /// <inheritdoc />
        /// <summary>
        /// Проверить ход.
        /// </summary>
        public bool CheckMove()
        {
            var stackPanel1 = (StackPanel)GetGridElement(View.CheckersGrid, CurrentMove.Piece1.Row, CurrentMove.Piece1.Column);
            var stackPanel2 = (StackPanel)GetGridElement(View.CheckersGrid, CurrentMove.Piece2.Row, CurrentMove.Piece2.Column);
            var button1 = (Button)stackPanel1.Children[0];
            var button2 = (Button)stackPanel2.Children[0];
            stackPanel1.Background = Brushes.Black;
            stackPanel2.Background = Brushes.Black;

            switch (Turn) {
                case "Black" when button1.Name.Contains("White"):
                    CurrentMove.Piece1 = null;
                    CurrentMove.Piece2 = null;
                    DisplayMessage("Сейчас ходят чёрные.");
                    return false;
                case "White" when button1.Name.Contains("Black"):
                    CurrentMove.Piece1 = null;
                    CurrentMove.Piece2 = null;
                    DisplayMessage("Сейчас ходят белые.");
                    return false;
            } // switch turn

            if (button1.Equals(button2)) {
                CurrentMove.Piece1 = null;
                CurrentMove.Piece2 = null;
                return false;
            } // if

            if (button1.Name.Contains("Black")) {
                return CheckMoveBlack(button1, button2);
            } // if
            if (button1.Name.Contains("White")) {
                return CheckMoveWhite(button1, button2);
            } // if
            CurrentMove.Piece1 = null;
            CurrentMove.Piece2 = null;
            Console.WriteLine(Resources.GameWithAiViewModel_CheckMove_CheckMove__False);
            return false;
        } // CheckMove


        /// <inheritdoc />
        /// <summary>
        /// Проверить ход для белых.
        /// </summary>
        /// <param name="button1">Кнопка 1.</param>
        /// <param name="button2">Кнопка 2.</param>
        public bool CheckMoveWhite(IFrameworkInputElement button1, IFrameworkInputElement button2)
        {
            var currentBoard = GetCurrentBoard();
            var jumpMoves = currentBoard.CheckJumps("White");

            if (jumpMoves.Count > 0) {
                var invalid = true;

                foreach (var move in jumpMoves)
                    if (CurrentMove.Equals(move))
                        invalid = false;

                if (invalid) {
                    DisplayMessage("Шашка оппонента должна быть взята.");
                    CurrentMove.Piece1 = null;
                    CurrentMove.Piece2 = null;
                    Console.WriteLine(Resources.GameWithAiViewModel_CheckMoveWhite_CheckMoveWhite__False);
                    return false;
                } // if
            } // if

            if (button1.Name.Contains("White"))
                if (button1.Name.Contains("King")) {
                    if (CurrentMove.IsAdjacent("King") && !button2.Name.Contains("Black") && !button2.Name.Contains("White"))
                        return true;

                    var middlePiece = CurrentMove.CheckJump("King");

                    if (middlePiece != null && !button2.Name.Contains("Black") && !button2.Name.Contains("White")) {
                        var middleStackPanel = (StackPanel)GetGridElement(View.CheckersGrid, middlePiece.Row, middlePiece.Column);
                        var middleButton = (Button)middleStackPanel.Children[0];

                        if (middleButton.Name.Contains("Black")) {
                            View.CheckersGrid.Children.Remove(middleStackPanel);
                            AddBlackButton(middlePiece);
                            return true;
                        } // if
                    } // if
                } else {
                    if (CurrentMove.IsAdjacent("White") && !button2.Name.Contains("Black") && !button2.Name.Contains("White"))
                        return true;

                    var middlePiece = CurrentMove.CheckJump("White");
                    if (middlePiece != null && !button2.Name.Contains("Black") && !button2.Name.Contains("White")) {
                        var middleStackPanel = (StackPanel)GetGridElement(View.CheckersGrid, middlePiece.Row, middlePiece.Column);
                        var middleButton = (Button)middleStackPanel.Children[0];
                        if (middleButton.Name.Contains("Black")) {
                            View.CheckersGrid.Children.Remove(middleStackPanel);
                            AddBlackButton(middlePiece);
                            return true;
                        } // if
                    } // if
                } // if-else
            CurrentMove = null;
            DisplayMessage("Недопустимый ход. Повторите снова.");
            return false;
        } // CheckMoveWhite


        /// <inheritdoc />
        /// <summary>
        /// Проверить ход для чёрных.
        /// </summary>
        /// <param name="button1">Кнопка 1.</param>
        /// <param name="button2">Кнопка 2.</param>
        public bool CheckMoveBlack(IFrameworkInputElement button1, IFrameworkInputElement button2)
        {
            var currentBoard = GetCurrentBoard();
            var jumpMoves = currentBoard.CheckJumps("Black");

            if (jumpMoves.Count > 0) {
                var invalid = true;

                foreach (var move in jumpMoves)
                    if (CurrentMove.Equals(move))
                        invalid = false;

                if (invalid) {
                    DisplayMessage("Шашка оппонента должна быть взята.");
                    CurrentMove.Piece1 = null;
                    CurrentMove.Piece2 = null;
                    Console.WriteLine(Resources.GameWithAiViewModel_CheckMoveBlack_CheckMoveBlack__False);
                    return false;
                } // if
            } // if

            if (button1.Name.Contains("Black"))
                if (button1.Name.Contains("King")) {
                    if (CurrentMove.IsAdjacent("King") && !button2.Name.Contains("Black") && !button2.Name.Contains("White"))
                        return true;
                    var middlePiece = CurrentMove.CheckJump("King");

                    if (middlePiece != null && !button2.Name.Contains("Black") && !button2.Name.Contains("White")) {
                        var middleStackPanel = (StackPanel)GetGridElement(View.CheckersGrid, middlePiece.Row, middlePiece.Column);
                        var middleButton = (Button)middleStackPanel.Children[0];

                        if (middleButton.Name.Contains("White")) {
                            View.CheckersGrid.Children.Remove(middleStackPanel);
                            AddBlackButton(middlePiece);
                            return true;
                        } // if
                    } // if
                } else {
                    if (CurrentMove.IsAdjacent("Black") && !button2.Name.Contains("Black") && !button2.Name.Contains("White"))
                        return true;

                    var middlePiece = CurrentMove.CheckJump("Black");

                    if (middlePiece != null && !button2.Name.Contains("Black") && !button2.Name.Contains("White")) {
                        var middleStackPanel = (StackPanel)GetGridElement(View.CheckersGrid, middlePiece.Row, middlePiece.Column);
                        var middleButton = (Button)middleStackPanel.Children[0];
                        if (middleButton.Name.Contains("White")) {
                            View.CheckersGrid.Children.Remove(middleStackPanel);
                            AddBlackButton(middlePiece);
                            return true;
                        } // if
                    } // if
                } // if-else
            CurrentMove = null;
            DisplayMessage("Недопустимый ход. Повторите снова.");
            return false;
        } // CheckMoveBlack


        /// <inheritdoc />
        /// <summary>
        /// Сделать ход.
        /// </summary>
        public void MakeMove()
        {
            if (CurrentMove.Piece1 == null || CurrentMove.Piece2 == null)
                return;

            Console.WriteLine(Resources.GameWithAiViewModel_MakeMove_P1_row + CurrentMove.Piece1.Row +
                              Resources.GameWithAiViewModel_MakeMove_col + CurrentMove.Piece1.Column);
            Console.WriteLine(Resources.GameWithAiViewModel_MakeMove_P2_row + CurrentMove.Piece2.Row +
                              Resources.GameWithAiViewModel_MakeMove_col + CurrentMove.Piece2.Column);

            var stackPanel1 = (StackPanel)GetGridElement(View.CheckersGrid, CurrentMove.Piece1.Row, CurrentMove.Piece1.Column);
            var stackPanel2 = (StackPanel)GetGridElement(View.CheckersGrid, CurrentMove.Piece2.Row, CurrentMove.Piece2.Column);
            View.CheckersGrid.Children.Remove(stackPanel1);
            View.CheckersGrid.Children.Remove(stackPanel2);
            Grid.SetRow(stackPanel1, CurrentMove.Piece2.Row);
            Grid.SetColumn(stackPanel1, CurrentMove.Piece2.Column);
            View.CheckersGrid.Children.Add(stackPanel1);
            Grid.SetRow(stackPanel2, CurrentMove.Piece1.Row);
            Grid.SetColumn(stackPanel2, CurrentMove.Piece1.Column);
            View.CheckersGrid.Children.Add(stackPanel2);
            CheckKing(CurrentMove.Piece2);

            // Добавляем в список истории.
            // Смотрим по номеру хода, чтобы было ровно 2 хода 
            // с одним и тем же номером: ход игрока1 и ход игрока2.
            var tempCount = 0;
            _observableHistoryMove
                .ToList()
                .ForEach(move => {
                    if (move.Number == _numberMove)
                        tempCount++;
                });
            _timerMove.Stop();
            if (tempCount < 2) {
                _observableHistoryMove.Add(new HistoryMove(_numberMove, _timePassed,
                                                           $"{new BoardRowCol(CurrentMove.Piece1.Row, CurrentMove.Piece1.Column)}" +
                                                           $" -> {new BoardRowCol(CurrentMove.Piece2.Row, CurrentMove.Piece2.Column)}"));
            } else if (tempCount == 2) {
                _observableHistoryMove.Add(new HistoryMove(++_numberMove, _timePassed,
                                                           $"{new BoardRowCol(CurrentMove.Piece1.Row, CurrentMove.Piece1.Column)}" +
                                                           $" -> {new BoardRowCol(CurrentMove.Piece2.Row, CurrentMove.Piece2.Column)}"));
            } // if
            // Прокручиваем дата грид к последнему элементу.
            View.DataGridHistoryMove.ScrollIntoView(View.DataGridHistoryMove.Items[View.DataGridHistoryMove.Items.Count - 1]);

            _timeMove = new DateTime();
            _timePassed = "00:00:00";
            _timerMove.Start();
            CurrentMove = null;

            switch (Turn) {
                case "Black":
                    View.TextBlockPlayerWhiteMove.Visibility = Visibility.Visible;
                    View.TextBlockPlayerBlackMove.Visibility = Visibility.Collapsed;
                    View.StatusText.Text = "Сейчас ходят белые.";
                    Turn = "White";
                    break;
                case "White":
                    View.TextBlockPlayerWhiteMove.Visibility = Visibility.Collapsed;
                    View.TextBlockPlayerBlackMove.Visibility = Visibility.Visible;
                    View.StatusText.Text = "Сейчас ходят чёрные.";
                    Turn = "Black";
                    break;
            } // switch

            CheckWin();
        } // MakeMove


        /// <inheritdoc />
        /// <summary>
        /// Получить текущую доску.
        /// </summary>
        /// <returns>Шашечная доска.</returns>
        public CheckerBoard GetCurrentBoard()
        {
            var board = new CheckerBoard();

            for (var r = 1; r < 9; r++) {
                for (var c = 0; c < 8; c++) {
                    var stackPanel = (StackPanel)GetGridElement(View.CheckersGrid, r, c);
                    if (stackPanel.Children.Count > 0) {
                        var button = (Button)stackPanel.Children[0];
                        if (button.Name.Contains("White"))
                            board.SetState(r - 1, c, button.Name.Contains("King") ? 3 : 1);
                        else if (button.Name.Contains("Black"))
                            board.SetState(r - 1, c, button.Name.Contains("King") ? 4 : 2);
                        else
                            board.SetState(r - 1, c, 0);
                    } else {
                        board.SetState(r - 1, c, -1);
                    } // if-else
                } // for c
            } // for r
            return board;
        } // GetCurrentBoard


        /// <inheritdoc />
        /// <summary>
        /// Проверить ячейку (фигуру) на способность стать дамкой.
        /// </summary>
        public void CheckKing(Piece tmpPiece)
        {
            var stackPanel = (StackPanel)GetGridElement(View.CheckersGrid, tmpPiece.Row, tmpPiece.Column);

            if (stackPanel.Children.Count <= 0)
                return;

            var button = (Button)stackPanel.Children[0];
            var whiteBrush = new ImageBrush {
                ImageSource = new BitmapImage(new Uri(@"..\..\Images\Figures\checkerWhiteKingRed.png", UriKind.Relative))
            };
            var blackBrush = new ImageBrush {
                ImageSource = new BitmapImage(new Uri(@"..\..\Images\Figures\checkerBlackKingRed.png", UriKind.Relative))
            };

            if (button.Name.Contains("Black") && !button.Name.Contains("King")) {
                if (tmpPiece.Row == 1) {
                    button.Name = "button" + "Black" + "King" + tmpPiece.Row + tmpPiece.Column;
                    button.Background = blackBrush;
                } // if
            } else if (button.Name.Contains("White") && !button.Name.Contains("King")) {
                if (tmpPiece.Row == 8) {
                    button.Name = "button" + "White" + "King" + tmpPiece.Row + tmpPiece.Column;
                    button.Background = whiteBrush;
                } // if
            } // if-else
        } // CheckKing


        /// <inheritdoc />
        /// <summary>
        /// Добавить чёрную кнопку.
        /// </summary>
        public void AddBlackButton(Piece middleMove)
        {
            var stackPanel = new StackPanel {
                Background = Brushes.Black
            };
            var button = new Button();
            button.Click += Button_Click;
            button.Height = 190;
            button.Width = 190;
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.VerticalAlignment = VerticalAlignment.Center;
            button.Background = Brushes.Black;
            button.Name = "button" + middleMove.Row + middleMove.Column;
            stackPanel.Children.Add(button);
            Grid.SetColumn(stackPanel, middleMove.Column);
            Grid.SetRow(stackPanel, middleMove.Row);
            View.CheckersGrid.Children.Add(stackPanel);
        } // AddBlackButton


        /// <inheritdoc />
        /// <summary>
        /// Проверить победителя.
        /// </summary>
        public void CheckWin()
        {
            int totalBlack = 0, totalWhite = 0;
            for (var r = 1; r < 9; r++) {
                for (var c = 0; c < 8; c++) {
                    var stackPanel = (StackPanel)GetGridElement(View.CheckersGrid, r, c);
                    if (stackPanel.Children.Count > 0) {
                        var button = (Button)stackPanel.Children[0];
                        if (button.Name.Contains("White"))
                            totalWhite++;
                        if (button.Name.Contains("Black"))
                            totalBlack++;
                    } // if
                } // for c
            } // for r

            // Ведём счёт
            View.ScoreBlacksLeft.Text = $"{totalBlack}";
            View.ScoreBlacksTaked.Text = $"{12 - totalWhite}";
            View.ScoreWhitesLeft.Text = $"{totalWhite}";
            View.ScoreWhitesTaked.Text = $"{12 - totalBlack}";

            if (totalBlack == 0)
                Winner = "White";
            if (totalWhite == 0)
                Winner = "Black";

            if (Winner != null) {
                for (var r = 1; r < 9; r++) {
                    for (var c = 0; c < 8; c++) {
                        var stackPanel = (StackPanel)GetGridElement(View.CheckersGrid, r, c);
                        if (stackPanel.Children.Count > 0) {
                            var button = (Button)stackPanel.Children[0];
                            button.Click -= Button_Click;
                        } // if
                    } // for c
                } // for r

                View.TextBlockWinner.Text = Winner == "Black" ? "Чёрные победили!" : "Белые победили!";

                // Добавляем участников игры в историю
                // TODO: Переделать, чтобы выборка статистики при входе шла по двум направлениям - кто и с кем.
                // Тогда в базу добавлять нужно будет только 1 запись.
                // Сейчас выборка истории идёт по полю Who, то есть кто. 
                // А потому надо добавлять 2 записи сразу по двум участникам,
                // чтобы проследить статистику при входе.

                // TODO: Проверить вставку
                if (_from.Item2 == "Чёрные" && Winner == "Black") {
                    // Победил
                    var gh = new GameHistory {
                        GameTime = DateTime.Now,
                        Who = _from.Item1,
                        WithWhom = _to.Item1,
                        IdResult = 1
                    };
                    Db.GameHistory.InsertOnSubmit(gh);
                    // Проиграл
                    gh = new GameHistory {
                        GameTime = DateTime.Now,
                        Who = _to.Item1,
                        WithWhom = _from.Item1,
                        IdResult = 2
                    };
                    Db.GameHistory.InsertOnSubmit(gh);
                    Db.SubmitChanges();
                } else if (_from.Item2 == "Белые" && Winner == "White") {
                    // Победил
                    var gh = new GameHistory {
                        GameTime = DateTime.Now,
                        Who = _from.Item1,
                        WithWhom = _to.Item1,
                        IdResult = 1
                    };
                    Db.GameHistory.InsertOnSubmit(gh);
                    // Проиграл
                    gh = new GameHistory {
                        GameTime = DateTime.Now,
                        Who = _to.Item1,
                        WithWhom = _from.Item1,
                        IdResult = 2
                    };
                    Db.GameHistory.InsertOnSubmit(gh);
                    Db.SubmitChanges();
                } else if (_to.Item2 == "Чёрные" && Winner == "Black") {
                    // Победил
                    var gh = new GameHistory {
                        GameTime = DateTime.Now,
                        Who = _to.Item1,
                        WithWhom = _from.Item1,
                        IdResult = 1
                    };
                    Db.GameHistory.InsertOnSubmit(gh);
                    // Проиграл
                    gh = new GameHistory {
                        GameTime = DateTime.Now,
                        Who = _from.Item1,
                        WithWhom = _to.Item1,
                        IdResult = 2
                    };
                    Db.GameHistory.InsertOnSubmit(gh);
                    Db.SubmitChanges();
                } else if (_to.Item2 == "Белые" && Winner == "White") {
                    // Победил
                    var gh = new GameHistory {
                        GameTime = DateTime.Now,
                        Who = _to.Item1,
                        WithWhom = _from.Item1,
                        IdResult = 1
                    };
                    Db.GameHistory.InsertOnSubmit(gh);
                    // Проиграл
                    gh = new GameHistory {
                        GameTime = DateTime.Now,
                        Who = _from.Item1,
                        WithWhom = _to.Item1,
                        IdResult = 2
                    };
                    Db.GameHistory.InsertOnSubmit(gh);
                    Db.SubmitChanges();
                } // if

                // Если создатель игры (пригласивший) это Вы, 
                // то по окончанию у вас есть право начать игру по новой
                if (_loginData.UserName == _from.Item1) {
                    View.DialogHostWinner.DialogClosing += (s, a) => {
                        if (!Equals(a.Parameter, true))
                            return;
                        _client.StartNewGame(_from.Item1, _to.Item1);
                    };
                } else {
                    View.TextBlockDialogWinner.Text = $"Ожидайте пока {_from.Item1} примет решение о новой игре.";
                    View.StackPanelDialogWinnerButtons.Visibility = Visibility.Collapsed;
                } // if-else
                View.DialogHostWinner.IsOpen = true;
            } // if
        } // CheckWin


        /// <inheritdoc />
        /// <summary>
        /// Начать новую игру.
        /// </summary>
        public void StartNewGame()
        {
            CurrentMove = null;
            Winner = null;
            Turn = _firstMove == "Чёрные" ? "Black" : "White";
            View.StatusText.Text = Turn == "Black" ? "Сейчас ходят чёрные." : "Сейчас ходят белые.";
            ClearBoard();
            MakeBoard();
            View.ScoreBlacksLeft.Text = "12";
            View.ScoreBlacksTaked.Text = "0";
            View.ScoreWhitesLeft.Text = "12";
            View.ScoreWhitesTaked.Text = "0";

            // Обнуляем таймер хода.
            _timerMove.Stop();
            _timeMove = new DateTime();
            _timePassed = "00:00:00";
            _timerMove.Start();

            // Очищаем список истории ходов.
            // Перезапускаем общий таймер.
            _observableHistoryMove.Clear();
            _commonTimer.Stop();
            _commonTime = new DateTime();
            View.TextBlockTimePassed.Text = "00:00:00";
            _commonTimer.Start();

            _numberMove = 1;
        } // StartNewGame


        /// <summary>
        /// Отображает заданное сообщение в Snack и Status барах.
        /// </summary>
        /// <param name="msg">Сообщение</param>
        private void DisplayMessage(string msg)
        {
            var message = new SnackbarMessage {
                Content = msg.ToUpper(),
            };
            View.StatusText.Text = msg;
            View.Snackbar.Message = message;
            View.Snackbar.IsActive = true;

            var seconds = 0;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (o, e) => {
                seconds++;
                if (seconds != 3)
                    return;
                View.Snackbar.IsActive = false;
                timer.Stop();
            };
            timer.Start();
        } // DisplayMessage
    } // GameNetClientModel class
} // Checkers.Models