using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ODAF.SilverlightApp.VO
{
    public class UserComment
    {
        public Comment Comment;
        public string CommentAuthor { get; set; }
        public string ServiceName { get; set; }
        public string Text
        {
            get
            {
                return this.Comment.Text;
            }
        }

        public string FormattedDate
        {
            get
            {
                if (this.Comment.Id == 0)
                {
                    return "";
                }
                else
                {
                    TimeSpan ts = DateTime.Now - Comment.CreatedOn.ToLocalTime();
                    if (Math.Abs(ts.TotalDays) > 7)
                    {
                        return this.Comment.CreatedOn.ToString("d MMM yyyy");
                    }
                    else if (Math.Abs(ts.TotalHours) > 36)
                    {
                        return string.Format("{0} days ago.", ts.Days);
                    }
                    else if (Math.Abs(ts.TotalMinutes) > 119)
                    {
                        return string.Format("{0} hours ago.", (int)ts.Hours);
                    }
                    else if (Math.Abs(ts.TotalMinutes) > 59)
                    {
                        return "about an hour ago";
                    }
                    else if (Math.Abs(ts.TotalSeconds) > 200)
                    {
                        return string.Format("{0} minutes ago.", (int)ts.TotalMinutes);
                    }
                    else
                    {
                        return "Moments ago.";
                    }               
                }
            }
        }
    }

    public class Comment
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedById { get; set; }

        public int SummaryId { get; set; }


    }
}
