using BuenosAiresExp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuenosAiresExp.Services
{
    internal class DistanceService
    {

        private const double EarthRadiusKm = 6371;

        public static double CalculateDistance(Location a, Location b)
        {
            double lat1 = ToRadians(a.Latitude);
            double lat2 = ToRadians(b.Latitude);
            double deltaLat = ToRadians(b.Latitude - a.Latitude);
            double deltaLng = ToRadians(b.Longitude - a.Longitude);

            double h = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
           Math.Cos(lat1) * Math.Cos(lat2) *
           Math.Sin(deltaLng / 2) * Math.Sin(deltaLng / 2);

            double distancia = 2 * EarthRadiusKm * Math.Asin(Math.Sqrt(h));

            return Math.Round(distancia, 2);
        }

        public static string FormatDistance(Location a, Location b)
        {
            var km = CalculateDistance(a, b);

            // se for menos de 1km, mostra em metros
            if (km < 1)
                return $"{(int)(km * 1000)} m";

            return $"{km:F1} km";
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
