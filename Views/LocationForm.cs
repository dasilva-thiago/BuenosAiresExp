using BuenosAiresExp.Models;
using BuenosAiresExp.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuenosAiresExp.Services;

namespace BuenosAiresExp
{
    public partial class LocationForm : Form
    {
        public Location? Result { get; private set; }

        private readonly Location? _editingLocation;
        private readonly LocationService _locationService;

        private RoundedTextBox _txtName;
        private RoundedComboBox _cmbCategory;
        private RoundedTextBox _txtAddress;
        private RoundedTextBox _txtLatitude;
        private RoundedTextBox _txtLongitude;
        private RoundedTextBox _txtNotes;
        private RoundedButton _btnSalvar;
        private RoundedButton _btnCancelar;
        private RoundedButton _btnBuscarCoordenadas;
        private RoundedButton _btnBuscarAutomatico;
        private Label _lblErro;

        private TableLayoutPanel _headerLayout;
        private Panel _pnlHeader;
        private Label _lblTitulo;
        private Label _lblSubtitulo;
        private Label _lblIcone;
        private Label _lblNome;
        private Label _lblEndereco;
        private Label _lblLatitude;
        private Label _lblLongitude;
        private Label _lblNotas;
        private Label _lblCategoria;

        private static readonly string[] DefaultCategories =
        { "Bairro", "Bar", "Biblioteca", "Café", "Cemitério", "Centro Cultural", "Estádio", "Feira", "Financeiro", "Hospedagem", "Igreja", "Livraria", "Mercado", "Milonga", "Mirante", "Monumento", "Museu", "Parque", "Parrilla", "Pizzaria",
            "Ponto Turístico", "Reserva Natural", "Restaurante", "Rua", "Saúde", "Shopping", "Sorveteria", "Supermercado", "Teatro", "Transporte", "Vida Noturna", "Outro"
        };

        public LocationForm() : this(null) { }

        public LocationForm(Location? location)
        {
            _editingLocation = location;
            _locationService = new LocationService();
            InitializeComponent();
            BuildLayout();
            Shown += LocationForm_Shown;


            if (_editingLocation != null)
                PopulateFields(_editingLocation);
        }

        private void BuildLayout()
        {
            BuenosAiresTheme.ApplyForm(this);
            Text = _editingLocation == null ? "Adicionar Local" : "Editar Local";

            Size = new Size(860, 700);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

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
                BackColor = BuenosAiresTheme.PrimaryColor
            };

            int pad = BuenosAiresExp.UI.BuenosAiresTheme.PaddingForm;
            int y = _pnlHeader.Height + pad;
            int widthField = ClientSize.Width - pad * 2;

            _lblTitulo = new Label
            {
                Text = Text,
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = Color.White,
                AutoSize = true,
                Margin = new Padding(0, 8, 0, 0)
            };
            

            _lblSubtitulo = new Label
            {
                Text = "Cadastre um novo ponto de interesse na cidade de Buenos Aires!",
                Font = BuenosAiresTheme.MutedFont,
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
                Margin = new Padding(12, 7, 0, 0)
            };
            var locationIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "location_icon.png");
            if (File.Exists(locationIconPath))
            {
                using var iconStream = File.OpenRead(locationIconPath);
                using var originalIcon = Image.FromStream(iconStream);
                _lblIcone.Image = new Bitmap(originalIcon, new Size(40, 40));
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

            //Declaração dos campos do formulário, seguindo a ordem: nome, categoria, endereço, latitude, longitude e notas.

            // Nome e botão de buscar automático lado a lado
            _lblNome = new Label
            {
                Text = "Nome",
                Location = new Point(pad, y)
            };
            BuenosAiresTheme.ApplyLabel(_lblNome);
            Controls.Add(_lblNome);
            y += 20;
            int btnW = 160;
           
            _txtName = new RoundedTextBox
            {
                Location = new Point(pad, y),
                Width = widthField - btnW - 8,
                Placeholder = "Ex: Café Tortoni"
            };
            Controls.Add(_txtName);

            _btnBuscarAutomatico = new RoundedButton
            {
                Text = "Busca Automática",
                Location = new Point(pad + _txtName.Width + 8, y),
                Width = btnW,
                Height = BuenosAiresTheme.InputHeight,
            };
            Controls.Add(_btnBuscarAutomatico);
            y += BuenosAiresTheme.InputHeight + BuenosAiresTheme.SpacingField;

            // Campo de categoria
            _lblCategoria = new Label { Text = "Categoria", Location = new Point(pad, y) };
            BuenosAiresTheme.ApplyLabel(_lblCategoria);
            Controls.Add(_lblCategoria);
            y += 20;

            // Categoria é um combo box que permite selecionar entre categorias pré-definidas ou digitar uma nova categoria.
            _cmbCategory = new RoundedComboBox()
            {
                 Location = new Point(pad, y),
                 Width = widthField,
                 DropDownStyle = ComboBoxStyle.DropDown,
                 Placeholder = "Selecione ou digite uma categoria"

            };
            Controls.Add(_cmbCategory);
            y += BuenosAiresTheme.InputHeight + BuenosAiresTheme.SpacingField;
            LoadCategories();


            // Endereço e botão de buscar coordenadas lado a lado
            _lblEndereco = new Label { Text = "Endereço", Location = new Point(pad, y) };
            BuenosAiresTheme.ApplyLabel(_lblEndereco);
            Controls.Add(_lblEndereco);
            y += 20;

            _txtAddress = new RoundedTextBox
            {
                Location = new Point(pad, y),
                Width = widthField - btnW - 8,
                Placeholder = "Ex: Av. de Mayo 825"
            };
            Controls.Add(_txtAddress);

            _btnBuscarCoordenadas = new RoundedButton
            {
                Text = "Buscar Coordenadas",
                Location = new Point(pad + _txtAddress.Width + 8, y),
                Width = btnW,
                Height = BuenosAiresTheme.InputHeight,
                ForeColor = BuenosAiresTheme.HeaderColor,
                FillColor = BuenosAiresTheme.AccentColor,
                HoverColor = BuenosAiresTheme.AccentColorMuted,
            };
            Controls.Add(_btnBuscarCoordenadas);
            y += BuenosAiresTheme.InputHeight + BuenosAiresTheme.SpacingField;


            // Latitude e Longitude lado a lado
            _lblLatitude = new Label { Text = "Latitude", Location = new Point(pad, y) };
            _lblLongitude = new Label { Text = "Longitude", Location = new Point(pad + (widthField - 8) / 2 + 8, y) };
            BuenosAiresTheme.ApplyLabel(_lblLatitude); BuenosAiresTheme.ApplyLabel(_lblLongitude);
            Controls.Add(_lblLatitude);
            Controls.Add(_lblLongitude);
            y += 20;

            _txtLatitude = new RoundedTextBox
            {
                Location = new Point(pad, y),
                Width = (widthField - 8) / 2,
                Placeholder = "Clique em Buscar Coordenadas"
            };
            Controls.Add(_txtLatitude);

            _txtLongitude = new RoundedTextBox
            {
                Location = new Point(pad + _txtLatitude.Width + 8, y),
                Width = (widthField - 8) / 2,
                Placeholder = "Clique em Buscar Coordenadas"
            };
            Controls.Add(_txtLongitude);
            y += BuenosAiresTheme.InputHeight + BuenosAiresTheme.SpacingField;

            // Campo de notas
            _lblNotas = new Label { Text = "Notas", Location = new Point(pad, y) };
            BuenosAiresTheme.ApplyLabel(_lblNotas);
            Controls.Add(_lblNotas);
            y += 20;

            _txtNotes = new RoundedTextBox
            {
                Location = new Point(pad, y),
                Width = widthField,
                Height = 100,
                Multiline = true,
                Placeholder = "Observações adicionais sobre o local..."
            };
            Controls.Add(_txtNotes);
            y += 80 + BuenosAiresTheme.SpacingField;


            _lblErro = new Label
            {
                Text = "",
                ForeColor = BuenosAiresTheme.DangerColor,
                Font = BuenosAiresTheme.MutedFont,
                AutoSize = true,
                Visible = false,
                Location = new Point(pad, y)
            };
            Controls.Add(_lblErro);

            // Buttons

            _btnSalvar = new RoundedButton
            {
                Text = "Cadastrar Local",
                Location = new Point(pad, y + 20),
                Width = widthField,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = BuenosAiresTheme.PrimaryColor,
            };
            Controls.Add(_btnSalvar);

            _btnCancelar = new RoundedButton
            {
                Text = "Cancelar",
                Location = new Point(pad, y + 62),
                Width = widthField,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = Color.Transparent,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                HoverColor = Color.Transparent,
            };

            Controls.Add(_btnCancelar);
            Controls.Add(_btnSalvar);

            //btn clicks

            _btnCancelar.Click += OnActionButtonClick;
            _btnSalvar.Click += OnActionButtonClick;
            _btnBuscarCoordenadas.Click += OnActionButtonClick;
            _btnBuscarAutomatico.Click += OnActionButtonClick;
        }

        private void LoadCategories()
        {
            var dbCategories = _locationService.GetAll()
                .Select(l => l.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var categories = new List<string>(DefaultCategories);
            foreach (var dbCategory in dbCategories)
            {
                if (!categories.Contains(dbCategory, StringComparer.OrdinalIgnoreCase))
                {
                    categories.Add(dbCategory);
                }
            }

            _cmbCategory.Items.Clear();
            foreach (var category in categories)
            {
                _cmbCategory.Items.Add(category);
            }
        }

        private void LocationForm_Shown(object? sender, EventArgs e)
        {
            BeginInvoke(new Action(() => ActiveControl = null));
        }



        private async void OnActionButtonClick(object? sender, EventArgs e)
        {
            if (ReferenceEquals(sender, _btnCancelar))
            {
                DialogResult = DialogResult.Cancel;
                return;
            }

            if (ReferenceEquals(sender, _btnSalvar))
            {
                Save();
                return;
            }

            if (ReferenceEquals(sender, _btnBuscarCoordenadas) || ReferenceEquals(sender, _btnBuscarAutomatico))
            {
                var buscarAutomatico = ReferenceEquals(sender, _btnBuscarAutomatico);
                await BuscarCoordenadasAsync(buscarAutomatico);
            }
        }

        private async Task BuscarCoordenadasAsync(bool buscarAutomatico)
        {
            var name = _txtName.Value?.Trim();
            var address = _txtAddress.Value?.Trim();

            if (buscarAutomatico && string.IsNullOrWhiteSpace(name))
            {
                ShowError("Preencha o nome para buscar coordenadas.");
                return;
            }

            if (!buscarAutomatico && string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(address))
            {
                ShowError("Preencha o nome ou endereço para buscar coordenadas.");
                return;
            }

            var query = $"{name} {address} Buenos Aires".Trim();

            if (string.IsNullOrWhiteSpace(query))
            {
                ShowError("Preencha o nome ou endereço para buscar coordenadas.");
                return;
            }

            var originalTextBuscarCoordenadas = _btnBuscarCoordenadas.Text;
            var originalTextBuscarAutomatico = _btnBuscarAutomatico.Text;

            _btnBuscarCoordenadas.Text = "Buscando...";
            _btnBuscarAutomatico.Text = "Buscando...";
            _btnBuscarCoordenadas.Enabled = false;
            _btnBuscarAutomatico.Enabled = false;

            try
            {
                var results = await GeocodingService.SearchCoordinatesAsync(query);

                if (results == null)
                {
                    ShowError("Erro ao buscar coordenadas. Tente um endereço mais específico.");
                    return;
                }

                _txtLatitude.Value = results?.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
                _txtLongitude.Value = results?.Lng.ToString(System.Globalization.CultureInfo.InvariantCulture);
                _txtAddress.Value = results?.Address ?? _txtAddress.Value;

                _lblErro.Visible = false;
            }
            finally
            {
                _btnBuscarCoordenadas.Text = originalTextBuscarCoordenadas;
                _btnBuscarAutomatico.Text = originalTextBuscarAutomatico;
                _btnBuscarCoordenadas.Enabled = true;
                _btnBuscarAutomatico.Enabled = true;
            }
        }


        private void PopulateFields(Location location)
        {
            _txtName.Value = location.Name;
            _cmbCategory.Value = location.Category;
            _txtAddress.Value = location.Address;
            _txtLatitude.Value = location.Latitude.ToString();
            _txtLongitude.Value = location.Longitude.ToString();
            _txtNotes.Value = location.Notes ?? "";
        }

        // validação dos campos.
        // seguindo o princípio de responsabilidade única, ValidateFields verifica se os campos estão preenchidos corretamente e exibe mensagens de erro específicas para cada campo,
        // enquanto o método Save é responsável por coletar os dados, criar o objeto Location e fechar o formulário. 
        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(_txtName.Value))
            {
                ShowError("O nome é obrigatório.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(_cmbCategory.Value))
            {
                ShowError("A categoria é obrigatória.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(_txtAddress.Value))
            {
                ShowError("O endereço é obrigatório.");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(_txtLatitude.Value) && (!double.TryParse(_txtLatitude.Value, out _)))
            {
                ShowError("Latitude deve ser um número válido. Use ponto como separador decimal.");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(_txtLongitude.Value) && (!double.TryParse(_txtLongitude.Value, out _)))
            {
                ShowError("Longitude deve ser um número válido. Use ponto como separador decimal.");
                return false;
            }
            return true;
        }

        private void ShowError(string message)
        {
            _lblErro.Text = message;
            _lblErro.Visible = true;
        }

        private void Save()
        {
            _lblErro.Visible = false;

            if (!ValidateFields())
                return;

            double.TryParse(_txtLatitude.Value, 
                System.Globalization.NumberStyles.Float, 
                System.Globalization.CultureInfo.InvariantCulture,
                out double lat);
            double.TryParse(_txtLongitude.Value, 
                System.Globalization.NumberStyles.Float, 
                System.Globalization.CultureInfo.InvariantCulture, 
                out double lng);

            Result = new Location
            {
                Id = _editingLocation?.Id ?? 0,
                Name = _txtName.Value.Trim(),
                Category = _cmbCategory.Value.Trim(),
                Address = _txtAddress.Value.Trim(),
                Latitude = lat,
                Longitude = lng,
                Notes = string.IsNullOrWhiteSpace(_txtNotes.Value) ? null : _txtNotes.Value.Trim()
            };

            var selectedCategory = _cmbCategory.Value.Trim();
            if (!string.IsNullOrWhiteSpace(selectedCategory) &&
                !_cmbCategory.Items.Cast<object>().Any(i =>
                    string.Equals(i?.ToString(), selectedCategory, StringComparison.OrdinalIgnoreCase)))
            {
                _cmbCategory.Items.Add(selectedCategory);
            }

            DialogResult = DialogResult.OK;
        }
    }
}
