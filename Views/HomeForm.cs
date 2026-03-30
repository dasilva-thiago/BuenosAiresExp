using BuenosAiresExp.Services;
using BuenosAiresExp.UI;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace BuenosAiresExp.Views
{
    public partial class HomeForm : Form
    {
        private Panel _pnlHeader;
        private Panel _pnlTabs;
        private Panel _pnlSeparator;
        private Panel _pnlSeparatorHeader;
        private Panel _pnlFooter;
        private Panel _pnlContent;
        private Panel _pnlCards;
        private Panel _spacerTop;
        private Panel _spacerCards;
        private Panel _bdgTxtPanel;
        private Panel _pnlFooterSeparator;
        private Panel _spacerHowToTitle;

        private RoundedPanel _pnlHowTo;
        private RoundedPanel _cardLocais;
        private RoundedPanel _cardRoteiros;
        private LocaisView _locaisView;
        private RoteirosView _roteirosView;

        private TabLabel _tabInicio;
        private TabLabel _tabLocais;
        private TabLabel _tabRoteiros;

        private RoundedButton _btnCardLocais;
        private RoundedButton _btnCardRoteiros;

        private TableLayoutPanel _headerLayout;
        private TableLayoutPanel _cardsLayout;
        private TableLayoutPanel _contentTextLayout;
        private TableLayoutPanel _bdgRow;
        private TableLayoutPanel _howToHeaderLayout;

        private StepBadge _bdg;
        private bool _locaisViewInitialized;


        private FlowLayoutPanel _flowTabs;

        private Label _infoIcon;
        private Label _lblHowTo;
        private Label _lblBdgTitle;
        private Label _lblBdgDesc;
        private Label _lblBoasVindas;
        private Label _lblDescricao;
        private Label _lblFooter;
        private Label _lblTitle;
        private Label _lblSubtitle;
        private Label _lblLogo;
        private Label _lblCardLocais;
        private Label _lblCardLocaisDesc;
        private Label _lblCardRoteiros;
        private Label _lblCardRoteirosDesc;
        private Label _lblCountLocais;
        private Label _lblCountRoteiros;

        public HomeForm()
        {
            BuildLayout();
        }

        private void BuildLayout()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            DoubleBuffered = true;

            BuenosAiresTheme.ApplyForm(this);
            Text = "Buenos Aires Explorer - Home";
            Size = new Size(1300, 940);
            MinimumSize = new Size(1000, 700);
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

            object LoadTabIcon(string fileName, string fallback)
            {
                var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", fileName);
                if (File.Exists(iconPath))
                {
                    using var iconStream = File.OpenRead(iconPath);
                    using var originalIcon = Image.FromStream(iconStream);
                    return new Bitmap(originalIcon, new Size(16, 16));
                }

                return fallback;
            }

            //abas da home
            _tabInicio = new TabLabel { Text = "Início", Icon = LoadTabIcon("home_icon.png", "⌂"), Width = 100, Height = 48, Margin = new Padding(0,0,0,0), IsActive = true };
            _tabLocais = new TabLabel { Text = "Locais", Icon = LoadTabIcon("tablocation_icon.png", "📍"), Width = 100, Height = 48, Margin = new Padding(0,0,0,0) };
            _tabRoteiros = new TabLabel { Text = "Roteiros", Icon = LoadTabIcon("roteiro_icon.png", "🗺"), Width = 100, Height = 48, Margin = new Padding(0, 0, 0, 0) };

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

            _locaisView = new LocaisView
            {
                Dock = DockStyle.Fill,
                Visible = false
            };
            Controls.Add(_locaisView);

            _roteirosView = new RoteirosView
            {
                Dock = DockStyle.Fill,
                Visible = false
            };
            Controls.Add(_roteirosView);

            _spacerTop = new Panel { Dock = DockStyle.Top, Height = 20, BackColor = Color.Transparent };

            _lblBoasVindas = new Label
            {
                Text = "Bem-vindo ao Buenos Aires Explorer!",
                Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 18f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.TextColor,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.BottomCenter,
                Height = 120
            };

            _lblDescricao = new Label
            {
                Text = "Planeje e organize suas viagens a Buenos Aires com precisão.\nMonte roteiros otimizados baseados na proximidade geográfica entre os locais.",
                Font = BuenosAiresTheme.BodyFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.TopCenter,
                Height = 44
            };

            _spacerCards = new Panel { Dock = DockStyle.Top, Height = 32, BackColor = Color.Transparent };
            var _spacerHowTo = new Panel { Dock = DockStyle.Top, Height = 34, BackColor = Color.Transparent };
            var _spacerBottom = new Panel { Dock = DockStyle.Top, Height = 24, BackColor = Color.Transparent };

            _contentTextLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.Transparent
            };

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
                ForeColor = BuenosAiresTheme.PrimaryColor,
                AutoSize = true,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _lblCountLocais = new Label
            {
                Text = "0",
                Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 28f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.PrimaryColor,
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 12, 0, 12)
            };

            var locationService = new LocationService();

            _lblCountLocais = new Label
            {
                Text = locationService.GetAll().Count.ToString(),
                Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 28f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.PrimaryColor,
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 12, 0, 12)
            };

            _lblCardLocaisDesc = new Label
            {
                Text = "Gerencie os locais savos no sistema.",
                Font = BuenosAiresTheme.BodyFont,
                ForeColor = BuenosAiresTheme.PrimaryColorDark,
                AutoSize = true,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 8, 0, 16)
            };

            _btnCardLocais = new RoundedButton
            {
                Text = "Adicionar Novo Local",
                Font = BuenosAiresTheme.ButtonFont,
                ForeColor = BuenosAiresTheme.PrimaryColorHighlight,
                FillColor = BuenosAiresTheme.PrimaryColor,
                BackColor = BuenosAiresTheme.PrimaryColorLight,
                HoverColor = BuenosAiresTheme.PrimaryButtonHover,
                Width = 120,
                Height = 40,
                Dock = DockStyle.Bottom
            };

            _btnCardLocais.Click += (s, e) =>
            {
                LocationForm locationForm = new LocationForm();
                locationForm.Location = new Point( this.Location.X + (this.Width - locationForm.Width) / 2, this.Location.Y + (this.Height - locationForm.Height) / 2);
                locationForm.ShowDialog();
                if (locationForm.Result != null)
                {
                    locationService.Add(locationForm.Result);
                    _lblCountLocais.Text = locationService.GetAll().Count.ToString();
                    MessageBox.Show("Local adicionado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (_locaisViewInitialized)
                    {
                        _locaisView.LoadLocations();
                    }
                }
            };

            //_btnRoteiros divide a tela horizontalmente com o _btnLocais, ambos com a mesma largura e altura, e ficam alinhados
            _cardRoteiros = new RoundedPanel
            {
                Dock  = DockStyle.Fill,
                FillColor = BuenosAiresTheme.AccentColorLight,
                BorderColor = BuenosAiresTheme.AccentColorHighlight,
                Margin = new Padding(12, 0, 0, 0),
                Padding = new Padding(20)
            };

            _lblCardRoteiros = new Label
            {
                Text = "Roteiros Criados",
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = BuenosAiresTheme.AccentTextDark,
                AutoSize = true,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _lblCountRoteiros = new Label
            {
                Text = "0", // aqui vai o contador de roteiros criados
                Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 28f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.AccentTextDark,
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 12, 0, 12)
            };

            _lblCardRoteirosDesc = new Label
            {
                Text = "Visualize e gerencie seus roteiros criados.",
                Font = BuenosAiresTheme.BodyFont,
                ForeColor = BuenosAiresTheme.AccentTextMid,
                AutoSize = true,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 8, 0, 16)
            };

            _btnCardRoteiros = new RoundedButton
            {
                Text = "Criar Novo Roteiro",
                Font = BuenosAiresTheme.ButtonFont,
                ForeColor = BuenosAiresTheme.AccentTextDark,
                FillColor = BuenosAiresTheme.AccentButtonFill,
                BackColor = BuenosAiresTheme.AccentColorLight,
                HoverColor = BuenosAiresTheme.AccentButtonHover,
                Width = 120,
                Height = 40,
                Dock = DockStyle.Bottom
            };

            _cardLocais.Controls.AddRange(new Control[]
            {
                _btnCardLocais,
                _lblCountLocais,
                _lblCardLocaisDesc,
                _lblCardLocais
            });

            _cardRoteiros.Controls.AddRange(new Control[]
            {
                _btnCardRoteiros,
                _lblCountRoteiros,
                _lblCardRoteirosDesc,
                _lblCardRoteiros
            });

            _cardsLayout.Controls.Add(_cardLocais, 0, 0);
            _cardsLayout.Controls.Add(_cardRoteiros, 1, 0);
            _pnlCards.Controls.Add(_cardsLayout);

            // How To: seção de instruções de uso do software, com um passo a passo simples e visual, usando o StepBadge para cada passo, e um texto explicativo ao lado de cada badge

            Panel MakeStep(int number, bool showLine, string title, string description)
            {
                _bdgRow = new TableLayoutPanel
                {
                    Dock = DockStyle.Top,
                    ColumnCount = 2,
                    RowCount = 1,
                    BackColor = Color.Transparent,
                    AutoSize = true
                };
                _bdgRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52));
                _bdgRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                _bdg = new StepBadge
                {
                    Number = number,
                    ShowLine = showLine,
                    LineHeight = 56,
                    Dock = DockStyle.Top
                };

                _lblBdgTitle = new Label
                {
                    Text = title,
                    Font = BuenosAiresTheme.ButtonFont,
                    ForeColor = BuenosAiresTheme.TextColor,
                    AutoSize = true,
                    Dock = DockStyle.Top,
                    Margin = new Padding(0, 8, 0, 4)
                };

                _lblBdgDesc = new Label
                {
                    Text = description,
                    Font = BuenosAiresTheme.BodyFont,
                    ForeColor = BuenosAiresTheme.TextMutedColor,
                    AutoSize = true,
                    Dock = DockStyle.Top
                };

                _bdgTxtPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent,
                    AutoSize = true
                };
                _bdgTxtPanel.Controls.AddRange(new Control[] { _lblBdgTitle, _lblBdgDesc });
                _bdgRow.Controls.Add(_bdg, 0, 0);
                _bdgRow.Controls.Add(_bdgTxtPanel, 1, 0);

                return _bdgRow;
            }

            _pnlHowTo = new RoundedPanel
            {
                Dock = DockStyle.Top,
                FillColor = Color.White,
                BorderColor = BuenosAiresTheme.BorderColor,
                Padding = new Padding(24,20,24,20),
                AutoSize = true 
            };

            _spacerHowToTitle = new Panel
            {
                Dock = DockStyle.Top,
                Height = 12,
                BackColor = Color.Transparent
            };

            _howToHeaderLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 36,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            _howToHeaderLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 28));
            _howToHeaderLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            _lblHowTo = new Label
            {
                Text = "Como funciona:",
                Font = new Font(BuenosAiresTheme.SubtitleFont.FontFamily, 13f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.TextColor,
                Dock = DockStyle.Fill,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0)
            };

            _infoIcon = new Label
            {
                Text = "",
                Font = new Font(BuenosAiresTheme.SubtitleFont.FontFamily, 13f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.PrimaryColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(5,5,0,0)
            };

            var infoIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "info_icon.png");
            if (File.Exists(infoIconPath))
            {
                using var infoIconStream = File.OpenRead(infoIconPath);
                using var originalInfoIcon = Image.FromStream(infoIconStream);
                _infoIcon.Image = new Bitmap(originalInfoIcon, new Size(18, 18));
            }
            else
            {
                _infoIcon.Text = "ℹ";
            }

            _howToHeaderLayout.Controls.Add(_infoIcon, 0, 0);
            _howToHeaderLayout.Controls.Add(_lblHowTo, 1, 0);


            var step1 = MakeStep(1, true, "Cadastre Locais", "Adicione restaurantes, pontos turísticos, museus e cafeterias.");
            var step2 = MakeStep(2, true, "Monte Roteiros Otimizados", "Selecione os locais e calcule distâncias automaticamente.");
            var step3 = MakeStep(3, false, "Planeje com Autonomia", "Visualize distâncias e organize seu dia com precisão.");

            _pnlHowTo.Controls.Add(step3);
            _pnlHowTo.Controls.Add(step2);
            _pnlHowTo.Controls.Add(step1);
            _pnlHowTo.Controls.Add(_spacerHowToTitle);
            _pnlHowTo.Controls.Add(_howToHeaderLayout);

            _pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                BackColor = BuenosAiresTheme.SurfaceMutedColor
            };

            _pnlFooterSeparator = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = BuenosAiresTheme.BorderColor
            };

            _lblFooter = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Buenos Aires Explorer • Planeje sua viagem com autonomia e controle",
                Font = BuenosAiresTheme.BodyFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                TextAlign = ContentAlignment.MiddleCenter
            };

            _pnlContent.Controls.AddRange(new Control[]
            {
                _spacerBottom,
                _pnlHowTo,
                _spacerHowTo,
                _pnlCards,
                _spacerCards,
                _lblDescricao,
                _lblBoasVindas,
                _spacerTop
            });

            _pnlFooter.Controls.Add(_lblFooter);
            _pnlFooter.Controls.Add(_pnlFooterSeparator);
            Controls.Add(_pnlFooter);
        }

        // ontabclicked para controlar o estado ativo das abas
        private void OnTabClicked(object? sender, EventArgs e)
        {
            _tabInicio.IsActive = false;
            _tabLocais.IsActive = false;
            _tabRoteiros.IsActive = false;

            if (sender is TabLabel tab)
            {
                tab.IsActive = true;

                SuspendLayout();

                bool isLocaisTab = ReferenceEquals(tab, _tabLocais);
                bool isRoteirosTab = ReferenceEquals(tab, _tabRoteiros);

                _pnlContent.Visible = !isLocaisTab && !isRoteirosTab;
                _locaisView.Visible = isLocaisTab;
                _roteirosView.Visible = isRoteirosTab;

                if (isLocaisTab)
                {
                    if (!_locaisViewInitialized)
                    {
                        _locaisView.LoadLocations();
                        _locaisViewInitialized = true;
                    }
                    _locaisView.BringToFront();
                    _pnlFooter.BringToFront();
                }
                else if (isRoteirosTab)
                {
                    //_roteirosView.LoadRoteiros(); quando houver método de carregamento
                    _roteirosView.BringToFront();
                    _pnlFooter.BringToFront();
                }
                else
                {
                    _pnlContent.BringToFront();
                    _pnlFooter.BringToFront();
                }

                ResumeLayout(true);
            }
        }
    }

        
}

