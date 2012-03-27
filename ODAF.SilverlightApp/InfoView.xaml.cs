using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ODAF.SilverlightApp.VO;

namespace ODAF.SilverlightApp
{
    public partial class InfoView : UserControl
    {
        public PointDataView ParentView { get; set; }

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

       

        public InfoView()
        {
            InitializeComponent();
        }


        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }

        public void UpdateView()
        {
            
            tbTitle.Text = Data.Name;
            if (Data.Description != null && Data.Description.Length > 0)
            {
                tbDescription.Text = Data.Description;
            }
            else
            {
                tbDescription.Text = Data.Name;
            }
            if (ParentView.CurrentPlaceMark.IsSystem)
            {
                imgCreator.DataContext = null;
                imgCreator.Visibility = Visibility.Collapsed;
            }
            else
            {
                imgCreator.DataContext = this.Data;
                imgCreator.Visibility = Visibility.Visible;

                if (this.ParentView.User.IsAuthenticated && (Data.CreatedById == this.ParentView.User.currentUser.Id))
                {

                    // TODO: add edit / delete buttons 
                    spOwnerButtons.Visibility = Visibility.Visible;
                }
                else
                {
                    spOwnerButtons.Visibility = Visibility.Collapsed;
                }
            }

            
        }

     

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {

            ParentView.MainPage.EditLandmark(ParentView.CurrentPlaceMark);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ParentView.MainPage.DeleteLandmark(ParentView.CurrentPlaceMark);
        }
    }
}
