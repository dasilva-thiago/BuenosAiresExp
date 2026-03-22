using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BuenosAiresExp.Models;

namespace BuenosAiresExp
{
    // Form simples para montar um roteiro de viagem por dia, usando locais existentes.
    public class ItineraryForm : Form
    {
        private readonly List<Location> _availableLocations;

        private DataGridView _dgvLocais;
        private TextBox _txtDia;
        private ListBox _lstRoteiro;
        private Button _btnAdicionar;
        private Button _btnRemover;
        private Button _btnSalvarTxt;

        public ItineraryForm(List<Location> locations)
        {
            _availableLocations = locations ?? new List<Location>();

            InitializeComponent();
            BuildLayout();
            WireEvents();
        }

        private void InitializeComponent()
        {
            Text = "Roteiro da viagem";
            Size = new Size(900, 600);
            MinimumSize = new Size(700, 500);
            StartPosition = FormStartPosition.CenterParent;
        }

        private void BuildLayout()
        {
            // grid com locais disponiveis
            _dgvLocais = new DataGridView
            {
                Dock = DockStyle.Left,
                Width = 420,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
            };

            var colName = new DataGridViewTextBoxColumn
            {
                Name = "colName",
                HeaderText = "Nome",
                DataPropertyName = "Name",
                FillWeight = 40,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            var colCategoria = new DataGridViewTextBoxColumn
            {
                Name = "colCategoria",
                HeaderText = "Categoria",
                DataPropertyName = "Category",
                FillWeight = 30,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            var colEndereco = new DataGridViewTextBoxColumn
            {
                Name = "colEndereco",
                HeaderText = "Endereço",
                DataPropertyName = "Address",
                FillWeight = 30,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            _dgvLocais.Columns.AddRange(colName, colCategoria, colEndereco);
            _dgvLocais.DataSource = _availableLocations;

            // painel direito com dia + roteiro
            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var lblDia = new Label
            {
                Text = "Dia (ex.: Segunda, 01/05, etc.):",
                AutoSize = true,
                Location = new Point(10, 10)
            };

            _txtDia = new TextBox
            {
                Location = new Point(10, 32),
                Width = 250
            };

            _btnAdicionar = new Button
            {
                Text = "Adicionar ao roteiro",
                Location = new Point(280, 30),
                Width = 150
            };

            _lstRoteiro = new ListBox
            {
                Location = new Point(10, 70),
                Size = new Size(420, 380)
            };

            _btnRemover = new Button
            {
                Text = "Remover selecionado",
                Location = new Point(10, 460),
                Width = 160
            };

            _btnSalvarTxt = new Button
            {
                Text = "Salvar roteiro (.txt)",
                Location = new Point(200, 460),
                Width = 160
            };

            rightPanel.Controls.Add(lblDia);
            rightPanel.Controls.Add(_txtDia);
            rightPanel.Controls.Add(_btnAdicionar);
            rightPanel.Controls.Add(_lstRoteiro);
            rightPanel.Controls.Add(_btnRemover);
            rightPanel.Controls.Add(_btnSalvarTxt);

            Controls.Add(rightPanel);
            Controls.Add(_dgvLocais);
        }

        private void WireEvents()
        {
            _btnAdicionar.Click += (s, e) => AddSelectedLocationToItinerary();
            _btnRemover.Click += (s, e) => RemoveSelectedItineraryItem();
            _btnSalvarTxt.Click += (s, e) => SaveItineraryAsText();
        }

        private void AddSelectedLocationToItinerary()
        {
            if (_dgvLocais.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um local na lista.", "Roteiro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var dia = _txtDia.Text.Trim();
            if (string.IsNullOrWhiteSpace(dia))
            {
                MessageBox.Show("Informe o dia (por exemplo: Segunda, 10/03, etc.).", "Roteiro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var location = _dgvLocais.SelectedRows[0].DataBoundItem as Location;
            if (location == null)
            {
                return;
            }

            var line = $"{dia}: {location.Name} - {location.Category} - {location.Address}";
            _lstRoteiro.Items.Add(line);
        }

        private void RemoveSelectedItineraryItem()
        {
            var index = _lstRoteiro.SelectedIndex;
            if (index < 0)
            {
                return;
            }

            _lstRoteiro.Items.RemoveAt(index);
        }

        private void SaveItineraryAsText()
        {
            if (_lstRoteiro.Items.Count == 0)
            {
                MessageBox.Show("Nenhum item no roteiro para salvar.", "Roteiro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Title = "Salvar roteiro";
                dialog.Filter = "Arquivo de texto (*.txt)|*.txt";
                dialog.FileName = "roteiro_viagem.txt";

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var lines = _lstRoteiro.Items.Cast<object>()
                    .Select(item => item?.ToString() ?? string.Empty)
                    .Where(text => !string.IsNullOrWhiteSpace(text));

                var header = "Roteiro da viagem" + Environment.NewLine + new string('-', 40) + Environment.NewLine;
                var content = header + string.Join(Environment.NewLine, lines);

                File.WriteAllText(dialog.FileName, content);

                MessageBox.Show("Roteiro salvo com sucesso.", "Roteiro", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
