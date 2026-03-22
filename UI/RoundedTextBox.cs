using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace BuenosAiresExp.UI
{
    internal class RoundedTextBox : UserControl
    {
        private TextBox _textBox;
        private int _borderRadius = 10;
        private Color _borderColor = BuenosAiresTheme.BorderColor;
        private Color _focusColor = BuenosAiresTheme.PrimaryColor;
        private bool _isFocused = false; // Variável para controlar o estado de foco

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Placeholder
        {
            set => _textBox.PlaceholderText = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Value
        {
            get => _textBox.Text;
            set => _textBox.Text = value;

        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPassword
        {
            set => _textBox.UseSystemPasswordChar = value;
        }

        public RoundedTextBox()
        {

            // enable double buffering and custom painting to reduce flicker and artifacts
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);


            _textBox = new TextBox()
            {
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = BuenosAiresTheme.TextColor,
                Font = BuenosAiresTheme.BodyFont,
                Dock = DockStyle.Fill,
                Margin = new Padding(12, 6, 10, 6),

            };


            _textBox.GotFocus += (s, e) => { _isFocused = true; Invalidate(); };
            _textBox.LostFocus += (s, e) => { _isFocused = false; Invalidate(); };

            Padding = new Padding (8,5,8,5);
            BackColor = Color.White;
            Height = BuenosAiresTheme.InputHeight;

            Controls.Add(_textBox);

            UpdateRegion();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // usa o mesmo retângulo da região para evitar desalinhamento da borda
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
            // deixa o base pintar o fundo; o Region recorta o retângulo padrão
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
