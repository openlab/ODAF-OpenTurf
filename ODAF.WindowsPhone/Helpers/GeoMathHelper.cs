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

namespace ODAF.WindowsPhone.Helpers
{
    public struct CalculateAmplitudeResult
    {
        public double MaxLatitude { get; set; }
        public double MinLatitude { get; set; }
        public double MaxLongitude { get; set; }
        public double MinLongitude { get; set; }
    }

    public static class GeoMathHelper
    {
        /// <summary>
        /// Méthode permettant de calculer un périmètre à partir d'un point de référence donné par sa latitude et sa longitude.
        /// </summary>
        /// <param name="latitude">latitude du point de référence</param>
        /// <param name="longitude">longitude du point de référence</param>
        /// <param name="distance">rayon d'action (en mètres)</param>
        /// <returns></returns>
        public static CalculateAmplitudeResult CalculateRadius(double latitude, double longitude, int rayon)
        {
            //1° de latitude = 111,11 Km, on fait donc un produit en croix (40 000/360 = 111,11)
            double offSetLat = rayon / 111110d;

            //1° de longitude à 'latitude' degrés de latitude correspond à OneLongitudeDegree mètres. (1° de longitude = 111,11 Km * cos (latitude))
            //On passe àa la méthode Math.Cos des radians
            double OneLongitudeDegree = 111110 * Math.Cos(latitude * Math.PI / 180);

            //produit en croix pour trouver le nombre de degrés auquel correspond le balyage de notre rayon sur ce cercle de longitude
            double offSetLong = rayon / OneLongitudeDegree;

            CalculateAmplitudeResult result = new CalculateAmplitudeResult
            {
                MaxLatitude = latitude + offSetLat,
                MinLatitude = latitude - offSetLat,
                MaxLongitude = longitude + offSetLong,
                MinLongitude = longitude - offSetLong
            };
            return result;
        }

        /// <summary>
        /// Méthode permettant de savoir si un point se trouve dans un périmètre donné.
        /// </summary>
        /// <param name="latitude">La latitude du point.</param>
        /// <param name="longitude">La longitude du point.</param>
        /// <param name="perimeter">Le périmètre.</param>
        /// <returns></returns>
        public static bool IsPointInRadius(double latitude, double longitude, CalculateAmplitudeResult perimeter)
        {
            if (latitude >= perimeter.MinLatitude && latitude <= perimeter.MaxLatitude
                && longitude >= perimeter.MinLongitude && longitude <= perimeter.MaxLongitude)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
