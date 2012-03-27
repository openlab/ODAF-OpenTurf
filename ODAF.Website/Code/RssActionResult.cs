using System.Web.Mvc;
using BitlyDotNET.Interfaces;
using BitlyDotNET.Implementations;
using System.Configuration;
using ODAF.Data;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace vancouveropendata
{

    public class RssActionResult : ActionResult
    {
        public SyndicationFeed Feed { get; set; }

        public RssActionResult(SyndicationFeed feed)
        {
            Feed = feed;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/rss+xml";

            Atom10FeedFormatter rssFormatter = new Atom10FeedFormatter(Feed);
            using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                rssFormatter.WriteTo(writer);
            }
        }
    }
}
