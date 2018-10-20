using System.Collections.Generic;


namespace Checkers.Models.GameCore
{
    /// <summary>
    /// Шашечная доска.
    /// </summary>
    public class CheckerBoard
    {
        /// <summary>
        /// Свойство доски.
        /// </summary>
        public int[,] Board = new int[8, 8];


        public CheckerBoard()
        {
            // Инициализируем начальное состояние.
            for (var r = 0; r < 8; r++) {
                for (var c = 0; c < 8; c++) {
                    Board[r, c] = -1;
                } // for c
            } // for r
        } // CheckersBoard


        /// <summary>
        /// Изменить состояние ячейки.
        /// </summary>
        /// <param name="r">Строка.</param>
        /// <param name="c">Столбец.</param>
        /// <param name="state">Состояние.</param>
        /// <returns>Успешно ли.</returns>
        public bool SetState(int r, int c, int state)
        {
            if (state > 4 || state < -1)
                return false;
            Board[r, c] = state;
            return true;
        } // SetState


        /// <summary>
        /// Получить состояние ячейки.
        /// </summary>
        /// <param name="r">Строка.</param>
        /// <param name="c">Столбец.</param>
        /// <returns>Состояние.</returns>
        public int GetState(int r, int c)
        {
            if (r > 7 || r < 0 || c > 7 || c < 0)
                return -1;
            return Board[r, c];
        } // GetState


        /// <summary>
        /// Проверка возможных "прыжков".
        /// </summary>
        /// <param name="color">Цвет фигуры в виде строки. "Black" или "White".</param>
        /// <returns>Список ходов.</returns>
        public List<Move> CheckJumps(string color)
        {
            // "Прыжки" (список ходов)
            var jumps = new List<Move>();

            // Проходим по всей доске
            for (var r = 0; r < 8; r++) {
                for (var c = 0; c < 8; c++) {
                    switch (color) {
                        // Для белых фигур
                        case "White":
                            switch (GetState(r, c)) {
                                case 3:
                                    if (GetState(r - 2, c - 2) == 0 && (GetState(r - 1, c - 1) == 2 || GetState(r - 1, c - 1) == 4))
                                        jumps.Add(new Move(new Piece(r + 1, c), new Piece(r - 1, c - 2)));
                                    if (GetState(r - 2, c + 2) == 0 && (GetState(r - 1, c + 1) == 2 || GetState(r - 1, c + 1) == 4))
                                        jumps.Add(new Move(new Piece(r + 1, c), new Piece(r - 1, c + 2)));
                                    if (GetState(r + 2, c - 2) == 0 && (GetState(r + 1, c - 1) == 2 || GetState(r + 1, c - 1) == 4))
                                        jumps.Add(new Move(new Piece(r + 1, c), new Piece(r + 3, c - 2)));
                                    if (GetState(r + 2, c + 2) == 0 && (GetState(r + 1, c + 1) == 2 || GetState(r + 1, c + 1) == 4))
                                        jumps.Add(new Move(new Piece(r + 1, c), new Piece(r + 3, c + 2)));
                                    break;
                                case 1:
                                    if (GetState(r + 2, c - 2) == 0 && (GetState(r + 1, c - 1) == 2 || GetState(r + 1, c - 1) == 4))
                                        jumps.Add(new Move(new Piece(r + 1, c), new Piece(r + 3, c - 2)));
                                    if (GetState(r + 2, c + 2) == 0 && (GetState(r + 1, c + 1) == 2 || GetState(r + 1, c + 1) == 4))
                                        jumps.Add(new Move(new Piece(r + 1, c), new Piece(r + 3, c + 2)));
                                    break;
                            } // switch GetState
                            break;
                        // Для чёрных фигур
                        case "Black":
                            switch (GetState(r, c)) {
                                case 4:
                                    if (GetState(r - 2, c - 2) == 0 && (GetState(r - 1, c - 1) == 1 || GetState(r - 1, c - 1) == 3))
                                        jumps.Add(new Move(new Piece(r + 1, c), new Piece(r - 1, c - 2)));
                                    if (GetState(r - 2, c + 2) == 0 && (GetState(r - 1, c + 1) == 1 || GetState(r - 1, c + 1) == 3))
                                        jumps.Add(new Move(new Piece(r + 1, c), new Piece(r - 1, c + 2)));
                                    if (GetState(r + 2, c - 2) == 0 && (GetState(r + 1, c - 1) == 1 || GetState(r + 1, c - 1) == 3))
                                        jumps.Add(new Move(new Piece(r + 1, c), new Piece(r + 3, c - 2)));
                                    if (GetState(r + 2, c + 2) == 0 && (GetState(r + 1, c + 1) == 1 || GetState(r + 1, c + 1) == 3))
                                        jumps.Add(new Move(new Piece(r + 1, c), new Piece(r + 3, c + 2)));
                                    break;
                                case 2:
                                    if (GetState(r - 2, c - 2) == 0 && (GetState(r - 1, c - 1) == 1 || GetState(r - 1, c - 1) == 3))
                                        jumps.Add(new Move(new Piece(r + 1, c), new Piece(r - 1, c - 2)));
                                    if (GetState(r - 2, c + 2) == 0 && (GetState(r - 1, c + 1) == 1 || GetState(r - 1, c + 1) == 3))
                                        jumps.Add(new Move(new Piece(r + 1, c), new Piece(r - 1, c + 2)));
                                    break;
                            } // switch GetState
                            break;
                    } // switch color
                } // for c
            } // for r

            return jumps;
        } // CheckJumps
    } // CheckersBoard class
} // Checkers.Model