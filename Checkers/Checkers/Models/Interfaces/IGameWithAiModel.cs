using System.Windows;
using System.Windows.Controls;
using Checkers.Models.GameCore;
using Checkers.Views;


namespace Checkers.Models.Interfaces
{
    /// <summary>
    /// Интерфейс модели игры с компьютером.
    /// </summary>
    public interface IGameWithAiModel
    {
        /// <summary>
        /// Визуальная часть. Окно построенное средствами XAML.
        /// </summary>
        /// <remarks>
        /// Было принято решение работать с видом напрямую, а не
        /// в рамках паттерна MVVM. Естественно, это отклонение,
        /// которое желательно избегать при применении
        /// паттерна MVVM, но, если не забывать, что это игра,
        /// где вид часто обновляется, то я считаю это вполне допустимо.
        /// </remarks>
        GameWithAiView View { get; set; }
        /// <summary>
        /// Текущий ход.
        /// </summary>
        Move CurrentMove { get; set; }
        /// <summary>
        /// Победитель.
        /// </summary>
        string Winner { get; set; }
        /// <summary>
        /// Очередь.
        /// </summary>
        string Turn { get; set; }

        /// <summary>
        /// Инициализация визуальной части.
        /// </summary>
        void InitializeView(GameWithAiView view);
        /// <summary>
        /// Очистить доску.
        /// </summary>
        void ClearBoard();
        /// <summary>
        /// "Сделать" доску.
        /// </summary>
        void MakeBoard();
        /// <summary>
        /// Сделать кнопки.
        /// </summary>
        void MakeButtons();
        /// <summary>
        /// Получить элемент макета Grid.
        /// </summary>
        UIElement GetGridElement(Panel g, int r, int c);
        /// <summary>
        /// Обработчик нажатия.
        /// </summary>
        void Button_Click(object sender, RoutedEventArgs e);
        /// <summary>
        /// Проверить ход.
        /// </summary>
        bool CheckMove();
        /// <summary>
        /// Проверить ход для белых.
        /// </summary>
        bool CheckMoveWhite(IFrameworkInputElement button1, IFrameworkInputElement button2);
        /// <summary>
        /// Проверить ход для чёрных.
        /// </summary>
        bool CheckMoveBlack(IFrameworkInputElement button1, IFrameworkInputElement button2);
        /// <summary>
        /// Сделать ход.
        /// </summary>
        void MakeMove();
        /// <summary>
        /// ИИ делает ход.
        /// </summary>
        void AiMakeMove();
        /// <summary>
        /// Получить текущую доску.
        /// </summary>
        CheckerBoard GetCurrentBoard();
        /// <summary>
        /// Проверить ячейку (фигуру) на способность стать дамкой.
        /// </summary>
        void CheckKing(Piece tmpPiece);
        /// <summary>
        /// Добавить чёрную кнопку.
        /// </summary>
        void AddBlackButton(Piece middleMove);
        /// <summary>
        /// Проверить победителя.
        /// </summary>
        void CheckWin();
        /// <summary>
        /// Начать новую игру.
        /// </summary>
        void StartNewGame();
    } // IGameWithAiModel interface
} // Checkers.Model.Interfaces