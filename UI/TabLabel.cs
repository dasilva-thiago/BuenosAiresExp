using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BuenosAiresExp.UI
{
    public class TabLabel : UserControl
    {
        private readonly FlowLayoutPanel _flowContent;
        private readonly Label _lblIcon;
        private readonly Label _lblText;
        private readonly Panel _pnlIndicator;

        private bool _isActive;
        private bool _isHovering;
        private object? _icon;
        private Control? _trackedParent;
        private readonly System.Windows.Forms.Timer _hoverGuardTimer;

        public event EventHandler? TabClicked;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get => _lblText.Text;
            set
            {
                _lblText.Text = value;
                CenterFlowContent();
            }
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object? Icon
        {
            get => _icon;
            set
            {
                _icon = value;

                if (value is Image image)
                {
                    _lblIcon.Image = image;
                    _lblIcon.Text = string.Empty;
                    _lblIcon.AutoSize = false;
                    _lblIcon.Size = new Size(20, 20);
                }
                else
                {
                    _lblIcon.Image = null;
                    _lblIcon.AutoSize = true;
                    _lblIcon.Text = value?.ToString() ?? string.Empty;
                }

                CenterFlowContent();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                ApplyVisualState();
            }
        }

        public TabLabel()
        {
            BackColor = Color.White;
            Cursor = Cursors.Hand;
            DoubleBuffered = true;
            //Height = 52;
            Margin = new Padding(0);

            _flowContent = new FlowLayoutPanel
            {
                AutoSize = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            _lblIcon = new Label
            {
                AutoSize = true,
                Font = BuenosAiresTheme.BodyFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                Margin = new Padding(0, 0, 6, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };

            _lblText = new Label
            {
                AutoSize = true,
                Font = BuenosAiresTheme.ButtonFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                Margin = new Padding(0),
                TextAlign = ContentAlignment.MiddleCenter
            };

            _flowContent.Controls.Add(_lblIcon);
            _flowContent.Controls.Add(_lblText);
            Controls.Add(_flowContent);

            _pnlIndicator = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 2,
                BackColor = BuenosAiresTheme.PrimaryColor,
                Visible = false
            };

            Controls.Add(_pnlIndicator);

            WireInteractions(this);
            WireInteractions(_flowContent);
            WireInteractions(_lblIcon);
            WireInteractions(_lblText);

            Resize += (_, _) => CenterFlowContent();
            ParentChanged += TabLabel_ParentChanged;

            _hoverGuardTimer = new System.Windows.Forms.Timer { Interval = 60 };
            _hoverGuardTimer.Tick += HoverGuardTimer_Tick;

            ApplyVisualState();
        }

        private void HoverGuardTimer_Tick(object? sender, EventArgs e)
        {
            if (!Visible)
            {
                SetHoverState(false);
                return;
            }

            var boundsOnScreen = RectangleToScreen(ClientRectangle);
            if (!boundsOnScreen.Contains(Cursor.Position))
            {
                SetHoverState(false);
            }
        }

        private void TabLabel_ParentChanged(object? sender, EventArgs e)
        {
            if (_trackedParent != null)
            {
                _trackedParent.MouseMove -= TrackedParent_MouseMove;
            }

            _trackedParent = Parent;

            if (_trackedParent != null)
            {
                _trackedParent.MouseMove += TrackedParent_MouseMove;
            }
        }

        private void TrackedParent_MouseMove(object? sender, MouseEventArgs e)
        {
            var cursorPoint = PointToClient(Cursor.Position);
            if (!ClientRectangle.Contains(cursorPoint))
            {
                SetHoverState(false);
            }
        }

        private void WireInteractions(Control control)
        {
            control.MouseEnter += (_, _) => SetHoverState(true);
            control.MouseLeave += (_, _) =>
            {
                var cursorPoint = PointToClient(Cursor.Position);
                if (!ClientRectangle.Contains(cursorPoint))
                {
                    SetHoverState(false);
                }
            };
            control.Click += (_, _) => TabClicked?.Invoke(this, EventArgs.Empty);
        }

        private void SetHoverState(bool isHovering)
        {
            if (_isHovering == isHovering)
                return;

            _isHovering = isHovering;

            if (_isHovering)
            {
                _hoverGuardTimer.Start();
            }
            else
            {
                _hoverGuardTimer.Stop();
            }

            if (!_isActive)
            {
                BackColor = _isHovering ? BuenosAiresTheme.PrimaryColorLight : Color.White;
            }
        }

        private void ApplyVisualState()
        {
            _pnlIndicator.Visible = _isActive;
            _lblText.ForeColor = _isActive ? BuenosAiresTheme.PrimaryColor : BuenosAiresTheme.TextMutedColor;
            _lblIcon.ForeColor = _isActive ? BuenosAiresTheme.PrimaryColor : BuenosAiresTheme.TextMutedColor;
            BackColor = _isActive ? Color.White : (_isHovering ? BuenosAiresTheme.PrimaryColorLight : Color.White);
            CenterFlowContent();
        }

        private void CenterFlowContent()
        {
            if (_flowContent.Parent == null)
            {
                return;
            }

            int contentY = (Height - _pnlIndicator.Height - _flowContent.Height) / 2;
            if (contentY < 0)
            {
                contentY = 0;
            }

            _flowContent.Location = new Point(
                Math.Max((Width - _flowContent.Width) / 2, 0),
                contentY);
        }

    }
}
