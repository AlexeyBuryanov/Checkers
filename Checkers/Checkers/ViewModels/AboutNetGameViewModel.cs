using System.Reflection;


namespace Checkers.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// View Model для окна "о программе" сетевой игры. 
    /// <see cref="T:Checkers.Views.AboutNetGameView" />.
    /// </summary>
    public sealed class AboutNetGameViewModel : BaseViewModel
    {
        public AboutNetGameViewModel()
        {
            ProductName = AssemblyProduct;
            Version = AssemblyVersion;
            Copyright = AssemblyCopyright;
            CompanyName = AssemblyCompany;
            Description = AssemblyDescription;
            Description += ". Сетевой режим.";
        } // AboutNetGameViewModel


        //******************** СВОЙСТВА СВЯЗАННЫЕ С ИНТЕРФЕЙСОМ ***********************//
        private string _productName;
        public string ProductName {
            get => _productName;
            set {
                _productName = value;
                OnPropertyChanged();
            } // set
        } // ProductName


        private string _version;
        public string Version {
            get => _version;
            set {
                _version = value;
                OnPropertyChanged();
            } // set
        } // Version


        private string _copyright;
        public string Copyright {
            get => _copyright;
            set {
                _copyright = value;
                OnPropertyChanged();
            } // set
        } // Copyright


        private string _companyName;
        public string CompanyName {
            get => _companyName;
            set {
                _companyName = value;
                OnPropertyChanged();
            } // set
        } // CompanyName


        private string _description;
        public string Description {
            get => _description;
            set {
                _description = value;
                OnPropertyChanged();
            } // set
        } // Description


        #region //************************* АТРИБУТЫ СБОРКИ ***************************//
        /// <summary>
        /// Заголовок программы
        /// </summary>
        public string AssemblyTitle {
            get {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length <= 0)
                    return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
                var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                return titleAttribute.Title != "" ? titleAttribute.Title : System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            } // get
        } // AssemblyTitle


        /// <summary>
        /// Версия
        /// </summary>
        public string AssemblyVersion
            => Assembly.GetExecutingAssembly().GetName().Version.ToString();


        /// <summary>
        /// Описание
        /// </summary>
        public string AssemblyDescription {
            get {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyDescriptionAttribute)attributes[0]).Description;
            } // get
        } // AssemblyDescription


        /// <summary>
        /// Имя продукта (приложения)
        /// </summary>
        public string AssemblyProduct {
            get {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyProductAttribute)attributes[0]).Product;
            } // get
        } // AssemblyProduct


        /// <summary>
        /// Авторские права
        /// </summary>
        public string AssemblyCopyright {
            get {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            } // get
        } // AssemblyCopyright


        /// <summary>
        /// Организация
        /// </summary>
        public string AssemblyCompany {
            get {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyCompanyAttribute)attributes[0]).Company;
            } // get
        } // AssemblyCompany
        #endregion
    } // AboutNetGameViewModel class
} // Checkers.ViewModels