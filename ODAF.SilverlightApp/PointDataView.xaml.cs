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
using Microsoft.Maps.MapControl.Core;
using ODAF.SilverlightApp.VO;
using ODAF.SilverlightApp.CloudService;


namespace ODAF.SilverlightApp
{

    public partial class PointDataView : UserControl
    {
        private PlaceMark currPlaceMark;
        public PlaceMark CurrentPlaceMark
        {
            get { return currPlaceMark; }
            set
            {
                this.currPlaceMark = value;
                this.UpdateData();
            }
        }

        private PointDataSummary _currentSummary;
        public PointDataSummary CurrentSummary
        {
            get { return _currentSummary; }
            set
            {
                _currentSummary = value;
                UpdateData();
            }
        }

        private SummariesController _summaryController;
        public SummariesController Summary 
        {
            get { return _summaryController; }
            set {

                _summaryController = value;
                _summaryController.SummaryUpdate += new SummaryUpdateEventHandler(OnSummaryUpdate);
            }
        
        }

        private UserController _userController;
        public UserController User
        {
            get { return _userController; }
            set
            {
                _userController = value;
                _userController.AuthUpdate += new AuthUpdateEventHandler(OnAuthUpdate);
            }
        }

        public MainPage MainPage { get; set; }

        void OnAuthUpdate(UserController sender)
        {
            if (sender.IsAuthenticated)
            {
                UpdateData();
            }
        }

        public UserControl CurrentView { get; set; }

        public PointDataView()
        {
            InitializeComponent();

            this.pinWheel.ViewStateChange += new PinWheelViewStateChangeEventHandler(pinWheel_ViewStateChange);
            this.pinWheel.ParentView = this;
            this.commentViewBox.ParentView = this;
            this.infoViewBox.ParentView = this;
            this.tagViewBox.ParentView = this;
            this.ratingViewBox.ParentView = this;

            CurrentView = this.infoViewBox;
        }

        public void HideView()
        {
            this.Visibility = Visibility.Collapsed;
        }

        void pinWheel_ViewStateChange(object sender, PinWheelViewStateChangeEventArgs e)
        {
            CurrentView.Visibility = Visibility.Collapsed;

            switch (e.viewState)
            {
                case ViewState.InfoViewState :
                    CurrentView = this.infoViewBox;
                    break;
                case ViewState.CommentViewState :
                    CurrentView = this.commentViewBox;
                    break;
                case ViewState.RatingViewState :
                    CurrentView = this.ratingViewBox;
                    break;
                case ViewState.TagViewState :
                    CurrentView = this.tagViewBox;
                    break;
            }

            CurrentView.Opacity = ( e.viewState == ViewState.AnimatingState ) ? 0 : 1;
            CurrentView.Visibility = Visibility.Visible;  
        }

        protected void UpdateData()
        {
            if (CurrentPlaceMark != null)
            {
                PointDataSummary pds = Summary.GetForPlaceMark(CurrentPlaceMark);

                infoViewBox.Data = pds;
                ratingViewBox.Data = pds;
                tagViewBox.Data = pds;
                commentViewBox.Data = pds;
            }
        }

        // Called by SummariesController when new data is retrieved
        // the data may not belong to our current placemark so we need to check the ids
        void  OnSummaryUpdate(object sender, SummaryUpdateArgs e)
        {
                PointDataSummary summary = e.result;
                if (CurrentPlaceMark != null && summary.Guid == CurrentPlaceMark.Id)
                {
                    // NOTE: because we are called by an ASynch thread, 
                    // we have to marshall to the UI thread with Dispatcher
                    this.Dispatcher.BeginInvoke(delegate()
                    {
                        CurrentSummary = summary;
                        infoViewBox.Data = summary;
                        ratingViewBox.Data = summary;
                        tagViewBox.Data = summary;
                        commentViewBox.Data = summary;
                    });
                }
        }

        public void AddItemRating(string itemGuid, int userRating)
        {
            Summary.AddRating(itemGuid, userRating);
        }

        public void AddItemTag(string itemGuid, string itemTag)
        {
            Summary.AddTag(itemGuid, itemTag);
        }
    }
}
