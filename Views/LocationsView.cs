using BuenosAiresExp.Models;
using BuenosAiresExp.Services;
using BuenosAiresExp.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BuenosAiresExp.Views
{
    public class LocaisView : UserControl
    {
        public event EventHandler? FirstRenderComplete;

        private const int CardWidth = 470;
        private const int CardHeight = 200;
        private const int CardsPerRow = 3;

        private readonly LocationService _locationService;
        private List<Location> _allLocations = new();
        private List<Location> _filteredLocations = new();
        private bool _isCardView = true;
        private bool _firstRenderFired = false;


        private Panel _pnlHeader;
        private TableLayoutPanel _headerLayout;
        private Label _lblTitle;
        private Label _lblSubtitle;
        private RoundedButton _btnNovoLocal;
        private Panel _pnlHeaderActions;
        private RoundedButton _btnFiltrar;
        private CheckedListBox _clbFiltroCategorias;

        
        private Panel _pnlToolbar;
        private RoundedTextBox _txtBuscar;
        private RoundedButton _btnViewCards;
        private RoundedButton _btnViewTable;

        
        private Panel _pnlContent;
        private FlowLayoutPanel _flowCards;
        private DataGridView _dgvLocais;

        
        private Label _lblStatus;

        public LocaisView()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);

            _locationService = new LocationService();
            BuildLayout();
            LoadLocations();
        }

        private void BuildLayout()
        {
            Dock = DockStyle.Fill;
            BackColor = BuenosAiresTheme.FillColor;

           
            _pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = BuenosAiresTheme.FillColor,
                Padding = new Padding(32, 16, 32, 0)
            };

            _headerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2
            };
            _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 440));
            _headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            _headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 45));

            _lblTitle = new Label
            {
                Text = "Locais",
                Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 20f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.TextColor,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                Margin = new Padding(0, 0, 5, 0)
            };

            _lblSubtitle = new Label
            {
                Text = "Gerencie pontos de interesse em Buenos Aires",
                Font = BuenosAiresTheme.BodyFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };

            _btnNovoLocal = new RoundedButton
            {
                Text = "+ Novo Local",
                Width = 130,
                Font = BuenosAiresTheme.ButtonFont,
                ForeColor = Color.White,
                FillColor = BuenosAiresTheme.PrimaryColor,
                HoverColor = BuenosAiresTheme.PrimaryColorDark,
                BackColor = BuenosAiresTheme.FillColor,
                Dock = DockStyle.Right,
                Margin = new Padding(8, 4, 0, 4)
            };

            _btnFiltrar = new RoundedButton
            {
                Text = "Filtrar",
                Width = 90,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = Color.Transparent,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                HoverColor = BuenosAiresTheme.PrimaryColorLight,
                BackColor = BuenosAiresTheme.FillColor,
                Dock = DockStyle.Right,
                Margin = new Padding(8, 4, 0, 4)
            };

            var filterIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "filter_icon.png");
            if (File.Exists(filterIconPath))
            {
                _btnFiltrar.Image = new Bitmap(Image.FromFile(filterIconPath), new Size(14, 14));
                _btnFiltrar.Text = " Filtrar";
            }

            _btnViewCards = new RoundedButton
            {
                Text = "⊞ Cards",
                Width = 90,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = BuenosAiresTheme.PrimaryColor,
                ForeColor = Color.White,
                BackColor = BuenosAiresTheme.FillColor,
                Dock = DockStyle.Right,
                Margin = new Padding(8, 4, 0, 4)
            };

            _btnViewTable = new RoundedButton
            {
                Text = "☰ Tabela",
                Width = 90,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = Color.Transparent,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                HoverColor = BuenosAiresTheme.PrimaryColorLight,
                BackColor = BuenosAiresTheme.FillColor,
                Dock = DockStyle.Right,
                Margin = new Padding(8, 4, 0, 4)
            };

            _pnlHeaderActions = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            _pnlHeaderActions.Controls.Add(_btnNovoLocal);
            _pnlHeaderActions.Controls.Add(_btnFiltrar);
            _pnlHeaderActions.Controls.Add(_btnViewTable);
            _pnlHeaderActions.Controls.Add(_btnViewCards);

            _clbFiltroCategorias = new CheckedListBox
            {
                Visible = false,
                CheckOnClick = true,
                IntegralHeight = false,
                BorderStyle = BorderStyle.FixedSingle,
                Width = 220,
                Height = 160
            };

            _headerLayout.Controls.Add(_lblTitle, 0, 0);
            _headerLayout.Controls.Add(_lblSubtitle, 0, 1);
            _headerLayout.Controls.Add(_pnlHeaderActions, 1, 0);
            _headerLayout.SetRowSpan(_pnlHeaderActions, 2);
            _pnlHeader.Controls.Add(_headerLayout);

            _pnlToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = BuenosAiresTheme.FillColor,
                Padding = new Padding(32, 8, 32, 8)
            };

            _txtBuscar = new RoundedTextBox
            {
                Width = 320,
                Height = 36,
                Placeholder = "Buscar por nome, categoria ou endereço...",
                Dock = DockStyle.Left
            };

            _lblStatus = new Label
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                Font = BuenosAiresTheme.MutedFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = new Padding(0, 0, 12, 0)
            };

            _pnlToolbar.Controls.Add(_txtBuscar);
            _pnlToolbar.Controls.Add(_lblStatus);

            
            _pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BuenosAiresTheme.FillColor,
                Padding = new Padding(32, 16, 32, 16),
                AutoScroll = true
            };

            _flowCards = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
                BackColor = Color.Transparent
            };

            _dgvLocais = new DataGridView
            {
                Dock = DockStyle.Fill,
                Visible = false,
                AutoGenerateColumns = false
            };
            BuenosAiresTheme.ApplyDataGridView(_dgvLocais);
            BuenosAiresTheme.ApplyDataGridViewHover(_dgvLocais);

            var colName = new DataGridViewTextBoxColumn { HeaderText = "Nome", DataPropertyName = "Name", FillWeight = 25, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            var colCat = new DataGridViewTextBoxColumn { HeaderText = "Categoria", DataPropertyName = "Category", FillWeight = 15, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            var colAddr = new DataGridViewTextBoxColumn { HeaderText = "Endereço", DataPropertyName = "Address", FillWeight = 35, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            var colLat = new DataGridViewTextBoxColumn { HeaderText = "Latitude", DataPropertyName = "Latitude", FillWeight = 10, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            var colLng = new DataGridViewTextBoxColumn { HeaderText = "Longitude", DataPropertyName = "Longitude", FillWeight = 10, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            var colNotes = new DataGridViewTextBoxColumn { HeaderText = "Notas", DataPropertyName = "Notes", FillWeight = 15, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            _dgvLocais.Columns.AddRange(colName, colCat, colAddr, colLat, colLng, colNotes);

            _pnlContent.Controls.Add(_flowCards);
            _pnlContent.Controls.Add(_dgvLocais);

      
            Controls.Add(_pnlContent);
            Controls.Add(_pnlToolbar);
            Controls.Add(_pnlHeader);
            Controls.Add(_clbFiltroCategorias);
            _clbFiltroCategorias.BringToFront();

            
            _btnNovoLocal.Click += (s, e) => OpenLocationForm(null);
            _btnViewCards.Click += (s, e) => SetView(true);
            _btnViewTable.Click += (s, e) => SetView(false);
            _btnFiltrar.Click += (s, e) => ToggleCategoryFilter();
            _txtBuscar.TextChanged += (s, e) => ApplyFilters();
            _flowCards.Resize += (s, e) =>
            {
                if (_isCardView && _filteredLocations.Count > 0)
                    RenderCards(_filteredLocations);

                PositionCategoryFilterPopup();
            };

            _pnlHeader.Resize += (s, e) => PositionCategoryFilterPopup();
            _pnlToolbar.Resize += (s, e) => PositionCategoryFilterPopup();

            _clbFiltroCategorias.ItemCheck += ClbFiltroCategorias_ItemCheck;

            this.MouseDown += HandleClickOutsideFilter;
            _pnlHeader.MouseDown += HandleClickOutsideFilter;
            _pnlToolbar.MouseDown += HandleClickOutsideFilter;
            _pnlContent.MouseDown += HandleClickOutsideFilter;
            _flowCards.MouseDown += HandleClickOutsideFilter;
            _dgvLocais.MouseDown += HandleClickOutsideFilter;

            _dgvLocais.CellDoubleClick += (s, e) =>
            {
                var loc = _dgvLocais.SelectedRows.Count > 0
                    ? _dgvLocais.SelectedRows[0].DataBoundItem as Location
                    : null;
                if (loc != null) ShowDetail(loc);
            };
        }

     

        private void SetView(bool cardView)
        {
            _isCardView = cardView;

            _flowCards.Visible = cardView;
            _dgvLocais.Visible = !cardView;

            // atualiza visual dos botões de toggle
            _btnViewCards.FillColor = cardView ? BuenosAiresTheme.PrimaryColor : Color.Transparent;
            _btnViewCards.ForeColor = cardView ? Color.White : BuenosAiresTheme.TextMutedColor;
            _btnViewTable.FillColor = !cardView ? BuenosAiresTheme.PrimaryColor : Color.Transparent;
            _btnViewTable.ForeColor = !cardView ? Color.White : BuenosAiresTheme.TextMutedColor;

            _btnViewCards.Invalidate();
            _btnViewTable.Invalidate();
        }

        public void LoadLocations()
        {
            _allLocations = _locationService.GetAll();
            // força layout calculado antes de renderizar
            _flowCards.SuspendLayout();
            ApplyFilters();
            _flowCards.ResumeLayout(true);
        }

        private void ApplyFilters()
        {
            var search = _txtBuscar?.Value ?? string.Empty;
            var categoriasSelecionadas = _clbFiltroCategorias.CheckedItems
                .Cast<string>()
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            IEnumerable<Location> query = _allLocations;

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(l =>
                    l.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    l.Category.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    l.Address.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (categoriasSelecionadas.Count > 0)
            {
                query = query.Where(l => categoriasSelecionadas.Contains(l.Category));
            }

            var filtered = query.ToList();

            _filteredLocations = filtered;

            _lblStatus.Text = $"{filtered.Count} local(is)";

            RenderCards(filtered);

            _dgvLocais.DataSource = null;
            _dgvLocais.DataSource = filtered;
        }

        private void RenderCards(List<Location> locations)
        {
            _flowCards.SuspendLayout();
            _flowCards.Controls.Clear();

            int cardWidth = CalculateCardWidth();
            int spacing = 16;
            int index = 0;

            foreach (var loc in locations.OrderBy(l => l.Name))
            {
                var card = CreateCard(loc, cardWidth);
                bool isEndOfRow = ((index + 1) % CardsPerRow) == 0;
                card.Margin = new Padding(0, 0, isEndOfRow ? 0 : spacing, 16);
                _flowCards.Controls.Add(card);
                index++;
            }

            _flowCards.ResumeLayout();

            // dispara o evento somente após a primeira pintura real
            if (!_firstRenderFired)
            {
                _firstRenderFired = true;
                void OnFirstPaint(object? s, PaintEventArgs pe)
                {
                    _flowCards.Paint -= OnFirstPaint;
                    FirstRenderComplete?.Invoke(this, EventArgs.Empty);
                }
                _flowCards.Paint += OnFirstPaint;
            }
        }

        private int CalculateCardWidth()
        {
            int spacing = 16;
            int availableWidth = _flowCards.ClientSize.Width - _flowCards.Padding.Horizontal;
            if (_flowCards.VerticalScroll.Visible)
                availableWidth -= SystemInformation.VerticalScrollBarWidth;

            int totalSpacing = spacing * (CardsPerRow - 1);
            int width = (availableWidth - totalSpacing) / CardsPerRow;
            return Math.Max(width, 280);
        }

        private Panel CreateCard(Location location, int cardWidth)
        {
            var latText = location.Latitude.ToString("F6", CultureInfo.InvariantCulture);
            var lngText = location.Longitude.ToString("F6", CultureInfo.InvariantCulture);

            var card = new RoundedPanel
            {
                Width = cardWidth,
                Height = CardHeight,
                FillColor = BuenosAiresTheme.SurfaceColor,
                BorderColor = BuenosAiresTheme.BorderColor,
                Padding = new Padding(18, 14, 18, 14)
            };

            var headerRow = new Panel
            {
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = Color.Transparent
            };

            var pnlActions = new Panel
            {
                Dock = DockStyle.Right,
                Width = 126,
                Height = 36,
                BackColor = Color.Transparent
            };

            var lblName = new Label
            {
                Text = location.Name,
                Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 12f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.TextColor,
                Dock = DockStyle.Fill,
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 2, 0, 0)
            };

            // barra de ações — botões editar, excluir, visualizar
            var btnEdit = MakeActionButton("Edit", BuenosAiresTheme.TextColor, "edit_icon.png");
            var btnDel = MakeActionButton("Del", BuenosAiresTheme.DangerColor, "delete_icon.png");
            var btnView = MakeActionButton("View", BuenosAiresTheme.TextColor, "view_icon.png");

            btnEdit.Dock = DockStyle.Right;
            btnDel.Dock = DockStyle.Right;
            btnView.Dock = DockStyle.Right;

            btnEdit.Click += (s, e) => OpenLocationForm(location);
            btnDel.Click += (s, e) => DeleteLocation(location);
            btnView.Click += (s, e) => ShowDetail(location);

            pnlActions.Controls.Add(btnView);
            pnlActions.Controls.Add(btnDel);
            pnlActions.Controls.Add(btnEdit);

            headerRow.Controls.Add(lblName);
            headerRow.Controls.Add(pnlActions);

            // badge de categoria colorido
            var flowContent = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };

            var badge = new Label
            {
                Text = location.Category,
                Font = new Font(BuenosAiresTheme.BodyFont.FontFamily, 10f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.GetCategoryTextColor(location.Category),
                BackColor = BuenosAiresTheme.GetCategoryColor(location.Category),
                AutoSize = true,
                Padding = new Padding(8, 2, 8, 2),
                Margin = new Padding(0, 6, 0, 14)
            };

            var lblAddrTitle = new Label
            {
                Text = "Endereço:",
                Font = new Font(BuenosAiresTheme.BodyFont.FontFamily, 10f, FontStyle.Regular),
                ForeColor = BuenosAiresTheme.TextMutedColor,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 2)
            };

            var lblAddrValue = new Label
            {
                Text = location.Address,
                Font = new Font(BuenosAiresTheme.BodyFont.FontFamily, 10f, FontStyle.Regular),
                ForeColor = BuenosAiresTheme.TextColor,
                AutoSize = true,
                MaximumSize = new Size(cardWidth - 40, 0),
                Margin = new Padding(0, 0, 0, 12)
            };

            var lblCoordTitle = new Label
            {
                Text = "Coordenadas:",
                Font = new Font(BuenosAiresTheme.BodyFont.FontFamily, 8f, FontStyle.Regular),
                ForeColor = BuenosAiresTheme.TextMutedColor,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 2)
            };

            var lblCoordValue = new Label
            {
                Text = $"{latText}, {lngText}",
                Font = new Font(BuenosAiresTheme.BodyFont.FontFamily, 8f, FontStyle.Regular),
                ForeColor = BuenosAiresTheme.TextColor,
                AutoSize = true
            };

            flowContent.Controls.AddRange(new Control[]
            {
                badge, lblAddrTitle, lblAddrValue, lblCoordTitle, lblCoordValue
            });

            card.Controls.Add(flowContent);
            card.Controls.Add(headerRow);

            return card;
        }

        private RoundedButton MakeActionButton(string text, Color color, string? iconFileName = null)
        {
            var button = new RoundedButton
            {
                Text = text,
                Width = 38,
                Height = 36,
                Font = new Font(BuenosAiresTheme.BodyFont.FontFamily, 10.5f, FontStyle.Regular),
                FillColor = Color.Transparent,
                ForeColor = color,
                HoverColor = BuenosAiresTheme.PrimaryColorLight,
                BackColor = BuenosAiresTheme.SurfaceColor,
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0)
            };

            if (!string.IsNullOrWhiteSpace(iconFileName))
            {
                var icon = TryLoadButtonIcon(iconFileName);
                if (icon != null)
                {
                    button.Image = icon;
                    button.Text = string.Empty;
                }
            }

            return button;
        }

        private static Image? TryLoadButtonIcon(string fileName)
        {
            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", fileName);
            if (!File.Exists(iconPath))
                return null;

            using var iconStream = File.OpenRead(iconPath);
            using var originalIcon = Image.FromStream(iconStream);
            return new Bitmap(originalIcon, new Size(16, 16));
        }

        private void PositionCategoryFilterPopup()
        {
            if (_btnFiltrar == null || _clbFiltroCategorias == null)
                return;

            var belowButtonScreen = _btnFiltrar.PointToScreen(new Point(0, _btnFiltrar.Height + 2));
            var belowButtonClient = PointToClient(belowButtonScreen);

            _clbFiltroCategorias.Location = belowButtonClient;
            _clbFiltroCategorias.BringToFront();
        }

        private void ToggleCategoryFilter()
        {
            if (!_clbFiltroCategorias.Visible)
            {
                PopulateCategoryFilter();
                _clbFiltroCategorias.Visible = true;
                PositionCategoryFilterPopup();
            }
            else
            {
                _clbFiltroCategorias.Visible = false;
            }
        }

        private void PopulateCategoryFilter()
        {
            if (_allLocations == null || _allLocations.Count == 0)
                return;

            var categorias = _allLocations
                .Select(l => l.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c)
                .ToList();

            var selecionadas = new HashSet<string>(
                _clbFiltroCategorias.CheckedItems.Cast<string>(),
                StringComparer.OrdinalIgnoreCase);

            _clbFiltroCategorias.Items.Clear();
            foreach (var cat in categorias)
            {
                int index = _clbFiltroCategorias.Items.Add(cat);
                if (selecionadas.Contains(cat))
                {
                    _clbFiltroCategorias.SetItemChecked(index, true);
                }
            }
        }

        private void ClbFiltroCategorias_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            BeginInvoke((Action)ApplyFilters);
        }

        private void HandleClickOutsideFilter(object? sender, MouseEventArgs e)
        {
            if (!_clbFiltroCategorias.Visible)
                return;

            if (ReferenceEquals(sender, _btnFiltrar) || ReferenceEquals(sender, _clbFiltroCategorias))
                return;

            var sourceControl = sender as Control ?? this;
            var clickScreenPoint = sourceControl.PointToScreen(e.Location);
            var clbScreenBounds = _clbFiltroCategorias.RectangleToScreen(_clbFiltroCategorias.ClientRectangle);

            if (!clbScreenBounds.Contains(clickScreenPoint))
            {
                _clbFiltroCategorias.Visible = false;
            }
        }

        private void OpenLocationForm(Location? location)
        {
            using var form = new LocationForm(location);
            if (form.ShowDialog() == DialogResult.OK && form.Result != null)
            {
                if (location == null)
                    _locationService.Add(form.Result);
                else
                    _locationService.Update(form.Result);

                LoadLocations();
            }
        }

        private void DeleteLocation(Location location)
        {
            //fix: avisa o usuário se o local estiver em roteiros
            int roteirosCount = _locationService.CountItinerariesUsingLocation(location.Id);

            string message = roteirosCount > 0
                ? $"O local \"{location.Name}\" está em {roteirosCount} roteiro(s).\n\n" +
                  $"Excluí-lo também o removerá desses roteiros.\n\nDeseja continuar?"
                : $"Deseja excluir \"{location.Name}\"?";

            var confirm = MessageBox.Show(
                message,
                "Confirmar exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                _locationService.Delete(location.Id);
                LoadLocations();
            }
        }

        private void ShowDetail(Location location)
        {
            using var form = new LocationDetailForm(location);
            form.ShowDialog();
        }
    }
}