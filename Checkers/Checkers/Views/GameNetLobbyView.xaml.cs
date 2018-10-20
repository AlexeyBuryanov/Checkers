using System.Diagnostics;
using System.Linq;
using System.Windows;
using Checkers.ViewModels;
using CommonServiceLocator;


namespace Checkers.Views
{
    public partial class GameNetLobbyView
    {
        public GameNetLobbyView()
        {
            InitializeComponent();
            DataContext = ServiceLocator.Current.GetInstance<GameNetLobbyViewModel>();
            Application.Current.Windows
                       .AsParallel()
                       .AsOrdered()
                       .Cast<Window>()
                       .ToList()
                       .ForEach(window => {
                           if (window.Tag as string != "Preloader")
                               return;
                           var view = window as PreloaderView;
                           Closing += (s, args) => {
                               Debug.Assert(view != null, nameof(view) + " != null");
                               view.ButtonOnlineGameClient.IsEnabled = true;
                               view.ButtonOnlineGameServer.IsEnabled = true;
                           };
                       });
        } // GameNetLobbyView
    } // GameNetLobbyView class
} // Checkers.Views