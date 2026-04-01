using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BuenosAiresExp.UI
{
    [DefaultEvent(nameof(ValueChanged))]
    internal class RoundedDateTimePicker : UserControl
    {
        private readonly DateTimePicker _datePicker;
        private bool _isFocused;
        private bool _isHovering;
        private bool _hasValue;
        private string _placeholder = "Selecione uma data";
        private int _borderRadius = 10;
        private Color _borderColor = BuenosAiresTheme.BorderColor;
        private Color _focusColor = BuenosAiresTheme.PrimaryColor;
        private Color _fillColor = Color.White;
        private bool _showCalendarIcon = true;

        public event EventHandler? ValueChanged;

        public RoundedDateTimePicker()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            DoubleBuffered = true;
            Height = BuenosAiresTheme.InputHeight;
            MinimumSize = new Size(120, BuenosAiresTheme.InputHeight);
            Padding = new Padding(8, 5, 8, 5);
            BackColor = Color.Transparent;

            _datePicker = new DateTimePicker
            {
                Location = new Point(-1000, -1000),
                Size = new Size(1, 1),
                TabStop = false,
                Format = DateTimePickerFormat.Short,
                ShowUpDown = false,
                Visible = false
            };

            _datePicker.ValueChanged += OnInnerValueChanged;
            _datePicker.CloseUp += OnInnerCloseUp;
            _datePicker.DropDown += OnInnerDropDown;
            Controls.Add(_datePicker);

            MouseEnter += (_, _) =>
            {
                _isHovering = true;
                Cursor = Enabled ? Cursors.Hand : Cursors.Default;
                Invalidate();
            };

            MouseLeave += (_, _) =>
            {
                _isHovering = false;
                Invalidate();
            };

            MouseClick += (_, _) => OpenCalendar();
            Cursor = Cursors.Hand;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public DateTime Value
        {
            get => _datePicker.Value;
            set
            {
                _datePicker.Value = value;
                _hasValue = true;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public DateTime MinDate
        {
            get => _datePicker.MinDate;
            set => _datePicker.MinDate = value;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public DateTime MaxDate
        {
            get => _datePicker.MaxDate;
            set => _datePicker.MaxDate = value;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public DateTimePickerFormat Format
        {
            get => _datePicker.Format;
            set
            {
                _datePicker.Format = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string CustomFormat
        {
            get => _datePicker.CustomFormat ?? string.Empty;
            set
            {
                _datePicker.CustomFormat = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Placeholder
        {
            get => _placeholder;
            set
            {
                _placeholder = value ?? string.Empty;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = Math.Max(2, value);
                Invalidate();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color FocusColor
        {
            get => _focusColor;
            set
            {
                _focusColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color FillColor
        {
            get => _fillColor;
            set
            {
                _fillColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowCalendarIcon
        {
            get => _showCalendarIcon;
            set
            {
                _showCalendarIcon = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public bool HasValue => _hasValue;

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Cursor = Enabled ? Cursors.Hand : Cursors.Default;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var borderRect = new Rectangle(0, 0, Width - 1, Height - 1);
            var fillRect = new Rectangle(1, 1, Width - 2, Height - 2);

            var currentFill = Enabled ? _fillColor : BuenosAiresTheme.FillColor;
            var currentBorder = GetCurrentBorderColor();
            var borderWidth = _isFocused ? 2f : 1.5f;
            var iconColor = Enabled
                ? (_isFocused ? BuenosAiresTheme.PrimaryColor : BuenosAiresTheme.TextMutedColor)
                : BuenosAiresTheme.TextMutedColor;

            using (var fillPath = CreateRoundedPath(fillRect, _borderRadius))
            using (var fillBrush = new SolidBrush(currentFill))
            {
                e.Graphics.FillPath(fillBrush, fillPath);
            }

            using (var borderPath = CreateRoundedPath(borderRect, _borderRadius))
            using (var borderPen = new Pen(currentBorder, borderWidth))
            {
                e.Graphics.DrawPath(borderPen, borderPath);
            }

            var leftPadding = _showCalendarIcon ? 36 : 12;
            var rightPadding = 28;
            var textBounds = new Rectangle(leftPadding, 0, Width - leftPadding - rightPadding, Height);

            if (_showCalendarIcon)
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    "📅",
                    BuenosAiresTheme.BodyFont,
                    new Rectangle(10, 0, 24, Height),
                    iconColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            }

            TextRenderer.DrawText(
                e.Graphics,
                GetDisplayText(),
                _hasValue ? BuenosAiresTheme.BodyFont : BuenosAiresTheme.PlaceholderFont,
                textBounds,
                _hasValue ? (Enabled ? BuenosAiresTheme.TextColor : BuenosAiresTheme.TextMutedColor) : BuenosAiresTheme.TextMutedColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

            TextRenderer.DrawText(
                e.Graphics,
                "▾",
                BuenosAiresTheme.BodyFont,
                new Rectangle(Width - 22, 0, 16, Height),
                iconColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }

        private void OpenCalendar()
        {
            if (!Enabled)
            {
                return;
            }

            _datePicker.Bounds = new Rectangle(0, 0, Width, Height);
            _datePicker.Visible = true;
            _datePicker.BringToFront();
            _datePicker.Focus();

            _isFocused = true;
            Invalidate();

            SendKeys.Send("{F4}");
        }

        private void OnInnerValueChanged(object? sender, EventArgs e)
        {
            _hasValue = true;
            ValueChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        private void OnInnerDropDown(object? sender, EventArgs e)
        {
            _isFocused = true;
            Invalidate();
        }

        private void OnInnerCloseUp(object? sender, EventArgs e)
        {
            _isFocused = false;
            _datePicker.Visible = false;
            _datePicker.Location = new Point(-1000, -1000);
            Invalidate();
        }

        private string GetDisplayText()
        {
            if (!_hasValue)
            {
                return _placeholder;
            }

            return _datePicker.Format switch
            {
                DateTimePickerFormat.Long => _datePicker.Value.ToLongDateString(),
                DateTimePickerFormat.Short => _datePicker.Value.ToShortDateString(),
                DateTimePickerFormat.Custom => _datePicker.Value.ToString(string.IsNullOrWhiteSpace(_datePicker.CustomFormat) ? "dd/MM/yyyy" : _datePicker.CustomFormat),
                _ => _datePicker.Value.ToShortDateString()
            };
        }

        private Color GetCurrentBorderColor()
        {
            if (!Enabled)
            {
                return Color.FromArgb(120, _borderColor);
            }

            if (_isFocused)
            {
                return _focusColor;
            }

            if (_isHovering)
            {
                return BuenosAiresTheme.PrimaryColorMuted;
            }

            return _borderColor;
        }

        private static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            var diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
