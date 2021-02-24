using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Gauss {
    public partial class MainForm : Form {

        public MainForm() {
            InitializeComponent();
        }

        private string DefaultText = String.Format("{0:f}", 0.0);

        // создает текстарию и назначает ему кое какие свойства
        private TextBox InitTextBox(bool readOnly) {
            TextBox textBox = new TextBox();
            textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox.Text = DefaultText;
            textBox.ReadOnly = readOnly;
            if (!readOnly) {
                textBox.CausesValidation = true;
                textBox.Validating += ValidateTextBox;
            }
            return textBox;
        }

        // проверка валидности текста в текстарии
        private void ValidateTextBox(object sender, CancelEventArgs e) {
            TextBox textBox = (TextBox)sender;
            double result;
            e.Cancel = !double.TryParse(textBox.Text, out result);
        }

        // создаЄт вдумерный массив текстарий, вставл€€ каждую в таблицу расположени€
        private TextBox[,] InitTextBoxMatrix(TableLayoutPanel layoutPanel, int count, bool readOnly) {
            layoutPanel.SuspendLayout();

            layoutPanel.Controls.Clear();

            layoutPanel.ColumnStyles.Clear();
            layoutPanel.ColumnCount = count;

            layoutPanel.RowStyles.Clear();
            layoutPanel.RowCount = count;

            TextBox[,] result = new TextBox[count, count];
            float cellSize = 1f / count * 100f;

            for (int col = 0; col < count; ++col) {
                layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, cellSize));
                for (int row = 0; row < count; ++row) {
                    layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, cellSize));

                    TextBox textBox = InitTextBox(readOnly);

                    layoutPanel.Controls.Add(textBox, col, row);
                    result[col, row] = textBox;
                }
            }

            layoutPanel.ResumeLayout(true);

            return result;
        }

        // создаЄт одномерный массив текстарий, вставл€€ каждую таблицу расположени€
        private TextBox[] InitTextBoxArray(TableLayoutPanel layoutPanel, int count, bool readOnly) {
            layoutPanel.SuspendLayout();

            layoutPanel.Controls.Clear();

            layoutPanel.ColumnStyles.Clear();
            layoutPanel.ColumnCount = 1;

            layoutPanel.RowStyles.Clear();
            layoutPanel.RowCount = count;

            TextBox[] result = new TextBox[count];
            float cellSize = 1f / count * 100f;

            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            for (int row = 0; row < count; ++row) {
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
        private TextBox[] vectorU;

        private void InitMatrixA() {
            matrixA = InitTextBoxMatrix(layoutMatrixA, n, false);
        }

        private void InitVectorX() {
            vectorX = InitTextBoxArray(layoutVectorX, n, true);
        }

        private void InitVectorB() {
            vectorB = InitTextBoxArray(layoutVectorB, n, false);
        }

        private void InitVectorU() {
            vectorU = InitTextBoxArray(layoutVectorU, n, true);
        }

        public int N {
            get { return n; }
            set {
                if (value != n && value > 0) {
                    n = value;
                    InitMatrixA();
                    InitVectorX();
                    InitVectorB();
                    InitVectorU();
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e) {
            N = (int)numericUpDown1.Value;
            ShowAboutBox();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            N = (int)numericUpDown1.Value;
        }

        private void button1_Click(object sender, EventArgs e) {
            if (Validate()) {
                try {
                    LinearSystem system = new LinearSystem(MatrixA, VectorB);
                    VectorX = system.XVector;
                    VectorU = system.UVector;
                } catch (Exception error) {
                    MessageBox.Show(error.Message);
                }
            }
        }

        public double[,] MatrixA {
            get {
                // собираем введЄнную пользователем матрицу A
                double[,] matrix_a = new double[n, n];
                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                        matrix_a[i, j] = double.Parse(matrixA[j, i].Text);
                return matrix_a;
            }
            set {
                // записываем в текстбоксы матрицу A
                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                        matrixA[j, i].Text = value[i, j].ToString("f");
            }
        }

        public double[] VectorB {
            get {
                // собираем введЄнный пользователем вектор B
                double[] vector_b = new double[n];
                for (int j = 0; j < n; ++j)
                    vector_b[j] = double.Parse(vectorB[j].Text);
                return vector_b;
            }
            set {
                // записываем в текстбоксы вектор B
                for (int j = 0; j < n; ++j)
                    vectorB[j].Text = value[j].ToString("f");
            }
        }

        public double[] VectorX {
            set {
                // показываем вычисленный результат X
                for (int j = 0; j < n; ++j)
                    vectorX[j].Text = value[j].ToString("f");
            }
        }

        public double[] VectorU {
            set {
                // показываем вычисленную нев€зку U
                for (int j = 0; j < n; ++j)
                    vectorU[j].Text = value[j].ToString("e");
            }
        }

        private void ShowAboutBox() {
            new AboutBox().ShowDialog(this);
        }

        private void оѕрогаммеToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowAboutBox();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void пример1ToolStripMenuItem_Click(object sender, EventArgs e) {
            numericUpDown1.Value = 3;
            MatrixA = new double[,]
                { { 3.0, -9.0,   3.0 },
                  { 2.0, -4.0,   4.0 },
                  { 1.0,  8.0, -18.0 } };
            VectorB = new double[] { -18.0, -10.0, 35.0 };
        }

        private void пример2ToolStripMenuItem_Click(object sender, EventArgs e) {
            numericUpDown1.Value = 4;
            MatrixA = new double[,]
                { { 2.0, 3.0, 5.0,  2.0 },
                  { 2.0, 2.0, 4.0,  3.0 },
                  { 3.0, 6.0, 3.0,  5.0 },
                  { 2.0, 5.0, 3.0,  7.0  } };
            VectorB = new double[] { 4.0, 3.0, 8.0, 9.0 };
        }

        private void пример3ToolStripMenuItem_Click(object sender, EventArgs e) {
            numericUpDown1.Value = 4;
            MatrixA = new double[,]
                { { 1.0, 6.0, 4.0,    1.0 },
                  { 6.0, 4.0, 4.0,    6.0 },
                  { 0.0, 1.0, 566.0,  7.0 },
                  { 2.0, 4.0, 5.0,    6.0  } };
            VectorB = new double[] { 4.0, 6.0, 6.0, 6.0 };
        }

        private void пример4ToolStripMenuItem_Click(object sender, EventArgs e) {
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