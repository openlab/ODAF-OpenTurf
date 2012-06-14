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
using System.Collections.ObjectModel;
using ODAF.WindowsPhone.Models;

namespace ODAF.WindowsPhone.Services
{
    /// <summary>
    /// A dummy design-time layer service
    /// </summary>
    public static class DesignTimeLayerService
    {
        /// <summary>
        /// Load dummy layers at design-time
        /// </summary>
        /// <param name="layers">The collection to be populated</param>
        public static void LoadLayers(Collection<LayerModel> layers)
        {
            LayerModel model1 = new LayerModel
            {
                Id = "2C8B8CF2-90DB-45f0-B8B4-A3D96DBEAFC3",
                Title = "Bornes pour véhicules électriques",
                Summary = "Cet ensemble de données contient les coordonnées des bornes de rechargement pour véhicules électriques dans Paris",
                Link = "http://ogdifrancedataservice.cloudapp.net/v1/frOpenData/ParisElectricite/?$filter=info eq'BVE' &amp;format=kml",
                ImageLink = "http://odafrance.cloudapp.net/images/Prise28.png"
            };

            LayerModel model2 = new LayerModel
            {
                Id = "0C244261-26E3-49f9-8EE5-140145E90B6C",
                Title = "Toilettes publiques",
                Summary = "Cet ensemble de données contient les coordonnées des sanisettes à Paris",
                Link = "http://ogdifrancedataservice.cloudapp.net/v1/frOpenData/Sanisettes/?$filter=&amp;format=kml",
                ImageLink = "http://odafrance.cloudapp.net/images/Toilettes28.png"
            };

            LayerModel model3 = new LayerModel
            {
                Id = "F29526CF-9A53-44c8-B71A-2D4EAA739F2F",
                Title = "Etablissements scolaires",
                Summary = "Cet ensemble de données contient les coordonnées des établissements scolaires à Paris",
                Link = "http://ogdifrancedataservice.cloudapp.net/v1/frOpenData/ParisEtablissements/?$filter=&amp;format=kml",
                ImageLink = "http://odafrance.cloudapp.net/images/Education28.png"
            };

            layers.Add(model1);
            layers.Add(model2);
            layers.Add(model3);
        }
    }
}
