using BuenosAiresExp.Models;
using System.Globalization;
using System.Text;

namespace BuenosAiresExp.Services
{
    public static class MapService
    {
        public static string GenerateItineraryMapHtml(List<Location> locations)
        {
            if (locations == null || locations.Count == 0)
                return GenerateEmptyMapHtml();

            var center = locations[0];

            var markersJs = new StringBuilder();
            for (int i = 0; i < locations.Count; i++)
            {
                var loc = locations[i];
                var lat = loc.Latitude.ToString(CultureInfo.InvariantCulture);
                var lng = loc.Longitude.ToString(CultureInfo.InvariantCulture);
                var name = loc.Name.Replace("'", "\\'");
                var address = loc.Address.Replace("'", "\\'");
                var category = loc.Category.Replace("'", "\\'");

                markersJs.AppendLine($@"
                    L.marker([{lat}, {lng}], {{ icon: createNumberedIcon({i + 1}) }})
                        .addTo(map)
                        .bindPopup('<b>{name}</b><br/><i>{category}</i><br/>{address}');");
            }

            var polylineCoords = string.Join(", ",
                locations.Select(l =>
                    $"[{l.Latitude.ToString(CultureInfo.InvariantCulture)}, {l.Longitude.ToString(CultureInfo.InvariantCulture)}]"));

            var distanceLabelsJs = new StringBuilder();
            for (int i = 0; i < locations.Count - 1; i++)
            {
                var a = locations[i];
                var b = locations[i + 1];
                var midLat = ((a.Latitude + b.Latitude) / 2).ToString(CultureInfo.InvariantCulture);
                var midLng = ((a.Longitude + b.Longitude) / 2).ToString(CultureInfo.InvariantCulture);
                var dist = DistanceService.FormatDistance(a, b);

                distanceLabelsJs.AppendLine($@"
                    L.marker([{midLat}, {midLng}], {{
                        icon: L.divIcon({{
                            className: 'distance-label',
                            html: '<div class=""dist-badge"">{dist}</div>',
                            iconAnchor: [30, 10]
                        }})
                    }}).addTo(map);");
            }

            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' />
    <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        html, body, #map {{ width: 100%; height: 100%; }}

        .numbered-icon {{
            background-color: #1b4f8a;
            color: white;
            border-radius: 50%;
            width: 28px;
            height: 28px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            font-size: 13px;
            border: 2px solid white;
            box-shadow: 0 2px 6px rgba(0,0,0,0.4);
        }}

        .dist-badge {{
            background: #c8a96e;
            color: #3d2200;
            font-size: 11px;
            font-weight: bold;
            padding: 2px 8px;
            border-radius: 10px;
            white-space: nowrap;
            box-shadow: 0 1px 4px rgba(0,0,0,0.2);
        }}
    </style>
</head>
<body>
    <div id='map'></div>
    <script>
        var map = L.map('map').setView(
            [{center.Latitude.ToString(CultureInfo.InvariantCulture)}, {center.Longitude.ToString(CultureInfo.InvariantCulture)}], 14
        );

        // Tile Layer (CartoDB)
        L.tileLayer('https://{{s}}.basemaps.cartocdn.com/light_all/{{z}}/{{x}}/{{y}}{{r}}.png', {{
            attribution:'© OpenStreetMap contributors © CARTO',
            subdomains: 'abcd',
            maxZoom: 20
        }}).addTo(map);

        function createNumberedIcon(number) {{
            return L.divIcon({{
                className: '',
                html: '<div class=""numbered-icon"">' + number + '</div>',
                iconSize: [28, 28],
                iconAnchor: [14, 14],
                popupAnchor: [0, -16]
            }});
        }}

        // Coordenadas da rota
        var routeCoords = [{polylineCoords}];

        if (routeCoords.length > 1) {{

            // Linha principal
            var routeLine = L.polyline(routeCoords, {{
                color: '#1b4f8a',
                weight: 4,
                opacity: 0.9
            }}).addTo(map);

            // Animação da linha (efeito fluxo)
            var offset = 0;
            setInterval(function() {{
                offset = (offset + 1) % 20;
                routeLine.setStyle({{
                    dashArray: '10, 10',
                    dashOffset: offset
                }});
            }}, 100);

            // Marcador início
            L.circleMarker(routeCoords[0], {{
                radius: 8,
                color: '#2ecc71',
                fillColor: '#2ecc71',
                fillOpacity: 1
            }}).addTo(map).bindPopup('Início');

            // Marcador fim
            L.circleMarker(routeCoords[routeCoords.length - 1], {{
                radius: 8,
                color: '#e74c3c',
                fillColor: '#e74c3c',
                fillOpacity: 1
            }}).addTo(map).bindPopup('Fim');

            // Ajuste de zoom com padding melhorado
            var bounds = L.latLngBounds(routeCoords);
            map.fitBounds(bounds.pad(0.2));
        }}

        // Marcadores numerados
        {markersJs}

        // Labels de distância
        {distanceLabelsJs}
    </script>
</body>
</html>";
        }

        private static string GenerateEmptyMapHtml() => $@"<!DOCTYPE html>
<html><head>
    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css'/>
    <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
    <style>html,body,#map{{width:100%;height:100%;margin:0}}</style>
</head><body>
    <div id='map'></div>
    <script>
        var map = L.map('map').setView([-34.6037, -58.3816], 13);

        L.tileLayer('https://{{s}}.basemaps.cartocdn.com/light_all/{{z}}/{{x}}/{{y}}{{r}}.png', {{
            subdomains: 'abcd',
            maxZoom: 20
        }}).addTo(map);
    </script>
</body></html>";

        public static string GenerateSingleLocationMapHtml(Location location)
        {
            var lat = location.Latitude.ToString(CultureInfo.InvariantCulture);
            var lng = location.Longitude.ToString(CultureInfo.InvariantCulture);
            var name = location.Name.Replace("'", "\\'");

            return $@"<!DOCTYPE html>
<html><head>
    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css'/>
    <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
    <style>html,body,#map{{width:100%;height:100%;margin:0}}</style>
</head><body>
    <div id='map'></div>
    <script>
        var map = L.map('map').setView([{lat},{lng}], 17);

        L.tileLayer('https://{{s}}.basemaps.cartocdn.com/light_all/{{z}}/{{x}}/{{y}}{{r}}.png', {{
            attribution: '© OpenStreetMap contributors © CARTO',
            subdomains: 'abcd',
            maxZoom: 20
        }}).addTo(map);

        L.marker([{lat},{lng}]).addTo(map)
            .bindPopup('<b>{name}</b>')
            .openPopup();
    </script>
</body></html>";
        }
    }
}