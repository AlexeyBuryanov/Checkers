using Checkers.ViewModels;
using CommonServiceLocator;


namespace Checkers.Views
{
    public partial class RegistrationView
    {
        public RegistrationView()
        {
            InitializeComponent();
            DataContext = ServiceLocator.Current.GetInstance<RegistrationViewModel>();
        } // RegistrationView ctor
    } // RegistrationView class
} // Checkers.Views