using BattleShip2077;
using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BattleShip2077
{
    public class Bot : Form
    {
        ApplicationContext db;
        public int[,] myMap = new int[Form1.mapSize, Form1.mapSize];//bot`s map
        public int[,] enemyMap = new int[Form1.mapSize, Form1.mapSize];//player`s map

        public Button[,] myButtons = new Button[Form1.mapSize, Form1.mapSize];
        public Button[,] enemyButtons = new Button[Form1.mapSize, Form1.mapSize];

        static bool isPlaying;
        public static string alphabet = "АБВГДЕЖЗИКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";


        public Bot(int[,] myMap, int[,] enemyMap, Button[,] myButtons, Button[,] enemyButtons)
        {
            this.myMap = myMap;
            this.enemyMap = enemyMap;
            this.enemyButtons = enemyButtons;
            this.myButtons = myButtons;

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("jsconfig.json");

            var config = builder.Build();
            string? connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            var options = optionsBuilder.UseSqlServer(connectionString).Options;

            db = new ApplicationContext(options);

        }

        public bool IsInsideMap(int i, int j)
        {
            if (i < 0 || j < 0 || i >= Form1.mapSize || j >= Form1.mapSize)
            {
                return false;
            }
            return true;
        }

        public bool IsEmpty1(int i, int j, int length) //проверка по горизонтали для бота
        {
            bool isEmpty = true;

            for (int k = j; k < j + length; k++)
            {
                if (IsInsideMap(i, k))
                {
                    if (myMap[i, k] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }

                if (IsInsideMap(i, k + 1))
                {
                    if (myMap[i, k + 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(i, k - 1))
                {
                    if (myMap[i, k - 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(i + 1, k))
                {
                    if (myMap[i + 1, k] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(i - 1, k))
                {
                    if (myMap[i - 1, k] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(i - 1, k - 1))
                {
                    if (myMap[i - 1, k - 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(i - 1, k + 1))
                {
                    if (myMap[i - 1, k + 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(i + 1, k - 1))
                {
                    if (myMap[i + 1, k - 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(i + 1, k + 1))
                {
                    if (myMap[i + 1, k + 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
            }

            return isEmpty;
        }
        public bool IsEmpty2(int i, int j, int length) //проверка по вертикали для бота
        {
            bool isEmpty = true;

            for (int k = i; k < i + length; k++)
            {
                if (IsInsideMap(k, j))
                {
                    if (myMap[k, j] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(k, j + 1))
                {
                    if (myMap[k, j + 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(k, j - 1))
                {
                    if (myMap[k, j - 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(k + 1, j))
                {
                    if (myMap[k + 1, j] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(k - 1, j))
                {
                    if (myMap[k - 1, j] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(k - 1, j - 1))
                {
                    if (myMap[k - 1, j - 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(k - 1, j + 1))
                {
                    if (myMap[k - 1, j + 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(k + 1, j - 1))
                {
                    if (myMap[k + 1, j - 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(k + 1, j + 1))
                {
                    if (myMap[k + 1, j + 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
            }

            return isEmpty;
        }

        public bool IsEmpty3(int i, int j, int length) //проверка по горизонтали для игрока
        {
            bool isEmpty = true;

            for (int k = j; k < j + length; k++)
            {
                if (IsInsideMap(i, k))
                {
                    if (enemyMap[i, k] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }

                if (IsInsideMap(i, k + 1))
                {
                    if (enemyMap[i, k + 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(i, k - 1))
                {
                    if (enemyMap[i, k - 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(i + 1, k))
                {
                    if (enemyMap[i + 1, k] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(i - 1, k))
                {
                    if (enemyMap[i - 1, k] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(i - 1, k - 1))
                {
                    if (enemyMap[i - 1, k - 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(i - 1, k + 1))
                {
                    if (enemyMap[i - 1, k + 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(i + 1, k - 1))
                {
                    if (enemyMap[i + 1, k - 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(i + 1, k + 1))
                {
                    if (enemyMap[i + 1, k + 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
            }

            return isEmpty;
        }
        public bool IsEmpty4(int i, int j, int length) //проверка по вертикали для игрока
        {
            bool isEmpty = true;

            for (int k = i; k < i + length; k++)
            {
                if (IsInsideMap(k, j))
                {
                    if (enemyMap[k, j] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(k, j + 1))
                {
                    if (enemyMap[k, j + 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(k, j - 1))
                {
                    if (enemyMap[k, j - 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(k + 1, j))
                {
                    if (enemyMap[k + 1, j] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(k - 1, j))
                {
                    if (enemyMap[k - 1, j] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(k - 1, j - 1))
                {
                    if (enemyMap[k - 1, j - 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(k - 1, j + 1))
                {
                    if (enemyMap[k - 1, j + 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(k + 1, j - 1))
                {
                    if (enemyMap[k + 1, j - 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (IsInsideMap(k + 1, j + 1))
                {
                    if (enemyMap[k + 1, j + 1] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
            }

            return isEmpty;
        }
        public int[,] ConfigureShips()
        {
            int lengthShip1 = 4;
            int lengthShip2 = 4;
            int cycleValue1 = 4;
            int cycleValue2 = 4;
            int shipsCount1 = 10;
            int shipsCount2 = 10;
            Random r = new Random();

            int direction;
            int posX;
            int posY;

            while (shipsCount1 > 0)
            {
                for (int i = 0; i < cycleValue1 / 4; i++)
                {
                    direction = r.Next(0, 2);
                    posX = r.Next(1, Form1.mapSize);
                    posY = r.Next(1, Form1.mapSize);

                    if (direction == 0) //горизонтальное направление
                    {
                        while (!IsInsideMap(posX, posY + lengthShip1 - 1) || !IsEmpty1(posX, posY, lengthShip1))
                        {
                            posX = r.Next(1, Form1.mapSize);
                            posY = r.Next(1, Form1.mapSize);
                        }
                        for (int y = posY; y < posY + lengthShip1; y++)
                        {
                            myMap[posX, y] = 1;


                        }
                    }
                    if (direction == 1) //вертикальное направление
                    {
                        while (!IsInsideMap(posX + lengthShip1 - 1, posY) || !IsEmpty2(posX, posY, lengthShip1))
                        {
                            posX = r.Next(1, Form1.mapSize);
                            posY = r.Next(1, Form1.mapSize);
                        }
                        for (int x = posX; x < posX + lengthShip1; x++)
                        {
                            myMap[x, posY] = 1;

                        }
                    }

                    shipsCount1--;
                    if (shipsCount1 <= 0)
                        break;
                }
                cycleValue1 += 4;
                lengthShip1--;
            }
            while (shipsCount2 > 0)
            {
                for (int i = 0; i < cycleValue2 / 4; i++)
                {
                    direction = r.Next(0, 2);
                    posX = r.Next(1, Form1.mapSize);
                    posY = r.Next(1, Form1.mapSize);

                    if (direction == 0) //горизонтальное направление
                    {
                        while (!IsInsideMap(posX, posY + lengthShip2 - 1) || !IsEmpty3(posX, posY, lengthShip2))
                        {
                            posX = r.Next(1, Form1.mapSize);
                            posY = r.Next(1, Form1.mapSize);
                        }
                        for (int y = posY; y < posY + lengthShip2; y++)
                        {
                            enemyMap[posX, y] = 1;
                            enemyButtons[posX, y].BackColor = Color.LightGreen;

                        }
                    }
                    if (direction == 1) //вертикальное направление
                    {
                        while (!IsInsideMap(posX + lengthShip2 - 1, posY) || !IsEmpty4(posX, posY, lengthShip2))
                        {
                            posX = r.Next(1, Form1.mapSize);
                            posY = r.Next(1, Form1.mapSize);
                        }
                        for (int x = posX; x < posX + lengthShip2; x++)
                        {
                            enemyMap[x, posY] = 1;
                            enemyButtons[x, posY].BackColor = Color.LightGreen;
                        }
                    }

                    shipsCount2--;
                    if (shipsCount2 <= 0)
                        break;
                }
                cycleValue2 += 4;
                lengthShip2--;
            }
            return myMap;
        }

        public bool Shoot()
        {
            enemyMap = Form1.myMap;
            bool hit = false;
            Random r = new Random(); ;
            isPlaying = Form1.isPlaying;

            char symbol = ' ';

            int posX = r.Next(1, Form1.mapSize);
            int posY = r.Next(1, Form1.mapSize);

            if (isPlaying)
            {
                while (enemyButtons[posX, posY].Text == "X" || enemyButtons[posX, posY].BackColor == Color.DarkGray)
                {
                    posX = r.Next(1, Form1.mapSize);
                    posY = r.Next(1, Form1.mapSize);
                }

                if (enemyMap[posX, posY] == 1)
                {
                    hit = true;
                    enemyMap[posX, posY] = 2;
                    enemyButtons[posX, posY].Text = "X";
                }
                else
                {
                    hit = false;
                    enemyMap[posX, posY] = 3;
                    enemyButtons[posX, posY].BackColor = Color.DarkGray;
                }
                for (int i = 0; i < posY; i++)
                {
                    symbol = alphabet[i];
                }

                FileStream protocol = new FileStream("protocol.txt", FileMode.Append);
                StreamWriter sw = new StreamWriter(protocol);
                sw.WriteLine("BOT_SHOOT = " + (symbol + "" + posX) + "; HIT = " + hit);

                sw.Close();
                protocol.Close();
                Protocol protocolBD = new Protocol { login = Form2.login, bot_shoot = (symbol + "" + posX + " " + hit) };
                db.Protocols.Add(protocolBD);
                db.SaveChanges();
                if (hit)
                    Shoot();

            }
            return hit;
        }

        public bool Shoot1()
        {
            enemyMap = Form1.myMap;
            bool hit = false;
            Random r = new Random(); ;
            isPlaying = Form1.isPlaying;

            char symbol = ' ';

            int posX = r.Next(1, Form1.mapSize);
            int posY = r.Next(1, Form1.mapSize);
            int direction;

            if (isPlaying)
            {
                while (enemyButtons[posX, posY].Text == "X" || enemyButtons[posX, posY].BackColor == Color.DarkGray)
                {
                    posX = r.Next(1, Form1.mapSize);
                    posY = r.Next(1, Form1.mapSize);
                }

                if (enemyMap[posX, posY] == 1)
                {
                    hit = true;
                    enemyMap[posX, posY] = 2;
                    enemyButtons[posX, posY].Text = "X";
                    direction = r.Next(0, 4);
                    if (direction == 0)
                    {
                        int X = posX - 1;
                        int Y = posY;
                        if (IsInsideMap(X, Y))
                        {
                            if ((enemyMap[X, Y] != 3) & (enemyMap[X, Y] != 2))
                            {
                                if (enemyMap[X, Y] == 1)
                                {
                                    enemyMap[X, Y] = 2;
                                    enemyButtons[X, Y].Text = "X";
                                    hit = true;
                                }
                                else
                                {
                                    enemyMap[X, Y] = 3;
                                    enemyButtons[X, Y].BackColor = Color.DarkGray;
                                    hit = false;
                                }
                            }
                            else
                            {
                                while (direction == 0)
                                {
                                    direction = r.Next(0, 4);
                                }
                            }

                        }
                        else
                        {
                            while (direction == 0)
                            {
                                direction = r.Next(0, 4);
                            }
                        }
                    }
                    if (direction == 1)
                    {
                        int X = posX + 1;
                        int Y = posY;
                        if (IsInsideMap(X, Y))
                        {
                            if ((enemyMap[X, Y] != 3) & (enemyMap[X, Y] != 2))
                            {
                                if (enemyMap[X, Y] == 1)
                                {
                                    enemyMap[X, Y] = 2;
                                    enemyButtons[X, Y].Text = "X";
                                    hit = true;
                                }
                                else
                                {
                                    enemyMap[X, Y] = 3;
                                    enemyButtons[X, Y].BackColor = Color.DarkGray;
                                    hit = false;
                                }
                            }
                            else
                            {
                                while (direction == 1)
                                {
                                    direction = r.Next(0, 4);
                                }
                            }

                        }
                        else
                        {
                            while (direction == 1)
                            {
                                direction = r.Next(0, 4);
                            }
                        }
                    }
                    if (direction == 2)
                    {
                        int X = posX;
                        int Y = posY - 1;
                        if (IsInsideMap(X, Y))
                        {
                            if ((enemyMap[X, Y] != 3) & (enemyMap[X, Y] != 2))
                            {
                                if (enemyMap[X, Y] == 1)
                                {
                                    enemyMap[X, Y] = 2;
                                    enemyButtons[X, Y].Text = "X";
                                    hit = true;
                                }
                                else
                                {
                                    enemyMap[X, Y] = 3;
                                    enemyButtons[X, Y].BackColor = Color.DarkGray;
                                    hit = false;
                                }
                            }
                            else
                            {
                                while (direction == 2)
                                {
                                    direction = r.Next(0, 4);
                                }
                            }

                        }
                        else
                        {
                            while (direction == 2)
                            {
                                direction = r.Next(0, 4);
                            }
                        }
                    }
                    if (direction == 3)
                    {
                        int X = posX;
                        int Y = posY + 1;
                        if (IsInsideMap(X, Y))
                        {
                            if ((enemyMap[X, Y] != 3) & (enemyMap[X, Y] != 2))
                            {
                                if (enemyMap[X, Y] == 1)
                                {
                                    enemyMap[X, Y] = 2;
                                    enemyButtons[X, Y].Text = "X";
                                    hit = true;
                                }
                                else
                                {
                                    enemyMap[X, Y] = 3;
                                    enemyButtons[X, Y].BackColor = Color.DarkGray;
                                    hit = false;
                                }
                            }
                            else
                            {
                                while (direction == 3)
                                {
                                    direction = r.Next(0, 4);
                                }
                            }

                        }
                        else
                        {
                            while (direction == 3)
                            {
                                direction = r.Next(0, 4);
                            }
                        }
                    }
                }
                else
                {
                    hit = false;
                    enemyMap[posX, posY] = 3;
                    enemyButtons[posX, posY].BackColor = Color.DarkGray;
                }
                for (int i = 0; i < posY; i++)
                {
                    symbol = alphabet[i];
                }
                FileStream protocol = new FileStream("protocol.txt", FileMode.Append);
                StreamWriter sw = new StreamWriter(protocol);
                sw.WriteLine("BOT_SHOOT = " + (symbol + "" + posX) + "; HIT = " + hit);

                sw.Close();
                protocol.Close();
                if (hit)
                    Shoot1();

            }
            return hit;
        }
    }
}