using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using BuenosAiresExp.Models;
using BuenosAiresExp.Services;
using BuenosAiresExp.UI;
using System.Drawing.Imaging;
using System.IO;

namespace BuenosAiresExp
{
    public partial class MainForm : Form
    {
        private readonly LocationService _locationService;
        private List<Location> _allLocations;

        private Panel _pnlHeader;
        private Panel _pnlToolbar;
        private Panel _pnlDetail;
        private DataGridView _dgvLocais;
        private RoundedButton _btnNovoLocal;
        private RoundedButton _btnEditar;
        private RoundedButton _btnExcluir;
        private RoundedButton _btnRoteiro;
        private RoundedButton _btnFiltrar;
        private CheckedListBox _clbFiltroCategorias;
        private RoundedTextBox _txtBuscar;
        private Label _lblTitulo;
        private Label _lblSubtitulo;
        private Label _lblDetailNome;
        private Label _lblDetailCategoria;
        private Label _lblDetailEndereco;
        private Label _lblDetailCoordenadas;
        private Label _lblDetailNotas;
        private Label _lblStatus;

        public MainForm()
        {
            _locationService = new LocationService();
            BuildLayout();
            Shown += MainForm_Shown;
            WireEvents();
            LoadLocations();
        }

        private void MainForm_Shown(object? sender, EventArgs e)
        {
            BeginInvoke(new Action(() => ActiveControl = null));
        }

        private void BuildLayout()
        {
            BuenosAiresTheme.ApplyForm(this);
            Text = "Buenos Aires Explorer";
            Size = new Size(1224, 840);
            MinimumSize = new Size(800, 500);
            StartPosition = FormStartPosition.CenterScreen;

            _pnlHeader = new Panel       
            {
                Dock = DockStyle.Top,
                Height = BuenosAiresTheme.HeaderHeight,
                Padding = new Padding(20, 0, 20, 0),
                BackColor = BuenosAiresTheme.PrimaryColor
            };

            _lblTitulo = new Label
            {
                Text = "Buenos Aires Explorer",
                AutoSize = true,
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = Color.White,
                Location = new Point(20, 12)
            };

            _lblSubtitulo = new Label
            {
                Text = "Seus lugares favoritos da cidade",
                AutoSize = true,
                Font = BuenosAiresTheme.SubtitleFont,
                ForeColor = Color.White,
                Location = new Point(22, 38)
            };

            _pnlHeader.Controls.Add(_lblTitulo);
            _pnlHeader.Controls.Add(_lblSubtitulo);

            _pnlToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = BuenosAiresTheme.PrimaryColorLight,
                Padding = new Padding(16, 0, 16, 0)
            };

            _btnNovoLocal = new RoundedButton
            {
                Text = "+ Novo Local",
                Width = 100,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = BuenosAiresTheme.PrimaryColor,
                HoverColor = BuenosAiresTheme.PrimaryColorMuted,
                Location = new Point(16, 10)
            };

            _btnEditar = new RoundedButton
            {
                Text = "Editar",
                Width = 70,
                FillColor = Color.Transparent,
                ForeColor = BuenosAiresTheme.PrimaryColor,
                HoverColor = BuenosAiresTheme.PrimaryColorLight,
                Location = new Point(128, 10)
            };

            _btnExcluir = new RoundedButton
            {
                Text = "Excluir",
                Width = 70,
                FillColor = Color.Transparent,
                ForeColor = BuenosAiresTheme.DangerColor,
                HoverColor = Color.FromArgb(255, 240, 240),
                Location = new Point(210, 10)
            };

            _btnRoteiro = new RoundedButton
            {
                Text = "Novo Roteiro",
                Width = 100,
                ForeColor = BuenosAiresTheme.HeaderColor,
                FillColor = BuenosAiresTheme.AccentColor,
                HoverColor = BuenosAiresTheme.AccentColorMuted,
                Location = new Point(292, 10)
            };

            _btnFiltrar = new RoundedButton
            {
                Text = "",
                Width = 30,
                Height = 28,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = BuenosAiresTheme.PrimaryColor,
                HoverColor = BuenosAiresTheme.PrimaryColorMuted,
                Location = new Point(552, 10)
            };
            var filterIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "filter_icon.png");
            if (File.Exists(filterIconPath))
            {
                _btnFiltrar.Image = Image.FromFile(filterIconPath);
            } 
            else { 
                _btnFiltrar.Text = "\\uE71C";
                Font = new Font("Segoe MDL2 Assets", 12f, FontStyle.Regular);
            }

            _clbFiltroCategorias = new CheckedListBox
            {
                Visible = false,
                CheckOnClick = true,
                IntegralHeight = false,
                BorderStyle = BorderStyle.FixedSingle,
                Width = 200,
                Height = 140
            };

            _txtBuscar = new RoundedTextBox
            {
                Placeholder = "Buscar por local...",
                Width = 320,
                Height = 28,
                Font = BuenosAiresTheme.BodyFont,
                BorderStyle = BorderStyle.FixedSingle,
            };

            const int rightMargin = 16;
            const int spacing = 10;

            _txtBuscar.Location = new Point(
                _pnlToolbar.Width - _txtBuscar.Width - rightMargin,
                (_pnlToolbar.Height - _txtBuscar.Height) / 2
            );

            _btnFiltrar.Location = new Point(
                _txtBuscar.Left - spacing - _btnFiltrar.Width,
                (_pnlToolbar.Height - _btnFiltrar.Height) / 2
            );
            PositionCategoryFilterPopup();

            _pnlToolbar.Resize += (s, e) =>
            {
                _txtBuscar.Location = new Point(
                    _pnlToolbar.Width - _txtBuscar.Width - rightMargin,
                    (_pnlToolbar.Height - _txtBuscar.Height) / 2
                );

                _btnFiltrar.Location = new Point(
                    _txtBuscar.Left - spacing - _btnFiltrar.Width,
                    (_pnlToolbar.Height - _btnFiltrar.Height) / 2
                );
                PositionCategoryFilterPopup();
            };

            _pnlToolbar.Controls.AddRange(new Control[]
            {
                _btnNovoLocal,
                _btnEditar,
                _btnExcluir,
                _btnRoteiro,
                _btnFiltrar,
                _txtBuscar
            });

            Controls.Add(_clbFiltroCategorias);
            _clbFiltroCategorias.BringToFront();

            //dgv de locais - colunas: nome, categoria, latitude, longitude, notas
            _dgvLocais = new DataGridView
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(16),
                AutoGenerateColumns = false
            };
            BuenosAiresTheme.ApplyDataGridView(_dgvLocais);

            var colName = new DataGridViewTextBoxColumn
            {
                Name = "colName",
                HeaderText = "Nome",
                DataPropertyName = "Name",
                FillWeight = 35
            };

            var colCategoria = new DataGridViewTextBoxColumn
            {
                Name = "colCategoria",
                HeaderText = "Categoria",
                DataPropertyName = "Category",
                FillWeight = 20
            };

            var colEndereco = new DataGridViewTextBoxColumn
            {
                Name = "colEndereco",
                HeaderText = "Endereço",
                DataPropertyName = "Address",
                FillWeight = 25
            };

            var colLatitude = new DataGridViewTextBoxColumn
            {
                Name = "colLatitude",
                HeaderText = "Latitude",
                DataPropertyName = "Latitude",
                FillWeight = 12,  
            };

            var colLongitude = new DataGridViewTextBoxColumn
            {
                Name = "colLongitude",
                HeaderText = "Longitude",
                DataPropertyName = "Longitude",
                FillWeight = 12,
                
            };

            var colNotas = new DataGridViewTextBoxColumn
            {
                Name = "colNotas",
                HeaderText = "Notas",
                DataPropertyName = "Notes",
                FillWeight = 21
            };

            _dgvLocais.Columns.AddRange(colName, colCategoria, colEndereco, colLatitude, colLongitude, colNotas);

            // painel de detalhes 

            _pnlDetail = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = BuenosAiresTheme.HeaderHeight + 10,
                Padding = new Padding(20, 12, 20, 12),
                BackColor = BuenosAiresTheme.PrimaryColor

            };

            _lblDetailNome = new Label
            {
                Text = "Selecione um local",
                AutoSize = true,
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = Color.White,
                Location = new Point(20, 12)
            };

            _lblDetailCategoria = new Label
            {
                Text = "",
                AutoSize = true,
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = Color.White,
                Location = new Point(20, 12)
            };

            _lblDetailEndereco = new Label
            {
                Text = "",
                AutoSize = true,
                Font = BuenosAiresTheme.BodyFont,
                ForeColor = Color.White,
                Location = new Point(22, 36)
            };

            _lblDetailNotas = new Label
            {
                Text = "",
                AutoSize = false,
                Font = BuenosAiresTheme.BodyFont,
                ForeColor = BuenosAiresTheme.AccentColor,
                Location = new Point(22, 52)

            };

            _lblStatus = new Label
            {
                Text = "",
                Dock = DockStyle.Bottom,
                AutoSize = false,
                Height = 25,
                Font = BuenosAiresTheme.SubtitleFont,
                ForeColor = BuenosAiresTheme.AccentColor,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 20, 0)

            };

            _pnlDetail.Controls.AddRange(new Control[]
            {
                _lblDetailNome,
                _lblDetailCategoria,
                _lblDetailEndereco,
                _lblDetailNotas,
                _lblStatus
             });

            Controls.Add(_dgvLocais);
            Controls.Add(_pnlDetail);
            Controls.Add(_pnlToolbar);
            Controls.Add(_pnlHeader);

            UpdateNotasEllipsis();
        }
        // dados - loadlocations, search, select, add, edit, delete, update status

        private void LoadLocations()
        {
            _allLocations = _locationService.GetAll();
            ApplyFilters();
        }

        private void UpdateStatus()
        {
            int total = _allLocations?.Count ?? 0;
            _lblStatus.Text = $"{total} loca{(total != 1 ? "is" : "")} cadastrado{(total != 1 ? "s" : "")}";
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

        private void ApplyFilters()
        {
            if (_allLocations == null) return;

            var search = _txtBuscar?.Value ?? string.Empty;

            var categoriasSelecionadas = _clbFiltroCategorias.CheckedItems
                .Cast<string>()
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            IEnumerable<Location> query = _allLocations;

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(l =>
                    l.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    l.Category.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (categoriasSelecionadas.Count > 0)
            {
                query = query.Where(l => categoriasSelecionadas.Contains(l.Category));
            }

            var filtered = query.OrderBy(l => l.Name).ToList();

            _dgvLocais.DataSource = null;
            _dgvLocais.DataSource = filtered;

            UpdateStatus();
        }

        private void UpdateDetailPanel(Location location)
        {
            if (location == null)
            {
                _lblDetailNome.Text = "Selecione um local";
                _lblDetailCategoria.Text = string.Empty;
                _lblDetailEndereco.Text = string.Empty;
                _lblDetailNotas.Text = string.Empty;
                UpdateNotasEllipsis();
                return;
            }

            _lblDetailNome.Text = $"{location.Name} - {location.Category}";
            _lblDetailEndereco.Text = location.Address;
            _lblDetailNotas.Text = location.Notes ?? string.Empty;

            UpdateNotasEllipsis();
        }
        private void UpdateNotasEllipsis()
        {
            if (_lblDetailNotas == null || _pnlDetail == null)
                return;

            int maxWidth = Math.Max(_pnlDetail.ClientSize.Width / 2, 50);

            _lblDetailNotas.MaximumSize = new Size(maxWidth, 0);
            _lblDetailNotas.AutoSize = true;
        }


        // eventos - click, text changed, selection changed 
        private void WireEvents()
        {
            _btnNovoLocal.Click += (s, e) => OpenLocationForm(null);
            _btnEditar.Click += (s, e) => OpenLocationForm(GetSelectedLocation());
            _btnExcluir.Click += (s, e) => DeleteSelectedLocation();
            _btnRoteiro.Click += (s, e) => OpenItineraryForm();
            _btnFiltrar.Click += (s, e) => ToggleCategoryFilter();

            _txtBuscar.TextChanged += (s, e) => ApplyFilters();

            _dgvLocais.SelectionChanged += (s, e) => UpdateDetailPanel(GetSelectedLocation());
            _dgvLocais.CellDoubleClick += (s, e) =>

            {
                var selected = GetSelectedLocation();
                if (selected != null)
                {
                    ShowLocationDetail(selected);
                }
            };

            // fecha o popup de categorias ao clicar fora dele
            this.MouseDown += HandleClickOutsideFilter;
            _pnlToolbar.MouseDown += HandleClickOutsideFilter;
            _pnlHeader.MouseDown += HandleClickOutsideFilter;
            _pnlDetail.MouseDown += HandleClickOutsideFilter;
            _dgvLocais.MouseDown += HandleClickOutsideFilter;

            this.Resize += (s, e) => UpdateNotasEllipsis();
        }

        private void ToggleCategoryFilter()
        {
            if (!_clbFiltroCategorias.Visible)
            {
                PopulateCategoryFilter();
                _clbFiltroCategorias.Visible = true;
                PositionCategoryFilterPopup();

                _clbFiltroCategorias.ItemCheck -= ClbFiltroCategorias_ItemCheck;
                _clbFiltroCategorias.ItemCheck += ClbFiltroCategorias_ItemCheck;
            }
            else
            {
                _clbFiltroCategorias.Visible = false;
            }
        }

        private void ClbFiltroCategorias_ItemCheck(object? sender, ItemCheckEventArgs e)
        {

            BeginInvoke((Action)ApplyFilters);
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

        private void HandleClickOutsideFilter(object? sender, MouseEventArgs e)
        {
            if (!_clbFiltroCategorias.Visible)
                return;

            // nao fecha quando o clique eh no proprio botao de filtro ou dentro da lista
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

        private Location GetSelectedLocation()
        {
            if (_dgvLocais.SelectedRows.Count == 0) return null;
            return _dgvLocais.SelectedRows[0].DataBoundItem as Location;
        }


        private void OpenLocationForm(Location? location)
        {

            using (var form = new LocationForm(location))
            {

                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    if (location == null)
                        _locationService.Add(form.Result);
                    else
                        _locationService.Update(form.Result);

                    LoadLocations();
                }
            }
        }

        private void OpenItineraryForm()
        {
            // garante que a lista de locais esteja carregada antes de abrir o roteiro
            if (_allLocations == null)
            {
                LoadLocations();
            }

            using (var form = new ItineraryForm(_allLocations))
            {
                form.ShowDialog(this);
            }
        }
        
        
        private void ShowLocationDetail(Location location)
        {
            // abre um form somente leitura com os detalhes completos do local selecionado
            using (var detailForm = new LocationDetailForm(location))
            {
                detailForm.ShowDialog(this);
            }
        }
        private void DeleteSelectedLocation()
        {
            var location = GetSelectedLocation();
            if (location == null) return;

            var confirm = MessageBox.Show(
                $"Deseja excluir \"{location.Name}\"?",
                "Confirmar exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm == DialogResult.Yes)
            {
                _locationService.Delete(location.Id);
                LoadLocations();
            }
        }
        

    }
}