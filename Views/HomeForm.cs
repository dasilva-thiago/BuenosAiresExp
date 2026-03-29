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
        private Panel _pnlCards;
        private RoundedPanel _cardLocais;
        private RoundedPanel _cardRoteiros;

        private TabLabel _tabInicio;
        private TabLabel _tabLocais;
        private TabLabel _tabRoteiros;

        private RoundedButton _btnCardLocais;
        private RoundedButton _btnCardRoteiros;

        private TableLayoutPanel _headerLayout;
        private TableLayoutPanel _cardsLayout;
        private TableLayoutPanel _contentTextLayout;

        private FlowLayoutPanel _flowTabs;

        private Label _lblBoasVindas;
        private Label _lblDescricao;
        private Label _lblTitle;
        private Label _lblSubtitle;
        private Label _lblLogo;
        private Label _lblCardLocais;
        private Label _lblCardRoteiros;

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

            // inicio do header com logo, titulo, subtitulo
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

            // painel de abas
            _pnlTabs = new Panel
            {
                Dock = DockStyle.Top,
                Height  = 24,
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

            // instruções de uso do software e boas vindas

            _pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BuenosAiresTheme.FillColor,
                AutoScroll = false,
                Padding = new Padding(48, 32, 48, 32)
            };
            Controls.Add(_pnlContent);

            _lblBoasVindas = new Label
            {
                Text = "Bem-vindo ao Buenos Aires Explorer!",
                Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 18f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.TextColor,
                AutoSize = true,
                Dock = DockStyle.None,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 0, 0, 16)
            };

            _lblDescricao = new Label
            {
                Text = "Planeje e organize suas viagens a Buenos Aires com precisão.\nMonte roteiros otimizados baseados na proximidade geográfica entre os locais.",
                Font = BuenosAiresTheme.BodyFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                AutoSize = true,
                Dock = DockStyle.None,
                MaximumSize = new Size(900, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0)
            };

            _contentTextLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.Transparent
            };
            _contentTextLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            _contentTextLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 12));
            _contentTextLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _contentTextLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _contentTextLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 65));

            _lblBoasVindas.Anchor = AnchorStyles.None;
            _lblDescricao.Anchor = AnchorStyles.None;

            _contentTextLayout.Controls.Add(_lblBoasVindas, 0, 1);
            _contentTextLayout.Controls.Add(_lblDescricao, 0, 2);

            _pnlContent.Controls.Add(_contentTextLayout);

            // botoes que levam a locationform e form do roteiro(sem nome)
            _pnlCards = new Panel
            {
                Dock = DockStyle.Top,
                Height = 200,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 24, 0, 0)
            };

            _cardsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            _cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            _cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            _cardLocais = new RoundedPanel
            {
                Dock = DockStyle.Fill,
                FillColor = BuenosAiresTheme.PrimaryColorLight,
                BorderColor = BuenosAiresTheme.PrimaryColorHighlight,
                Margin = new Padding(0, 0, 12, 0),
                Padding = new Padding(20)
            };

            _lblCardLocais = new Label
            {
                Text = "Locais Cadastrados",
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = Color.White,
                AutoSize = true,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _btnCardLocais = new RoundedButton
            {
                Text = "Adicionar Novo Local",
                Font = BuenosAiresTheme.ButtonFont,
                ForeColor = Color.White,
                BackColor = BuenosAiresTheme.PrimaryColor,
                FlatStyle = FlatStyle.Flat,
                Width = 120,
                Height = 40,
                Dock = DockStyle.Bottom,
                Margin = new Padding(0, 0, 0, 0)
            };

           
            _cardLocais.Controls.Add(_btnCardLocais);  
            _cardLocais.Controls.Add(_lblCardLocais);  

            _cardsLayout.Controls.Add(_cardLocais, 0, 0);
            _pnlCards.Controls.Add(_cardsLayout);

            _pnlContent.Controls.Add(_contentTextLayout);
            _pnlContent.Controls.Add(_pnlCards);

        }

        // ontabclicked para controlar o estado ativo das abas
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
