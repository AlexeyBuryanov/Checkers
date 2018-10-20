using System.Linq;
using System.Windows;
using Checkers.ViewModels;
using CommonServiceLocator;


namespace Checkers.Views
{
    public partial class AboutNetGameView
    {
        public AboutNetGameView()
        {
            InitializeComponent();
            DataContext = ServiceLocator.Current.GetInstance<AboutNetGameViewModel>();
            Owner = Application.Current.Windows
                               .OfType<GameNetClientView>()
                               .FirstOrDefault();
        } // AboutNetGameView ctor
    } // AboutNetGameView
} // Checkers.Views