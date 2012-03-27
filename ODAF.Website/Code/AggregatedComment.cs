using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ODAF.Data;

namespace vancouveropendata
{
    public class AggregatedComment
    {
        public PointDataComment Comment { get; set; }
        public string CommentAuthor { get; set; }
        public string ServiceName { get; set; }
    }
}
