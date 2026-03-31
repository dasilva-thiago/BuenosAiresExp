using BuenosAiresExp.Models;
using BuenosAiresExp.Services;
using BuenosAiresExp.UI;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BuenosAiresExp.Views
{
    public class ItineraryMapForm : Form
    {
        private readonly List<Location> _locations;
        private WebView2 _webView;

        public ItineraryMapForm(List<Location> locations, string itineraryName)
        {
            _locations = locations.OrderBy(l => l.Name).ToList();

            Text = $"Mapa do Roteiro — {itineraryName}";
            Size = new Size(1000, 700);
            MinimumSize = new Size(700, 500);
            StartPosition = FormStartPosition.CenterParent;
            BuenosAiresTheme.ApplyForm(this);

            BuildLayout();
        }

        private void BuildLayout()
        {
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = BuenosAiresTheme.PrimaryColor,
                Padding = new Padding(16, 0, 16, 0)
            };

            var lblTitle = new Label
            {
                Text = $"📍 {_locations.Count} paradas  •  Distância total: {CalculateTotalDistance()}",
                Font = BuenosAiresTheme.ButtonFont,
                ForeColor = Color.White,
                Dock = DockStyle.Left,
                AutoSize = false,
                Width = 500,
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlHeader.Controls.Add(lblTitle);

            _webView = new WebView2 { Dock = DockStyle.Fill };
            _webView.CoreWebView2InitializationCompleted += (s, e) =>
            {
                if (e.IsSuccess)
                    LoadMap();
            };

            Controls.Add(_webView);
            Controls.Add(pnlHeader);

            // inicializa WebView2 de forma assíncrona
            _ = _webView.EnsureCoreWebView2Async(null);
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