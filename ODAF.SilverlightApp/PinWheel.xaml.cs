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
using Microsoft.Maps.MapControl;

namespace ODAF.SilverlightApp
{
    public enum ViewState
    {
        AnimatingState,
        InfoViewState,
        CommentViewState,
        RatingViewState,
        TagViewState
    }

    public class PinWheelViewStateChangeEventArgs : EventArgs
    {
        public ViewState viewState;

        public PinWheelViewStateChangeEventArgs(ViewState currentState)
        {
            this.viewState = currentState;
        }
    }

    public delegate void PinWheelViewStateChangeEventHandler(object sender, PinWheelViewStateChangeEventArgs e);

    public partial class PinWheel : UserControl
    {
        public event PinWheelViewStateChangeEventHandler ViewStateChange;

        public PinWheel()
        {
            InitializeComponent();

            imgStar.MouseLeftButtonUp += imgStar_MouseLeftButtonUp;
            imgComment.MouseLeftButtonUp += imgComment_MouseLeftButtonUp;
            imgTag.MouseLeftButtonUp += imgTag_MouseLeftButtonUp;
            imgInfo.MouseLeftButtonUp += imgInfo_MouseLeftButtonUp;

            Loaded += (s, e) =>
            {
                PinWheelViewStateChangeEventArgs args = new PinWheelViewStateChangeEventArgs(ViewState.InfoViewState);
                if (ViewStateChange != null)
                {
                    ViewStateChange(this, args);
                }
                linkInfo.Focus();
            };

        }

        void imgInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // dispatch some sort of event for ViewState changed
            PinWheelViewStateChangeEventArgs args = new PinWheelViewStateChangeEventArgs(ViewState.InfoViewState);
            if (ViewStateChange != null)
            {
                ViewStateChange(this, args);
            }
        }

        void imgTag_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // dispatch some sort of event for ViewState changed
            PinWheelViewStateChangeEventArgs args = new PinWheelViewStateChangeEventArgs(ViewState.TagViewState);
            if (ViewStateChange != null)
            {
                ViewStateChange(this, args);
            }
        }

        void imgStar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // dispatch some sort of event for ViewState changed
            PinWheelViewStateChangeEventArgs args = new PinWheelViewStateChangeEventArgs(ViewState.RatingViewState);
            if (ViewStateChange != null)
            {
                ViewStateChange(this, args);
            }
        }

        void imgComment_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // dispatch some sort of event for ViewState changed
            PinWheelViewStateChangeEventArgs args = new PinWheelViewStateChangeEventArgs(ViewState.CommentViewState);
            if (ViewStateChange != null)
            {
                ViewStateChange(this, args);
            }
        }

        private void linkInfo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            imgInfo_MouseLeftButtonUp(sender, null);
        }

        private void linkTags_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            imgTag_MouseLeftButtonUp(sender, null);
        }

        private void linkComments_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            imgComment_MouseLeftButtonUp(sender, null);
        }

        private void linkratings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            imgStar_MouseLeftButtonUp(sender, null);
        }

        private void OnCloseButton(object sender, MouseButtonEventArgs e)
        {
            ParentView.HideView();
        }

        public PointDataView ParentView { get; set; }
    }
}
