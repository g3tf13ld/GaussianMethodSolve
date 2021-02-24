using System;
using System.ComponentModel;
using System.Windows.Forms;
using GaussianSolution;

namespace GaussSolution {
    public partial class MainForm : Form {

        public MainForm() {
            InitializeComponent();
        }

        private string DefaultText = String.Format("{0:f}", 0.0);
        
        // Creating textbox and initializing properties.
        private TextBox InitTextBox(bool readOnly) {
            var textBox = new TextBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right, Text = DefaultText, ReadOnly = readOnly
            };
            if (!readOnly) {
                textBox.CausesValidation = true;
                textBox.Validating += ValidateTextBox;
            }
            return textBox;
        }

        // Validating textbox text.
        private void ValidateTextBox(object sender, CancelEventArgs e) {
            var textBox = (TextBox)sender;
            e.Cancel = !double.TryParse(textBox.Text, out _);
        }
        
        // Creating 2-dimensional textbox array by inserting each layout table.
        private TextBox[,] InitTextBoxMatrix(TableLayoutPanel layoutPanel, int count, bool readOnly) {
            layoutPanel.SuspendLayout();

            layoutPanel.Controls.Clear();

            layoutPanel.ColumnStyles.Clear();
            layoutPanel.ColumnCount = count;

            layoutPanel.RowStyles.Clear();
            layoutPanel.RowCount = count;

            var result = new TextBox[count, count];
            var cellSize = 1f / count * 100f;

            for (var col = 0; col < count; ++col) {
                layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, cellSize));
                for (var row = 0; row < count; ++row) {
                    layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, cellSize));

                    var textBox = InitTextBox(readOnly);

                    layoutPanel.Controls.Add(textBox, col, row);
                    result[col, row] = textBox;
                }
            }

            layoutPanel.ResumeLayout(true);

            return result;
        }

        // Creating 1-dimensional textbox array by inserting each layout table.
        private TextBox[] InitTextBoxArray(TableLayoutPanel layoutPanel, int count, bool readOnly) {
            layoutPanel.SuspendLayout();

            layoutPanel.Controls.Clear();

            layoutPanel.ColumnStyles.Clear();
            layoutPanel.ColumnCount = 1;

            layoutPanel.RowStyles.Clear();
            layoutPanel.RowCount = count;

            var result = new TextBox[count];
            var cellSize = 1f / count * 100f;

            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            for (var row = 0; row < count; ++row) {
                layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, cellSize));

                TextBox textBox = InitTextBox(readOnly);

                layoutPanel.Controls.Add(textBox, 0, row);
                result[row] = textBox;
            }

            layoutPanel.ResumeLayout(true);

            return result;
        }

        private int n;
        private TextBox[,] matrixA;
        private TextBox[] vectorB;
        private TextBox[] vectorX;

        private void InitMatrixA() {
            matrixA = InitTextBoxMatrix(layoutMatrixA, n, false);
        }

        private void InitVectorX() {
            vectorX = InitTextBoxArray(layoutVectorX, n, true);
        }

        private void InitVectorB() {
            vectorB = InitTextBoxArray(layoutVectorB, n, false);
        }

        public int N {
            get { return n; }
            set {
                if (value != n && value > 0) {
                    n = value;
                    InitMatrixA();
                    InitVectorX();
                    InitVectorB();
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e) {
            N = (int)numericUpDown1.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            N = (int)numericUpDown1.Value;
        }

        private void button1_Click(object sender, EventArgs e) {
            if (Validate()) {
                try {
                    var linearSystem = new LinearSystem(MatrixA, VectorB);
                    VectorX = linearSystem.XVector;
                } catch (Exception error) {
                    MessageBox.Show(error.Message);
                }
            }
        }

        public double[,] MatrixA {
            get {
                // Build the A-matrix entered by the user.
                var tempMatrixA = new double[n, n];
                for (var i = 0; i < n; ++i)
                    for (var j = 0; j < n; ++j)
                        tempMatrixA[i, j] = double.Parse(matrixA[j, i].Text);
                return tempMatrixA;
            }
            set {
                // Write the A-matrix into textboxes.
                for (var i = 0; i < n; ++i)
                    for (var j = 0; j < n; ++j)
                        matrixA[j, i].Text = value[i, j].ToString("f");
            }
        }

        public double[] VectorB {
            get {
                // Build the B-vector entered by the user.
                var tempVectorB = new double[n];
                for (var j = 0; j < n; ++j)
                    tempVectorB[j] = double.Parse(vectorB[j].Text);
                return tempVectorB;
            }
            set {
                // Write the B-vector into textboxes.
                for (var j = 0; j < n; ++j)
                    vectorB[j].Text = value[j].ToString("f");
            }
        }

        public double[] VectorX {
            set {
                // Showing the calculated X-result.
                for (var j = 0; j < n; ++j)
                    vectorX[j].Text = value[j].ToString("f");
            }
        }

        private void Example1ToolStripMenuItem_Click(object sender, EventArgs e) {
            numericUpDown1.Value = 3;
            MatrixA = new double[,]
                { { 3.0, -9.0,   3.0 },
                  { 2.0, -4.0,   4.0 },
                  { 1.0,  8.0, -18.0 } };
            VectorB = new double[] { -18.0, -10.0, 35.0 };
        }

        private void Example2ToolStripMenuItem_Click(object sender, EventArgs e) {
            numericUpDown1.Value = 4;
            MatrixA = new double[,]
                { { 2.0, 3.0, 5.0,  2.0 },
                  { 2.0, 2.0, 4.0,  3.0 },
                  { 3.0, 6.0, 3.0,  5.0 },
                  { 2.0, 5.0, 3.0,  7.0  } };
            VectorB = new double[] { 4.0, 3.0, 8.0, 9.0 };
        }

        private void Example3ToolStripMenuItem_Click(object sender, EventArgs e) {
            numericUpDown1.Value = 4;
            MatrixA = new double[,]
                { { 1.0, 6.0, 4.0,    1.0 },
                  { 6.0, 4.0, 4.0,    6.0 },
                  { 0.0, 1.0, 566.0,  7.0 },
                  { 2.0, 4.0, 5.0,    6.0  } };
            VectorB = new double[] { 4.0, 6.0, 6.0, 6.0 };
        }

        private void Example4ToolStripMenuItem_Click(object sender, EventArgs e) {
            numericUpDown1.Value = 5;
            MatrixA = new double[,] {
                {7.0, 1.0, 3.0, 2.0, 4.0},
                {9.0, 2.0, 4.0, 2.0, 1.0},
                {4.0, 2.0, 1.0, 3.0, 4.0},
                {1.0, 3.0, 1.0, 2.0, 1.0},
                {2.0, 1.0, 2.0, 4.0, 3.0}
            };
            VectorB = new double[] { 4.0, 2.0, 5.0, 3.0, 1.0 };
        }

    }
}