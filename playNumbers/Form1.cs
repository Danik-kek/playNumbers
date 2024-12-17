using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace playNumbers
{
    public partial class Form1 : Form
    {
        private int targetNumber; // Целевое число, которое нужно угадать
        private int attempts; // Количество попыток
        private Label messageLabel; // Метка для отображения сообщений
        private string failedAttemptsText = "Неудачные попытки: "; // Текст для отображения неудачных попыток
        private bool isCheckingGuess = false; // Переменная состояния

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Установка свойств прогресс-бара
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 30; // Максимум по умолчанию
            progressBar1.Value = 15; // Устанавливаем начальное значение
            attempts = 0; // Инициализация попыток

            // Создание метки для сообщений
            messageLabel = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(10, 100), // Позиция метки
                Visible = false // Сначала скрываем
            };
            this.Controls.Add(messageLabel); // Добавляем метку на форму

            // Создание метки для неудачных попыток
            Label failedAttemptsLabel = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(10, 130), // Позиция метки для неудачных попыток
                Text = failedAttemptsText
            };
            this.Controls.Add(failedAttemptsLabel); // Добавляем метку на форму

            // Подписка на события
            this.KeyDown += Form1_KeyDown;
            this.KeyPreview = true; // Убедитесь, что свойство KeyPreview установлено в true
            radioButton1.CheckedChanged += RadioButton_CheckedChanged;
            radioButton2.CheckedChanged += RadioButton_CheckedChanged;
            radioButton3.CheckedChanged += RadioButton_CheckedChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ResetGame(); // Сброс игры при загрузке формы
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
                e.Handled = true; // Указываем, что событие обработано
            }
        }

        private bool IsGameReady()
        {
            if (targetNumber == 0)
            {
                MessageBox.Show("Пожалуйста, выберите уровень сложности и начните игру.");
                return false; // Игра не готова
            }
            return true; // Игра готова
        }

        private async void CheckGuess()
        {
            if (isCheckingGuess) return;

            isCheckingGuess = true; // Устанавливаем состояние проверки
            ToggleInput(false); // Отключаем ввод

            if (int.TryParse(textBox1.Text, out int userGuess))
            {
                attempts++; // Увеличиваем количество попыток
                await ProcessGuess(userGuess);
            }
            else
            {
                await ShowMessage("Введите корректное число.");
            }

            ToggleInput(true); // Включаем ввод
            isCheckingGuess = false; // Сбрасываем состояние проверки
        }

        private async Task ProcessGuess(int userGuess)
        {
            if (userGuess == targetNumber)
            {
                await ShowMessage("Поздравляем! Вы угадали число!");
                ResetGame(); // Сброс игры после победы
            }
            else
            {
                if (progressBar1.Value > progressBar1.Minimum)
                {
                    progressBar1.Value--;
                    failedAttemptsText += userGuess + ", "; // Добавляем неудачную попытку
                    UpdateFailedAttemptsLabel(); // Обновляем метку неудачных попыток
                    await ShowMessage("Неправильно! Попробуйте снова.");
                    label1.Text += $", {userGuess}";
                    textBox1.Clear();
                    textBox1.Focus();
                }
                else
                {
                    await ShowMessage($"Вы проиграли :( Правильное число было {targetNumber}.");
                    ResetGame(); // Сброс игры после поражения
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
            // Обновляем текст метки для неудачных попыток
            var failedAttemptsLabel = this.Controls.OfType<Label>().FirstOrDefault(l => l.Location == new System.Drawing.Point(10, 130));
            if (failedAttemptsLabel != null)
            {
                failedAttemptsLabel.Text = failedAttemptsText.TrimEnd(',', ' '); // Убираем последнюю запятую и пробел
            }
        }

        private void UpdateGameDifficulty()
        {
            // Обновление уровня сложности и целевого числа
            if (radioButton1.Checked)
            {
                progressBar1.Maximum = 2; // Уровень легкости
                GenerateTargetNumber(1, 3);
            }
            else if (radioButton2.Checked)
            {
                progressBar1.Maximum = 10; // Уровень нормальный
                GenerateTargetNumber(1, 15);
            }
            else if (radioButton3.Checked)
            {
                progressBar1.Maximum = 15; // Уровень сложности
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
            // Сброс состояния игры
            targetNumber = 0; // Сбрасываем целевое число
            attempts = 0; // Сброс попыток
            progressBar1.Value = progressBar1.Maximum; // Сброс прогресс-бара
            textBox1.Clear(); // Очистка текстового поля
            messageLabel.Visible = false; // Скрываем сообщение
            failedAttemptsText = "Неудачные попытки: "; // Сбрасываем текст неудачных попыток
            UpdateFailedAttemptsLabel(); // Обновляем метку неудачных попыток
            label1.Text = "Ваши попытки ";
            label3.Text = "Ваше число ...";

            // Сбрасываем радиокнопки
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton1.Enabled = true; radioButton2.Enabled = true; radioButton3.Enabled = true;


            // Отключаем кнопку проверки
            button1.Enabled = false;

            // Показываем сообщение о сбросе игры
            MessageBox.Show("Игра сброшена. Выберите уровень сложности и начинайте игру!");
        }

        private void ToggleInput(bool isEnabled)
        {
            button1.Enabled = isEnabled; // Включаем/выключаем кнопку проверки
            textBox1.Enabled = isEnabled; // Включаем/выключаем текстовое поле
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Проверка состояния радиокнопок и активация кнопки проверки
            if (radioButton1.Checked || radioButton2.Checked || radioButton3.Checked)
            {
                button1.Enabled = true; // Разрешаем кнопку проверки, если выбрана сложность
                UpdateGameDifficulty(); // Обновляем уровень сложности при изменении радиокнопки
            }
        }

        //debug метод
        private void label2_Click(object sender, EventArgs e)
        {
           this.Text += $" {targetNumber}";
        }
    }
}
