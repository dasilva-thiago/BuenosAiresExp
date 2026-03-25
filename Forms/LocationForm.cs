using BuenosAiresExp.Models;
using BuenosAiresExp.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BuenosAiresExp.Services;

namespace BuenosAiresExp
{
    public partial class LocationForm : Form
    {
        public Location? Result { get; private set; }

        private readonly Location? _editingLocation;

        private RoundedTextBox _txtName;
        private RoundedTextBox _txtCategory;
        private RoundedTextBox _txtAddress;
        private RoundedTextBox _txtLatitude;
        private RoundedTextBox _txtLongitude;
        private RoundedTextBox _txtNotes;
        private RoundedButton _btnSalvar;
        private RoundedButton _btnCancelar;
        private RoundedButton _btnBuscarCoordenadas;
        private Label _lblErro;

        private TableLayoutPanel _headerLayout;
        private Panel _pnlHeader;
        private Label _lblTitulo;
        private Label _lblSubtitulo;
        private Label _lblIcone;

        public LocationForm() : this(null) { }

        public LocationForm(Location? location)
        {
            _editingLocation = location;
            InitializeComponent();
            BuildLayout();


            if (_editingLocation != null)
                PopulateFields(_editingLocation);
        }

        private void BuildLayout()
        {
            BuenosAiresTheme.ApplyForm(this);
            Text = _editingLocation == null ? "Adicionar Local" : "Editar Local";

            Size = new Size(860, 660);
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
                Text = "Preencha os detalhes do local",
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


            RoundedTextBox AddField(string labelTexto)
            {
                var lbl = new Label
                {
                    Text = labelTexto,
                    Location = new Point(pad, y)
                };

                BuenosAiresTheme.ApplyLabel(lbl);
                Controls.Add(lbl);
                y += 20;

                var field = new RoundedTextBox
                {
                    Location = new Point(pad, y),
                    Width = widthField,
                };

                Controls.Add(field);
                y += BuenosAiresTheme.InputHeight + BuenosAiresTheme.SpacingField;

                return field;
            }


            _txtName = AddField("Nome");
            _txtCategory = AddField("Categoria");
            _txtAddress = AddField("Endereço");
            _txtLatitude = AddField("Latitude");
            _txtLongitude = AddField("Longitude");
            _txtNotes = AddField("Notas");

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

            _btnCancelar = new RoundedButton
            {
                Text = "Cancelar",
                Location = new Point(pad, y + 20),
                Width = 110,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = Color.Transparent,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                HoverColor = BuenosAiresTheme.PrimaryColorLight,
            };

            _btnBuscarCoordenadas = new RoundedButton
            {
                Text = "Buscar Coordenadas",
                Location = new Point(pad + 127, y + 20),
                Width = 150,
                Font = BuenosAiresTheme.ButtonFont,
                ForeColor = BuenosAiresTheme.HeaderColor,
                FillColor = BuenosAiresTheme.AccentColor,
                HoverColor = BuenosAiresTheme.AccentColorMuted,
            };

            _btnSalvar = new RoundedButton
            {
                Text = "Salvar",
                Location = new Point(ClientSize.Width - pad - 110, y + 20),
                Width = 110,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = BuenosAiresTheme.PrimaryColor,
            };

            Controls.Add(_btnCancelar);
            Controls.Add(_btnBuscarCoordenadas);
            Controls.Add(_btnSalvar);

            //btn clicks

            _btnCancelar.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
            };

            _btnSalvar.Click += (s,e) => Save();

            _btnBuscarCoordenadas.Click += async (s, e) =>
            {
                var query = $"{_txtName.Value} {_txtAddress.Value} Buenos Aires".Trim();

                if (string.IsNullOrWhiteSpace(query))
                {
                    ShowError("Preencha o nome ou endereço para buscar coordenadas.");
                    return;
                }
                var originalText = _btnBuscarCoordenadas.Text;
                _btnBuscarCoordenadas.Text = "Buscando...";
                _btnBuscarCoordenadas.Enabled = false;
                try
                {
                    var results = await GeocodingService.SearchCoordinatesAsync(query);

                    if (results == null)
                    {
                        ShowError("Erro ao buscar coordenadas. Tente um endereço mais específico.");
                    }

                    _txtLatitude.Value = results?.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    _txtLongitude.Value = results?.Lng.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    _txtAddress.Value = results?.Address ?? _txtAddress.Value;

                    _lblErro.Visible = false;
                }
                finally
                {
                    _btnBuscarCoordenadas.Text = originalText;
                    _btnBuscarCoordenadas.Enabled = true;
                }
            };
        }

        private void PopulateFields(Location location)
        {
            _txtName.Value = location.Name;
            _txtCategory.Value = location.Category;
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
            if (string.IsNullOrWhiteSpace(_txtCategory.Value))
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
                Category = _txtCategory.Value.Trim(),
                Address = _txtAddress.Value.Trim(),
                Latitude = lat,
                Longitude = lng,
                Notes = string.IsNullOrWhiteSpace(_txtNotes.Value) ? null : _txtNotes.Value.Trim()
            };
            DialogResult = DialogResult.OK;
        }
    }
}
