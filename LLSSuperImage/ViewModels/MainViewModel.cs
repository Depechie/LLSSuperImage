using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LLSSuperImage.Controls;
using LLSSuperImage.Framework;
using LLSSuperImage.Model;

namespace LLSSuperImage.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private SortedObservableCollection<Event> _events = new SortedObservableCollection<Event>();
        public SortedObservableCollection<Event> Events
        {
            get { return _events; }
            set
            {
                if (_events == value)
                    return;

                _events = value;
                RaisePropertyChanged("Events");
            }
        }

        private bool _viewLoadedCommandEnabled = true;
        public bool ViewLoadedCommandEnabled
        {
            get
            {
                return _viewLoadedCommandEnabled;
            }
            set
            {
                if (_viewLoadedCommandEnabled == value)
                    return;

                _viewLoadedCommandEnabled = value;
                RaisePropertyChanged("ViewLoadedCommandEnabled");
            }
        }

        public RelayCommand ViewLoadedCommand { get; protected set; }

        public MainViewModel()
        {
            this.ViewLoadedCommand = new RelayCommand(async () =>
            {
                await this.GetEvents();
                //Only keep track of the view loaded event once, at program startup
                ViewLoadedCommandEnabled = false;
            });
        }

        private async Task GetEvents()
        {
            int concertsLoaded = 0;
            int currentDistance = 5;
            bool firstLoad = true;
            while (concertsLoaded < 15)
            {
                concertsLoaded = await this.LoadEvents(currentDistance, firstLoad, false);
                if (concertsLoaded < 15)
                    currentDistance *= 2;

                firstLoad = false;
            }
        }

        private async Task<int> LoadEvents(int distance, bool clearLists, bool triggerIsBusy = true)
        {
            if (clearLists)
                this.Events.Clear();

            var events = await Task.Run(() => DataService.GetInstance().GetEventsAsync(51.1274, 4.4971, distance));
            events.ForEach(item =>
            {
                ThreadPool.QueueUserWorkItem(state => item.DistanceToCurrentUserPosition = item.venue.DistanceToCurrentUserPosition = Geocoding.GetInstance().CalculateDistanceFrom(double.Parse(item.venue.location.geopoint.geolat, new CultureInfo("en-US")), double.Parse(item.venue.location.geopoint.geolong, new CultureInfo("en-US")), 51.1274, 4.4971));
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (!this.Events.Any(i => i.id.Equals(item.id, StringComparison.OrdinalIgnoreCase)))
                        this.Events.Add(item);
                }
                );
            });

            return this.Events.Count;
        }
    }
}
