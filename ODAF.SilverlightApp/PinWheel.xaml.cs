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

            imgStar.MouseLeftButtonUp += new MouseButtonEventHandler(imgStar_MouseLeftButtonUp);
            imgStar.MouseEnter += new MouseEventHandler(imgStar_MouseEnter);
            imgStar.MouseLeave += new MouseEventHandler(imgStar_MouseLeave);

            imgComment.MouseLeftButtonUp += new MouseButtonEventHandler(imgComment_MouseLeftButtonUp);
            imgComment.MouseEnter += new MouseEventHandler(imgComment_MouseEnter);
            imgComment.MouseLeave += new MouseEventHandler(imgComment_MouseLeave);

            imgTag.MouseLeftButtonUp +=new MouseButtonEventHandler(imgTag_MouseLeftButtonUp);
            imgTag.MouseEnter += new MouseEventHandler(imgTag_MouseEnter);
            imgTag.MouseLeave += new MouseEventHandler(imgTag_MouseLeave);


            imgInfo.MouseLeftButtonUp += new MouseButtonEventHandler(imgInfo_MouseLeftButtonUp);
            imgInfo.MouseEnter += new MouseEventHandler(imgInfo_MouseEnter);
            imgInfo.MouseLeave += new MouseEventHandler(imgInfo_MouseLeave);

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

        void imgStar_MouseLeave(object sender, MouseEventArgs e)
        {
            upStarImg.Visibility = Visibility.Visible;
            hoverStarImg.Visibility = Visibility.Collapsed;
        }

        void imgStar_MouseEnter(object sender, MouseEventArgs e)
        {
            hoverStarImg.Visibility = Visibility.Visible;
            upStarImg.Visibility = Visibility.Collapsed;
        }

        void imgComment_MouseLeave(object sender, MouseEventArgs e)
        {
            upCommentImg.Visibility = Visibility.Visible;
            hoverCommentImg.Visibility = Visibility.Collapsed;
        }

        void imgComment_MouseEnter(object sender, MouseEventArgs e)
        {
            hoverCommentImg.Visibility = Visibility.Visible;
            upCommentImg.Visibility = Visibility.Collapsed;
        }

        void imgTag_MouseLeave(object sender, MouseEventArgs e)
        {
            upTagImg.Visibility = Visibility.Visible;
            hoverTagImg.Visibility = Visibility.Collapsed;
        }

        void imgTag_MouseEnter(object sender, MouseEventArgs e)
        {
            hoverTagImg.Visibility = Visibility.Visible;
            upTagImg.Visibility = Visibility.Collapsed;
        }

        void imgInfo_MouseLeave(object sender, MouseEventArgs e)
        {
            upInfoImg.Visibility = Visibility.Visible;
            hoverInfoImg.Visibility = Visibility.Collapsed;
        }

        void imgInfo_MouseEnter(object sender, MouseEventArgs e)
        {
            hoverInfoImg.Visibility = Visibility.Visible;
            upInfoImg.Visibility = Visibility.Collapsed;
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
