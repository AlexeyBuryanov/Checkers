using System.Linq;
using System.Windows;
using Checkers.ViewModels;
using CommonServiceLocator;


namespace Checkers.Views
{
    public partial class GameWithAiView
    {
        public GameWithAiView()
        {
            InitializeComponent();
            DataContext = ServiceLocator.Current.GetInstance<GameWithAiViewModel>();
            Application.Current.Windows
                       .AsParallel()
                       .AsOrdered()
                       .Cast<Window>()
                       .ToList()
                       .ForEach(window => {
                           if (window.Tag as string != "Preloader")
                               return;
                           Loaded += (s, args) => { window.Hide(); };
                           Closing += (s, args) => { window.Show(); };
                       });
        } // GameWithAiView ctor
    } // GameWithAiView class
} // Checkers.View