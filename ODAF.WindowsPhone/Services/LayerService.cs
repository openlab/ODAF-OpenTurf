using System;
using System.Collections.ObjectModel;
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
using System.Xml.Linq;
using ODAF.WindowsPhone.Models;
using System.ComponentModel;

namespace ODAF.WindowsPhone.Services
{
    /// <summary>
    /// This helper class proposes a method to download the layers of the ODAF website.
    /// </summary>
    public static class LayerService
    {
        /// <summary>
        /// Helper method to asynchronously load layers from the supplied URI and populate the layers collection supplied as a parameter
        /// </summary>
        /// <param name="layersUri">URI to the ODAF PointSources.xml file</param>
        /// <param name="layers">The LayerModel collection to be populated</param>
        public static void LoadLayersAsync(Uri layersUri, Collection<LayerModel> layers)
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(LoadLayersAsync_DownloadStringCompleted);
            client.DownloadStringAsync(layersUri, layers);
        }

        private static void LoadLayersAsync_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            WebClient client = sender as WebClient;
            Collection<LayerModel> layers = e.UserState as Collection<LayerModel>;

            if (layers == null)
            {
                throw new ArgumentNullException("layers", "The layers parameter provided to the LoadLayersAsync method is null.");
            }
            if (e.Error != null)
            {
                throw e.Error;
            }

            XElement root = XElement.Parse(e.Result);
            XNamespace atom = "http://www.w3.org/2005/Atom";

            var entries = from entry in root.Descendants(atom + "entry")
                          select new
                          {
                              Id = entry.Element(atom + "id").Value,
                              Title = entry.Element(atom + "title").Value,
                              Summary = entry.Element(atom + "summary").Value,
                              Link = entry.Elements(atom + "link").First(elm => elm.Attribute("type").Value == "application/vnd.google-earth.kml+xml").Attribute("href").Value,
                              ImageLink = entry.Elements(atom + "link").First(elm => elm.Attribute("type").Value == "image/png").Attribute("href").Value
                          };

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (send, evt) =>
            {
                foreach (var entry in entries)
                {
                    LayerModel model = new LayerModel
                    {
                        Id = entry.Id,
                        Title = entry.Title,
                        Summary = entry.Summary,
                        Link = entry.Link,
                        ImageLink = entry.ImageLink.StartsWith("http") ? entry.ImageLink : App.OdafWebsiteUrl + "/" + entry.ImageLink
                    };
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        layers.Add(model);
                    });
                }
            };
            worker.RunWorkerAsync();
        }
    }
}
