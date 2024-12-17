using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace playNumbers
{
    public partial class Form1 : Form
    {
        private int targetNumber; // ������� �����, ������� ����� �������
        private int attempts; // ���������� �������
        private Label messageLabel; // ����� ��� ����������� ���������
        private string failedAttemptsText = "��������� �������: "; // ����� ��� ����������� ��������� �������
        private bool isCheckingGuess = false; // ���������� ���������

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // ��������� ������� ��������-����
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 30; // �������� �� ���������
            progressBar1.Value = 15; // ������������� ��������� ��������
            attempts = 0; // ������������� �������

            // �������� ����� ��� ���������
            messageLabel = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(10, 100), // ������� �����
                Visible = false // ������� ��������
            };
            this.Controls.Add(messageLabel); // ��������� ����� �� �����

            // �������� ����� ��� ��������� �������
            Label failedAttemptsLabel = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(10, 130), // ������� ����� ��� ��������� �������
                Text = failedAttemptsText
            };
            this.Controls.Add(failedAttemptsLabel); // ��������� ����� �� �����

            // �������� �� �������
            this.KeyDown += Form1_KeyDown;
            this.KeyPreview = true; // ���������, ��� �������� KeyPreview ����������� � true
            radioButton1.CheckedChanged += RadioButton_CheckedChanged;
            radioButton2.CheckedChanged += RadioButton_CheckedChanged;
            radioButton3.CheckedChanged += RadioButton_CheckedChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ResetGame(); // ����� ���� ��� �������� �����
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (IsGameReady())
            {
                CheckGuess();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && IsGameReady())
            {
                CheckGuess();
                e.Handled = true; // ���������, ��� ������� ����������
            }
        }

        private bool IsGameReady()
        {
            if (targetNumber == 0)
            {
                MessageBox.Show("����������, �������� ������� ��������� � ������� ����.");
                return false; // ���� �� ������
            }
            return true; // ���� ������
        }

        private async void CheckGuess()
        {
            if (isCheckingGuess) return;

            isCheckingGuess = true; // ������������� ��������� ��������
            ToggleInput(false); // ��������� ����

            if (int.TryParse(textBox1.Text, out int userGuess))
            {
                attempts++; // ����������� ���������� �������
                await ProcessGuess(userGuess);
            }
            else
            {
                await ShowMessage("������� ���������� �����.");
            }

            ToggleInput(true); // �������� ����
            isCheckingGuess = false; // ���������� ��������� ��������
        }

        private async Task ProcessGuess(int userGuess)
        {
            if (userGuess == targetNumber)
            {
                await ShowMessage("�����������! �� ������� �����!");
                ResetGame(); // ����� ���� ����� ������
            }
            else
            {
                if (progressBar1.Value > progressBar1.Minimum)
                {
                    progressBar1.Value--;
                    failedAttemptsText += userGuess + ", "; // ��������� ��������� �������
                    UpdateFailedAttemptsLabel(); // ��������� ����� ��������� �������
                    await ShowMessage("�����������! ���������� �����.");
                    label1.Text += $", {userGuess}";
                    textBox1.Clear();
                    textBox1.Focus();
                }
                else
                {
                    await ShowMessage($"�� ��������� :( ���������� ����� ���� {targetNumber}.");
                    ResetGame(); // ����� ���� ����� ���������
                }
            }
        }

        private async Task ShowMessage(string message)
        {
            MessageBox.Show(message);
            textBox1.Focus();
        }

        private void UpdateFailedAttemptsLabel()
        {
            // ��������� ����� ����� ��� ��������� �������
            var failedAttemptsLabel = this.Controls.OfType<Label>().FirstOrDefault(l => l.Location == new System.Drawing.Point(10, 130));
            if (failedAttemptsLabel != null)
            {
                failedAttemptsLabel.Text = failedAttemptsText.TrimEnd(',', ' '); // ������� ��������� ������� � ������
            }
        }

        private void UpdateGameDifficulty()
        {
            // ���������� ������ ��������� � �������� �����
            if (radioButton1.Checked)
            {
                progressBar1.Maximum = 2; // ������� ��������
                GenerateTargetNumber(1, 3);
            }
            else if (radioButton2.Checked)
            {
                progressBar1.Maximum = 10; // ������� ����������
                GenerateTargetNumber(1, 15);
            }
            else if (radioButton3.Checked)
            {
                progressBar1.Maximum = 15; // ������� ���������
                GenerateTargetNumber(1, 20);
            }
        }

        private void GenerateTargetNumber(int min, int max, bool isEven = false)
        {
            label3.Text += $" {min},{max}";
            this.Text = $"{progressBar1.Maximum}";
            Random random = new Random();
            if (isEven)
            {
                List<int> evenNumbers = new List<int>();
                for (int i = min; i <= max; i++)
                {
                    if (i % 2 == 0) evenNumbers.Add(i);
                }
                targetNumber = evenNumbers[random.Next(evenNumbers.Count)];
            }
            else
            {
                targetNumber = random.Next(min, max + 1);
            }
            radioButton1.Enabled = false ; radioButton2.Enabled = false; radioButton3.Enabled = false;
        }

        private void ResetGame()
        {
            // ����� ��������� ����
            targetNumber = 0; // ���������� ������� �����
            attempts = 0; // ����� �������
            progressBar1.Value = progressBar1.Maximum; // ����� ��������-����
            textBox1.Clear(); // ������� ���������� ����
            messageLabel.Visible = false; // �������� ���������
            failedAttemptsText = "��������� �������: "; // ���������� ����� ��������� �������
            UpdateFailedAttemptsLabel(); // ��������� ����� ��������� �������
            label1.Text = "���� ������� ";
            label3.Text = "���� ����� ...";

            // ���������� �����������
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton1.Enabled = true; radioButton2.Enabled = true; radioButton3.Enabled = true;


            // ��������� ������ ��������
            button1.Enabled = false;

            // ���������� ��������� � ������ ����
            MessageBox.Show("���� ��������. �������� ������� ��������� � ��������� ����!");
        }

        private void ToggleInput(bool isEnabled)
        {
            button1.Enabled = isEnabled; // ��������/��������� ������ ��������
            textBox1.Enabled = isEnabled; // ��������/��������� ��������� ����
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // �������� ��������� ����������� � ��������� ������ ��������
            if (radioButton1.Checked || radioButton2.Checked || radioButton3.Checked)
            {
                button1.Enabled = true; // ��������� ������ ��������, ���� ������� ���������
                UpdateGameDifficulty(); // ��������� ������� ��������� ��� ��������� �����������
            }
        }

        //debug �����
        private void label2_Click(object sender, EventArgs e)
        {
           this.Text += $" {targetNumber}";
        }
    }
}
