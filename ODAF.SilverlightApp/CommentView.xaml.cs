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
    public partial class CommentView : UserControl
    {
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


        public PointDataView ParentView { get; set; }

        private UserComment[] EmptyList;

        public CommentView()
        {
            InitializeComponent();
            
            UserComment noComments = new UserComment();
                        noComments.Comment = new Comment();
                        noComments.Comment.Text= "No Comments";
                        EmptyList = new UserComment[1] { noComments };
            
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }

        public void UpdateView()
        {
            tbTitle.Text = Data.Name;
            
            if(Data.UserComments != null && Data.UserComments.Length > 0)
            {
                if (Data.UserComments.Length == 1)
                {
                    tbCommentListLabel.Text = "1 Comment";
                }
                else
                {
                    tbCommentListLabel.Text = string.Format("{0} Comments", Data.UserComments.Length);
                }
            }
            else
            {
                tbCommentListLabel.Text = "No Comments";
            }
           
            lbComments.ItemsSource = Data.UserComments;
            CommentOutput.Visibility = Visibility.Visible;
            CommentEntry.Visibility = Visibility.Collapsed;

            if (ParentView.User.IsAuthenticated)
            {
                AddBtn.Visibility = Visibility.Visible;
                signinHint.Visibility = Visibility.Collapsed;
                tweetImage.Visibility = Visibility.Visible;
            }
            else
            {
                AddBtn.Visibility = Visibility.Collapsed;
                signinHint.Visibility = Visibility.Visible;
                tweetImage.Visibility = Visibility.Collapsed;
            }

        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Check if user is authenticated!
            tbCommentEntry.Text = "";
            CommentOutput.Visibility = Visibility.Collapsed;
            CommentEntry.Visibility = Visibility.Visible;


        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CommentOutput.Visibility = Visibility.Visible;
            CommentEntry.Visibility = Visibility.Collapsed;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Validate, excape it!
            String comment = tbCommentEntry.Text;
            bool bAutoTweet = this.cbAutoTweet.IsChecked == true;
            if (comment.Length > 0)
            {
                this.ParentView.Summary.AddComment(Data.Guid, comment, bAutoTweet);
            }
           
        }

    
        private void TwitterImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (App.Current.RootVisual as MainPage).ShowTweetWin(this.Data);
        }
    }
}
