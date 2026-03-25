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

        // dependencies

        private readonly LocationService _locationService;
        private List<Location> _allLocations;

        // screen controls 

        private Panel pnlHeader;
        private Panel pnlToolbar;
        private Panel pnlDetail;
        private DataGridView dgvLocais;
        private RoundedButton btnNovoLocal;
        private RoundedButton btnEditar;
        private RoundedButton btnExcluir;
        private RoundedButton btnRoteiro;
        private RoundedButton btnFiltrar;
        private CheckedListBox clbFiltroCategorias;
        private RoundedTextBox txtBuscar;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private Label lblDetailNome;
        private Label lblDetailCategoria;
        private Label lblDetailEndereco;
        private Label lblDetailCoordenadas;
        private Label lblDetailNotas;
        private Label lblStatus;

        public MainForm()
        {
            _locationService = new LocationService();
            BuildLayout();
            WireEvents();
            LoadLocations();
        }

        private void BuildLayout()
        {
            BuenosAiresTheme.ApplyForm(this);
            Text = "Buenos Aires Explorer";
            Size = new Size(1024, 640);
            MinimumSize = new Size(800, 500);
            StartPosition = FormStartPosition.CenterScreen;

            pnlHeader = new Panel       
            {
                Dock = DockStyle.Top,
                Height = BuenosAiresTheme.HeaderHeight,
                Padding = new Padding(20, 0, 20, 0),
                BackColor = BuenosAiresTheme.PrimaryColor
            };

            lblTitulo = new Label
            {
                Text = "Buenos Aires Explorer",
                AutoSize = true,
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = Color.White,
                Location = new Point(20, 12)
            };

            lblSubtitulo = new Label
            {
                Text = "Seus lugares favoritos da cidade",
                AutoSize = true,
                Font = BuenosAiresTheme.SubtitleFont,
                ForeColor = Color.White,
                Location = new Point(22, 38)
            };

            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Controls.Add(lblSubtitulo);

            pnlToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = BuenosAiresTheme.PrimaryColorLight,
                Padding = new Padding(16, 0, 16, 0)
            };

            btnNovoLocal = new RoundedButton
            {
                Text = "+ Novo Local",
                Width = 100,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = BuenosAiresTheme.PrimaryColor,
                HoverColor = BuenosAiresTheme.PrimaryColorMuted,
                Location = new Point(16, 10)
            };

            btnEditar = new RoundedButton
            {
                Text = "Editar",
                Width = 70,
                FillColor = Color.Transparent,
                ForeColor = BuenosAiresTheme.PrimaryColor,
                HoverColor = BuenosAiresTheme.PrimaryColorLight,
                Location = new Point(128, 10)
            };

            btnExcluir = new RoundedButton
            {
                Text = "Excluir",
                Width = 70,
                FillColor = Color.Transparent,
                ForeColor = BuenosAiresTheme.DangerColor,
                HoverColor = Color.FromArgb(255, 240, 240),
                Location = new Point(210, 10)
            };

            btnRoteiro = new RoundedButton
            {
                Text = "Novo Roteiro",
                Width = 100,
                ForeColor = BuenosAiresTheme.HeaderColor,
                FillColor = BuenosAiresTheme.AccentColor,
                HoverColor = BuenosAiresTheme.AccentColorMuted,
                Location = new Point(292, 10)
            };

            btnFiltrar = new RoundedButton
            {
                Text = "",
                Width = 30,
                Height = 28,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = BuenosAiresTheme.PrimaryColor,
                HoverColor = BuenosAiresTheme.PrimaryColorMuted,
                Location = new Point(552, 10)
            };
            // carrega o icone de filtro se o arquivo existir; evita excecao em tempo de execucao
            var filterIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "filter_icon.png");
            if (File.Exists(filterIconPath))
            {
                btnFiltrar.Image = Image.FromFile(filterIconPath);
            } else { btnFiltrar.Text = "F";}

            clbFiltroCategorias = new CheckedListBox
            {
                Visible = false,
                CheckOnClick = true,
                IntegralHeight = false,
                BorderStyle = BorderStyle.FixedSingle,
                Width = 200,
                Height = 140
            };

            txtBuscar = new RoundedTextBox
            {
                Placeholder = "Buscar por local...",
                Width = 320,
                Height = 28,
                Font = BuenosAiresTheme.BodyFont,
                BorderStyle = BorderStyle.FixedSingle,
            };

            const int rightMargin = 16;
            const int spacing = 10;

            txtBuscar.Location = new Point(
                pnlToolbar.Width - txtBuscar.Width - rightMargin,
                (pnlToolbar.Height - txtBuscar.Height) / 2
            );

            btnFiltrar.Location = new Point(
                txtBuscar.Left - spacing - btnFiltrar.Width,
                (pnlToolbar.Height - btnFiltrar.Height) / 2
            );
            PositionCategoryFilterPopup();

            pnlToolbar.Resize += (s, e) =>
            {
                txtBuscar.Location = new Point(
                    pnlToolbar.Width - txtBuscar.Width - rightMargin,
                    (pnlToolbar.Height - txtBuscar.Height) / 2
                );

                btnFiltrar.Location = new Point(
                    txtBuscar.Left - spacing - btnFiltrar.Width,
                    (pnlToolbar.Height - btnFiltrar.Height) / 2
                );
                PositionCategoryFilterPopup();
            };

            pnlToolbar.Controls.AddRange(new Control[]
            {
                btnNovoLocal,
                btnEditar,
                btnExcluir,
                btnRoteiro,
                btnFiltrar,
                txtBuscar
            });

            Controls.Add(clbFiltroCategorias);
            clbFiltroCategorias.BringToFront();

            //dgv de locais - colunas: nome, categoria, latitude, longitude, notas
            dgvLocais = new DataGridView
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(16),
                AutoGenerateColumns = false
            };
            BuenosAiresTheme.ApplyDataGridView(dgvLocais);

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

            dgvLocais.Columns.AddRange(colName, colCategoria, colEndereco, colLatitude, colLongitude, colNotas);

            // painel de detalhes 

            pnlDetail = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = BuenosAiresTheme.HeaderHeight + 10,
                Padding = new Padding(20, 12, 20, 12),
                BackColor = BuenosAiresTheme.PrimaryColor

            };

            lblDetailNome = new Label
            {
                Text = "Selecione um local",
                AutoSize = true,
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = Color.White,
                Location = new Point(20, 12)
            };

            lblDetailCategoria = new Label
            {
                Text = "",
                AutoSize = true,
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = Color.White,
                Location = new Point(20, 12)
            };

            lblDetailEndereco = new Label
            {
                Text = "",
                AutoSize = true,
                Font = BuenosAiresTheme.BodyFont,
                ForeColor = Color.White,
                Location = new Point(22, 36)
            };

            lblDetailNotas = new Label
            {
                Text = "",
                AutoSize = false,
                Font = BuenosAiresTheme.BodyFont,
                ForeColor = BuenosAiresTheme.FillColor,
                Location = new Point(22, 52)

            };

            lblStatus = new Label
            {
                Text = "",
                Dock = DockStyle.Bottom,
                AutoSize = false,
                Height = 25,
                Font = BuenosAiresTheme.SubtitleFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 20, 0)

            };

            pnlDetail.Controls.AddRange(new Control[]
            {
                lblDetailNome,
                lblDetailCategoria,
                lblDetailEndereco,
                lblDetailNotas,
                lblStatus
             });

            Controls.Add(dgvLocais);
            Controls.Add(pnlDetail);
            Controls.Add(pnlToolbar);
            Controls.Add(pnlHeader);

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
            lblStatus.Text = $"{total} loca{(total != 1 ? "is" : "")} cadastrado{(total != 1 ? "s" : "")}";
        }

        private void PositionCategoryFilterPopup()
        {
            if (btnFiltrar == null || clbFiltroCategorias == null)
                return;

            var belowButtonScreen = btnFiltrar.PointToScreen(new Point(0, btnFiltrar.Height + 2));
            var belowButtonClient = PointToClient(belowButtonScreen);

            clbFiltroCategorias.Location = belowButtonClient;
            clbFiltroCategorias.BringToFront();
        }

        private void ApplyFilters()
        {
            if (_allLocations == null) return;

            var search = txtBuscar?.Value ?? string.Empty;

            var categoriasSelecionadas = clbFiltroCategorias.CheckedItems
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

            dgvLocais.DataSource = null;
            dgvLocais.DataSource = filtered;

            UpdateStatus();
        }

        private void UpdateDetailPanel(Location location)
        {
            if (location == null)
            {
                lblDetailNome.Text = "Selecione um local";
                lblDetailCategoria.Text = string.Empty;
                lblDetailEndereco.Text = string.Empty;
                lblDetailNotas.Text = string.Empty;
                UpdateNotasEllipsis();
                return;
            }

            lblDetailNome.Text = $"{location.Name} - {location.Category}";
            lblDetailEndereco.Text = location.Address;
            lblDetailNotas.Text = location.Notes ?? string.Empty;

            UpdateNotasEllipsis();
        }
        private void UpdateNotasEllipsis()
        {
            if (lblDetailNotas == null || pnlDetail == null)
                return;

            int maxWidth = Math.Max(pnlDetail.ClientSize.Width / 2, 50);

            lblDetailNotas.MaximumSize = new Size(maxWidth, 0);
            lblDetailNotas.AutoSize = true;
        }


        // eventos - click, text changed, selection changed 
        private void WireEvents()
        {
            btnNovoLocal.Click += (s, e) => OpenLocationForm(null);
            btnEditar.Click += (s, e) => OpenLocationForm(GetSelectedLocation());
            btnExcluir.Click += (s, e) => DeleteSelectedLocation();
            btnRoteiro.Click += (s, e) => OpenItineraryForm();
            btnFiltrar.Click += (s, e) => ToggleCategoryFilter();

            txtBuscar.TextChanged += (s, e) => ApplyFilters();

            dgvLocais.SelectionChanged += (s, e) => UpdateDetailPanel(GetSelectedLocation());
            dgvLocais.CellDoubleClick += (s, e) =>

            {
                var selected = GetSelectedLocation();
                if (selected != null)
                {
                    ShowLocationDetail(selected);
                }
            };

            // fecha o popup de categorias ao clicar fora dele
            this.MouseDown += HandleClickOutsideFilter;
            pnlToolbar.MouseDown += HandleClickOutsideFilter;
            pnlHeader.MouseDown += HandleClickOutsideFilter;
            pnlDetail.MouseDown += HandleClickOutsideFilter;
            dgvLocais.MouseDown += HandleClickOutsideFilter;

            this.Resize += (s, e) => UpdateNotasEllipsis();
        }

        private void ToggleCategoryFilter()
        {
            if (!clbFiltroCategorias.Visible)
            {
                PopulateCategoryFilter();
                clbFiltroCategorias.Visible = true;
                PositionCategoryFilterPopup();

                clbFiltroCategorias.ItemCheck -= ClbFiltroCategorias_ItemCheck;
                clbFiltroCategorias.ItemCheck += ClbFiltroCategorias_ItemCheck;
            }
            else
            {
                clbFiltroCategorias.Visible = false;
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
                clbFiltroCategorias.CheckedItems.Cast<string>(),
                StringComparer.OrdinalIgnoreCase);

            clbFiltroCategorias.Items.Clear();
            foreach (var cat in categorias)
            {
                int index = clbFiltroCategorias.Items.Add(cat);
                if (selecionadas.Contains(cat))
                {
                    clbFiltroCategorias.SetItemChecked(index, true);
                }
            }
        }

        private void HandleClickOutsideFilter(object? sender, MouseEventArgs e)
        {
            if (!clbFiltroCategorias.Visible)
                return;

            // nao fecha quando o clique eh no proprio botao de filtro ou dentro da lista
            if (ReferenceEquals(sender, btnFiltrar) || ReferenceEquals(sender, clbFiltroCategorias))
                return;

            var sourceControl = sender as Control ?? this;
            var clickScreenPoint = sourceControl.PointToScreen(e.Location);
            var clbScreenBounds = clbFiltroCategorias.RectangleToScreen(clbFiltroCategorias.ClientRectangle);

            if (!clbScreenBounds.Contains(clickScreenPoint))
            {
                clbFiltroCategorias.Visible = false;
            }
        }

        private Location GetSelectedLocation()
        {
            if (dgvLocais.SelectedRows.Count == 0) return null;
            return dgvLocais.SelectedRows[0].DataBoundItem as Location;
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