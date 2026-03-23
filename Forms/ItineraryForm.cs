using BuenosAiresExp.Models;
using BuenosAiresExp.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BuenosAiresExp.Services;

namespace BuenosAiresExp
{
    // Form simples para montar um roteiro de viagem por dia, usando locais existentes.
    public class ItineraryForm : Form
    {
        private readonly List<Location> _availableLocations;
        private readonly List<Location> _roteiroDodia = new List<Location>();

        private DataGridView _dgvLocais;

        private DateTimePicker _datePicker;
        private FlowLayoutPanel _flowRoteiro;
        private RoundedButton _btnAdicionar;
        private RoundedButton _btnRemover;
        private RoundedButton _btnGerarRoteiro;

        private readonly HashSet<Location> _selectedCards = new HashSet<Location>();


        public ItineraryForm(List<Location> locations)
        {
            _availableLocations = locations ?? new List<Location>();
            BuildLayout();
            WireEvents();
        }

        private void BuildLayout()
        {
            Text = "Roteiro da Viagem";
            Size = new Size(1000, 640);
            MinimumSize = new Size(800, 500);
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            BuenosAiresTheme.ApplyForm(this);

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                BackColor = BuenosAiresTheme.FillColor
            };


            split.HandleCreated += (s, e) =>
            {
                split.SplitterDistance = split.Width / 2;
            };

            Controls.Add(split);

            var lblLocaisTitle = new Label
            {
                Text = "Locais Disponíveis",
                Height = 32,
                Font = BuenosAiresTheme.SubtitleFont,
                ForeColor = BuenosAiresTheme.PrimaryColor,
                BackColor = BuenosAiresTheme.PrimaryColorLight,
                Dock = DockStyle.Top,
                Padding = new Padding(8, 8, 0, 0)
            };

            _dgvLocais = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToDeleteRows = false,
                BorderStyle = BorderStyle.None,
                MultiSelect = true,
            };
            BuenosAiresTheme.ApplyDataGridView(_dgvLocais);
            BuenosAiresTheme.ApplyDataGridViewHover(_dgvLocais);
            _dgvLocais.MultiSelect = true;


            split.Panel1.Controls.Add(lblLocaisTitle);
            split.Panel1.Controls.Add(_dgvLocais);


            var colName = new DataGridViewTextBoxColumn
            {
                Name = "colName",
                HeaderText = "Nome",
                DataPropertyName = "Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            var colCategoria = new DataGridViewTextBoxColumn
            {
                Name = "colCategoria",
                HeaderText = "Categoria",
                DataPropertyName = "Category",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            var colEndereco = new DataGridViewTextBoxColumn
            {
                Name = "colEndereco",
                HeaderText = "Endereco",
                DataPropertyName = "Address",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            _dgvLocais.Columns.AddRange(colName, colCategoria, colEndereco);
            _dgvLocais.DataSource = _availableLocations;

            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = BuenosAiresTheme.PrimaryColorLight
            };

            var lblRoteiroTitle = new Label
            {
                Text = "Roteiro do Dia",
                Font = BuenosAiresTheme.SubtitleFont,
                ForeColor = BuenosAiresTheme.PrimaryColor,
                AutoSize = true,
                Location = new Point(12, 8)
            };

            _datePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 120,
                Location = new Point(12, 30)
            };

            pnlHeader.Controls.Add(lblRoteiroTitle);
            pnlHeader.Controls.Add(_datePicker);

            _flowRoteiro = new FlowLayoutPanel
            {
                BackColor = BuenosAiresTheme.FillColor,
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(8, 4, 8, 8)
            };

            var pnlButtons = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                ColumnCount = 3,
                RowCount = 1,
                Height = 52,
                Padding = new Padding(12, 8, 12, 8),
                BackColor = BuenosAiresTheme.PrimaryColorLight
            };

          
            split.Panel2.Controls.Add(_flowRoteiro);
            split.Panel2.Controls.Add(pnlHeader);
            split.Panel2.Controls.Add(pnlButtons);


            _btnAdicionar = new RoundedButton
            {
                Text = "+ Adicionar",
                Width = 110,
                Font = BuenosAiresTheme.ButtonFont,
                ForeColor = Color.White,
                FillColor = BuenosAiresTheme.PrimaryColor,
            };

            _btnRemover = new RoundedButton
            {
                Text = "Remover",
                Width = 100,
                Font = BuenosAiresTheme.ButtonFont,
                ForeColor = BuenosAiresTheme.DangerColor,
                HoverColor = Color.FromArgb(255, 240, 240),
                FillColor = Color.Transparent,
            };

            _btnGerarRoteiro = new RoundedButton
            {
                Text = "Gerar Roteiro",
                Width = 100,
                Font = BuenosAiresTheme.ButtonFont,
                ForeColor = BuenosAiresTheme.HeaderColor,
                FillColor = BuenosAiresTheme.AccentColor,
                HoverColor = BuenosAiresTheme.AccentColorLight,
            };

            pnlButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3f));
            pnlButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3f));
            pnlButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3f));

            _btnAdicionar.Anchor = AnchorStyles.None;
            _btnRemover.Anchor = AnchorStyles.None;
            _btnGerarRoteiro.Anchor = AnchorStyles.None;

            pnlButtons.Controls.Add(_btnAdicionar, 0, 0);
            pnlButtons.Controls.Add(_btnRemover, 1, 0);
            pnlButtons.Controls.Add(_btnGerarRoteiro, 2, 0);
        }

        private void WireEvents()
        {
            _btnAdicionar.Click += (s, e) => AddSelectedLocation();
            _btnRemover.Click += (s, e) => RemoveSelectedLocation();
        }

        private void AddSelectedLocation()
        {
            if (_dgvLocais.SelectedRows.Count == 0) return;

            foreach (DataGridViewRow row in _dgvLocais.SelectedRows)
            {
                var location = row.DataBoundItem as Location;
                if (location != null && !_roteiroDodia.Contains(location))
                    _roteiroDodia.Add(location);
            }

            _dgvLocais.ClearSelection();
            RenderCards();
        }

        private void RemoveSelectedLocation()
        {
            if (_selectedCards.Count == 0) return;

            foreach (var location in _selectedCards)
                _roteiroDodia.Remove(location);

            _selectedCards.Clear();
            RenderCards();
        }


        private void RenderCards()
        {
            _flowRoteiro.SuspendLayout();
            try
            {
                _flowRoteiro.Controls.Clear();

                for (int i = 0; i < _roteiroDodia.Count; i++)
                {
                    var location = _roteiroDodia[i];

                    var distance = i < _roteiroDodia.Count - 1
                        ? DistanceService.FormatDistance(location, _roteiroDodia[i + 1])
                        : null;

                    var card = CreateCard(location, distance);
                    _flowRoteiro.Controls.Add(card);
                }
            }
            finally
            {
                _flowRoteiro.ResumeLayout();
            }
        }

        private void UpdateCardSelection(Panel card, Location location)
        {
            bool isSelected = _selectedCards.Contains(location);

            card.BackColor = isSelected
                ? BuenosAiresTheme.PrimaryColor
                : BuenosAiresTheme.SurfaceColor;

            
            foreach (Control control in card.Controls)
            {
                if (control is Label lbl)
                {
                    lbl.ForeColor = isSelected
                        ? Color.White
                        : (lbl.Font == BuenosAiresTheme.CardTitleFont
                            ? BuenosAiresTheme.PrimaryColor
                            : BuenosAiresTheme.TextMutedColor);
                }
            }
        }

        private void WireCardEvents(Panel card, Label lblNome, Label lblCategoria, Location location)
        {
            // clique
            card.Click += (s, e) =>
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                    if (_selectedCards.Contains(location))
                        _selectedCards.Remove(location);
                    else
                        _selectedCards.Add(location);

                    UpdateCardSelection(card, location);
                }
                else
                {
                    _selectedCards.Clear();
                    _selectedCards.Add(location);

                    foreach (Control ctrl in _flowRoteiro.Controls)
                    {
                        if (ctrl is Panel cardPanel)
                        {
                            var cardLocation = _roteiroDodia[_flowRoteiro.Controls.IndexOf(cardPanel)];
                            UpdateCardSelection(cardPanel, cardLocation);
                        }
                    }
                }
            };

            // hover — card e labels
            void onEnter() { if (!_selectedCards.Contains(location)) card.BackColor = BuenosAiresTheme.PrimaryColorLight; }
            void onLeave() { UpdateCardSelection(card, location); }

            card.MouseEnter += (s, e) => onEnter();
            card.MouseLeave += (s, e) => onLeave();
            lblNome.MouseEnter += (s, e) => onEnter();
            lblNome.MouseLeave += (s, e) => onLeave();
            lblCategoria.MouseEnter += (s, e) => onEnter();
            lblCategoria.MouseLeave += (s, e) => onLeave();
        }


        private Panel CreateCard(Location location, string distance)
        {
            var card = new Panel
            {
                Width = _flowRoteiro.ClientSize.Width - 24,
                Height = BuenosAiresTheme.CardHeight,
                BackColor = BuenosAiresTheme.SurfaceColor,
                Padding = new Padding(BuenosAiresTheme.CardPadding),
                Margin = new Padding(4, 2, 4, 0),
                Cursor = Cursors.Hand
            };

            var lblNome = new Label
            {
                Text = location.Name,
                Font = BuenosAiresTheme.CardTitleFont,
                ForeColor = BuenosAiresTheme.PrimaryColor,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var lblCategoria = new Label
            {
                Text = location.Category,
                Font = BuenosAiresTheme.CardBodyFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                AutoSize = true,
                Location = new Point(0, 20)
            };

            card.Controls.Add(lblNome);
            card.Controls.Add(lblCategoria);

           
            
            WireCardEvents(card, lblNome, lblCategoria, location);

            if (!string.IsNullOrEmpty(distance))
            {
                var lblDistance = new Label
                {
                    Text = $"↕ {distance}",
                    Font = BuenosAiresTheme.CardBodyFont,
                    ForeColor = BuenosAiresTheme.AccentColor,
                    AutoSize = true,
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                };

                lblDistance.Location = new Point(
                    card.Width - lblDistance.PreferredWidth - BuenosAiresTheme.CardPadding,
                    card.Height - lblDistance.PreferredHeight - BuenosAiresTheme.CardPadding
                );

                card.Controls.Add(lblDistance);
            }

            return card;


        }

    }
}
