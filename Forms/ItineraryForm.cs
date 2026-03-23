using BuenosAiresExp.Models;
using BuenosAiresExp.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BuenosAiresExp.Services;

namespace BuenosAiresExp
{
    // Form simples para montar um roteiro de viagem por dia, usando locais existentes.
    public class ItineraryForm : Form
    {
        private readonly List<Location> _availableLocations;
        private readonly List<Location> _roteiroDodia = new List<Location>();

        private DataGridView _dgvLocais;

        private DateTimePicker _datePicker;
        private FlowLayoutPanel _flowRoteiro;
        private RoundedButton _btnAdicionar;
        private RoundedButton _btnRemover;
        private RoundedButton _btnGerarRoteiro;


        public ItineraryForm(List<Location> locations)
        {
            _availableLocations = locations ?? new List<Location>();
            BuildLayout();


        }



        private void BuildLayout()
        {
            Text = "Roteiro da Viagem";
            Size = new Size(1000, 640);
            MinimumSize = new Size(800, 500);
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            BuenosAiresTheme.ApplyForm(this);

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                BackColor = BuenosAiresTheme.FillColor
            };


            split.HandleCreated += (s, e) =>
            {
                split.SplitterDistance = split.Width / 2;
            };

            Controls.Add(split);

            var lblLocaisTitle = new Label
            {
                Text = "Locais Disponíveis",
                Height = 32,
                Font = BuenosAiresTheme.SubtitleFont,
                ForeColor = BuenosAiresTheme.PrimaryColor,
                BackColor = BuenosAiresTheme.PrimaryColorLight,
                Dock = DockStyle.Top,
                Padding = new Padding(8, 8, 0, 0)
            };

            _dgvLocais = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToDeleteRows = false,
                BorderStyle = BorderStyle.None
            };
            BuenosAiresTheme.ApplyDataGridView(_dgvLocais);
            split.Panel1.Controls.Add(lblLocaisTitle);
            split.Panel1.Controls.Add(_dgvLocais);


            var colName = new DataGridViewTextBoxColumn
            {
                Name = "colName",
                HeaderText = "Nome",
                DataPropertyName = "Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            var colCategoria = new DataGridViewTextBoxColumn
            {
                Name = "colCategoria",
                HeaderText = "Categoria",
                DataPropertyName = "Category",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            var colEndereco = new DataGridViewTextBoxColumn
            {
                Name = "colEndereco",
                HeaderText = "Endereco",
                DataPropertyName = "Address",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            _dgvLocais.Columns.AddRange(colName, colCategoria, colEndereco);
            _dgvLocais.DataSource = _availableLocations;

            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = BuenosAiresTheme.PrimaryColorLight
            };

            var lblRoteiroTitle = new Label
            {
                Text = "Roteiro do Dia",
                Font = BuenosAiresTheme.SubtitleFont,
                ForeColor = BuenosAiresTheme.PrimaryColor,
                AutoSize = true,
                Location = new Point(12, 8)
            };

            _datePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 120,
                Location = new Point(12, 30)
            };

            pnlHeader.Controls.Add(lblRoteiroTitle);
            pnlHeader.Controls.Add(_datePicker);

            _flowRoteiro = new FlowLayoutPanel
            {
                BackColor = BuenosAiresTheme.FillColor,
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(8)
            };

            var pnlButtons = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                ColumnCount = 3,
                RowCount = 1,
                Height = 52,
                Padding = new Padding(12, 8, 12, 8),
                BackColor = BuenosAiresTheme.PrimaryColorLight
            };

            split.Panel2.Controls.Add(pnlButtons);
            split.Panel2.Controls.Add(pnlHeader);
            split.Panel2.Controls.Add(_flowRoteiro);


            _btnAdicionar = new RoundedButton
            {
                Text = "+ Adicionar",
                Width = 110,
                Font = BuenosAiresTheme.ButtonFont,
                ForeColor = Color.White,
                FillColor = BuenosAiresTheme.PrimaryColor,
            };

            _btnRemover = new RoundedButton
            {
                Text = "Remover",
                Width = 100,
                Font = BuenosAiresTheme.ButtonFont,
                ForeColor = BuenosAiresTheme.DangerColor,
                HoverColor = Color.FromArgb(255, 240, 240),
                FillColor = Color.Transparent,
            };

            _btnGerarRoteiro = new RoundedButton
            {
                Text = "Gerar Roteiro",
                Width = 100,
                Font = BuenosAiresTheme.ButtonFont,
                ForeColor = BuenosAiresTheme.HeaderColor,
                FillColor = BuenosAiresTheme.AccentColor,
                HoverColor = BuenosAiresTheme.AccentColorLight,
            };

            pnlButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3f));
            pnlButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3f));
            pnlButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3f));

            _btnAdicionar.Anchor = AnchorStyles.None;
            _btnRemover.Anchor = AnchorStyles.None;
            _btnGerarRoteiro.Anchor = AnchorStyles.None;

            pnlButtons.Controls.Add(_btnAdicionar, 0, 0);
            pnlButtons.Controls.Add(_btnRemover, 1, 0);
            pnlButtons.Controls.Add(_btnGerarRoteiro, 2, 0);
        }

        private Panel CreateCard(Location location, string distance)
        {
            var card = new Panel
            {
                Width = _flowRoteiro.ClientSize.Width - 20,
                Height = BuenosAiresTheme.CardHeight,
                BackColor = BuenosAiresTheme.SurfaceColor,
                Padding = new Padding(BuenosAiresTheme.CardPadding),
                Margin = new Padding(4, 4, 4, 0),
                Cursor = Cursors.Hand
            };
            var lblNome = new Label
            {
                Text = location.Name,
                Font = BuenosAiresTheme.CardTitleFont,
                ForeColor = BuenosAiresTheme.PrimaryColor,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var lblCategoria = new Label
            {
                Text = location.Category,
                Font = BuenosAiresTheme.CardBodyFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                AutoSize = true,
                Location = new Point(0, 20)
            };

            card.Controls.Add(lblNome);
            card.Controls.Add(lblCategoria);

            return card;


        }

    }
}
