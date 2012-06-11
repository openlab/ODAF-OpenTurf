using System;
using System.Collections.Generic;
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
using ODAF.WindowsPhone.ViewModels;
using ODAF.WindowsPhone.Resources;

namespace ODAF.WindowsPhone.Views
{
    public partial class TwitterPage : PhoneApplicationPage
    {
        public TwitterPage()
        {
            InitializeComponent();
            var vm = this.Resources["TwitterViewModel"] as TwitterViewModel;
            vm.PropertyChanged +=TwitterAuthorizationUri_PropertyChanged;
            vm.TweetSent += new EventHandler(vm_TweetSent);
        }

        void vm_TweetSent(object sender, EventArgs e)
        {
            MessageBox.Show(TwitterResource.TweetSent);
            NavigationService.GoBack();
        }

        void TwitterAuthorizationUri_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TwitterAuthorizationUri")
            {
                Dispatcher.BeginInvoke(() =>
                {
                    var vm = this.Resources["TwitterViewModel"] as TwitterViewModel;
                    if (vm.TwitterAuthorizationUri != null)
                    {
                        Browser.Navigate(vm.TwitterAuthorizationUri);
                        MessageBox.Show(TwitterResource.PleaseLogon, TwitterResource.ApplicationAuthorization, MessageBoxButton.OK);
                    }                 
                });
            }
        }

        private void BackIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void PhotoIconButton_Click(object sender, EventArgs e)
        {
            var vm = this.Resources["TwitterViewModel"] as TwitterViewModel;
            vm.TakePhotoCommand.Execute(null);
        }

        private void ResetIconButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(TwitterResource.AreYouSureToReset, TwitterResource.Reset, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                var vm = this.Resources["TwitterViewModel"] as TwitterViewModel;
                vm.ResetTwitterCredentialsCommand.Execute(null);
                NavigationService.GoBack();
            }
        }

        #region AppBar

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UpdateApplicationBar();
        }

        private void UpdateApplicationBar()
        {
            UpdateButton(0, TwitterResource.Back);
            UpdateButton(1, TwitterResource.Photo);
            UpdateButton(2, TwitterResource.Reset);
        }

        protected void UpdateButton(int index, string text)
        {
            var button = this.ApplicationBar.Buttons[index] as Microsoft.Phone.Shell.ApplicationBarIconButton;
            if (null != button)
            {
                button.Text = text.ToLower();
            }
        }

        #endregion
    }
}