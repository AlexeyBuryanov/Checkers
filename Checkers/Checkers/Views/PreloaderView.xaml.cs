using Checkers.ViewModels;
using CommonServiceLocator;


namespace Checkers.Views
{
    public partial class PreloaderView
    {
        public PreloaderView()
        {
            InitializeComponent();
            DataContext = ServiceLocator.Current.GetInstance<PreloaderViewModel>();
        } // PreloaderView ctor
    } // PreloaderView class
} // Checkers.View