namespace Checkers.Services
{
    /// <summary>
    /// Данные, которые отображаются в DataTemplate ListBox.
    /// </summary>
    public class UserTemplate
    {
        /// <summary>
        /// Имя/ник.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// В сети или в игре.
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Путь к картинке.
        /// </summary>
        public string Avatar { get; set; }

        public UserTemplate(string name, string status, string avatar)
        {
            Name = name;
            Status = status;
            Avatar = avatar;
        } // UserTemplate ctor
    } // UserTemplate class
} // Checkers.Services