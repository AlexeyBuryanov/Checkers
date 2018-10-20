using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Checkers.Models;
using Checkers.Models.Interfaces;
using Checkers.ViewModels;


namespace Checkers.IoC
{
    /// <inheritdoc />
    /// <summary>
    /// Здесь регистрируются основные типы, которые будут создаваться при помощи
    /// DI (Dependency Injection).
    /// </summary>
    public class MainModule : Module
    {
        /// <inheritdoc />
        /// <summary>
        /// Участвует в построении контейнера зависимостей.
        /// </summary>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<GameWithAiModel>().As<IGameWithAiModel>();
            builder.RegisterType<GameNetServerModel>().As<IGameNetServerModel>();
            builder.RegisterType<GameNetLoginModel>().As<IGameNetLoginModel>();
            builder.RegisterType<GameNetLobbyModel>().As<IGameNetLobbyModel>();
            builder.RegisterType<GameNetClientModel>().As<IGameNetClientModel>();

            // Модели для взаимодействия и в окне предзагрузки, и во ViewModel.
            // -----------------------------------------------------------------
            // Модель игры с ИИ.
            var gameWithAiModel = new GameWithAiModel();
            // Модель сервера.
            var gameNetServerModel = new GameNetServerModel();
            // Модель входа на сервер.
            var gameNetLoginModel = new GameNetLoginModel();
            // Модель лобби.
            var gameNetLobbyModel = new GameNetLobbyModel();


            builder.RegisterType<PreloaderModel>().As<IPreloaderModel>()
                   .WithParameters(new List<Parameter> {
                       new NamedParameter("gameWithAiModel", gameWithAiModel),
                       new NamedParameter("gameNetServerModel", gameNetServerModel),
                       new NamedParameter("gameNetLoginModel", gameNetLoginModel),
                       new NamedParameter("gameNetLobbyModel", gameNetLobbyModel)
                   });

            builder.RegisterType<PreloaderViewModel>();
            builder.RegisterType<GameWithAiViewModel>()
                   .WithParameters(new List<Parameter> {
                       new NamedParameter("gameWithAiModel", gameWithAiModel)
                   });
            builder.RegisterType<GameNetServerViewModel>()
                   .WithParameters(new List<Parameter> {
                       new NamedParameter("gameNetServerModel", gameNetServerModel)
                   });
            builder.RegisterType<GameNetLoginViewModel>()
                   .WithParameters(new List<Parameter> {
                       new NamedParameter("gameNetLoginModel", gameNetLoginModel)
                   });
            builder.RegisterType<GameNetLobbyViewModel>()
                   .WithParameters(new List<Parameter> {
                       new NamedParameter("gameNetLobbyModel", gameNetLobbyModel)
                   });
            builder.RegisterType<RegistrationViewModel>();
            builder.RegisterType<AboutGameWithAiViewModel>();
            builder.RegisterType<AboutNetGameViewModel>();
        } // Load
    } // CheckersModule class
} // Checkers.IoC