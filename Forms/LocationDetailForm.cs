using System;
using System.Drawing;
using System.Windows.Forms;
using BuenosAiresExp.Models;
using BuenosAiresExp.UI;

namespace BuenosAiresExp
{
    public class LocationDetailForm : Form
    {
        private readonly Location _location;

        private Label _lblTitle;
        private Label _lblCategory;
        private Label _lblAddressCaption;
        private Label _lblAddressValue;
        private Label _lblCoordinates;
        private Label _lblNotesCaption;
        private TextBox _txtNotes;

        public LocationDetailForm(Location location)
        {
            _location = location ?? throw new ArgumentNullException(nameof(location));

            BuildLayout();
            ApplyTheme();
            PopulateFields();
        }

        private void BuildLayout()
        {
            Text = "Detalhes do local";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(600, 400);
            MinimumSize = new Size(500, 320);

            Padding = new Padding(BuenosAiresTheme.PaddingForm);

            _lblTitle = new Label
            {
                AutoSize = true,
                Location = new Point(20, 20)
            };

            _lblCategory = new Label
            {
                AutoSize = true,
                Location = new Point(20, 60)
            };

            _lblAddressCaption = new Label
            {
                Text = "Endere\u00e7o:",
                AutoSize = true,
                Location = new Point(20, 90)
            };

            _lblAddressValue = new Label
            {
                AutoSize = true,
                Location = new Point(100, 90)
            };

            _lblCoordinates = new Label
            {
                AutoSize = true,
                Location = new Point(20, 120)
            };

            _lblNotesCaption = new Label
            {
                Text = "Notas:",
                AutoSize = true,
                Location = new Point(20, 130)
            };

            _txtNotes = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(20, 170),
                Width = ClientSize.Width - 40,
                Height = ClientSize.Height - 210,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            Controls.Add(_lblTitle);
            Controls.Add(_lblCategory);
            Controls.Add(_lblAddressCaption);
            Controls.Add(_lblAddressValue);
            Controls.Add(_lblCoordinates);
            Controls.Add(_lblNotesCaption);
            Controls.Add(_txtNotes);
        }

        private void ApplyTheme()
        {
            BuenosAiresTheme.ApplyForm(this);

            _lblTitle.Font = BuenosAiresTheme.TitleFont;
            _lblTitle.ForeColor = BuenosAiresTheme.TextColor;

            _lblCategory.Font = BuenosAiresTheme.SubtitleFont;
            _lblCategory.ForeColor = BuenosAiresTheme.TextMutedColor;

            BuenosAiresTheme.ApplyLabel(_lblAddressCaption, isMuted: true);
            _lblAddressValue.Font = BuenosAiresTheme.BodyFont;
            _lblAddressValue.ForeColor = BuenosAiresTheme.TextColor;

            _lblCoordinates.Font = BuenosAiresTheme.FontMono;
            _lblCoordinates.ForeColor = BuenosAiresTheme.TextMutedColor;

            BuenosAiresTheme.ApplyLabel(_lblNotesCaption, isMuted: true);

            BuenosAiresTheme.ApplyTextBox(_txtNotes);
            _txtNotes.ReadOnly = true;
        }

        private void PopulateFields()
        {
            _lblTitle.Text = _location.Name;
            _lblCategory.Text = _location.Category;
            _lblCoordinates.Text = $"lat {_location.Latitude:F4}   lng {_location.Longitude:F4}";
            _lblAddressValue.Text = _location.Address;
            _txtNotes.Text = _location.Notes ?? string.Empty;
        }
    }
}
