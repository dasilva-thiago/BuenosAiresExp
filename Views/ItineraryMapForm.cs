using BuenosAiresExp.Models;
using BuenosAiresExp.Services;
using BuenosAiresExp.UI;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;

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
            _webView.CoreWebView2InitializationCompleted += OnWebViewInitializationCompleted;

            Controls.Add(_webView);
            Controls.Add(_pnlHeader);

            _ = InitWebViewAsync();
        }

        private async Task InitWebViewAsync()
        {
            try
            {
                var runtimePath = FindWebView2RuntimePath();
                var env = await CoreWebView2Environment.CreateAsync(
                    browserExecutableFolder: runtimePath,
                    userDataFolder: Path.Combine(Path.GetTempPath(), "BuenosAiresExp"),
                    options: null);

                await _webView.EnsureCoreWebView2Async(env);
            }
            catch
            {
                await _webView.EnsureCoreWebView2Async(null);
            }
        }

        private static string? FindWebView2RuntimePath()
        {
            var baseDir = Path.GetDirectoryName(Environment.ProcessPath)
                          ?? AppContext.BaseDirectory;

            var candidates = new[] { "WebView2Runtime", "Microsoft.WebView2.FixedVersionRuntime" };

            foreach (var name in candidates)
            {
                var exact = Path.Combine(baseDir, name);
                if (Directory.Exists(exact))
                    return exact;

                var wildcard = Directory.GetDirectories(baseDir, name + "*").FirstOrDefault();
                if (wildcard != null)
                    return wildcard;
            }

            return null;
        }

        private void OnWebViewInitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
                LoadMap();
        }

        private void LoadMap()
        {
            var html = MapService.GenerateItineraryMapHtml(_locations);
            _webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "localapp", AppDomain.CurrentDomain.BaseDirectory,
                CoreWebView2HostResourceAccessKind.Allow);
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