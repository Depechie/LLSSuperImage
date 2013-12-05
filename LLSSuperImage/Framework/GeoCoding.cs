using System;
using System.Device.Location;
using System.Threading.Tasks;

namespace LLSSuperImage.Framework
{
    public enum DistanceMeasure
    {
        Miles,
        Kilometers
    }

    public class Geocoding
    {
        private const Double EarthRadiusInMiles = 3956.0;
        private const Double EarthRadiusInKm = 6366.707019;

        public GeoCoordinate CurrentPosition { get; set; }

        private static Geocoding s_Instance;
        private static object s_InstanceSync = new object();

        protected Geocoding()
        {
        }

        /// <summary>
        /// Returns an instance (a singleton)
        /// </summary>
        /// <returns>a singleton</returns>
        /// <remarks>
        /// This is an implementation of the singelton design pattern.
        /// </remarks>
        public static Geocoding GetInstance()
        {
            // This implementation of the singleton design pattern prevents 
            // unnecessary locks (using the double if-test)
            if (s_Instance == null)
            {
                lock (s_InstanceSync)
                {
                    if (s_Instance == null)
                    {
                        s_Instance = new Geocoding();
                    }
                }
            }
            return s_Instance;
        }

        //helper method to make reading the lambda a bit easier
        private double ToRadian(double val)
        {
            return val * (Math.PI / 180);
        }

        private double ToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        //helper method for converting Radians, making the lamda easier to read
        private double DiffRadian(double val1, double val2)
        {
            return ToRadian(val2) - ToRadian(val1);
        }

        public double CalculateDistanceFrom(GeoCoordinate from, GeoCoordinate to)
        {
            return CalculateDistanceFrom(from.Latitude, from.Longitude, to.Latitude, to.Longitude);
        }

        public double CalculateDistanceFrom(double fromLat, double fromLon, double toLat, double toLong)
        {
            Func<double, double, double, double, double> CalcDistance = (lat1, lon1, lat2, lon2) =>
            EarthRadiusInKm * 2 *
            (
                Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lon1, lon2)) / 2.0), 2.0)))))
            );

            return CalcDistance(fromLat, fromLon, toLat, toLong);
        }
    }
}
