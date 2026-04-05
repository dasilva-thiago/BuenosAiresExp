using BuenosAiresExp.Models;
using BuenosAiresExp.Services;
using BuenosAiresExp.UI;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

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

        private Bitmap? _loadedImage;
        private Panel _pnlImage;
        private Label _lblImagePlaceholder;

        private RoundedButton _btnMap;
        private RoundedButton _btnClose;
        private TableLayoutPanel _layoutRoot;
        private TableLayoutPanel _layoutBody;
        private TableLayoutPanel _layoutLeft;

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

           
            _layoutRoot = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(24, 20, 24, 20),
                BackColor = Color.Transparent
            };
            _layoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // body
            _layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 16)); // spacer
            _layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); // buttons
            Controls.Add(_layoutRoot);

            
            _layoutBody = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            _layoutBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            _layoutBody.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
            _layoutRoot.Controls.Add(_layoutBody, 0, 0);

           
            _layoutLeft = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 20, 0)
            };
            _layoutLeft.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // name
            _layoutLeft.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // category badge
            _layoutLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 18)); // spacer
            _layoutLeft.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // address caption + value
            _layoutLeft.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // coords
            _layoutLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 14)); // spacer
            _layoutLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // notes
            _layoutBody.Controls.Add(_layoutLeft, 0, 0);

            
            _lblName = new Label
            {
                Text = "",
                Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 18f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.PrimaryColor,
                Dock = DockStyle.Fill,
                AutoSize = true,
                MaximumSize = new Size(400, 0),
                Margin = new Padding(0, 0, 0, 4)
            };
            _layoutLeft.Controls.Add(_lblName, 0, 0);

          
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
            _layoutLeft.Controls.Add(_lblCategoryBadge, 0, 1);

            
            _layoutLeft.Controls.Add(new Panel { BackColor = Color.Transparent }, 0, 2);

           
            var pnlAddress = new Panel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 8)
            };

            _lblAddressCaption = new Label
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
                Font = new Font(BuenosAiresTheme.BodyFont.FontFamily, 10f, FontStyle.Bold),
                ForeColor = BuenosAiresTheme.TextColor,
                MaximumSize = new Size(400, 0),
                AutoSize = true,
                Location = new Point(0, 18)
            };

            pnlAddress.Controls.Add(_lblAddressCaption);
            pnlAddress.Controls.Add(_lblAddressValue);
            pnlAddress.Height = 70;
            _layoutLeft.Controls.Add(pnlAddress, 0, 3);

            // Coords block
            var pnlCoords = new Panel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 0)
            };

            _lblCoordsCaption = new Label
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

            pnlCoords.Controls.Add(_lblCoordsCaption);
            pnlCoords.Controls.Add(_lblCoordsValue);
            pnlCoords.Height = 50;
            _layoutLeft.Controls.Add(pnlCoords, 0, 4);

            
            _layoutLeft.Controls.Add(new Panel { BackColor = Color.Transparent }, 0, 5);

            var pnlNotes = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            _lblNotesCaption = new Label
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
                Font = new Font(BuenosAiresTheme.NotesFont.FontFamily, 9.5f, FontStyle.Italic),
                ForeColor = BuenosAiresTheme.TextColor,
                AutoSize = false,
                Location = new Point(0, 18),
                Size = new Size(380, 60)
            };

            pnlNotes.Controls.Add(_lblNotesCaption);
            pnlNotes.Controls.Add(_lblNotesValue);
            _layoutLeft.Controls.Add(pnlNotes, 0, 6);

            _pnlImage = new Panel
            {
                Width = 200,
                Height = 200,
                BackColor = BuenosAiresTheme.PrimaryColorLight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Margin = new Padding(0, 4, 0, 0)
            };
            _pnlImage.Paint += PnlImage_Paint;

            //label de carregamento exibido enquanto a imagem não chega
            _lblImagePlaceholder = new Label
            {
                Text = "Buscando imagem...",
                Font = BuenosAiresTheme.MutedFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            _pnlImage.Controls.Add(_lblImagePlaceholder);

            _layoutBody.Controls.Add(_pnlImage, 1, 0);


            var btnRow = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            btnRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // map button (wide)
            btnRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130)); // close button
            _layoutRoot.Controls.Add(btnRow, 0, 2);

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
            _btnClose.Click += BtnClose_Click;
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

            Shown -= OnFormShown;
            Shown += OnFormShown;
        }

        private async Task LoadImageAsync()
        {
            var imageUrl = await WikimediaImageService.SearchImageUrlAsync(
                _location.Latitude, _location.Longitude, _location.Name);

            if (imageUrl == null)
            {
                _lblImagePlaceholder.Text = "Imagem não encontrada.\n\nImagens dependem da disponibilidade na Wikipedia/Wikimedia.";
                return;
            }

            var bitmap = await WikimediaImageService.DownloadImageAsync(imageUrl);

            if (bitmap == null)
            {
                _lblImagePlaceholder.Text = "Erro ao carregar imagem";
                return;
            }

            try
            {
                if (IsDisposed || !IsHandleCreated) return;

                if (InvokeRequired)
                    Invoke(new Action(() => ApplyLoadedImage(bitmap)));
                else
                    ApplyLoadedImage(bitmap);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void ApplyLoadedImage(Bitmap bitmap)
        {
            if (IsDisposed) return;

            _loadedImage = bitmap;
            _lblImagePlaceholder.Visible = false;
            _pnlImage.Invalidate();
        }


        private void BtnClose_Click(object? sender, EventArgs e) => Close();

        private async void OnFormShown(object? sender, EventArgs e)
        {
            ActiveControl = null;
            BeginInvoke(new Action(() => ActiveControl = null));
            await LoadImageAsync();
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

            // Recorta o conteúdo nos cantos arredondados
            e.Graphics.SetClip(path);

            if (_loadedImage != null)
            {
                //exibe a imagem real baixada da Wikimedia, com cover fit
                var destRect = CoverFit(_loadedImage.Size, p.ClientRectangle);
                e.Graphics.DrawImage(_loadedImage, destRect);
            }
            else
            {
                // Placeholder enquanto carrega
                using var bgBrush = new SolidBrush(BuenosAiresTheme.PrimaryColorLight);
                e.Graphics.FillPath(bgBrush, path);

                using var iconFont = new Font("Segoe UI Emoji", 26f);
                using var iconBrush = new SolidBrush(BuenosAiresTheme.PrimaryColor);
                e.Graphics.DrawString("📍", iconFont, iconBrush,
                    new RectangleF(0, 20, p.Width, p.Height - 60),
                    new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    });
            }

            e.Graphics.ResetClip();

            // Borda arredondada por cima do conteúdo
            using var borderPen = new Pen(BuenosAiresTheme.BorderColor, 1.5f);
            e.Graphics.DrawPath(borderPen, path);
        }

        //calcula o retângulo de destino para o efeito "cover" (sem distorção)
        private static Rectangle CoverFit(Size imageSize, Rectangle container)
        {
            float scaleW = (float)container.Width / imageSize.Width;
            float scaleH = (float)container.Height / imageSize.Height;
            float scale = Math.Max(scaleW, scaleH);

            int w = (int)(imageSize.Width * scale);
            int h = (int)(imageSize.Height * scale);
            int x = container.X + (container.Width - w) / 2;
            int y = container.Y + (container.Height - h) / 2;

            return new Rectangle(x, y, w, h);
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
