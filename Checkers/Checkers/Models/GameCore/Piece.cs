namespace Checkers.Models.GameCore
{
    /// <summary>
    /// Представляет собой ячейку. Грубо говоря фигура.
    /// </summary>
    public class Piece
    {
        /// <summary>Строка.</summary>
        public int Row { get; }
        /// <summary>Столбец.</summary>
        public int Column { get; }


        public Piece(int row, int col)
        {
            Row = row;
            Column = col;
        } // Piece ctor


        public override bool Equals(object obj)
        {
            if (!(obj is Piece piece)) {
                return false;
            } // if

            return Row == piece.Row && Column == piece.Column;
        } // Equals


        public override int GetHashCode()
        {
            var hashCode = 240067226;
            hashCode = hashCode * -1521134295 + Row.GetHashCode();
            hashCode = hashCode * -1521134295 + Column.GetHashCode();
            return hashCode;
        } // GetHashCode
    } // Piece class
} // Checkers.Model