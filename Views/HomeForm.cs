using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BuenosAiresExp.UI;


namespace BuenosAiresExp.Views
{
    public partial class HomeForm : Form
    {
        private Panel _pnlHeader;
        private Panel _pnlTabs;
        private Panel _pnlSeparator;
        private Panel _pnlSeparatorHeader;
        private Panel _pnlContent;

        private TabLabel _tabInicio;
        private TabLabel _tabLocais;
        private TabLabel _tabRoteiros;

        private TableLayoutPanel _headerLayout;

        private FlowLayoutPanel _flowTabs;

        private Label _lblTitle;
        private Label _lblSubtitle;
        private Label _lblLogo;

        public HomeForm()
        {
            BuildLayout();
        }

        private void BuildLayout()
        {
            BuenosAiresTheme.ApplyForm(this);
            Text = "Buenos Aires Explorer - Home";
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;

            _pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = BuenosAiresTheme.HeaderHeight +13,
                BackColor = BuenosAiresTheme.PrimaryColor,
                Padding = new Padding(24, 10, 24, 10)
            };

            _headerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(16, 0, 16,0)
            };
            _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 64));
            _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            _headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            _headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            _lblLogo = new Label
            {
                Text = "",
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };

            var logoIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "map_icon.png");
            if (File.Exists(logoIconPath))
            {
                using var iconStream = File.OpenRead(logoIconPath);
                using var originalIcon = Image.FromStream(iconStream);
                _lblLogo.Image = new Bitmap(originalIcon, new Size(40, 40));
            }
            else
            {
                _lblLogo.Text = "🗺";
            }

            _lblTitle = new Label
            {
                Text = "Buenos Aires Explorer",
                Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 20f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                TextAlign = ContentAlignment.BottomLeft,
                Margin = new Padding(0)
            };

            _lblSubtitle = new Label
            {
                Text = "Sua viagem planejada com precisão!",
                Font = new Font(BuenosAiresTheme.BodyFont.FontFamily, 12f, FontStyle.Regular),
                ForeColor = BuenosAiresTheme.PrimaryColorHighlight,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                TextAlign = ContentAlignment.TopLeft,
                Margin = new Padding(4, 2, 0, 0)
            };

            _headerLayout.Controls.Add(_lblLogo, 0, 0);
            _headerLayout.SetRowSpan(_lblLogo, 2);
            _headerLayout.Controls.Add(_lblTitle, 1, 0);
            _headerLayout.Controls.Add(_lblSubtitle, 1, 1);
            _pnlHeader.Controls.Add(_headerLayout);

            _pnlTabs = new Panel
            {
                Dock = DockStyle.Top,
                Height  = 48-22,
                BackColor = Color.White,
                Padding = new Padding(36, 0, 0, 0)
            };

            _pnlSeparator = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = BuenosAiresTheme.BorderColor
            };

            _pnlSeparatorHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = BuenosAiresTheme.BorderColor
            };

            _flowTabs = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(16,0,0,0),
                BackColor = Color.White,
            };

            //abas da home
            _tabInicio = new TabLabel { Text = "Início", Icon = "⌂", Width = 100, Height = 48, Margin = new Padding(0,0,0,0), IsActive = true };
            _tabLocais = new TabLabel { Text = "Locais", Icon = "📍", Width = 100, Height = 48, Margin = new Padding(0,0,0,0) };
            _tabRoteiros = new TabLabel { Text = "Roteiros", Icon = "🗺", Width = 100, Height = 48, Margin = new Padding(0, 0, 0, 0) };

            _tabInicio.TabClicked += OnTabClicked;
            _tabLocais.TabClicked += OnTabClicked;
            _tabRoteiros.TabClicked += OnTabClicked;

            _flowTabs.Controls.AddRange(new Control[] { _tabInicio, _tabLocais, _tabRoteiros });
            _pnlTabs.Controls.Add(_flowTabs);
            Controls.Add(_pnlSeparator);
            Controls.Add(_pnlTabs);
            Controls.Add(_pnlSeparatorHeader);
            Controls.Add(_pnlHeader);
        }
        private void OnTabClicked(object? sender, EventArgs e)
        {
            _tabInicio.IsActive = false;
            _tabLocais.IsActive = false;
            _tabRoteiros.IsActive = false;

            if (sender is TabLabel tab)
                tab.IsActive = true;
        }
    }
}
