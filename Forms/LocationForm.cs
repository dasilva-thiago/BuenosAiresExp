using BuenosAiresExp.Models;
using BuenosAiresExp.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuenosAiresExp
{
    public partial class LocationForm : Form
    {
        public Location? Result { get; private set; }

        private readonly Location? _editingLocation;

        private RoundedTextBox txtName;
        private RoundedTextBox txtCategory;
        private RoundedTextBox txtAddress;
        private RoundedTextBox txtLatitude;
        private RoundedTextBox txtLongitude;
        private RoundedTextBox txtNotes;
        private RoundedButton btnSalvar;
        private RoundedButton btnCancelar;
        private Label lblErro;

        private Label lblTitulo;

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

            // window settings
            BuenosAiresTheme.ApplyForm(this);
            Text = _editingLocation == null ? "Adicionar Local" : "Editar Local";

            Size = new Size(460, 560);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // -- layout settings

            int pad = BuenosAiresExp.UI.BuenosAiresTheme.PaddingForm;
            int y = pad;
            int widthField = ClientSize.Width - pad * 2;

            lblTitulo = new Label
            {
                Text = Text,
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = BuenosAiresTheme.PrimaryColor,
                AutoSize = true,
                Location = new Point(pad, y)
            };
            Controls.Add(lblTitulo);
            y += 40;

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

            // Usando a função AddField para criar os campos de entrada

            txtName = AddField("Nome");
            txtCategory = AddField("Categoria");
            txtAddress = AddField("Endereço");
            txtLatitude = AddField("Latitude");
            txtLongitude = AddField("Longitude");
            txtNotes = AddField("Notas");

            lblErro = new Label
            {
                Text = "",
                ForeColor = BuenosAiresTheme.DangerColor,
                Font = BuenosAiresTheme.MutedFont,
                AutoSize = true,
                Visible = false,
                Location = new Point(pad, y)
            };
            Controls.Add(lblErro);

            // Buttons

            btnCancelar = new RoundedButton
            {
                Text = "Cancelar",
                Location = new Point(pad, y + 20),
                Width = 110,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = Color.Transparent,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                HoverColor = BuenosAiresTheme.PrimaryColorLight,
            };

            btnSalvar = new RoundedButton
            {
                Text = "Salvar",
                Location = new Point(ClientSize.Width - pad - 110, y + 20),
                Width = 110,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = BuenosAiresTheme.PrimaryColor,
            };

            Controls.Add(btnCancelar);
            Controls.Add(btnSalvar);

            //btn clicks

            btnCancelar.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
            };
            btnSalvar.Click += (s,e) => Save();
        }

        private void PopulateFields(Location location)
        {
            txtName.Value = location.Name;
            txtCategory.Value = location.Category;
            txtAddress.Value = location.Address;
            txtLatitude.Value = location.Latitude.ToString();
            txtLongitude.Value = location.Longitude.ToString();
            txtNotes.Value = location.Notes ?? "";
        }

        // validação dos campos.
        // segue o srp 

        private bool ValidadeFields()
        {
            if (string.IsNullOrWhiteSpace(txtName.Value))
            {
                ShowError("O nome é obrigatório.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCategory.Value))
            {
                ShowError("A categoria é obrigatória.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtAddress.Value))
            {
                ShowError("O endereço é obrigatório.");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(txtLatitude.Value) && (!double.TryParse(txtLatitude.Value, out _)))
            {
                ShowError("Latitude deve ser um número válido. Use ponto como separador decimal.");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(txtLongitude.Value) && (!double.TryParse(txtLongitude.Value, out _)))
            {
                ShowError("Longitude deve ser um número válido. Use ponto como separador decimal.");
                return false;
            }
            return true;
        }

        private void ShowError(string message)
        {
            lblErro.Text = message;
            lblErro.Visible = true;
        }

        private void Save()
        {
            lblErro.Visible = false;

            if (!ValidadeFields())
                return;

            double.TryParse(txtLatitude.Value, out double lat);
            double.TryParse(txtLongitude.Value, out double lng);

            Result = new Location
            {
                Id = _editingLocation?.Id ?? 0,
                Name = txtName.Value.Trim(),
                Category = txtCategory.Value.Trim(),
                Address = txtAddress.Value.Trim(),
                Latitude = lat,
                Longitude = lng,
                Notes = string.IsNullOrWhiteSpace(txtNotes.Value) ? null : txtNotes.Value.Trim()
            };
            DialogResult = DialogResult.OK;
        }
    }
}
