using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BankReader.Messages;
using BankReader.ViewModels;
using Caliburn.Micro;
using Microsoft.Practices.ServiceLocation;

namespace BankReader
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IHasBusyIndicator, IHandle<BusyMessage>,
        IShell
    {
        private readonly IEventAggregator _aggregator;
        private bool _isBusy;

        public ShellViewModel(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            Tabs = new List<IScreen>
            {
                ServiceLocator.Current.GetInstance<TransactionsViewModel>(),
                ServiceLocator.Current.GetInstance<SettingsViewModel>()
            };

            ActivateItem(Tabs.First());

            aggregator.Subscribe(this);
        }

        public override string DisplayName
        {
            get => string.Concat("Bankreader", " ", Assembly.GetExecutingAssembly().GetName().Version);
            set { }
        }

        public IList<IScreen> Tabs { get; set; }

        public void Handle(BusyMessage message)
        {
            IsBusy = message.IsBusy;
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

        public IResult Reload()
        {
            var task = Task.Factory.StartNew(() => _aggregator.PublishOnUIThread(new RefreshEvent()));
            return new Finished(task);
        }
    }
}