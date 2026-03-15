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
        private int _borderRadius = 6;
        private Color _borderColor = BuenosAiresTheme.BorderColor;
        private Color _focusColor = BuenosAiresTheme.PrimaryColor;
        private bool _isFocused = false; // Variável para controlar o estado de foco


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Value
        {
            get => _textBox.Text;
            set => _textBox.Text = value;

        }

        public bool IsPassword
        {
            set => _textBox.UseSystemPasswordChar = value;
        }

    }
}
