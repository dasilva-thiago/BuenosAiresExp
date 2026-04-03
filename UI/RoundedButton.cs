using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BuenosAiresExp.UI
{
    public class RoundedButton : Button
    {
        private int _borderRadius = 6;
        private Color _fillColor = BuenosAiresTheme.PrimaryColor;
        private Color _textColor = Color.White;
        private Color _hoverColor = Color.FromArgb(21, 65, 115);
        private bool _isHovering = false;


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color FillColor
        {
            get => _fillColor;
            set { _fillColor = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color HoverColor
        {
            get => _hoverColor;
            set { _hoverColor = value; Invalidate(); }
        }

        public RoundedButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            ForeColor = _textColor;
            Font = BuenosAiresTheme.ButtonFont;
            Height = BuenosAiresTheme.ButtonHeight;
            Cursor = Cursors.Hand; // Cursor de mão para indicar que é clicável
            DoubleBuffered = true; // Habilita o double buffering para reduzir o flickering

            SetStyle(ControlStyles.UserPaint |
             ControlStyles.AllPaintingInWmPaint |
             ControlStyles.DoubleBuffer, true);
        }

        protected override void WndProc(ref Message m)
        {
            // Bloqueia o paint nativo do Windows (WM_PAINT = 0xF)
            if (m.Msg == 0xF)
            {
                base.WndProc(ref m);
                return;
            }
            base.WndProc(ref m);
        }

        // mouse hover

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovering = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovering = false;
            Invalidate();
            base.OnMouseLeave(e);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor == Color.Transparent
                ? Parent?.BackColor ?? SystemColors.Control
                : BackColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color currentColor = _isHovering ? _hoverColor : _fillColor;
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            using (GraphicsPath path = CreateRoundedRectangle(rect, _borderRadius))
            {
                // Preenche o fundo somente se não for transparente
                if (currentColor != Color.Transparent)
                {
                    using (SolidBrush brush = new SolidBrush(currentColor))
                        e.Graphics.FillPath(brush, path);
                }

                // Desenha borda quando for transparente
                if (_fillColor == Color.Transparent)
                {
                    using (Pen borderPen = new Pen(ForeColor, 1.5f))
                        e.Graphics.DrawPath(borderPen, path);
                }

                // Desenha ícone centralizado quando existir imagem e não houver texto
                if (Image != null && string.IsNullOrEmpty(Text))
                {
                    var imgSize = Image.Size;
                    int imgX = (Width - imgSize.Width) / 2;
                    int imgY = (Height - imgSize.Height) / 2;

                    // Garante que o alpha do PNG seja respeitado durante a renderização
                    e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    e.Graphics.DrawImage(Image, imgX, imgY, imgSize.Width, imgSize.Height);
                }

                else
                {
                    // Caso padrão: desenha texto centralizado
                    StringFormat format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    using (SolidBrush textBrush = new SolidBrush(ForeColor))
                        e.Graphics.DrawString(Text, Font, textBrush, rect, format);
                }
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;

        }
    }
}
