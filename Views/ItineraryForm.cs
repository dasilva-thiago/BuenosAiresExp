using BuenosAiresExp.Models;
using BuenosAiresExp.Services;
using BuenosAiresExp.UI;
using BuenosAiresExp.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BuenosAiresExp
{
    public class ItineraryForm : Form
    {
        private readonly List<Location> _availableLocations;
        private readonly List<Location> _roteiroLocations = new();
        private readonly ItineraryService _service = new();

        private RoundedTextBox _txtNome;
        private RoundedDateTimePicker _datePicker;
        private RoundedButton _btnCriar;
        private RoundedTextBox _txtBuscarLocal;
        private FlowLayoutPanel _flowCheckboxes;
        private Label _lblContagem;
        private Label _lblTitulo;
        private Label _lblSubtitulo;
        private Label _lblIcone;
        private FlowLayoutPanel _flowRoteiro;
        private Label _lblDistanciaTotal;
        private RoundedButton _btnVerMapa;

        private Panel _pnlHeader;

        private TableLayoutPanel _layoutForm;
        private TableLayoutPanel _layoutFormButtons;
        private TableLayoutPanel _layoutLocations;
        private TableLayoutPanel _layoutRight;
        private TableLayoutPanel _layoutFooterDistance;
        private TableLayoutPanel _headerLayout;

        private Itinerary? _editingItinerary;

        public ItineraryForm(List<Location> locations)
        {
            _availableLocations = locations ?? new();
            BuildLayout();
            PopulateCheckboxes(_availableLocations);
        }

        // Sobrecarga: abrir formulário já preenchido para editar um roteiro
        public ItineraryForm(List<Location> locations, Itinerary? itinerary)
        {
            _availableLocations = locations ?? new();

            if (itinerary != null && itinerary.Items != null)
            {
                // pré-preenche a lista de locais do roteiro respeitando a ordem
                foreach (var item in itinerary.Items.OrderBy(i => i.Order))
                {
                    var loc = _availableLocations.FirstOrDefault(l => l.Id == item.LocationId) ?? item.Location;
                    if (loc != null && !_roteiroLocations.Any(r => r.Id == loc.Id))
                        _roteiroLocations.Add(loc);
                }
            }

            BuildLayout();
            PopulateCheckboxes(_availableLocations);

            if (itinerary != null)
            {
                _editingItinerary = itinerary;
                // preencher campos
                _txtNome.Value = itinerary.Name;
                _datePicker.Value = itinerary.Date;
                RenderRoteiro();
                _btnCriar.Text = "Salvar Roteiro";
            }
        }


        private void BuildLayout()
        {
            BuenosAiresTheme.ApplyForm(this);
            Text = "Novo Roteiro";
            Size = new Size(1100, 850);
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;

            _headerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(16, 16, 16, 12)
            };
            _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 72));
            _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            _headerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _headerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            _pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 96,
                BackColor = BuenosAiresTheme.PrimaryColor,
                Padding = new Padding(0, 0, 0, 0)
            };

            int pad = BuenosAiresExp.UI.BuenosAiresTheme.PaddingForm;
            int y = _pnlHeader.Height + pad;
            int widthField = ClientSize.Width - pad * 2;

            _lblTitulo = new Label
            {
                Text = Text,
                Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Margin = new Padding(0, 8, 0, 0)
            };


            _lblSubtitulo = new Label
            {
                Text = "Cadastre um plano de visitas na cidade de Buenos Aires!",
                Font = new Font(BuenosAiresTheme.MutedFont.FontFamily, 12, FontStyle.Regular),
                ForeColor = Color.White,
                AutoSize = true,
                Margin = new Padding(0, 2, 0, 0)
            };

            _lblIcone = new Label
            {
                Text = "",
                Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 36, FontStyle.Regular),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(44, 44),
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Margin = new Padding(12, 15, 0, 0)
            };
            var locationIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "route_icon.png");
            if (File.Exists(locationIconPath))
            {
                using var iconStream = File.OpenRead(locationIconPath);
                using var originalIcon = Image.FromStream(iconStream);
                _lblIcone.Image = new Bitmap(originalIcon, new Size(44, 44));
            }
            else
            {
                _lblIcone.Text = "📍";
            }

            _headerLayout.Controls.Add(_lblTitulo, 1, 0);
            _headerLayout.Controls.Add(_lblSubtitulo, 1, 1);
            _headerLayout.Controls.Add(_lblIcone, 0, 0);
            _headerLayout.SetRowSpan(_lblIcone, 2);

            _pnlHeader.Controls.Add(_headerLayout);
            Controls.Add(_pnlHeader);

            // Split principal
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                BackColor = BuenosAiresTheme.BorderColor,
                SplitterWidth = 1
            };
            split.Panel1.BackColor = BuenosAiresTheme.FillColor;
            split.Panel2.BackColor = BuenosAiresTheme.FillColor;
            split.HandleCreated += OnSplitHandleCreated;

            Controls.Add(split);
            Controls.Add(_pnlHeader);

            BuildLeftPanel(split.Panel1);
            BuildRightPanel(split.Panel2);
        }

        //Painel esquerdo 
        private void BuildLeftPanel(SplitterPanel panel)
        {
            var scroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(24, 20, 24, 20),
                BackColor = BuenosAiresTheme.FillColor
            };
            panel.Controls.Add(scroll);

            //Card: formulário do roteiro
            var cardForm = new RoundedPanel
            {
                Dock = DockStyle.Top,
                Height = 250,
                FillColor = BuenosAiresTheme.SurfaceColor,
                BorderColor = BuenosAiresTheme.BorderColor,
                Padding = new Padding(20, 16, 20, 16),
                Margin = new Padding(0, 0, 0, 12)
            };

            _layoutForm = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                BackColor = Color.Transparent
            };
            _layoutForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 36)); // título
            _layoutForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 20)); // label nome
            _layoutForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 44)); // input nome
            _layoutForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 20)); // label data
            _layoutForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 44)); // datepicker
            _layoutForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));

            var pnlTitle = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            var lblCardTitle = new Label
            {
                Text = "Novo Roteiro",
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = BuenosAiresTheme.TextColor,
                AutoSize = true,
                Location = new Point(33, 6)
            };
            var lblIcon = new Label
            {
                Text = "📅",
                Font = new Font(BuenosAiresTheme.BodyFont.FontFamily, 13f),
                AutoSize = true,
                Location = new Point(0, 6)
            };
            var lblCardSub = new Label
            {
                Text = "Crie um novo itinerário de viagem",
                Font = BuenosAiresTheme.MutedFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                AutoSize = true,
                Location = new Point(0, 24)
            };
            pnlTitle.Controls.AddRange(new Control[] { lblIcon, lblCardTitle });
            _layoutForm.Controls.Add(pnlTitle, 0, 0);

            // Label + input nome
            var lblNome = new Label
            {
                Text = "Nome do Roteiro *",
                Font = BuenosAiresTheme.LabelFont,
                ForeColor = BuenosAiresTheme.TextColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft
            };
            _layoutForm.Controls.Add(lblNome, 0, 1);

            _txtNome = new RoundedTextBox
            {
                Dock = DockStyle.Fill,
                Placeholder = "Ex: Dia 1 - Centro Histórico"
            };
            _layoutForm.Controls.Add(_txtNome, 0, 2);

            // Label + datepicker
            var lblData = new Label
            {
                Text = "Data *",
                Font = BuenosAiresTheme.LabelFont,
                ForeColor = BuenosAiresTheme.TextColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft
            };
            _layoutForm.Controls.Add(lblData, 0, 3);

            _datePicker = new RoundedDateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 180,
                Dock = DockStyle.Left
            };
            _layoutForm.Controls.Add(_datePicker, 0, 4);

            // Botões
            _layoutFormButtons = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            _layoutFormButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            _layoutFormButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

            _btnCriar = new RoundedButton
            {
                Text = "Criar Roteiro",
                Dock = DockStyle.Fill,
                FillColor = BuenosAiresTheme.PrimaryColor,
                ForeColor = Color.White,
                BackColor = BuenosAiresTheme.SurfaceColor,
                Margin = new Padding(0, 8, 0, 0)
            };
            var btnCancelar = new RoundedButton
            {
                Text = "Cancelar",
                Dock = DockStyle.Fill,
                FillColor = Color.Transparent,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                HoverColor = BuenosAiresTheme.FillColor,
                BackColor = BuenosAiresTheme.SurfaceColor,
                Margin = new Padding(0, 8, 0, 0)
            };

            _btnCriar.Click += (s, e) => SaveItinerary();
            btnCancelar.Click += (s, e) => Close();

            _layoutFormButtons.Controls.Add(_btnCriar, 0, 0);
            _layoutFormButtons.Controls.Add(btnCancelar, 1, 0);
            _layoutForm.Controls.Add(_layoutFormButtons, 0, 5);

            cardForm.Controls.Add(_layoutForm);

            // Card: selecionar locais
            var spacer = new Panel { Dock = DockStyle.Top, Height = 12, BackColor = Color.Transparent };

            var cardLocais = new RoundedPanel
            {
                Dock = DockStyle.Top,
                Height = 420,
                FillColor = BuenosAiresTheme.SurfaceColor,
                BorderColor = BuenosAiresTheme.BorderColor,
                Padding = new Padding(20, 16, 20, 16)
            };

            _layoutLocations = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.Transparent
            };
            _layoutLocations.RowStyles.Add(new RowStyle(SizeType.Absolute, 22)); // título
            _layoutLocations.RowStyles.Add(new RowStyle(SizeType.Absolute, 18)); // subtítulo
            _layoutLocations.RowStyles.Add(new RowStyle(SizeType.Absolute, 44)); // busca
            _layoutLocations.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // lista

            var lblLocTitle = new Label
            {
                Text = "📍  Selecionar Locais",
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = BuenosAiresTheme.TextColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            var lblLocSub = new Label
            {
                Text = "Escolha os pontos de interesse para este roteiro",
                Font = BuenosAiresTheme.MutedFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _txtBuscarLocal = new RoundedTextBox
            {
                Dock = DockStyle.Fill,
                Placeholder = "Buscar locais...",
                Margin = new Padding(0, 4, 0, 4)
            };
            _flowCheckboxes = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.Transparent
            };

            _txtBuscarLocal.TextChanged += OnSearchLocationTextChanged;

            _layoutLocations.Controls.Add(lblLocTitle, 0, 0);
            _layoutLocations.Controls.Add(lblLocSub, 0, 1);
            _layoutLocations.Controls.Add(_txtBuscarLocal, 0, 2);
            _layoutLocations.Controls.Add(_flowCheckboxes, 0, 3);
            cardLocais.Controls.Add(_layoutLocations);

            scroll.Controls.Add(cardLocais);
            scroll.Controls.Add(spacer);
            scroll.Controls.Add(cardForm);
        }

        //Painel direito 
        private void BuildRightPanel(SplitterPanel panel)
        {
            _layoutRight = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 20, 24, 16)
            };
            _layoutRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); // header
            _layoutRight.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // lista
            _layoutRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); // footer

            // Header
            var pnlRHeader = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            _lblContagem = new Label
            {
                Text = "Roteiro (0 locais)",
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = BuenosAiresTheme.TextColor,
                AutoSize = true,
                Location = new Point(0, 0)
            };
            var lblRSub = new Label
            {
                Text = "Organize os locais na ordem de visita. Distâncias calculadas automaticamente.",
                Font = BuenosAiresTheme.MutedFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                AutoSize = true,
                Location = new Point(0, 26)
            };
            pnlRHeader.Controls.AddRange(new Control[] { _lblContagem, lblRSub });

            // Lista
            _flowRoteiro = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 4, 0, 4)
            };

            // fix: handler que re-renderiza a lista quando o painel é redimensionado, para manter os cards com a largura correta, tambem corrige crash
            _flowRoteiro.Resize += OnRoteiroResize;

            // Footer: distância total
            var footerCard = new RoundedPanel
            {
                Dock = DockStyle.Fill,
                FillColor = BuenosAiresTheme.SurfaceColor,
                BorderColor = BuenosAiresTheme.BorderColor,
                Padding = new Padding(16, 0, 16, 0),
                Margin = new Padding(0)
            };

            _layoutFooterDistance = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            _layoutFooterDistance.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            _layoutFooterDistance.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

            var lblDistLabel = new Label
            {
                Text = "Distância Total:",
                Font = BuenosAiresTheme.ButtonFont,
                ForeColor = BuenosAiresTheme.TextColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _lblDistanciaTotal = new Label
            {
                Text = "—",
                Font = BuenosAiresTheme.ButtonFont,
                ForeColor = BuenosAiresTheme.AccentTextDark,
                BackColor = BuenosAiresTheme.AccentCardFill,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 8, 0, 8)
            };

            _layoutFooterDistance.Controls.Add(lblDistLabel, 0, 0);
            _layoutFooterDistance.Controls.Add(_lblDistanciaTotal, 1, 0);
            footerCard.Controls.Add(_layoutFooterDistance);

            _btnVerMapa = new RoundedButton
            {
                Text = "Ver no Mapa",
                Dock = DockStyle.Fill,
                FillColor = BuenosAiresTheme.AccentColor,
                ForeColor = BuenosAiresTheme.HeaderColor,
                HoverColor = BuenosAiresTheme.AccentColorMuted,
                BackColor = BuenosAiresTheme.FillColor,
                Font = BuenosAiresTheme.ButtonFont,
                Margin = new Padding(0, 8, 0, 0)
            };
            _btnVerMapa.Click += OnViewMapClick;

            _layoutRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            _layoutRight.Controls.Add(_btnVerMapa, 0, 3);

            _layoutRight.Controls.Add(pnlRHeader, 0, 0);
            _layoutRight.Controls.Add(_flowRoteiro, 0, 1);
            _layoutRight.Controls.Add(footerCard, 0, 2);
            panel.Controls.Add(_layoutRight);
        }

        // Checkboxes
        private void PopulateCheckboxes(List<Location> locations)
        {
            _flowCheckboxes.SuspendLayout();
            _flowCheckboxes.Controls.Clear();

            foreach (var loc in locations.OrderBy(l => l.Name))
            {
                var isChecked = _roteiroLocations.Any(r => r.Id == loc.Id);

                var item = new Panel
                {
                    Width = _flowCheckboxes.ClientSize.Width - 4,
                    Height = 54,
                    BackColor = isChecked ? BuenosAiresTheme.PrimaryColorLight : BuenosAiresTheme.SurfaceColor,
                    Padding = new Padding(10, 8, 10, 8),
                    Margin = new Padding(0, 0, 0, 4),
                    Cursor = Cursors.Hand
                };
                item.Tag = loc;

                var chk = new CheckBox
                {
                    Checked = isChecked,
                    AutoSize = false,
                    Size = new Size(18, 18),
                    Location = new Point(10, 18),
                    BackColor = Color.Transparent,
                    FlatStyle = FlatStyle.Standard
                };

                var lblNome = new Label
                {
                    Text = loc.Name,
                    Font = BuenosAiresTheme.ButtonFont,
                    ForeColor = BuenosAiresTheme.TextColor,
                    AutoSize = true,
                    Location = new Point(34, 7)
                };

                var lblSub = new Label
                {
                    Text = $"{loc.Category}  •  {TruncateAddress(loc.Address)}",
                    Font = BuenosAiresTheme.MutedFont,
                    ForeColor = BuenosAiresTheme.TextMutedColor,
                    AutoSize = false,
                    Size = new Size(item.Width - 50, 16),
                    Location = new Point(34, 28)
                };

                item.Controls.AddRange(new Control[] { chk, lblNome, lblSub });

                Action toggle = () =>
                {
                    if (_roteiroLocations.Any(r => r.Id == loc.Id))
                    {
                        _roteiroLocations.RemoveAll(r => r.Id == loc.Id);
                        chk.Checked = false;
                        item.BackColor = BuenosAiresTheme.SurfaceColor;
                    }
                    else
                    {
                        _roteiroLocations.Add(loc);
                        chk.Checked = true;
                        item.BackColor = BuenosAiresTheme.PrimaryColorLight;
                    }

                    RenderRoteiro();
                };

                item.Click += (s, e) => toggle();
                chk.Click += (s, e) => toggle();
                lblNome.Click += (s, e) => toggle();
                lblSub.Click += (s, e) => toggle();

                _flowCheckboxes.Resize += (s, e) =>
                {
                    item.Width = _flowCheckboxes.ClientSize.Width - 4;
                    lblSub.Size = new Size(item.Width - 50, 16);
                };

                _flowCheckboxes.Controls.Add(item);
            }

            _flowCheckboxes.ResumeLayout();
        }

        //Render da lista
        private void RenderRoteiro()
        {
            _flowRoteiro.SuspendLayout();
            _flowRoteiro.Controls.Clear();

            _lblContagem.Text =
                $"Roteiro ({_roteiroLocations.Count} loca{(_roteiroLocations.Count != 1 ? "is" : "l")})";

            for (int i = 0; i < _roteiroLocations.Count; i++)
            {
                var loc = _roteiroLocations[i];
                var hasNext = i < _roteiroLocations.Count - 1;
                int idx = i;

                // Card
                var card = new RoundedPanel
                {
                    Width = _flowRoteiro.ClientSize.Width - 8,
                    Height = 66,
                    FillColor = BuenosAiresTheme.SurfaceColor,
                    BorderColor = BuenosAiresTheme.BorderColor,
                    Padding = new Padding(12, 8, 12, 8),
                    Margin = new Padding(0, 0, 0, 0)
                };

                var tbl = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 4,
                    RowCount = 1,
                    BackColor = Color.Transparent
                };
                tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 28)); // setas
                tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 28)); // número
                tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // conteúdo
                tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 28)); // botão X

                // Setas
                var pnlArrows = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    BackColor = BuenosAiresTheme.SurfaceColor,
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };

                var btnUp = MakeArrowButton("↑");
                btnUp.Margin = new Padding(0, 3, 0, 1); // 3px do topo, 1px entre os botões
                btnUp.Click += (s, e) =>
                {
                    if (idx == 0) return;
                    (_roteiroLocations[idx], _roteiroLocations[idx - 1]) =
                        (_roteiroLocations[idx - 1], _roteiroLocations[idx]);
                    RenderRoteiro();
                };

                var btnDown = MakeArrowButton("↓");
                btnDown.Margin = new Padding(0, 1, 0, 0); // 1px entre os botões
                btnDown.Click += (s, e) =>
                {
                    if (idx >= _roteiroLocations.Count - 1) return;
                    (_roteiroLocations[idx], _roteiroLocations[idx + 1]) =
                        (_roteiroLocations[idx + 1], _roteiroLocations[idx]);
                    RenderRoteiro();
                };

                pnlArrows.Controls.AddRange(new Control[] { btnUp, btnDown });

                // Número
                var lblNum = new Label
                {
                    Text = $"{i + 1}.",
                    Font = BuenosAiresTheme.ButtonFont,
                    ForeColor = BuenosAiresTheme.PrimaryColor,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                // Conteúdo
                var pnlInfo = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
                var lblNome = new Label
                {
                    Text = loc.Name,
                    Font = BuenosAiresTheme.ButtonFont,
                    ForeColor = BuenosAiresTheme.TextColor,
                    AutoSize = true,
                    Location = new Point(0, 4)
                };
                var lblBadge = new Label
                {
                    Text = loc.Category,
                    Font = BuenosAiresTheme.BadgeFont,
                    ForeColor = BuenosAiresTheme.GetCategoryTextColor(loc.Category),
                    BackColor = BuenosAiresTheme.GetCategoryColor(loc.Category),
                    AutoSize = true,
                    Padding = new Padding(5, 2, 5, 2),
                    Location = new Point(0, 30)
                };
                pnlInfo.Controls.AddRange(new Control[] { lblNome, lblBadge });

                // Botão remover
                var btnRemove = new Label
                {
                    Text = "×",
                    Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 15f),
                    ForeColor = BuenosAiresTheme.TextMutedColor,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand
                };
                btnRemove.Click += (s, e) =>
                {
                    _roteiroLocations.RemoveAll(r => r.Id == loc.Id);
                    PopulateCheckboxes(_availableLocations
                        .Where(l => l.Name.Contains(_txtBuscarLocal.Value, StringComparison.OrdinalIgnoreCase)
                                    || l.Category.Contains(_txtBuscarLocal.Value, StringComparison.OrdinalIgnoreCase)
                                    || string.IsNullOrWhiteSpace(_txtBuscarLocal.Value))
                        .ToList());
                    RenderRoteiro();
                };

                tbl.Controls.Add(pnlArrows, 0, 0);
                tbl.Controls.Add(lblNum, 1, 0);
                tbl.Controls.Add(pnlInfo, 2, 0);
                tbl.Controls.Add(btnRemove, 3, 0);
                card.Controls.Add(tbl);

                _flowRoteiro.Controls.Add(card);

                // Conector de distância
                if (hasNext)
                {
                    var dist = DistanceService.FormatDistance(loc, _roteiroLocations[i + 1]);
                    var connector = new Panel
                    {
                        Width = _flowRoteiro.ClientSize.Width - 8,
                        Height = 24,
                        BackColor = Color.Transparent,
                        Margin = new Padding(0)
                    };
                    var lblDist = new Label
                    {
                        Text = $"↓  {dist}",
                        Font = BuenosAiresTheme.BadgeFont,
                        ForeColor = BuenosAiresTheme.AccentTextDark,
                        BackColor = BuenosAiresTheme.AccentCardFill,
                        AutoSize = true,
                        Padding = new Padding(8, 2, 8, 2)
                    };
                    connector.Controls.Add(lblDist);
                    connector.Paint += (s, e) =>
                    {
                        lblDist.Left = (connector.Width - lblDist.Width) / 2;
                        lblDist.Top = (connector.Height - lblDist.Height) / 2;
                    };
                    _flowRoteiro.Controls.Add(connector);
                }

            }

            // Distância total
            if (_roteiroLocations.Count >= 2)
            {
                double total = 0;
                for (int i = 0; i < _roteiroLocations.Count - 1; i++)
                    total += DistanceService.CalculateDistance(
                        _roteiroLocations[i], _roteiroLocations[i + 1]);
                _lblDistanciaTotal.Text = total < 1 ? $"{(int)(total * 1000)} m" : $"{total:F2} km";
            }
            else
            {
                _lblDistanciaTotal.Text = "—";
            }

            _flowRoteiro.ResumeLayout();
        }

        // método do save roteiro
        private void SaveItinerary()
        {
            if (!ValidateItineraryInput())
                return;

            var itinerary = BuildItineraryFromSelection();

            try
            {
                if (_editingItinerary == null)
                {
                    _service.Add(itinerary);
                    MessageBox.Show($"Roteiro \"{itinerary.Name}\" salvo! ✔", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    itinerary.Id = _editingItinerary.Id;
                    _service.Update(itinerary);
                    MessageBox.Show($"Roteiro \"{itinerary.Name}\" atualizado! ✔", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Após atualizar, recarregue do banco para evitar duplicação visual
                    var roteiroAtualizado = _service.GetAll().FirstOrDefault(r => r.Id == itinerary.Id);
                    if (roteiroAtualizado != null)
                    {
                        _roteiroLocations.Clear();
                        foreach (var item in roteiroAtualizado.Items.OrderBy(i => i.Order))
                        {
                            if (item.Location != null)
                                _roteiroLocations.Add(item.Location);
                        }

                        RenderRoteiro();
                    }
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar:\n{ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateItineraryInput()
        {
            if (string.IsNullOrWhiteSpace(_txtNome.Value))
            {
                MessageBox.Show("Preencha o nome do roteiro.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_roteiroLocations.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um local.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private Itinerary BuildItineraryFromSelection()
        {
            return new Itinerary
            {
                Name = _txtNome.Value.Trim(),
                Date = _datePicker.Value.Date,
                Items = _roteiroLocations
                    .Select((loc, idx) => new ItineraryItem
                    {
                        LocationId = loc.Id,
                        Location = null,
                        Order = idx
                    }).ToList()
            };
        }

        private void OnViewMapClick(object? sender, EventArgs e)
        {
            if (_roteiroLocations.Count == 0)
            {
                MessageBox.Show("Adicione locais ao roteiro para visualizar no mapa.",
                    "Mapa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var mapForm = new ItineraryMapForm(_roteiroLocations, _txtNome.Value);
            mapForm.ShowDialog(this);
        }

        private void OnSplitHandleCreated(object? sender, EventArgs e)
        {
            if (sender is SplitContainer split)
            {
                split.SplitterDistance = split.Width / 2;
            }
        }

        private void OnSearchLocationTextChanged(object? sender, EventArgs e)
        {
            var q = _txtBuscarLocal.Value;
            PopulateCheckboxes(_availableLocations
                .Where(l => l.Name.Contains(q, StringComparison.OrdinalIgnoreCase)
                            || l.Category.Contains(q, StringComparison.OrdinalIgnoreCase))
                .ToList());
        }

        private void OnRoteiroResize(object? sender, EventArgs e)
        {
            if (_roteiroLocations.Count > 0)
            {
                RenderRoteiro();
            }
        }

        // Helpers: botões de seta e truncar endereço
        private static RoundedButton MakeArrowButton(string fallbackText, string? iconFileName = null)
        {
            var button = new RoundedButton
            {
                Text = fallbackText,
                Width = 22,
                Height = 22,
                Font = new Font(BuenosAiresTheme.BodyFont.FontFamily, 8f),
                FillColor = Color.Transparent,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                HoverColor = BuenosAiresTheme.PrimaryColorLight,
                BackColor = BuenosAiresTheme.SurfaceColor,
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0)
            };

            if (!string.IsNullOrWhiteSpace(iconFileName))
            {
                var icon = TryLoadArrowIcon(iconFileName);
                if (icon != null)
                {
                    button.Image = icon;
                    button.Text = string.Empty;
                }
            }

            return button;
        }

        private static string TruncateAddress(string address)
        {
            const int maxLength = 35;
            if (string.IsNullOrWhiteSpace(address))
                return string.Empty;
            
            return address.Length <= maxLength 
                ? address 
                : address.Substring(0, maxLength) + "...";
        }

        private static Image? TryLoadArrowIcon(string fileName)
        {
            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", fileName);
            if (!File.Exists(iconPath))
                return null;

            Bitmap source;
            using (var stream = File.OpenRead(iconPath))
                source = new Bitmap(stream);

            // Cria bitmap com canal alpha garantido
            var result = new Bitmap(12, 12, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(result))
            {
                g.Clear(Color.Transparent);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.DrawImage(source, 0, 0, 12, 12);
            }

            source.Dispose();
            return result;
        }
    }
}