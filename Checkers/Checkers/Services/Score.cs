namespace Checkers.Services
{
    /// <summary>
    /// Служебный класс для удобства ведения счёта игры.
    /// </summary>
    public class Score
    {
        /// <summary>
        /// Осталось шашек.
        /// </summary>
        public int Left { get; set; }
        /// <summary>
        /// Взято шашек.
        /// </summary>
        public int Taked { get; set; }
    } // Score class
} // Checkers.Services