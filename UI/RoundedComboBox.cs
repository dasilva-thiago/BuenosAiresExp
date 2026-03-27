using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BuenosAiresExp.UI
{
    public class RoundedComboBox : UserControl
    {
        private readonly ComboBox _comboBox;
        private int _borderRadius = 10;
        private Color _borderColor = BuenosAiresTheme.BorderColor;
        private Color _focusColor = BuenosAiresTheme.PrimaryColor;
        private Color _fillColor = Color.White;
        private Color _hoverColor = BuenosAiresTheme.PrimaryColorLight;
        private bool _isFocused;
        private bool _isHovering;
        private string _placeholder = string.Empty;
        private bool _showingPlaceholder;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; UpdateRegion(); Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color FocusColor
        {
            get => _focusColor;
            set { _focusColor = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color FillColor
        {
            get => _fillColor;
            set { _fillColor = value; _comboBox.BackColor = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color HoverColor
        {
            get => _hoverColor;
            set { _hoverColor = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Placeholder
        {
            get => _placeholder;
            set
            {
                _placeholder = value ?? string.Empty;
                ShowPlaceholder();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Value
        {
            get => _showingPlaceholder ? string.Empty : _comboBox.Text;
            set
            {
                _showingPlaceholder = false;
                _comboBox.ForeColor = BuenosAiresTheme.TextColor;
                _comboBox.Font = BuenosAiresTheme.BodyFont;
                _comboBox.Text = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ComboBoxStyle DropDownStyle
        {
            get => _comboBox.DropDownStyle;
            set => _comboBox.DropDownStyle = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get => _comboBox.SelectedIndex;
            set => _comboBox.SelectedIndex = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object? SelectedItem
        {
            get => _comboBox.SelectedItem;
            set => _comboBox.SelectedItem = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object? SelectedValue
        {
            get => _comboBox.SelectedValue;
            set => _comboBox.SelectedValue = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object? DataSource
        {
            get => _comboBox.DataSource;
            set => _comboBox.DataSource = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DisplayMember
        {
            get => _comboBox.DisplayMember;
            set => _comboBox.DisplayMember = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ValueMember
        {
            get => _comboBox.ValueMember;
            set => _comboBox.ValueMember = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ComboBox.ObjectCollection Items => _comboBox.Items;

        public event EventHandler? SelectedIndexChanged
        {
            add => _comboBox.SelectedIndexChanged += value;
            remove => _comboBox.SelectedIndexChanged -= value;
        }

        public RoundedComboBox()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            Padding = new Padding(8, 4, 8, 4);
            BackColor = Color.Transparent;
            Height = BuenosAiresTheme.InputHeight;

            _comboBox = new ComboBox
            {
                FlatStyle = FlatStyle.Flat,
                Font = BuenosAiresTheme.BodyFont,
                ForeColor = BuenosAiresTheme.TextColor,
                BackColor = _fillColor,
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDown,
            };

            _comboBox.Enter += (_, _) => { HidePlaceholder(); _isFocused = true; Invalidate(); };
            _comboBox.Leave += (_, _) => { ShowPlaceholder(); _isFocused = false; Invalidate(); };
            _comboBox.SelectedIndexChanged += (_, _) => { HidePlaceholder(); Invalidate(); };
            _comboBox.MouseEnter += (_, _) => { _isHovering = true; Invalidate(); };
            _comboBox.MouseLeave += (_, _) => { _isHovering = false; Invalidate(); };
            _comboBox.MouseDown += (_, _) => OpenDropDown();

            MouseDown += (_, _) => OpenDropDown();

            Controls.Add(_comboBox);
            UpdateRegion();
        }

        private void OpenDropDown()
        {
            if (!Enabled)
                return;

            _comboBox.Focus();

            if (!_comboBox.DroppedDown)
            {
                _comboBox.DroppedDown = true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            var border = _isFocused ? _focusColor : _borderColor;
            var fill = _isHovering ? _hoverColor : _fillColor;

            using var path = CreateRoundedRectangle(rect, _borderRadius);
            using var bgBrush = new SolidBrush(fill);
            using var borderPen = new Pen(border, 1.5f);

            e.Graphics.FillPath(bgBrush, path);
            e.Graphics.DrawPath(borderPen, path);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRegion();
        }

        private void ShowPlaceholder()
        {
            if (_comboBox.DropDownStyle == ComboBoxStyle.DropDownList)
                return;

            if (_comboBox.SelectedIndex >= 0)
                return;

            if (!string.IsNullOrEmpty(_comboBox.Text))
                return;

            _showingPlaceholder = true;
            _comboBox.Text = _placeholder;
            _comboBox.ForeColor = BuenosAiresTheme.TextMutedColor;
            _comboBox.Font = BuenosAiresTheme.PlaceholderFont;
        }

        private void HidePlaceholder()
        {
            if (!_showingPlaceholder)
                return;

            _showingPlaceholder = false;
            _comboBox.Text = string.Empty;
            _comboBox.ForeColor = BuenosAiresTheme.TextColor;
            _comboBox.Font = BuenosAiresTheme.BodyFont;
        }

        private void UpdateRegion()
        {
            using var path = CreateRoundedRectangle(new Rectangle(0, 0, Width - 1, Height - 1), _borderRadius);
            Region = new Region(path);
        }

        private static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
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
