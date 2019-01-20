using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Reflection;
using System.Windows;
using BankReader.API.Services;
using BankReader.ViewModels;
using Caliburn.Micro;
using Microsoft.Mef.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;

namespace BankReader
{
    public class AppBootstrapper : BootstrapperBase
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        /// <summary>
        ///     By default, we are configured to use MEF
        /// </summary>
        protected override void Configure()
        {
            var registrationBuilder = new RegistrationBuilder();
            registrationBuilder.ForTypesDerivedFrom<IWindowManager>().ExportInterfaces();
            registrationBuilder.ForTypesDerivedFrom<IEventAggregator>().ExportInterfaces();
            registrationBuilder.ForType<TransactionsViewModel>().Export();
            registrationBuilder.ForType<SettingsViewModel>().Export();
            registrationBuilder.ForTypesDerivedFrom<IParser>().ExportInterfaces();
            registrationBuilder.ForTypesDerivedFrom<IShell>().ExportInterfaces();

            var aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(IWindowManager).Assembly, registrationBuilder));
            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(IEventAggregator).Assembly, registrationBuilder));
            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(IParser).Assembly, registrationBuilder));
            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly(), registrationBuilder));

            var compositionContainer = new CompositionContainer(aggregateCatalog);
            ServiceLocator.SetLocatorProvider(() => new MefServiceLocator(compositionContainer));

            var invokeAction = ActionMessage.InvokeAction;
            ActionMessage.InvokeAction = context =>
            {
                // Find the parent view

                if (context.View is IHasBusyIndicator shell)
                {
                    shell.IsBusy = true;

                    void CoroutineOnCompleted(object sender, ResultCompletionEventArgs args)
                    {
                        shell.IsBusy = false;
                        Coroutine.Completed -= CoroutineOnCompleted;
                    }

                    Coroutine.Completed += CoroutineOnCompleted;
                }

                invokeAction(context);
            };
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            return ServiceLocator.Current.GetInstance(serviceType, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return ServiceLocator.Current.GetAllInstances(serviceType);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<IShell>();
        }
    }
}