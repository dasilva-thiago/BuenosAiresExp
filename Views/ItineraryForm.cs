using BuenosAiresExp.Models;
using BuenosAiresExp.Services;
using BuenosAiresExp.UI;
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
        private DateTimePicker _datePicker;
        private RoundedButton _btnCriar;
        private RoundedTextBox _txtBuscarLocal;
        private FlowLayoutPanel _flowCheckboxes;
        private Label _lblContagem;
        private FlowLayoutPanel _flowRoteiro;
        private Label _lblDistanciaTotal;

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
                    if (loc != null)
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
            Size = new Size(1100, 800);
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;

            // Header
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                BackColor = BuenosAiresTheme.PrimaryColor,
                Padding = new Padding(20, 0, 20, 0)
            };
            var lblBack = new Label
            {
                Text = "← Voltar para Roteiros",
                ForeColor = Color.White,
                Font = BuenosAiresTheme.ButtonFont,
                AutoSize = true,
                Cursor = Cursors.Hand,
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft
            };
            lblBack.Click += (s, e) => Close();
            pnlHeader.Controls.Add(lblBack);

            // Split principal
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                BackColor = BuenosAiresTheme.BorderColor,
                SplitterWidth = 1
            };
            split.Panel1.BackColor = BuenosAiresTheme.FillColor;
            split.Panel2.BackColor = BuenosAiresTheme.FillColor;
            split.HandleCreated += (s, e) => split.SplitterDistance = split.Width / 2;

            Controls.Add(split);
            Controls.Add(pnlHeader);

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

            var tblForm = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                BackColor = Color.Transparent
            };
            tblForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 36)); // título
            tblForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 20)); // label nome
            tblForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 44)); // input nome
            tblForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 20)); // label data
            tblForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 44)); // datepicker
            tblForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));  

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
            tblForm.Controls.Add(pnlTitle, 0, 0);

            // Label + input nome
            var lblNome = new Label
            {
                Text = "Nome do Roteiro *",
                Font = BuenosAiresTheme.LabelFont,
                ForeColor = BuenosAiresTheme.TextColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft
            };
            tblForm.Controls.Add(lblNome, 0, 1);

            _txtNome = new RoundedTextBox
            {
                Dock = DockStyle.Fill,
                Placeholder = "Ex: Dia 1 - Centro Histórico"
            };
            tblForm.Controls.Add(_txtNome, 0, 2);

            // Label + datepicker
            var lblData = new Label
            {
                Text = "Data *",
                Font = BuenosAiresTheme.LabelFont,
                ForeColor = BuenosAiresTheme.TextColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft
            };
            tblForm.Controls.Add(lblData, 0, 3);

            _datePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = BuenosAiresTheme.BodyFont,
                Width = 180,
                Dock = DockStyle.Left
            };
            tblForm.Controls.Add(_datePicker, 0, 4);

            // Botões
            var pnlBtns = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            pnlBtns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            pnlBtns.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

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

            pnlBtns.Controls.Add(_btnCriar, 0, 0);
            pnlBtns.Controls.Add(btnCancelar, 1, 0);
            tblForm.Controls.Add(pnlBtns, 0, 5);

            cardForm.Controls.Add(tblForm);

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

            var tblLocais = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.Transparent
            };
            tblLocais.RowStyles.Add(new RowStyle(SizeType.Absolute, 22)); // título
            tblLocais.RowStyles.Add(new RowStyle(SizeType.Absolute, 18)); // subtítulo
            tblLocais.RowStyles.Add(new RowStyle(SizeType.Absolute, 44)); // busca
            tblLocais.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // lista

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

            _txtBuscarLocal.TextChanged += (s, e) =>
            {
                var q = _txtBuscarLocal.Value;
                PopulateCheckboxes(_availableLocations
                    .Where(l => l.Name.Contains(q, StringComparison.OrdinalIgnoreCase)
                             || l.Category.Contains(q, StringComparison.OrdinalIgnoreCase))
                    .ToList());
            };

            tblLocais.Controls.Add(lblLocTitle, 0, 0);
            tblLocais.Controls.Add(lblLocSub, 0, 1);
            tblLocais.Controls.Add(_txtBuscarLocal, 0, 2);
            tblLocais.Controls.Add(_flowCheckboxes, 0, 3);
            cardLocais.Controls.Add(tblLocais);

            scroll.Controls.Add(cardLocais);
            scroll.Controls.Add(spacer);
            scroll.Controls.Add(cardForm);
        }

        //Painel direito 
        private void BuildRightPanel(SplitterPanel panel)
        {
            var outer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 20, 24, 16)
            };
            outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); // header
            outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // lista
            outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); // footer

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
                Text = "Organize os locais na ordem de visita. Distâncias calculadas pela fórmula de Haversine.",
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
            _flowRoteiro.Resize += (s, e) =>
            {
                if (_roteiroLocations.Count > 0)
                    RenderRoteiro();
            };

            // Footer: distância total
            var footerCard = new RoundedPanel
            {
                Dock = DockStyle.Fill,
                FillColor = BuenosAiresTheme.SurfaceColor,
                BorderColor = BuenosAiresTheme.BorderColor,
                Padding = new Padding(16, 0, 16, 0)
            };

            var tblFooter = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            tblFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tblFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

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

            tblFooter.Controls.Add(lblDistLabel, 0, 0);
            tblFooter.Controls.Add(_lblDistanciaTotal, 1, 0);
            footerCard.Controls.Add(tblFooter);

            outer.Controls.Add(pnlRHeader, 0, 0);
            outer.Controls.Add(_flowRoteiro, 0, 1);
            outer.Controls.Add(footerCard, 0, 2);
            panel.Controls.Add(outer);
        }

        // Checkboxes
        private void PopulateCheckboxes(List<Location> locations)
        {
            _flowCheckboxes.SuspendLayout();
            _flowCheckboxes.Controls.Clear();

            foreach (var loc in locations.OrderBy(l => l.Name))
            {
                var isChecked = _roteiroLocations.Contains(loc);

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
                    if (_roteiroLocations.Contains(loc))
                    {
                        _roteiroLocations.Remove(loc);
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

            _lblContagem.Text = $"Roteiro ({_roteiroLocations.Count} loca{(_roteiroLocations.Count != 1 ? "is" : "l")})";

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
                var pnlArrows = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
                var btnUp = MakeArrowButton("↑");
                btnUp.Location = new Point(0, 2);
                btnUp.Click += (s, e) =>
                {
                    if (idx == 0) return;
                    (_roteiroLocations[idx], _roteiroLocations[idx - 1]) =
                        (_roteiroLocations[idx - 1], _roteiroLocations[idx]);
                    RenderRoteiro();
                };
                var btnDown = MakeArrowButton("↓");
                btnDown.Location = new Point(0, 30);
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
                    ForeColor = BuenosAiresTheme.TextMutedColor,
                    BackColor = BuenosAiresTheme.CategoryDefaultBg,
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
                    _roteiroLocations.Remove(loc);
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
            if (string.IsNullOrWhiteSpace(_txtNome.Value))
            {
                MessageBox.Show("Preencha o nome do roteiro.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_roteiroLocations.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um local.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var itinerary = new Itinerary
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
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar:\n{ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helpers: botões de seta e truncar endereço
        private static RoundedButton MakeArrowButton(string text)
            => new RoundedButton
            {
                Text = text,
                Width = 22,
                Height = 22,
                Font = new Font(BuenosAiresTheme.BodyFont.FontFamily, 8f),
                FillColor = Color.Transparent,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                HoverColor = BuenosAiresTheme.PrimaryColorLight,
                BackColor = Color.Transparent
            };

        private static string TruncateAddress(string address, int max = 55)
            => address?.Length > max ? address[..max] + "…" : address ?? "";
    }
}