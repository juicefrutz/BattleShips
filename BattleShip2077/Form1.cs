using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Diagnostics.Eventing.Reader;

namespace BattleShip2077
{
    public partial class Form1 : Form
    {
        ApplicationContext db;
        public int ID0;
        public const int mapSize = 11;
        public int cellSize = 40;
        public static string alphabet = "АБВГДЕЖЗИКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

        public static int[,] myMap = new int[mapSize, mapSize];
        public static int[,] enemyMap = new int[mapSize, mapSize];

        public Button[,] myButtons = new Button[mapSize, mapSize];
        public Button[,] enemyButtons = new Button[mapSize, mapSize];

        public static bool isPlaying = false;
        public static int difficulty = 1;

        static string file_name;
        static int time;
        static int timetrue;

        public Bot bot;

        public Form1()
        {

            InitializeComponent();
            this.Text = "Морской бой";

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("jsconfig.json");

            var config = builder.Build();
            string? connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            var options = optionsBuilder.UseSqlServer(connectionString).Options;

            db = new ApplicationContext(options);

            Init();

        }

        public void Init()
        {
            MessageBox.Show("Генерируем карту....", "Генерация");
            isPlaying = false;
            CreateMaps();
            bot = new Bot(enemyMap, myMap, enemyButtons, myButtons);
            enemyMap = bot.ConfigureShips();
        }
        public int GetLastID()
        {
            var Results = db.Results
                                   .OrderBy(c => c.ID);

            foreach (Result c in Results)
                ID0 = c.ID;

            return ID0;
        }

        public void CreateMaps()
        {
            this.Width = mapSize * 2 * cellSize + 600;
            this.Height = (mapSize + 3) * cellSize + 100;
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    myMap[i, j] = 0;

                    Button button = new Button();
                    button.Location = new Point(j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.BackColor = Color.White;
                    if (j == 0 || i == 0)
                    {
                        button.BackColor = Color.LightGray;
                        if (i == 0 && j > 0)
                            button.Text = alphabet[j - 1].ToString();
                        if (j == 0 && i > 0)
                            button.Text = i.ToString();
                    }
                    myButtons[i, j] = button;
                    this.Controls.Add(button);
                }
            }
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    myMap[i, j] = 0;
                    enemyMap[i, j] = 0;

                    Button button = new Button();
                    button.Location = new Point(690 + j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.BackColor = Color.White;
                    if (j == 0 || i == 0)
                    {
                        button.BackColor = Color.LightGray;
                        if (i == 0 && j > 0)
                            button.Text = alphabet[j - 1].ToString();
                        if (j == 0 && i > 0)
                            button.Text = i.ToString();
                    }
                    else
                    {
                        button.Click += new EventHandler(PlayerShoot);
                    }
                    enemyButtons[i, j] = button;
                    this.Controls.Add(button);
                }
            }
            if (Form2.admin_mode == true)
            {
                Button showEnemyShipsButton = new Button();
                showEnemyShipsButton.Text = "Показать корабли противника";
                float currentSize4 = showEnemyShipsButton.Font.Size;
                currentSize4 += 2;
                showEnemyShipsButton.Font = new Font(showEnemyShipsButton.Font.Name, currentSize4, showEnemyShipsButton.Font.Style, showEnemyShipsButton.Font.Unit);
                showEnemyShipsButton.Height = 100;
                showEnemyShipsButton.Width = 200;
                showEnemyShipsButton.Click += new EventHandler(showEnemyShips);
                showEnemyShipsButton.Location = new Point(300 + mapSize * cellSize * 2, 100);
                this.Controls.Add(showEnemyShipsButton);

                Button hideEnemyShipsButton = new Button();
                hideEnemyShipsButton.Text = "Спрятать корабли противника";
                float currentSize5 = hideEnemyShipsButton.Font.Size;
                currentSize5 += 2;
                hideEnemyShipsButton.Font = new Font(hideEnemyShipsButton.Font.Name, currentSize5, hideEnemyShipsButton.Font.Style, hideEnemyShipsButton.Font.Unit);
                hideEnemyShipsButton.Height = 100;
                hideEnemyShipsButton.Width = 200;
                hideEnemyShipsButton.Click += new EventHandler(hideEnemyShips);
                hideEnemyShipsButton.Location = new Point(300 + mapSize * cellSize * 2, 300);
                this.Controls.Add(hideEnemyShipsButton);

                Label debug_menu = new Label();
                debug_menu.Text = "Меню разработчика";
                float currentSize0 = debug_menu.Font.Size;
                currentSize0 += 2;
                debug_menu.Font = new Font(debug_menu.Font.Name, currentSize0, debug_menu.Font.Style, debug_menu.Font.Unit);
                debug_menu.BackColor = Color.MediumPurple;
                debug_menu.Width = 225;
                debug_menu.Height = 50;
                debug_menu.Location = new Point(300 + mapSize * cellSize * 2, 30);
                this.Controls.Add(debug_menu);
            }

            Label map1 = new Label();
            map1.Text = "Карта игрока";
            map1.BackColor = Color.FromArgb(221, 160, 221);
            float currentSize1 = map1.Font.Size;
            currentSize1 += 2;
            map1.Font = new Font(map1.Font.Name, currentSize1, map1.Font.Style, map1.Font.Unit);
            map1.Width = 200;
            map1.Height = 50;
            map1.Location = new Point(mapSize * cellSize - 300, mapSize * cellSize + 20);
            this.Controls.Add(map1);

            Label map2 = new Label();
            map2.Text = "Карта противника";
            map2.BackColor = Color.FromArgb(153, 102, 204);
            float currentSize2 = map2.Font.Size;
            currentSize2 += 2;
            map2.Font = new Font(map2.Font.Name, currentSize2, map2.Font.Style, map2.Font.Unit);
            map2.Width = 200;
            map2.Height = 50;
            map2.Location = new Point(+mapSize * cellSize * 2 - 90, mapSize * cellSize + 20);
            this.Controls.Add(map2);

            Button startButton = new Button();
            startButton.Text = "Начать";
            float currentSize3 = startButton.Font.Size;
            currentSize3 += 2;
            startButton.Font = new Font(startButton.Font.Name, currentSize3, startButton.Font.Style, startButton.Font.Unit);
            startButton.Height = 60;
            startButton.Width = 200;
            startButton.Click += new EventHandler(Start);
            startButton.Location = new Point(mapSize * cellSize + 25, mapSize * cellSize + 100);
            this.Controls.Add(startButton);

            Button generationbutton = new Button();
            generationbutton.Text = "Генерировать новую карту";
            generationbutton.Font = new Font(generationbutton.Font.Name, currentSize3, generationbutton.Font.Style, generationbutton.Font.Unit);
            generationbutton.Height = 100;
            generationbutton.Width = 200;
            generationbutton.Click += new EventHandler(generation);
            generationbutton.Location = new Point(25 + mapSize * cellSize, mapSize * cellSize / 2 - 100);
            this.Controls.Add(generationbutton);

            Button difficult = new Button();
            difficult.Text = "Изменить сложность";
            difficult.Font = new Font(difficult.Font.Name, currentSize3, difficult.Font.Style, generationbutton.Font.Unit);
            difficult.Height = 100;
            difficult.Width = 200;
            difficult.Click += new EventHandler(change_difficult);
            difficult.Location = new Point(25 + mapSize * cellSize, mapSize * cellSize / 2 + 100);
            this.Controls.Add(difficult);

            Button save = new Button();
            save.Text = "Сохраниться";
            save.Font = new Font(save.Font.Name, currentSize3, save.Font.Style, save.Font.Unit);
            save.Height = 60;
            save.Width = 200;
            save.Click += new EventHandler(saveProgress);
            save.Location = new Point(mapSize * cellSize - save.Width, mapSize * cellSize + 100);
            this.Controls.Add(save);

            Button load = new Button();
            load.Text = "Загрузить";
            load.Font = new Font(load.Font.Name, currentSize3, load.Font.Style, load.Font.Unit);
            load.Height = 60;
            load.Width = 200;
            load.Click += new EventHandler(loadProgress);
            load.Location = new Point(mapSize * cellSize * 2 - 190, mapSize * cellSize + 100);
            this.Controls.Add(load);

        }

        public void Start(object sender, EventArgs e)
        {
            MessageBox.Show("Начать игру", "Начало");
            isPlaying = true;
            Protocol protocolBD = new Protocol { login = Form2.login, start_game = "Yes"};
            db.Protocols.Add(protocolBD);
            db.SaveChanges();
            FileStream protocol = new FileStream("protocol.txt", FileMode.Append);
            StreamWriter sw = new StreamWriter(protocol);
            sw.WriteLine(".....START_GAME.....");
            sw.Close();
            protocol.Close();

        }

        public int CheckIfMapIsNotEmpty()
        {
            bool isEmpty1 = true;
            bool isEmpty2 = true;
            for (int i = 1; i < mapSize; i++)
            {
                for (int j = 1; j < mapSize; j++)
                {
                    if (myMap[i, j] == 1)
                        isEmpty1 = false;
                    if (enemyMap[i, j] == 1)
                        isEmpty2 = false;
                }
            }
            if (isEmpty1) return 0;
            else if (isEmpty2) return 1;
            else if (!isEmpty1) return 2;
            else if (!isEmpty2) return 3;
            else return 4;
        }

        public async void PlayerShoot(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;
            bool playerTurn = Shoot(enemyMap, pressedButton);
            if (!playerTurn)
            {
                disable_buttons();
                await Task.Delay(500);
                if (difficulty == 0)
                {
                    bot.Shoot();
                }
                else
                {
                    bot.Shoot1();
                }
                enable_buttons();
            }
            if (CheckIfMapIsNotEmpty() == 0)
            {
                int ID0 = GetLastID();
                MessageBox.Show("Вы проиграли!", "Поражение");
                ID0 += 1;
                Protocol protocolBD = new Protocol { login = Form2.login, end_game = "Yes", victory = "Bot" };
                db.Protocols.Add(protocolBD);
                db.SaveChanges();
                Result u = new Result { PLAYER_WIN = "NO", BOT_WIN = "YES", LOGIN = Form2.login, DATE_TIME = DateTime.Now };
                db.Results.Add(u);
                db.SaveChanges();

                FileStream protocol = new FileStream("protocol.txt", FileMode.Append);
                StreamWriter sw = new StreamWriter(protocol);
                sw.WriteLine("PLAYER_WIN = FALSE, BOT_WIN = TRUE");

                sw.Close();
                protocol.Close();

                this.Controls.Clear();
                Init();
            }
            else if (CheckIfMapIsNotEmpty() == 1)
            {
                int ID0 = GetLastID();
                MessageBox.Show("Вы выиграли!", "Победа");
                ID0 += 1;
                Protocol protocolBD = new Protocol { login = Form2.login, end_game = "Yes", victory = "Player" };
                db.Protocols.Add(protocolBD);
                db.SaveChanges();
                Result u = new Result { PLAYER_WIN = "YES", BOT_WIN = "NO", LOGIN = Form2.login, DATE_TIME = DateTime.Now };
                db.Results.Add(u);
                db.SaveChanges();

                FileStream protocol = new FileStream("protocol.txt", FileMode.Append);
                StreamWriter sw = new StreamWriter(protocol);
                sw.WriteLine("PLAYER_WIN = TRUE, BOT_WIN = FALSE");
                sw.WriteLine(".....GAME_OVER.....");

                sw.Close();
                protocol.Close();
                this.Controls.Clear();
                Init();
            }
        }

        public bool Shoot(int[,] map, Button pressedButton)
        {
            bool hit = false;
            char symbol = ' ';
            if (isPlaying)
            {
                int delta = 0;
                if (pressedButton.Location.X > 690)
                    delta = 690;
                if (map[pressedButton.Location.Y / cellSize, (pressedButton.Location.X - delta) / cellSize] == 1)
                {
                    hit = true;
                    map[pressedButton.Location.Y / cellSize, (pressedButton.Location.X - delta) / cellSize] = 2;
                    pressedButton.BackColor = Color.MediumPurple;
                    pressedButton.Text = "X";
                    pressedButton.Enabled = false;
                }
                else
                {
                    hit = false;
                    map[pressedButton.Location.Y / cellSize, (pressedButton.Location.X - delta) / cellSize] = 3;
                    pressedButton.BackColor = Color.DarkGray;
                    pressedButton.Enabled = false;
                }
                for (int i = 0; i < ((pressedButton.Location.X - delta) / cellSize); i++)
                {
                    symbol = alphabet[i];
                }
                FileStream protocol = new FileStream("protocol.txt", FileMode.Append);
                StreamWriter sw = new StreamWriter(protocol);
                sw.WriteLine("PLAYER_SHOOT = " + (symbol + (pressedButton.Location.Y / cellSize).ToString()) + "; HIT = " + hit);

                sw.Close();
                protocol.Close();
                Protocol protocolBD = new Protocol { login = Form2.login, player_shoot = (symbol + (pressedButton.Location.Y / cellSize).ToString() + " " + hit) };
                db.Protocols.Add(protocolBD);
                db.SaveChanges();
            }

            return hit;
        }

        public void showEnemyShips(object sender, EventArgs e)
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int k = 0; k < mapSize; k++)
                {
                    if (enemyMap[i, k] == 1)
                    {
                        enemyButtons[i, k].BackColor = Color.MediumPurple;
                    }
                }
            }
        }
        public void loadEnemyShips()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int k = 0; k < mapSize; k++)
                {
                    if (enemyMap[i, k] == 2)
                    {
                        enemyButtons[i, k].BackColor = Color.MediumPurple;
                        enemyButtons[i, k].Text = "X";
                        enemyButtons[i, k].Enabled = false;
                    }
                    else if (enemyMap[i, k] == 3)
                    {
                        enemyButtons[i, k].BackColor = Color.DarkGray;
                        enemyButtons[i, k].Enabled = false;
                    }
                }
            }
        }

        public void hideEnemyShips(object sender, EventArgs e)
        {
            for (int i = 1; i < mapSize; i++)
            {
                for (int k = 1; k < mapSize; k++)
                {
                    if (enemyMap[i, k] == 1)
                    {
                        enemyButtons[i, k].BackColor = Color.White;
                    }
                }
            }
        }
        public void clearEnemyShips()
        {
            for (int i = 1; i < mapSize; i++)
            {
                for (int k = 1; k < mapSize; k++)
                {
                    enemyButtons[i, k].BackColor = Color.White;
                    enemyButtons[i, k].Text = "";
                    enemyButtons[i, k].Enabled = true;
                }
            }
        }
        public void showMyShips()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int k = 0; k < mapSize; k++)
                {
                    if (myMap[i, k] == 1)
                    {
                        myButtons[i, k].BackColor = Color.LightGreen;
                    }
                    else if (myMap[i, k] == 2)
                    {
                        myButtons[i, k].BackColor = Color.LightGreen;
                        myButtons[i, k].Text = "X";
                    }
                    else if (myMap[i, k] == 3)
                    {
                        myButtons[i, k].BackColor = Color.DarkGray;
                    }
                }
            }
        }
        public void clearMyShips()
        {
            for (int i = 1; i < mapSize; i++)
            {
                for (int k = 1; k < mapSize; k++)
                {
                    myButtons[i, k].BackColor = Color.White;
                    myButtons[i, k].Text = "";
                }
            }
        }
        public void disable_buttons()
        {
            for (int i = 1; i < mapSize; i++)
            {
                for (int k = 1; k < mapSize; k++)
                {
                    enemyButtons[i, k].Enabled = false;
                }
            }
        }
        public void enable_buttons()
        {
            for (int i = 1; i < mapSize; i++)
            {
                for (int k = 1; k < mapSize; k++)
                {
                    if (enemyMap[i, k] == 2)
                    {
                        enemyButtons[i, k].Enabled = false;
                    }
                    else if (enemyMap[i, k] == 3)
                    {
                        enemyButtons[i, k].Enabled = false;
                    }
                    else
                    {
                        enemyButtons[i, k].Enabled = true;
                    }
                }
            }
        }


        public void saveProgress(object sender, EventArgs e)
        {
            DialogResult result = InputBox("Сохранение", "Введите название сохранения", ref file_name);
            if (result == DialogResult.OK)
            {
                var jsonString1 = JsonConvert.SerializeObject(enemyMap);
                File.WriteAllText(Form2.login + file_name + "enemyMap.json", jsonString1);
                var jsonString2 = JsonConvert.SerializeObject(myMap);
                File.WriteAllText(Form2.login + file_name + "myMap.json", jsonString2);
                MessageBox.Show("Сохранение прошло успешно", "Успех");
            }

        }
        public void loadProgress(object sender, EventArgs e)
        {
            DialogResult result = InputBox("Загрузка", "Введите название сохранения", ref file_name);
            if (result == DialogResult.OK)
            {
                try
                {
                    var file1 = File.ReadAllText(Form2.login + file_name + "enemyMap.json");
                    int[,] enemyLoadMap = JsonConvert.DeserializeObject<int[,]>(file1);
                    enemyMap = enemyLoadMap;
                    var file2 = File.ReadAllText(Form2.login + file_name + "myMap.json");
                    try
                    {
                        int[,] myLoadMap = JsonConvert.DeserializeObject<int[,]>(file2);
                        myMap = myLoadMap;
                        clearEnemyShips();
                        loadEnemyShips();
                        clearMyShips();
                        showMyShips();
                        MessageBox.Show("Загрузка прошла успешно", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show("Не удалось загрузить игру. Проверьте загрузочный файл.", "Провал", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch
                {
                    MessageBox.Show("Не найдено сохранение", "Провал", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public void generation(object sender, EventArgs e)
        {
            this.Controls.Clear();
            Init();
        }
        public void change_difficult(object sender, EventArgs e)
        {
            if (difficulty == 1)
            {
                DialogResult result = MessageBox.Show("Изменить сложность бота? Сейчас уровень - сложный", "Сложность", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                {
                    difficulty = 0;
                }
            }
            else
            {
                DialogResult result = MessageBox.Show("Изменить сложность бота? Сейчас уровень - простой", "Сложность", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                {
                    difficulty = 1;
                }
            }


        }
        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "ОК";
            buttonCancel.Text = "Отмена";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 60, 372, 20);
            buttonOk.SetBounds(150, 102, 75, 40);
            buttonCancel.SetBounds(268, 102, 100, 40);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 150);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }
}