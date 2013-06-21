using System;
using System.Web.Mvc;
using System.Web.Routing;
using ODAF.Data;
using System.Linq;
using System.Collections.Generic;
using System.Transactions;
using SubSonic.DataProviders;
using SubSonic.Schema;
using System.ServiceModel.Syndication;
using website_mvc.Code;
using System.Web.Script.Serialization;

namespace vancouveropendata.Controllers
{
    public class FeedsController : BaseController
    {


        public ActionResult Index(long? id, string format)
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [JsonpFilter]
        public ActionResult List()
        {
            try
            {

                var sources = from s in PointDataSource.Find(p => p.Active) orderby s.Title select new 
                { 
                    Title = s.Title, 
                    Description = s.Description, 
                    Id = s.UniqueId, 
                    BoundaryPolygon = s.BoundaryPolygon
                };
                return Json(sources, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 400;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// This method is the same as the regular list with the exception of how it returns the boundary
        ///  Unlike the list method, this will return proper json and not json containing a string containing json.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        [JsonpFilter]
        public ActionResult ProperList()
        {
            try
            {
                var jss = new JavaScriptSerializer();

                var sources = from s in PointDataSource.Find(p => p.Active)
                              orderby s.Title
                              select new
                              {
                                  Title = s.Title,
                                  Description = s.Description,
                                  Id = s.UniqueId,
                                  BoundaryPolygon = jss.DeserializeObject(s.BoundaryPolygon)
                              };
                return Json(sources, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 400;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [JsonpFilter]
        public ActionResult Get(string id, string type = "point", string format = "xml")
        {
            try
            {
                type = type.ToLower();
                PointDataSource source = PointDataSource.SingleOrDefault(p => p.UniqueId == id && p.Active);

                if (source == null)
                {
                    throw new Exception("Feed does not exist.");
                }

                var feeds = source.PointDataFeeds.Where(p => p.IsRegion == (type == "region") && p.Active);

                if (format == "json")
                {
                    var result = new JsonResult
                    {
                        Data = feeds,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                    return result;
                }

                List<SyndicationItem> items = new List<SyndicationItem>();
                foreach (PointDataFeed feed in feeds)
                {
                    SyndicationItem item = new SyndicationItem();
                    item.Id = feed.UniqueId;
                    item.LastUpdatedTime = feed.UpdatedOn;
                    item.Summary = new TextSyndicationContent(feed.Summary);
                    item.Title = new TextSyndicationContent(feed.Title);
                    if (feed.KMLFeedUrl != "")
                    {
                        if (Uri.IsWellFormedUriString(feed.KMLFeedUrl, UriKind.Absolute))
                            item.Links.Add(new SyndicationLink(new Uri(feed.KMLFeedUrl), "enclosure", "", "application/vnd.google-earth.kml+xml", 0));
                        else
                        {
                            string scheme = Request.Url.Scheme;
                            Uri hostAndPort = new Uri(scheme + Uri.SchemeDelimiter + Request.ServerVariables["HTTP_HOST"]);
                            UriBuilder uriBuilder = new UriBuilder(Request.Url);
                            uriBuilder.Scheme = scheme;
                            uriBuilder.Host = hostAndPort.Host;
                            uriBuilder.Port = hostAndPort.Port;
                            uriBuilder.Path = System.Web.VirtualPathUtility.ToAbsolute(feed.KMLFeedUrl);
                            item.Links.Add(new SyndicationLink(uriBuilder.Uri, "enclosure", "", "application/vnd.google-earth.kml+xml", 0));
                        }
                    }
                    if (feed.ImageUrl != "")
                    {
                        if (Uri.IsWellFormedUriString(feed.ImageUrl, UriKind.Absolute))
                            item.Links.Add(new SyndicationLink(new Uri(feed.ImageUrl), "enclosure", "", "image/png", 0));
                        else
                        {
                            string scheme = Request.Url.Scheme;
                            Uri hostAndPort = new Uri(scheme + Uri.SchemeDelimiter + Request.ServerVariables["HTTP_HOST"]);
                            UriBuilder uriBuilder = new UriBuilder(Request.Url);
                            uriBuilder.Scheme = scheme;
                            uriBuilder.Host = hostAndPort.Host;
                            uriBuilder.Port = hostAndPort.Port;
                            uriBuilder.Path = System.Web.VirtualPathUtility.ToAbsolute(feed.ImageUrl);
                            item.Links.Add(new SyndicationLink(uriBuilder.Uri, "enclosure", "", "image/png", 0));
                        }
                    }
                    items.Add(item);
                }

                

                SyndicationFeed syndicationFeed = new SyndicationFeed(items);
                syndicationFeed.Authors.Add(new SyndicationPerson(source.AuthorEmail, source.AuthorName, ""));
                syndicationFeed.Id = source.UniqueId;
                syndicationFeed.Title = new TextSyndicationContent(source.Title);

                return new RssActionResult(syndicationFeed);
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 400;
                JsonResult result = new JsonResult();
                result.Data = ex.Message;
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult MobileFeeds()
        {
            try
            {
                var type = "point";
                var format = "xml";
                IEnumerable<PointDataSource> sources = PointDataSource.Find(x => x.Active == true);

                if (sources == null)
                {
                    throw new Exception("Feed does not exist.");
                }

                
                var feeds = new List<PointDataFeed>();

                foreach(var source in sources)
                    feeds.AddRange(source.PointDataFeeds.Where(p => p.IsRegion == (type == "region") && p.Active));

                if (format == "json")
                {
                    var result = new JsonResult
                    {
                        Data = feeds,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                    return result;
                }

                List<SyndicationItem> items = new List<SyndicationItem>();
                foreach (PointDataFeed feed in feeds)
                {
                    SyndicationItem item = new SyndicationItem();
                    item.Id = feed.UniqueId;
                    item.LastUpdatedTime = feed.UpdatedOn;
                    item.Summary = new TextSyndicationContent(feed.Summary);
                    item.Title = new TextSyndicationContent(sources.FirstOrDefault(x => x.PointDataSourceId == feed.PointDataSourceId).Title + " - " + feed.Title);
                    if (feed.KMLFeedUrl != "")
                    {
                        if (Uri.IsWellFormedUriString(feed.KMLFeedUrl, UriKind.Absolute))
                            item.Links.Add(new SyndicationLink(new Uri(feed.KMLFeedUrl), "enclosure", "", "application/vnd.google-earth.kml+xml", 0));
                        else
                        {
                            string scheme = Request.Url.Scheme;
                            Uri hostAndPort = new Uri(scheme + Uri.SchemeDelimiter + Request.ServerVariables["HTTP_HOST"]);
                            UriBuilder uriBuilder = new UriBuilder(Request.Url);
                            uriBuilder.Scheme = scheme;
                            uriBuilder.Host = hostAndPort.Host;
                            uriBuilder.Port = hostAndPort.Port;
                            uriBuilder.Path = System.Web.VirtualPathUtility.ToAbsolute(feed.KMLFeedUrl);
                            item.Links.Add(new SyndicationLink(uriBuilder.Uri, "enclosure", "", "application/vnd.google-earth.kml+xml", 0));
                        }
                    }
                    if (feed.ImageUrl != "")
                    {
                        if (Uri.IsWellFormedUriString(feed.ImageUrl, UriKind.Absolute))
                            item.Links.Add(new SyndicationLink(new Uri(feed.ImageUrl), "enclosure", "", "image/png", 0));
                        else
                        {
                            string scheme = Request.Url.Scheme;
                            Uri hostAndPort = new Uri(scheme + Uri.SchemeDelimiter + Request.ServerVariables["HTTP_HOST"]);
                            UriBuilder uriBuilder = new UriBuilder(Request.Url);
                            uriBuilder.Scheme = scheme;
                            uriBuilder.Host = hostAndPort.Host;
                            uriBuilder.Port = hostAndPort.Port;
                            uriBuilder.Path = System.Web.VirtualPathUtility.ToAbsolute(feed.ImageUrl);
                            item.Links.Add(new SyndicationLink(uriBuilder.Uri, "enclosure", "", "image/png", 0));
                        }
                    }
                    items.Add(item);
                }


                var sourcedesc = sources.FirstOrDefault();
                SyndicationFeed syndicationFeed = new SyndicationFeed(items);
                syndicationFeed.Authors.Add(new SyndicationPerson(sourcedesc.AuthorEmail, sourcedesc.AuthorName, ""));
                syndicationFeed.Id = sourcedesc.UniqueId;
                syndicationFeed.Title = new TextSyndicationContent(sourcedesc.Title);

                return new RssActionResult(syndicationFeed);
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 400;
                JsonResult result = new JsonResult();
                result.Data = ex.Message;
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }


        [AcceptVerbs(HttpVerbs.Get)]
        [JsonpFilter]
        public ActionResult RegionsMobile(string format = "xml")
        {
            try
            {
                var type = "region";
                var sources = PointDataSource.Find(p => p.Active);

                if (sources == null)
                {
                    throw new Exception("There is no source.");
                }

                List<SyndicationItem> items = new List<SyndicationItem>();
                foreach (var source in sources)
                {

                    var feeds = source.PointDataFeeds.Where(p => p.IsRegion == (type == "region") && p.Active);

                    if (format == "json")
                    {
                        var result = new JsonResult
                        {
                            Data = feeds,
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                        return result;
                    }

                    foreach (PointDataFeed feed in feeds)
                    {
                        SyndicationItem item = new SyndicationItem();
                        item.Id = feed.UniqueId;
                        item.LastUpdatedTime = feed.UpdatedOn;
                        item.Summary = new TextSyndicationContent(feed.Summary);
                        item.Title = new TextSyndicationContent(sources.FirstOrDefault(x => x.PointDataSourceId == feed.PointDataSourceId).Title + " - " + feed.Title);
                        if (feed.KMLFeedUrl != "")
                        {
                            if (Uri.IsWellFormedUriString(feed.KMLFeedUrl, UriKind.Absolute))
                                item.Links.Add(new SyndicationLink(new Uri(feed.KMLFeedUrl), "enclosure", "", "application/vnd.google-earth.kml+xml", 0));
                            else
                            {
                                string scheme = Request.Url.Scheme;
                                Uri hostAndPort = new Uri(scheme + Uri.SchemeDelimiter + Request.ServerVariables["HTTP_HOST"]);
                                UriBuilder uriBuilder = new UriBuilder(Request.Url);
                                uriBuilder.Scheme = scheme;
                                uriBuilder.Host = hostAndPort.Host;
                                uriBuilder.Port = hostAndPort.Port;
                                uriBuilder.Path = System.Web.VirtualPathUtility.ToAbsolute(feed.KMLFeedUrl);
                                item.Links.Add(new SyndicationLink(uriBuilder.Uri, "enclosure", "", "application/vnd.google-earth.kml+xml", 0));
                            }
                        }
                        if (feed.ImageUrl != "")
                        {
                            if (Uri.IsWellFormedUriString(feed.ImageUrl, UriKind.Absolute))
                                item.Links.Add(new SyndicationLink(new Uri(feed.ImageUrl), "enclosure", "", "image/png", 0));
                            else
                            {
                                string scheme = Request.Url.Scheme;
                                Uri hostAndPort = new Uri(scheme + Uri.SchemeDelimiter + Request.ServerVariables["HTTP_HOST"]);
                                UriBuilder uriBuilder = new UriBuilder(Request.Url);
                                uriBuilder.Scheme = scheme;
                                uriBuilder.Host = hostAndPort.Host;
                                uriBuilder.Port = hostAndPort.Port;
                                uriBuilder.Path = System.Web.VirtualPathUtility.ToAbsolute(feed.ImageUrl);
                                item.Links.Add(new SyndicationLink(uriBuilder.Uri, "enclosure", "", "image/png", 0));
                            }
                        }
                        items.Add(item);
                    }

                }

                var sourcedesc = sources.FirstOrDefault();
                SyndicationFeed syndicationFeed = new SyndicationFeed(items);
                syndicationFeed.Authors.Add(new SyndicationPerson(sourcedesc.AuthorEmail, sourcedesc.AuthorName, ""));
                syndicationFeed.Id = sourcedesc.UniqueId;
                syndicationFeed.Title = new TextSyndicationContent(sourcedesc.Title);

                return new RssActionResult(syndicationFeed);
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 400;
                JsonResult result = new JsonResult();
                result.Data = ex.Message;
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
    }
}