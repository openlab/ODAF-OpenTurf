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
    public class PointDataSummary
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Guid { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CommentCount { get; set; }
        public int RatingCount { get; set; }
        public long RatingTotal { get; set; }
        public string LayerId { get; set; }
        public int CurrentUserRating { get; set; }
        public string Tag { get; set; }
        public int CreatedById { get; set; }

        public string Name { get; set; }

        public DateTime LastRefreshed { get; set; }

        public UserComment[] UserComments { get; set; }

        public string CreatorProfileImageUrl
        {
            get
            {
                return ((App)Application.Current).pageRootUrl + "user/image/" + this.CreatedById;
            }

        }
    }
}
