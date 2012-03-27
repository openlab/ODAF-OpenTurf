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

namespace ODAF.SilverlightApp
{
    public partial class RatingView : UserControl
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

        public RatingView()
        {
            InitializeComponent();

            rating.ValueChanged += new RoutedPropertyChangedEventHandler<double?>(rating_ValueChanged);
        }

        void rating_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (e.NewValue.HasValue)
            {
                int newRate =(int)(e.NewValue.Value * 100);
                ParentView.AddItemRating(this.Data.Guid, newRate);
            }
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }

        public void UpdateView()
        {
            if (Data.RatingCount > 0)
            {
                double ratingVal = Data.RatingTotal / Data.RatingCount / 100.0; // out of 5 stars, but stored from 0-100
                rating.ValueChanged -= new RoutedPropertyChangedEventHandler<double?>(rating_ValueChanged);
                rating.Value = ratingVal;
                rating.ValueChanged += new RoutedPropertyChangedEventHandler<double?>(rating_ValueChanged);
            }
            else
            {
                rating.ValueChanged -= new RoutedPropertyChangedEventHandler<double?>(rating_ValueChanged);
                rating.Value = 0;
                rating.ValueChanged += new RoutedPropertyChangedEventHandler<double?>(rating_ValueChanged);
                //tbRatingText.Text = "No Ratings";
            }

            tbTitle.Text = Data.Name;
            if (Data.CurrentUserRating > 0)
            {
                tbCurrentUserRating.Text = string.Format("You rated this landmark {0} stars", (Data.CurrentUserRating / 20));
                rating.IsReadOnly = true;
            }
            else
            {
                if (ParentView.User.IsAuthenticated)
                {
                    tbCurrentUserRating.Text = "Click a star to add your rating.";
                    rating.IsReadOnly = false;
                }
                else
                {
                    tbCurrentUserRating.Text = "You need to sign in to rate this landmark.";
                    rating.IsReadOnly = true;
                }
            }
        }

        private void btnStar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Data.CurrentUserRating == 0)
            {
                Image star = (Image)sender;
                int rating = 0;

                switch (star.Tag.ToString())
                {
                    case "1":
                        rating = 20; break;
                    case "2":
                        rating = 40; break;
                    case "3":
                        rating = 60; break;
                    case "4":
                        rating = 80; break;
                    case "5":
                        rating = 100; break;

                }

                ParentView.AddItemRating(this.Data.Guid, rating);
            }

        }

        private void OnCloseButton(object sender, MouseButtonEventArgs e)
        {
            ParentView.HideView();
        }
    }
}
