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
using ODAF.SilverlightApp.VO;
using System.Windows.Browser;
using Microsoft.Maps.MapControl;
using System.Windows.Data;

namespace ODAF.SilverlightApp
{
    public partial class CreatePointView : UserControl
    {

        public PointDataViewCreate ParentView { get; set; }

        private PointDataSummary _data;

        public PointDataSummary Data
        {
            get { return _data; }


            set
            {
                _data = value;
                UpdateView();
            }
        }


        public CreatePointView()
        {
            InitializeComponent();
            tbName.DataContext = this;
            tbDescription.DataContext = this;
        }


        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }

        public void UpdateView()
        {

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ParentView.Cancel();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

            if (tbName.Text.Length < 8)
            {
                tbName.BorderBrush = new SolidColorBrush(Colors.Red);
                tbNameReq.Visibility = Visibility.Visible;
                return;
            }

            if (tbDescription.Text.Length < 8)
            {
                tbName.BorderBrush = new SolidColorBrush(Colors.Black);
                tbDescription.BorderBrush = new SolidColorBrush(Colors.Red);
                tbNameReq.Visibility = Visibility.Collapsed;
                tbDescReq.Visibility = Visibility.Visible;
                return;
            }


            PointDataSummary temp = new PointDataSummary();
            temp.Description = HttpUtility.HtmlEncode(this.tbDescription.Text);
            temp.Name = HttpUtility.HtmlEncode(this.tbName.Text);
            temp.LayerId = this.ParentView.User.currentUser.user_id.ToString();
            temp.Latitude = this.ParentView.Pin.Location.Latitude;
            temp.Longitude = this.ParentView.Pin.Location.Longitude;
            //temp.Tag = this.ParentView.User.currentUser.screen_name;
            temp.Guid = Guid.NewGuid().ToString();

            //"Description", "LayerId", "Latitude", "Longitude", "Tag", "Guid", "Name"
            //string name = this.tbName.Text

            this.ParentView.CurrentSummary = temp;
            this.ParentView.Summary.CreatePointDataSummary(temp);

            ClearForm();
            this.ParentView.Visibility = Visibility.Collapsed;
        }

        public void ClearForm()
        {
            tbDescription.Text = "";
            tbName.Text = "";
        }

        private void btnClose_MouseEnter(object sender, MouseEventArgs e)
        {
            // TODO: Rollover image
        }

        private void btnClose_MouseLeave(object sender, MouseEventArgs e)
        {
            // TODO: Rollover image
        }

        private void onTextChanged(object sender, TextChangedEventArgs e)
        {
            tbDescription.BorderBrush = new SolidColorBrush(Colors.Black);
            tbDescReq.Visibility = Visibility.Collapsed;
        }

        private void tbName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbName.Text.Length < 8)
            {
                tbName.BorderBrush = new SolidColorBrush(Colors.Red);
                tbNameReq.Visibility = Visibility.Visible;
            }
            else
            {
                tbName.BorderBrush = new SolidColorBrush(Colors.Black);
                tbNameReq.Visibility = Visibility.Collapsed;
            }
        }

        private void tbName_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbName.BorderBrush = new SolidColorBrush(Colors.Black);
            tbNameReq.Visibility = Visibility.Collapsed;
        }

        private void btnClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Cancel_Click(null, null);
        }

    }
}
