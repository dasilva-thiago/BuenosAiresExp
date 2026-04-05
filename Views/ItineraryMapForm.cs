using BuenosAiresExp.Models;
using BuenosAiresExp.Services;
using BuenosAiresExp.UI;
using Microsoft.Web.WebView2.WinForms;

namespace BuenosAiresExp.Views
{
    public class ItineraryMapForm : Form
    {
        private readonly List<Location> _locations;
        private WebView2 _webView;
        private Panel _pnlHeader;
        private Label _lblTitle;

        public ItineraryMapForm(List<Location> locations, string itineraryName)
        {
            _locations = locations?.ToList() ?? new List<Location>();

            Text = $"Mapa do Roteiro — {itineraryName}";
            Size = new Size(1000, 700);
            MinimumSize = new Size(700, 500);
            StartPosition = FormStartPosition.CenterParent;
            BuenosAiresTheme.ApplyForm(this);

            BuildLayout();
        }

        private void BuildLayout()
        {
            _pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = BuenosAiresTheme.PrimaryColor,
                Padding = new Padding(16, 0, 16, 0)
            };

            _lblTitle = new Label
            {
                Text = $"📍 {_locations.Count} paradas  •  Distância total: {CalculateTotalDistance()}",
                Font = BuenosAiresTheme.ButtonFont,
                ForeColor = Color.White,
                Dock = DockStyle.Left,
                AutoSize = false,
                Width = 500,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _pnlHeader.Controls.Add(_lblTitle);

            _webView = new WebView2 { Dock = DockStyle.Fill };
            _webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;

            Controls.Add(_webView);
            Controls.Add(_pnlHeader);

            // inicializa WebView2 de forma assíncrona
            _ = _webView.EnsureCoreWebView2Async(null);
        }

        private void WebView_CoreWebView2InitializationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                LoadMap();
            }
        }

        private void LoadMap()
        {
            var html = MapService.GenerateItineraryMapHtml(_locations);

            // WebView2 precisa de uma pasta base para carregar recursos locais
            _webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "localapp", AppDomain.CurrentDomain.BaseDirectory,
                Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);

            _webView.NavigateToString(html);
        }

        private string CalculateTotalDistance()
        {
            if (_locations.Count < 2) return "—";

            double total = 0;
            for (int i = 0; i < _locations.Count - 1; i++)
                total += DistanceService.CalculateDistance(_locations[i], _locations[i + 1]);

            return total < 1 ? $"{(int)(total * 1000)} m" : $"{total:F1} km";
        }
    }
}