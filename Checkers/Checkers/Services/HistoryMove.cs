namespace Checkers.Services
{
    /// <summary>
    /// Служебный класс для удобства работы с данными 
    /// в DataGrid - DataGridHistoryMove.
    /// </summary>
    public class HistoryMove
    {
        /// <summary>
        /// Номер хода.
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Время хода.
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// Откуда и куда. В виде строки. Например f2-f3.
        /// </summary>
        public string Move { get; set; }

        public HistoryMove(int number, string time, string move)
        {
            Number = number;
            Time = time;
            Move = move;
        } // HistoryMove ctor
    } // HistoryMove class
} // Checkers.Services