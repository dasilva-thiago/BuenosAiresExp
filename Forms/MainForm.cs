using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BuenosAiresExp.Models;
using BuenosAiresExp.Services;
using BuenosAiresExp.UI;

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


        public MainForm()
        {
            _locationService = new LocationService();
            InitializeComponent();
            BuildLayout();
            //ApplyTheme();
            //LoadLocations();

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
                Height = 52,
                BackColor = BuenosAiresTheme.PrimaryColorLight,
                Padding = new Padding(16, 0, 16, 0)
            };

            btnNovoLocal = new RoundedButton
            {
                Text = "+ Novo Local",
                Width = 120,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = BuenosAiresTheme.PrimaryColorLight,
                Location = new Point(16, 10)
            };

            btnEditar = new RoundedButton
            {
                Text = "Editar",
                Width = 90,
                FillColor = Color.Transparent
            };
        }
    }
}