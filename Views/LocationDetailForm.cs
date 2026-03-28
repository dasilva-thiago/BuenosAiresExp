using System;
using System.Drawing;
using System.Windows.Forms;
using BuenosAiresExp.Models;
using BuenosAiresExp.UI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace BuenosAiresExp
{
    public class LocationDetailForm : Form
    {
        private readonly Location _location;

        private Label _lblTitle;
        private Label _lblCategory;
        private Label _lblAddress;
        private Label _lblCoordinates;
        private Label _lblNotesCaption;
        private TextBox _txtNotes;

        public LocationDetailForm(Location location)
        {
            _location = location ?? throw new ArgumentNullException(nameof(location));

            BuildLayout();
            PopulateFields();
        }

        private void BuildLayout()
        {
            Text = "Detalhes do Local";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Size = new Size(520, 460);
            MinimumSize = new Size(420, 380);
            MaximizeBox = false;
            MinimizeBox = false;
            Padding = new Padding(BuenosAiresTheme.PaddingForm);
            BuenosAiresTheme.ApplyForm(this);

            var table = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                Padding = new Padding(4)


            };

            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // ocupa o form todo
            Controls.Add(table);

            // 6 linhas fixas que se ajustam ao conteúdo
            for (int i = 0; i < 5; i++)
                table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // linha que ocupa o espaço restante para a caixa de notas
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));



            _lblTitle = new Label
            {
                Text = "",
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = BuenosAiresTheme.PrimaryColor,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 2)
            };
            table.Controls.Add(_lblTitle);

            _lblCategory = new Label
            {
                Text = "",
                Font = BuenosAiresTheme.SubtitleFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 16)
            };
            table.Controls.Add(_lblCategory);

            _lblAddress = new Label
            {
                Text = "",
                Font = BuenosAiresTheme.BodyFont,
                ForeColor = BuenosAiresTheme.TextColor,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 2)
            };
            table.Controls.Add(_lblAddress);

            /*icone de localização - colocar depois
            var lblIcon = new Label
            {
                Text = "\uE81D",
                Font = new Font("Segoe MDL2 Assets", 10f),
                ForeColor = BuenosAiresTheme.PrimaryColor,
                AutoSize = true,
            };
            */

            _lblCoordinates = new Label
            {
                Text = "",
                Font = BuenosAiresTheme.FontMono,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 16)
            };
            table.Controls.Add(_lblCoordinates);

            var separator = new Panel
            {
                Height = 1,
                Dock = DockStyle.Fill,
                BackColor = BuenosAiresTheme.BorderColor,
                Margin = new Padding(0, 0, 0, 12)
            };
            table.Controls.Add(separator);

            _txtNotes = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                BackColor = BuenosAiresTheme.PrimaryColorLight,
                ForeColor = BuenosAiresTheme.TextColor,
                Font = BuenosAiresTheme.NotesFont,
                Margin = new Padding(0,0,0,12)
             };
            table.Controls.Add(_txtNotes);

            var btnFechar = new RoundedButton
            {
                Text = "Fechar",
                Width = 100,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = BuenosAiresTheme.PrimaryColor,
                Anchor = AnchorStyles.Right,
            };
            btnFechar.Click += (s, e) => Close();
            table.Controls.Add(btnFechar);

        }


        private void PopulateFields()
        {
            _lblTitle.Text = _location.Name;
            _lblCategory.Text = _location.Category;
            _lblCoordinates.Text = $"📍 {_location.Latitude:F4}  {_location.Longitude:F4}";
            _lblAddress.Text = _location.Address;
            _txtNotes.Text = _location.Notes ?? string.Empty;

            Shown += (s, e) =>
            {
                _txtNotes.SelectionLength = 0;  // remove a seleção
                ActiveControl = null;           // tira o foco do TextBox
            };
        }
    }
}
