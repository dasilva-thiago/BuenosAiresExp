using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace BuenosAiresExp.UI
{
    internal class RoundedTextBox : UserControl
    {
        private TextBox _textBox;
        private int _borderRadius = 10;
        private Color _borderColor = BuenosAiresTheme.BorderColor;
        private Color _focusColor = BuenosAiresTheme.PrimaryColor;
        private bool _isFocused = false; // Variável para controlar o estado de foco
        private string _placeholder = "";
        private bool _showingPlaceholder = false;



        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Placeholder
        {
            set
            {
                _placeholder = value;
                ShowPlaceholder();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Value
        {
            get => _showingPlaceholder ? "" : _textBox.Text;
            set
            {
                _showingPlaceholder = false;
                _textBox.ForeColor = BuenosAiresTheme.TextColor;
                _textBox.Font = BuenosAiresTheme.BodyFont;
                _textBox.Text = value ?? "";

                if (string.IsNullOrEmpty(_textBox.Text))
                {
                    ShowPlaceholder();
                }
            }

        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Multiline
        {
            get => _textBox.Multiline;
            set => _textBox.Multiline = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPassword
        {
            set => _textBox.UseSystemPasswordChar = value;
        }

        private void ShowPlaceholder()
        {
            if (string.IsNullOrEmpty(_textBox.Text))
            {
                _showingPlaceholder = true;
                _textBox.Text = _placeholder;
                _textBox.ForeColor = BuenosAiresTheme.TextMutedColor;
                _textBox.Font = BuenosAiresTheme.PlaceholderFont;
            }
        }

        private void HidePlaceholder()
        {
            if (_showingPlaceholder)
            {
                _showingPlaceholder = false;
                _textBox.Text = "";
                _textBox.ForeColor = BuenosAiresTheme.TextColor;
                _textBox.Font = BuenosAiresTheme.BodyFont;
            }
        }

        public RoundedTextBox()
        {


            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);


            _textBox = new TextBox()
            {
                BorderStyle = BorderStyle.None,
                BackColor = BuenosAiresTheme.OffWhiteColor,
                ForeColor = BuenosAiresTheme.TextColor,
                Font = BuenosAiresTheme.BodyFont,
                Dock = DockStyle.Fill,
                Margin = new Padding(12, 6, 10, 6),

            };


            _textBox.GotFocus += (s, e) => { HidePlaceholder(); _isFocused = true; Invalidate(); };
            _textBox.LostFocus += (s, e) => { ShowPlaceholder(); _isFocused = false; Invalidate(); };
            _textBox.TextChanged += (s, e) =>
            {
                if (!_showingPlaceholder)
                {
                    OnTextChanged(e);
                }
            };

            Padding = new Padding(8, 5, 8, 5);
            BackColor = BuenosAiresTheme.OffWhiteColor;
            Height = BuenosAiresTheme.InputHeight;

            Controls.Add(_textBox);

            UpdateRegion();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;


            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            Color borderColor = _isFocused ? _focusColor : _borderColor;

            using (GraphicsPath path = CreateRoundedRectangle(rect, _borderRadius))
            using (SolidBrush bgBrush = new SolidBrush(Color.White))
            using (Pen borderPen = new Pen(borderColor, 1.5f))
            {
                g.FillPath(bgBrush, path);
                g.DrawPath(borderPen, path);
            }
        }
        private void UpdateRegion()
        {
            var path = CreateRoundedRectangle(
                new Rectangle(0, 0, Width - 1, Height - 1),
                _borderRadius
            );
            Region = new Region(path);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateRegion();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRegion();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {

            base.OnPaintBackground(e);
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
