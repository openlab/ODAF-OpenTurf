using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using ODAF.WindowsPhone.Commands;
using Microsoft.Phone.Tasks;

namespace ODAF.WindowsPhone.Models
{
    public class Comment : INotifyPropertyChanged
    {      
        private DateTime _CreatedOn;
        public DateTime CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                if (value != _CreatedOn)
                {
                    _CreatedOn = value;
                    NotifyPropertyChanged("CreatedOn");
                }
            }
        }

        private int _Id;
        public int Id
        {
            get { return _Id; }
            set
            {
                if (value != _Id)
                {
                    _Id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        private string _Text;
        public string Text
        {
            get { return _Text; }
            set
            {
                if (value != _Text)
                {
                    _Text = value;
                    NotifyPropertyChanged("Text");
                }
            }
        }
        
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }

    public class CommentModel : INotifyPropertyChanged
    {
        #region Commands


        private ICommand _NavigateToBrowserCommand;
        public ICommand NavigateToBrowserCommand
        {
            get { return _NavigateToBrowserCommand; }
            set
            {
                if (value != _NavigateToBrowserCommand)
                {
                    _NavigateToBrowserCommand = value;
                    NotifyPropertyChanged("NavigateToBrowserCommand");
                }
            }
        }
        

        #endregion

        #region Properties

        private Comment _Comment;
        public Comment Comment
        {
            get { return _Comment; }
            set
            {
                if (value != _Comment)
                {
                    _Comment = value;
                    NotifyPropertyChanged("Comment");
                }
            }
        }

        public string CommentTextWithoutUrl
        {
            get 
            {
                string[] explode = Comment.Text.Split(new[] { "http://" }, StringSplitOptions.None);
                return explode.First();
            }
        }

        public string CommentUrl
        {
            get 
            {
                string[] explode = Comment.Text.Split(new []{"http://"}, StringSplitOptions.RemoveEmptyEntries);
                if (explode.Count() > 1)
                {
                    return "http://" + explode.Last();
                }
                else
                {
                    return null;
                }
            }
        }
        
        private string _CommentAuthor;
        public string CommentAuthor
        {
            get { return _CommentAuthor; }
            set
            {
                if (value != _CommentAuthor)
                {
                    _CommentAuthor = value;
                    NotifyPropertyChanged("CommentAuthor");
                }
            }
        }

        private string _ServiceName;
        public string ServiceName
        {
            get { return _ServiceName; }
            set
            {
                if (value != _ServiceName)
                {
                    _ServiceName = value;
                    NotifyPropertyChanged("ServiceName");
                }
            }
        }

        #endregion

        public CommentModel()
        {
            NavigateToBrowserCommand = new DelegateCommand(NavigateToBrowser, (obj) => true);
        }

        private void NavigateToBrowser(object obj)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(this.CommentUrl, UriKind.Absolute);
            task.Show();
        }
        
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
