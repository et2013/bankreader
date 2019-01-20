using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BankReader.Messages;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using BankReader.API.Services;

namespace BankReader.ViewModels
{
    public sealed class TransactionsViewModel : Screen, IHasBusyIndicator, IHandle<RefreshEvent>, IReloadable
    {
        private ObservableCollection<BankTransfer> _accounts;
        private bool _isBusy;
        private DateTime _dateFilter;
        private readonly IEventAggregator _aggregator;

        protected override void OnDeactivate(bool close)
        {
            _aggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        protected override void OnActivate()
        {
            _aggregator.Subscribe(this);
            base.OnActivate();
        }

        protected override void OnViewLoaded(object view)
        {
            _aggregator.Subscribe(this);
            base.OnViewLoaded(view);
        }

        public ObservableCollection<BankTransfer> Accounts
        {
            get { return _accounts; }
            set
            {
                _accounts = value;
                NotifyOfPropertyChange(() => Accounts);
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

        public DateTime DateFilter
        {
            get { return _dateFilter; }
            set
            {
                _dateFilter = value;
                NotifyOfPropertyChange(() => DateFilter);
            }
        }

        public TransactionsViewModel(IEventAggregator aggregator)
        {
            DisplayName = "Transactions";
            _aggregator = aggregator;
            DateFilter = DateTime.Now;
        }

        public IResult Reload()
        {
            var dialog = new OpenFileDialog { Multiselect = true };
            var showDialog = dialog.ShowDialog().GetValueOrDefault();

            if (!showDialog)
                return new Finished();

            var task = Task.Factory.StartNew(() => dialog.FileNames
                                                         .SelectMany(BankParser.Parse)
                                                         //.Where(x => x.Date >= DateFilter)
                                                         .ToList())
                           .ContinueWith(rslt =>
                               {
                                   Accounts = new ObservableCollection<BankTransfer>(rslt.Result);
                                   IsBusy = false;
                               }, TaskScheduler.FromCurrentSynchronizationContext());

            return new Finished(task);
        }

        public void Handle(RefreshEvent @event)
        {
            Reload();
        }
    }
}