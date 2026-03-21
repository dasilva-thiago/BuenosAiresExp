using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BuenosAiresExp.UI;
using System.Linq;
using System.Threading.Tasks;

namespace BuenosAiresExp
{
    public partial class LocationForm : Form
    {
        public LocationForm()
        {
            InitializeComponent();
            Text = "Buenos Aires Explorer";
            Size = new Size(1024, 640);
            MinimumSize = new Size(800, 500);
            StartPosition = FormStartPosition.CenterScreen;
        }
    }
}
