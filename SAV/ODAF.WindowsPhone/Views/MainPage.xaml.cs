using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using ODAF.WindowsPhone.Models;
using ODAF.WindowsPhone.ViewModels;
using System.Windows.Markup;
using ODAF.WindowsPhone.Helpers;
using ODAF.WindowsPhone.Resources;

namespace ODAF.WindowsPhone.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Pushpin _precedentPushpin;
        private Pushpin _userLocationPushpin;
        private GeoCoordinateWatcher _watcher;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            object obj;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("Authorized", out obj) == false)
            {
                var result = MessageBox.Show(MainResource.GeoLocalizationInfo, MainResource.AuthorizationRequest, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    throw new Exception();
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings["Authorized"] = true;
                }
            }

            string template = "<ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><Image Source=\"/Images/cible_black.png\" Height=\"50\" Width=\"50\" /></ControlTemplate>";
            _userLocationPushpin = new Pushpin();
            _userLocationPushpin.Template = (ControlTemplate)XamlReader.Load(template);
            _userLocationPushpin.PositionOrigin = PositionOrigin.Center;

            UserLocationMapLayer.Children.Add(_userLocationPushpin);

            //GPS
            _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            _watcher.MovementThreshold = 3.0;
            _watcher.TryStart(false, TimeSpan.FromMilliseconds(2000));
            _watcher.PositionChanged += ((watcherSender, watcherEventArgs) =>
            {
                GeoCoordinate coord = watcherEventArgs.Position.Location;
                if (coord.IsUnknown != true)
                {
                    _userLocationPushpin.Location = coord;

                }
            });
        }

        //Raised when a pushpin is touched
        private void Pushpin_Tap(object sender, GestureEventArgs e)
        {
            Pushpin pushpin = (sender as Pushpin);
            if (_precedentPushpin != null & _precedentPushpin != pushpin)
            {
                VisualStateManager.GoToState(_precedentPushpin, "Normal", true);
            }

            var vm = this.Resources["MainViewModel"] as MainViewModel;
            if (vm != null)
            {
                vm.SelectedPushpin = pushpin.DataContext as PushpinModel;
                _precedentPushpin = pushpin;
            }

            e.Handled = true;
        }

        //Raised when the map is touched (not slided)
        private void MapItemsControl_Tap(object sender, GestureEventArgs e)
        {
            if (_precedentPushpin != null)
            {
                VisualStateManager.GoToState(_precedentPushpin, "Normal", true);
            }

            var vm = this.Resources["MainViewModel"] as MainViewModel;
            if (vm != null)
            {
                vm.SelectedPushpin = null;
            }
        }

        private void TwitterIconButton_Click(object sender, EventArgs e)
        {
            var vm = this.Resources["MainViewModel"] as MainViewModel;
            if (vm != null)
            {
                PushpinModel pushpin = vm.SelectedPushpin;
                if (pushpin != null)
                {
                    var radius = GeoMathHelper.CalculateRadius(pushpin.Location.Latitude, pushpin.Location.Longitude, App.Radius);
                    bool isInRadius = GeoMathHelper.IsPointInRadius(_userLocationPushpin.Location.Latitude, _userLocationPushpin.Location.Longitude, radius);
                    if (isInRadius == true)
                    {
                        IsolatedStorageSettings.ApplicationSettings["SelectedPushpinLatitude"] = pushpin.Location.Latitude;
                        IsolatedStorageSettings.ApplicationSettings["SelectedPushpinLongitude"] = pushpin.Location.Longitude;
                        NavigationService.Navigate(new Uri("/Views/TwitterPage.xaml", UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show(MainResource.NotCloseEnoughToTweet);
                    }
                }
            }
        }

        private void FacebookIconButton_Click(object sender, EventArgs e)
        {
            var vm = this.Resources["MainViewModel"] as MainViewModel;
            if (vm != null)
            {
                PushpinModel pushpin = vm.SelectedPushpin;
                if (pushpin != null)
                {
                    var radius = GeoMathHelper.CalculateRadius(pushpin.Location.Latitude, pushpin.Location.Longitude, App.Radius);
                    bool isInRadius = GeoMathHelper.IsPointInRadius(_userLocationPushpin.Location.Latitude, _userLocationPushpin.Location.Longitude, radius);
                    if (isInRadius == true)
                    {
                        NavigationService.Navigate(new Uri("/Views/FacebookPage.xaml", UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show(MainResource.NotCloseEnoughToFacebook);
                    }
                }
            }
        }

        private void MoiIconButton_Click(object sender, EventArgs e)
        {
            Map.SetView(_watcher.Position.Location, Map.ZoomLevel);
        }

        #region AppBar

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UpdateApplicationBar();
        }

        private void UpdateApplicationBar()
        {
           UpdateButton(0, MainResource.Facebook);
           UpdateButton(1, MainResource.Twitter);
           UpdateButton(2, MainResource.Me);
        }

        protected void UpdateButton(int index, string text)
        {
            var button = this.ApplicationBar.Buttons[index] as Microsoft.Phone.Shell.ApplicationBarIconButton;
            if(null != button)  
            {
                button.Text = text.ToLower();
            }
        }

        #endregion
    }
}