using Checkers.ViewModels;
using CommonServiceLocator;


namespace Checkers.Views
{
    public partial class GameNetLoginView
    {
        public GameNetLoginView()
        {
            InitializeComponent();
            DataContext = ServiceLocator.Current.GetInstance<GameNetLoginViewModel>();
        } // GameNetLoginView ctor
    } // GameNetLoginView class
} // Checkers.Views