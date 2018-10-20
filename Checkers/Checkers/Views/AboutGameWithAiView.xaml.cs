using System.Linq;
using System.Windows;
using Checkers.ViewModels;
using CommonServiceLocator;


namespace Checkers.Views
{
    public partial class AboutGameWithAiView
    {
        public AboutGameWithAiView()
        {
            InitializeComponent();
            DataContext = ServiceLocator.Current.GetInstance<AboutGameWithAiViewModel>();
            Owner = Application.Current.Windows
                               .OfType<GameWithAiView>()
                               .FirstOrDefault();
        } // AboutGameWithAiView ctor
    } // AboutGameWithAiView class
} // Checkers.View