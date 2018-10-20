using System.Diagnostics;
using System.Linq;
using System.Windows;
using Checkers.ViewModels;
using CommonServiceLocator;


namespace Checkers.Views
{
    public partial class GameNetServerView
    {
        public GameNetServerView()
        {
            InitializeComponent();
            DataContext = ServiceLocator.Current.GetInstance<GameNetServerViewModel>();
            Application.Current.Windows
                       .AsParallel()
                       .AsOrdered()
                       .Cast<Window>()
                       .ToList()
                       .ForEach(window => {
                           if (window.Tag as string != "Preloader")
                               return;
                           Loaded += (s, args) => { window.Hide(); };
                           var view = window as PreloaderView;
                           Closing += (s, args) => {
                               Debug.Assert(view != null, nameof(view) + " != null");
                               view.ButtonOnlineGameServer.IsEnabled = true;
                               view.ButtonOnlineGameClient.IsEnabled = true;
                               window.Show();
                           };
                       });
        } // GameNetServer ctor
    } // GameNetServer class
} // Checkers.Views