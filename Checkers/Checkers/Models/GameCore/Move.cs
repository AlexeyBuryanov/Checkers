using System.Collections.Generic;


namespace Checkers.Models.GameCore
{
    /// <summary>
    /// Движение/ход.
    /// </summary>
    public class Move
    {
        /// <summary>Ячейка 1.</summary>
        public Piece Piece1 { get; set; }
        /// <summary>Ячейка 2.</summary>
        public Piece Piece2 { get; set; }


        public Move()
        {
            Piece1 = null;
            Piece2 = null;
        } // Move ctor

        public Move(Piece piece1, Piece piece2)
        {
            Piece1 = piece1;
            Piece2 = piece2;
        } // Move ctor


        /// <summary>
        /// Смежный ли ход.
        /// </summary>
        /// <param name="color">Цвет фигуры в виде строки. "Black" или "White".</param>
        /// <returns>true or false.</returns>
        public bool IsAdjacent(string color)
        {
            switch (color) {
                // Для чёрных фигур
                case "Black":
                    if (Piece1.Row - 1 == Piece2.Row && Piece1.Column - 1 == Piece2.Column)
                        return true;
                    if (Piece1.Row - 1 == Piece2.Row && Piece1.Column + 1 == Piece2.Column)
                        return true;
                    break;
                // Для белых фигур
                case "White":
                    if (Piece1.Row + 1 == Piece2.Row && Piece1.Column - 1 == Piece2.Column)
                        return true;
                    if (Piece1.Row + 1 == Piece2.Row && Piece1.Column + 1 == Piece2.Column)
                        return true;
                    break;
                // Для дамок
                case "King":
                    if (Piece1.Row - 1 == Piece2.Row && Piece1.Column - 1 == Piece2.Column)
                        return true;
                    if (Piece1.Row - 1 == Piece2.Row && Piece1.Column + 1 == Piece2.Column)
                        return true;
                    if (Piece1.Row + 1 == Piece2.Row && Piece1.Column - 1 == Piece2.Column)
                        return true;
                    if (Piece1.Row + 1 == Piece2.Row && Piece1.Column + 1 == Piece2.Column)
                        return true;
                    break;
            } // switch
            return false;
        } // IsAdjacent


        /// <summary>
        /// Проверить "прыжок".
        /// </summary>
        /// <param name="color">Цвет фигуры в виде строки. "Black" или "White".</param>
        /// <returns>Ячейка.</returns>
        public Piece CheckJump(string color)
        {
            switch (color) {
                // Для чёрных фигур
                case "Black":
                    if (Piece1.Row - 2 == Piece2.Row && Piece1.Column - 2 == Piece2.Column)
                        return new Piece(Piece1.Row - 1, Piece1.Column - 1);
                    if (Piece1.Row - 2 == Piece2.Row && Piece1.Column + 2 == Piece2.Column)
                        return new Piece(Piece1.Row - 1, Piece1.Column + 1);
                    break;
                // Для белых фигур
                case "White":
                    if (Piece1.Row + 2 == Piece2.Row && Piece1.Column - 2 == Piece2.Column)
                        return new Piece(Piece1.Row + 1, Piece1.Column - 1);
                    if (Piece1.Row + 2 == Piece2.Row && Piece1.Column + 2 == Piece2.Column)
                        return new Piece(Piece1.Row + 1, Piece1.Column + 1);
                    break;
                // Для дамок
                case "King":
                    if (Piece1.Row - 2 == Piece2.Row && Piece1.Column - 2 == Piece2.Column)
                        return new Piece(Piece1.Row - 1, Piece1.Column - 1);
                    if (Piece1.Row - 2 == Piece2.Row && Piece1.Column + 2 == Piece2.Column)
                        return new Piece(Piece1.Row - 1, Piece1.Column + 1);
                    if (Piece1.Row + 2 == Piece2.Row && Piece1.Column - 2 == Piece2.Column)
                        return new Piece(Piece1.Row + 1, Piece1.Column - 1);
                    if (Piece1.Row + 2 == Piece2.Row && Piece1.Column + 2 == Piece2.Column)
                        return new Piece(Piece1.Row + 1, Piece1.Column + 1);
                    break;
            } // switch
            return null;
        } // CheckJump


        public override bool Equals(System.Object obj)
        {
            if (!(obj is Move move)) {
                return false;
            } // if

            return Piece1.Equals(move.Piece1) && Piece2.Equals(move.Piece2);
        } // Equals


        public override int GetHashCode()
        {
            var hashCode = 1659376913;
            hashCode = hashCode * -1521134295 + EqualityComparer<Piece>.Default.GetHashCode(Piece1);
            hashCode = hashCode * -1521134295 + EqualityComparer<Piece>.Default.GetHashCode(Piece2);
            return hashCode;
        } // GetHashCode
    } // Move class
} // Checkers.Model