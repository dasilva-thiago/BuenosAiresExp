using BuenosAiresExp.Models;
using BuenosAiresExp.UI;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace BuenosAiresExp
{
    public class LocationDetailForm : Form
    {
        private readonly Location _location;

        
        private Label _lblName;
        private Label _lblCategoryBadge;
        private Label _lblAddressCaption;
        private Label _lblAddressValue;
        private Label _lblCoordsCaption;
        private Label _lblCoordsValue;
        private Label _lblNotesCaption;
        private Label _lblNotesValue;

        
        private Panel _pnlImage;
        private Label _lblImagePlaceholder;

        private RoundedButton _btnMap;
        private RoundedButton _btnClose;

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
            Size = new Size(700, 480);
            MaximizeBox = false;
            MinimizeBox = false;
            BuenosAiresTheme.ApplyForm(this);
            BackColor = BuenosAiresTheme.FillColor;

           
            var outer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(24, 20, 24, 20),
                BackColor = Color.Transparent
            };
            outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // body
            outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 16)); // spacer
            outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); // buttons
            Controls.Add(outer);

            
            var body = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
            outer.Controls.Add(body, 0, 0);

           
            var left = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 20, 0)
            };
            left.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // name
            left.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // category badge
            left.RowStyles.Add(new RowStyle(SizeType.Absolute, 18)); // spacer
            left.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // address caption + value
            left.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // coords
            left.RowStyles.Add(new RowStyle(SizeType.Absolute, 14)); // spacer
            left.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // notes
            body.Controls.Add(left, 0, 0);

            
            _lblName = new Label
            {
                Text = "",
                Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 18f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.TextColor,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 4)
            };
            left.Controls.Add(_lblName, 0, 0);

          
            _lblCategoryBadge = new Label
            {
                Text = "",
                Font = new Font(BuenosAiresTheme.BodyFont.FontFamily, 9f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.CategoryDefaultFg,
                BackColor = BuenosAiresTheme.CategoryDefaultBg,
                AutoSize = true,
                Padding = new Padding(10, 3, 10, 3),
                Margin = new Padding(0, 0, 0, 0)
            };
            left.Controls.Add(_lblCategoryBadge, 0, 1);

            
            left.Controls.Add(new Panel { BackColor = Color.Transparent }, 0, 2);

           
            var pnlAddress = new Panel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 8)
            };

            var lblAddrIcon = new Label
            {
                Text = "ENDEREÇO",
                Font = new Font(BuenosAiresTheme.BodyFont.FontFamily, 7.5f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.TextMutedColor,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            _lblAddressValue = new Label
            {
                Text = "",
                Font = BuenosAiresTheme.BodyFont,
                ForeColor = BuenosAiresTheme.TextColor,
                MaximumSize = new Size(400, 0),
                AutoSize = true,
                Location = new Point(0, 18)
            };

            pnlAddress.Controls.Add(lblAddrIcon);
            pnlAddress.Controls.Add(_lblAddressValue);
            pnlAddress.Height = 70;
            left.Controls.Add(pnlAddress, 0, 3);

            // Coords block
            var pnlCoords = new Panel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 0)
            };

            var lblCoordsIcon = new Label
            {
                Text = "COORDENADAS GPS",
                Font = new Font(BuenosAiresTheme.BodyFont.FontFamily, 7.5f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.TextMutedColor,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            _lblCoordsValue = new Label
            {
                Text = "",
                Font = new Font("Consolas", 9f, FontStyle.Regular),
                ForeColor = BuenosAiresTheme.TextColor,
                BackColor = BuenosAiresTheme.PrimaryColorLight,
                AutoSize = true,
                Padding = new Padding(8, 4, 8, 4),
                Location = new Point(0, 18)
            };

            pnlCoords.Controls.Add(lblCoordsIcon);
            pnlCoords.Controls.Add(_lblCoordsValue);
            pnlCoords.Height = 50;
            left.Controls.Add(pnlCoords, 0, 4);

            
            left.Controls.Add(new Panel { BackColor = Color.Transparent }, 0, 5);

            var pnlNotes = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            var lblNotesCaption = new Label
            {
                Text = "NOTAS",
                Font = new Font(BuenosAiresTheme.BodyFont.FontFamily, 7.5f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.TextMutedColor,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            _lblNotesValue = new Label
            {
                Text = "",
                Font = BuenosAiresTheme.NotesFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                AutoSize = false,
                Location = new Point(0, 18),
                Size = new Size(380, 60)
            };

            pnlNotes.Controls.Add(lblNotesCaption);
            pnlNotes.Controls.Add(_lblNotesValue);
            left.Controls.Add(pnlNotes, 0, 6);

            _pnlImage = new Panel
            {
                Width = 200,
                Height = 200,
                BackColor = BuenosAiresTheme.PrimaryColorLight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Margin = new Padding(0, 4, 0, 0)
            };
            _pnlImage.Paint += PnlImage_Paint;

            _lblImagePlaceholder = new Label
            {
                Text = "Imagem\nilustrativa",
                Font = BuenosAiresTheme.MutedFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            _pnlImage.Controls.Add(_lblImagePlaceholder);

            body.Controls.Add(_pnlImage, 1, 0);

            var btnRow = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            btnRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // map button (wide)
            btnRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130)); // close button
            outer.Controls.Add(btnRow, 0, 2);

            _btnMap = new RoundedButton
            {
                Text = "Ver no Mapa",
                Dock = DockStyle.Fill,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = BuenosAiresTheme.PrimaryColor,
                ForeColor = Color.White,
                HoverColor = BuenosAiresTheme.PrimaryColorDark,
                BackColor = BuenosAiresTheme.FillColor,
                Margin = new Padding(0, 0, 10, 10)
            };
            _btnMap.Click += BtnMap_Click;
            btnRow.Controls.Add(_btnMap, 0, 0);

            _btnClose = new RoundedButton
            {
                Text = "Fechar",
                Dock = DockStyle.Fill,
                Font = BuenosAiresTheme.ButtonFont,
                FillColor = Color.Transparent,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                HoverColor = BuenosAiresTheme.PrimaryColorLight,
                BackColor = BuenosAiresTheme.FillColor,
                Margin = new Padding(0, 0, 0, 10)
            };
            _btnClose.Click += (s, e) => Close();
            btnRow.Controls.Add(_btnClose, 1, 0);
        }

        private void PopulateFields()
        {
            _lblName.Text = _location.Name;
            _lblCategoryBadge.Text = $"  {_location.Category}  ";
            _lblAddressValue.Text = _location.Address;
            _lblCoordsValue.Text = $"  {_location.Latitude:F6},  {_location.Longitude:F6}  ";
            _lblNotesValue.Text = string.IsNullOrWhiteSpace(_location.Notes)
                ? "(sem notas)"
                : _location.Notes;

            var (bg, fg) = BuenosAiresTheme.GetCategoryColors(_location.Category);
            _lblCategoryBadge.BackColor = bg;
            _lblCategoryBadge.ForeColor = fg;

            Shown += (s, e) =>
            {
                ActiveControl = null;
                BeginInvoke(new Action(() => ActiveControl = null));
            };
        }

        private void PnlImage_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel p) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, p.Width - 1, p.Height - 1);
            int radius = 14;
            int d = radius * 2;

            using var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            using var bg = new SolidBrush(BuenosAiresTheme.PrimaryColorLight);
            using var border = new Pen(BuenosAiresTheme.BorderColor, 1.5f);
            e.Graphics.FillPath(bg, path);
            e.Graphics.DrawPath(border, path);

        
            using var iconBrush = new SolidBrush(BuenosAiresTheme.PrimaryColor);
            using var iconFont = new Font("Segoe UI Emoji", 26f);
            e.Graphics.DrawString("📍", iconFont, iconBrush,
                new RectangleF(0, 20, p.Width, p.Height - 60),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            using var capFont = new Font("Segoe UI", 8f, FontStyle.Italic);
            using var capBrush = new SolidBrush(BuenosAiresTheme.TextMutedColor);
            e.Graphics.DrawString(_location.Name, capFont, capBrush,
                new RectangleF(8, p.Height - 40, p.Width - 16, 36),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            _lblImagePlaceholder.Visible = false;
        }

        private void BtnMap_Click(object? sender, EventArgs e)
        {
            var lat = _location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var lng = _location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var url = $"https://www.openstreetmap.org/?mlat={lat}&mlon={lng}&zoom=17#map=17/{lat}/{lng}";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}
