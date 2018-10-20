using System.Collections.Generic;


namespace Checkers.Models.GameCore
{
    /// <summary>
    /// Искусственный интеллект. 
    /// </summary>
    public class CheckersAi
    {
        /// <summary>
        /// Получить ход.
        /// </summary>
        /// <param name="currentBoard">Текущая доска.</param>
        /// <returns>Ход.</returns>
        public static Move GetMove(CheckerBoard currentBoard)
        {
            var availableMoves = GetAvailableMoves(currentBoard);
            availableMoves.Shuffle();
            return availableMoves.Count < 1 ? null : availableMoves[0];
        } // GetMove


        /// <summary>
        /// Получить доступные ходы.
        /// </summary>
        /// <param name="currentBoard">Текущая доска.</param>
        /// <returns>Список ходов.</returns>
        private static List<Move> GetAvailableMoves(CheckerBoard currentBoard)
        {
            // Текущие фигуры
            var currentPieces = new List<Piece>();
            // Доступные ходы
            var availableMoves = new List<Move>();
            // TODO: "Прыжки"
            var jumpMoves = currentBoard.CheckJumps("White");

            // Если есть "прыжок" - возвращаем их список
            if (jumpMoves.Count > 0) {
                return jumpMoves;
            } // if

            for (var r = 0; r < 8; r++) {
                for (var c = 0; c < 8; c++) {
                    if (currentBoard.GetState(r, c) == 1 || currentBoard.GetState(r, c) == 3) {
                        currentPieces.Add(new Piece(r, c));
                    } // if
                } // for c
            } // for r

            foreach (var p in currentPieces) {
                availableMoves.AddRange(CheckForMoves(p, currentBoard));
            } // foreach

            return availableMoves;
        } // GetAvailableMoves


        /// <summary>
        /// Проверить ходы.
        /// </summary>
        /// <param name="piece">Фигура.</param>
        /// <param name="currentBoard">Текущая доска.</param>
        /// <returns>Перечисление ходов.</returns>
        private static IEnumerable<Move> CheckForMoves(Piece piece, CheckerBoard currentBoard)
        {
            var moves = new List<Move>();

            switch (currentBoard.GetState(piece.Row, piece.Column)) {
                case 3:
                    if (currentBoard.GetState(piece.Row - 1, piece.Column - 1) == 2 || currentBoard.GetState(piece.Row - 1, piece.Column - 1) == 4)
                        if (currentBoard.GetState(piece.Row - 2, piece.Column - 2) == 0)
                            moves.Add(new Move(new Piece(piece.Row + 1, piece.Column), new Piece(piece.Row - 1, piece.Column - 2)));
                    if (currentBoard.GetState(piece.Row - 1, piece.Column + 1) == 2 || currentBoard.GetState(piece.Row - 1, piece.Column + 1) == 4)
                        if (currentBoard.GetState(piece.Row - 2, piece.Column + 2) == 0)
                            moves.Add(new Move(new Piece(piece.Row + 1, piece.Column), new Piece(piece.Row - 1, piece.Column + 2)));
                    if (currentBoard.GetState(piece.Row + 1, piece.Column - 1) == 2 || currentBoard.GetState(piece.Row + 1, piece.Column - 1) == 4)
                        if (currentBoard.GetState(piece.Row + 2, piece.Column - 2) == 0)
                            moves.Add(new Move(new Piece(piece.Row + 1, piece.Column), new Piece(piece.Row + 3, piece.Column - 2)));
                    if (currentBoard.GetState(piece.Row + 1, piece.Column + 1) == 2 || currentBoard.GetState(piece.Row + 1, piece.Column + 1) == 4)
                        if (currentBoard.GetState(piece.Row + 2, piece.Column + 2) == 0)
                            moves.Add(new Move(new Piece(piece.Row + 1, piece.Column), new Piece(piece.Row + 3, piece.Column + 2)));
                    if (currentBoard.GetState(piece.Row - 1, piece.Column - 1) == 0)
                        moves.Add(new Move(new Piece(piece.Row + 1, piece.Column), new Piece(piece.Row, piece.Column - 1)));
                    if (currentBoard.GetState(piece.Row - 1, piece.Column + 1) == 0)
                        moves.Add(new Move(new Piece(piece.Row + 1, piece.Column), new Piece(piece.Row, piece.Column + 1)));
                    if (currentBoard.GetState(piece.Row + 1, piece.Column - 1) == 0)
                        moves.Add(new Move(new Piece(piece.Row + 1, piece.Column), new Piece(piece.Row + 2, piece.Column - 1)));
                    if (currentBoard.GetState(piece.Row + 1, piece.Column + 1) == 0)
                        moves.Add(new Move(new Piece(piece.Row + 1, piece.Column), new Piece(piece.Row + 2, piece.Column + 1)));
                    break;
                case 1:
                    if (currentBoard.GetState(piece.Row + 1, piece.Column - 1) == 2 || currentBoard.GetState(piece.Row + 1, piece.Column - 1) == 4)
                        if (currentBoard.GetState(piece.Row + 2, piece.Column - 2) == 0)
                            moves.Add(new Move(new Piece(piece.Row + 1, piece.Column), new Piece(piece.Row + 3, piece.Column - 2)));
                    if (currentBoard.GetState(piece.Row + 1, piece.Column + 1) == 2 || currentBoard.GetState(piece.Row + 1, piece.Column + 1) == 4)
                        if (currentBoard.GetState(piece.Row + 2, piece.Column + 2) == 0)
                            moves.Add(new Move(new Piece(piece.Row + 1, piece.Column), new Piece(piece.Row + 3, piece.Column + 2)));
                    if (currentBoard.GetState(piece.Row + 1, piece.Column - 1) == 0)
                        moves.Add(new Move(new Piece(piece.Row + 1, piece.Column), new Piece(piece.Row + 2, piece.Column - 1)));
                    if (currentBoard.GetState(piece.Row + 1, piece.Column + 1) == 0)
                        moves.Add(new Move(new Piece(piece.Row + 1, piece.Column), new Piece(piece.Row + 2, piece.Column + 1)));
                    break;
            } // switch GetState
            return moves;
        } // CheckForMoves
    } // CheckersAi class
} // Checkers.Model