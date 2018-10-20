using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Checkers.IoC;
using Checkers.Views;
using CommonServiceLocator;


namespace Checkers
{
    /// <inheritdoc />
    /// <summary>Логика взаимодействия приложения.</summary>
    public partial class App
    {
        private Mutex _mutex;


        public App()
        {
            InitializeDependencies();
            InitializeComponent();
        } // App ctor


        /// <summary>Инициализация зависимостей.</summary>
        public void InitializeDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new MainModule());

            var locator = new AutofacServiceLocator(builder.Build());
            ServiceLocator.SetLocatorProvider(() => locator);
        } // InitializeDependencies


        /// <inheritdoc />
        /// <summary>
        /// Точка входа приложения.
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Перехват необработанных исключений.
            DispatcherUnhandledException += UnhandledExceptionExecute;
            //Dispatcher.UnhandledException += UnhandledExceptionExecute;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            
            // Запуск одного экземпляра приложения.
            _mutex = new Mutex(true, "AppCheckersMutexUnique12345", out var createdNew);
            if (!createdNew) {
                //Shutdown();
            } // if

            // Отображаем окно предзагрузки, оно же окно-меню.
            new PreloaderView().Show();
        } // OnStartup


        /// <summary>
        /// Для перехвата необработанных исключений диспетчера.
        /// </summary>
        private static void UnhandledExceptionExecute(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Ошибка!",
                MessageBoxButton.OK, MessageBoxImage.Error);
            // Синхронно завершаем процесс работы диспетчера.
            e.Dispatcher.InvokeShutdown();
            e.Handled = true;
        } // UnhandledExceptionExecute


        /// <summary>
        /// Для перехвата необработанных исключений текущего домена приложения.
        /// </summary>
        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString(), "Необработанное исключение!",
                MessageBoxButton.OK, MessageBoxImage.Error);
            // Закрываем все окна.
            foreach (Window window in Windows) {
                window.Close();
            } // foreach
        } // CurrentDomainUnhandledException
    } // App class
} // Checkers