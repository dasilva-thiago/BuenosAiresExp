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
        private TextBox txtBuscar;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private Label lblDetailNome;
        private Label lblDetailCategoria;
        private Label lblDetailCoordenadas;
        private Label lblDetailNotas;
        private Label lblStatus;

        private Image _backgroundImage;

        public MainForm()
        {
            _locationService = new LocationService();
            InitializeComponent();
            BuildLayout();
            ApplyTheme(); // 270
            LoadLocations();
        }

        private void BuildLayout()
        {
            Text = "Buenos Aires Explorer";
            Size = new Size(1024, 640);
            MinimumSize = new Size(800, 500);
            StartPosition = FormStartPosition.CenterScreen;

            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = BuenosAiresTheme.HeaderHeight,
                Padding = new Padding(20, 0, 20, 0),
                BackColor = BuenosAiresTheme.HeaderColor
            };

            lblTitulo = new Label
            {
                Text = "Buenos Aires Explorer",
                AutoSize = true,
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = BuenosAiresTheme.AccentColor,
                Location = new Point(20, 12)
            };

            lblSubtitulo = new Label
            {
                Text = "Seus lugares favoritos da cidade",
                AutoSize = true,
                Font = BuenosAiresTheme.SubtitleFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
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
                Width = 120,
                Font = BuenosAiresTheme.ButtonFont,
                // Botao principal preenchido com a cor primaria
                FillColor = BuenosAiresTheme.PrimaryColor,
                Location = new Point(16, 10)
            };

            btnEditar = new RoundedButton
            {
                Text = "Editar",
                Width = 90,
                FillColor = Color.Transparent,
                ForeColor = BuenosAiresTheme.PrimaryColor,
                HoverColor = BuenosAiresTheme.PrimaryColorLight,
                Location = new Point(148, 10)
            };

            btnExcluir = new RoundedButton
            {
                Text = "Excluir",
                Width = 90,
                FillColor = Color.Transparent,
                ForeColor = BuenosAiresTheme.DangerColor,
                HoverColor = Color.FromArgb(255, 240, 240),
                Location = new Point(250, 10)
            };

            txtBuscar = new TextBox
            {
                PlaceholderText = "Buscar por local...",
                Width = 200,
                Height = 28,
                Font = BuenosAiresTheme.BodyFont,
                BorderStyle = BorderStyle.FixedSingle,
            };

            txtBuscar.Location = new Point(
                pnlToolbar.Width - txtBuscar.Width - 16,
                (pnlToolbar.Height - txtBuscar.Height) / 2
            );

            pnlToolbar.Resize += (s, e) =>
            {
                txtBuscar.Location = new Point(
                    pnlToolbar.Width - txtBuscar.Width - 16,
                    (pnlToolbar.Height - txtBuscar.Height) / 2
                );
            };

            pnlToolbar.Controls.AddRange(new Control[]
            {
                btnNovoLocal,
                btnEditar,
                btnExcluir,
                txtBuscar
            });



            //dgv de locais - colunas: nome, categoria, latitude, longitude, notas

            dgvLocais = new DataGridView
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(16)
            };
            BuenosAiresTheme.ApplyDataGridView(dgvLocais);

            dgvLocais.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colName",
                HeaderText = "Nome",
                DataPropertyName = "Name",
                FillWeight = 35
            });
            dgvLocais.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCategoria",
                HeaderText = "Categoria",
                DataPropertyName = "Category",
                FillWeight = 20
            });
            dgvLocais.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colLatitude",
                HeaderText = "Latitude",
                DataPropertyName = "Latitude",
                FillWeight = 15
            });
            dgvLocais.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colLongitude",
                HeaderText = "Longitude",
                DataPropertyName = "Longitude",
                FillWeight = 15
            });
            dgvLocais.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNotas",
                HeaderText = "Notas",
                DataPropertyName = "Notes",
                FillWeight = 15
            });

            // painel de detalhes 

            pnlDetail = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = BuenosAiresTheme.HeaderHeight,
                BackColor = BuenosAiresTheme.HeaderColor,
                Padding = new Padding(20, 12, 20, 12),

            };

            lblDetailNome = new Label
            {
                Text = "Selecione um local",
                AutoSize = true,
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = BuenosAiresTheme.TextColor,
                Location = new Point(20, 12)
            };

            lblDetailCategoria = new Label
            {
                Text = "",
                AutoSize = true,
                Font = BuenosAiresTheme.SubtitleFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                Location = new Point(20, 38)
            };

            lblDetailCoordenadas = new Label
            {
                Text = "",
                AutoSize = true,
                Font = BuenosAiresTheme.FontMono,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                Location = new Point(20, 58)
            };

            lblDetailNotas = new Label
            {
                Text = "",
                AutoSize = true,
                Font = BuenosAiresTheme.BodyFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                Location = new Point(20, 76)
            };

            lblStatus = new Label
            {
                Text = "",
                Dock = DockStyle.Bottom,
                AutoSize = false,
                Height = 25,
                Font = BuenosAiresTheme.MutedFont,
                ForeColor = BuenosAiresTheme.AccentColor,
                BackColor = BuenosAiresTheme.HeaderColor,      
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };

            pnlDetail.Controls.AddRange(new Control[]
            {
                lblDetailNome,
                lblDetailCategoria,
                lblDetailCoordenadas,
                lblDetailNotas
             });

            // dock - montar os paineis

            Controls.Add(dgvLocais);
            Controls.Add(pnlDetail);
            Controls.Add(pnlToolbar);
            Controls.Add(pnlHeader);
            Controls.Add(lblStatus);
        }


        // chama applytheme
            private void ApplyTheme()
            {
            BuenosAiresTheme.ApplyForm(this);
            }

        // dados - loadlocations, search, select, add, edit, delete, update status

        private void LoadLocations(string filter = "")
        {
            _allLocations = _locationService.GetAll();

            var filtered = string.IsNullOrWhiteSpace(filter)
                ? _allLocations
                : _allLocations.Where(l =>
                    l.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    l.Category.Contains(filter, StringComparison.OrdinalIgnoreCase)
                  ).ToList();

            dgvLocais.DataSource = null;
            dgvLocais.DataSource = filtered;

            UpdateStatus();
        }

        private void UpdateStatus()
        {
            int total = _allLocations?.Count ?? 0;
            lblStatus.Text = $"  {total} loca{(total != 1 ? "is" : "")} cadastrado{(total != 1 ? "s" : "")}";
        }

        private void UpdateDetailPanel(Location location)
        {
            if (location == null)
            {
                lblDetailNome.Text = "Selecione um local";
                lblDetailCategoria.Text = string.Empty;
                lblDetailCoordenadas.Text = string.Empty;
                lblDetailNotas.Text = string.Empty;
                return;
            }

            lblDetailNome.Text = location.Name;
            lblDetailCategoria.Text = location.Category;
            lblDetailCoordenadas.Text = $"lat {location.Latitude:F4}   lng {location.Longitude:F4}";
            lblDetailNotas.Text = location.Notes ?? string.Empty;
        }


        // eventos - click, text changed, selection changed 

        /*
        private void WireEvents()
        {
            btnNovoLocal.Click += (s, e) => OpenLocationForm(null);
            btnEditar.Click += (s, e) => OpenLocationForm(GetSelectedLocation());
            btnExcluir.Click += (s, e) => DeleteSelectedLocation();

            txtBuscar.TextChanged += (s, e) => LoadLocations(txtBuscar.Text);

            dgvLocais.SelectionChanged += (s, e) => UpdateDetailPanel(GetSelectedLocation());
        }

        private Location GetSelectedLocation()
        {
            if (dgvLocais.SelectedRows.Count == 0) return null;
            return dgvLocais.SelectedRows[0].DataBoundItem as Location;
        }

        
        private void OpenLocationForm(Location location)
        {
            using var form = new LocationForm(location);
            if (form.ShowDialog() == DialogResult.OK)
                LoadLocations();
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
        */

    }
}