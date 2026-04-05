using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace BuenosAiresExp.UI
{
    public class StepBadge : UserControl
    {
        private int _number = 1;
        private bool _showLine = true;
        private int _circleSize = 36;
        private int _lineHeight = 40;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowLine
        {
            get => _showLine;
            set
            {
                _showLine = value;
                UpdateControlSize();
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CircleSize
        {
            get => _circleSize;
            set
            {
                _circleSize = Math.Max(12, value);
                UpdateControlSize();
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int LineHeight
        {
            get => _lineHeight;
            set
            {
                _lineHeight = Math.Max(0, value);
                UpdateControlSize();
                Invalidate();
            }
        }

        public StepBadge()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            Width = 36;
            UpdateControlSize();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int circleX = (Width - _circleSize) / 2;
            int circleY = 0;
            var circleRect = new Rectangle(circleX, circleY, _circleSize, _circleSize);

            using (var fillBrush = new SolidBrush(BuenosAiresTheme.PrimaryColorLight))
            using (var borderPen = new Pen(BuenosAiresTheme.PrimaryColor, 1.5f))
            {
                e.Graphics.FillEllipse(fillBrush, circleRect);
                e.Graphics.DrawEllipse(borderPen, circleRect);
            }

            using (var textBrush = new SolidBrush(BuenosAiresTheme.PrimaryColor))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                e.Graphics.DrawString(_number.ToString(), BuenosAiresTheme.ButtonFont, textBrush, circleRect, format);
            }

            if (_showLine)
            {
                int centerX = circleRect.Left + (circleRect.Width / 2);
                int startY = circleRect.Bottom;
                int endY = Height;

                using (var dashPen = new Pen(BuenosAiresTheme.BorderColor, 1.5f))
                {
                    dashPen.DashStyle = DashStyle.Dash;
                    e.Graphics.DrawLine(dashPen, centerX, startY, centerX, endY);
                }
            }
        }

        private void UpdateControlSize()
        {
            Height = _circleSize + (_showLine ? _lineHeight : 2);
        }
    }
}
