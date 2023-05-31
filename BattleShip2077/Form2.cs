using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleShip2077
{
    public partial class Form2 : Form
    {
        ApplicationContext db;
        public int ID0;
        public static string login = "";
        public string password = "";
        private byte[] tmpSource;
        private byte[] tmpHash;
        public static bool window2_close = false;
        public static bool admin_mode = false;
        public Form2()
        {
            InitializeComponent();

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("jsconfig.json");

            var config = builder.Build();
            string? connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            var options = optionsBuilder.UseSqlServer(connectionString).Options;

            db = new ApplicationContext(options);

        }
        private int GetLastID()
        {
            var Logs = db.Logs
                                   .OrderBy(c => c.ID);

            foreach (Log c in Logs)
                ID0 = c.ID;

            return ID0;
        }
        private bool isTruePass(string a, string b)
        {
            bool isTrue = false;
            var Logs = db.Logs.Where(p => p.login == a);
            foreach (Log log in Logs)
                if (log.password.Replace(" ", "") == b) isTrue = true;
            return isTrue;
        }
        private bool isTrueLog(string a)
        {
            bool isTrue = false;
            var Logs = db.Logs.Where(p => p.login == a);
            foreach (Log log in Logs)
                isTrue = true;
            return isTrue;


        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Replace(" ", "") != "" & textBox2.Text.Replace(" ", "") != "")
            {
                login = textBox1.Text.Replace(' ', '\0');

                //Create a byte array from source data.
                tmpSource = ASCIIEncoding.ASCII.GetBytes(textBox2.Text);
                //Compute hash based on source data.
                tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);

                static string ByteArrayToString(byte[] arrInput)
                {
                    int i;
                    StringBuilder sOutput = new StringBuilder(arrInput.Length);
                    for (i = 0; i < arrInput.Length; i++)
                    {
                        sOutput.Append(arrInput[i].ToString("X2"));
                    }
                    return sOutput.ToString();
                }
                password = ByteArrayToString(tmpHash);
                if (isTrueLog(login))
                {
                    if (isTruePass(login, password))
                    {
                        MessageBox.Show("Вы успешно вошли в аккаунт", "Успех");
                        if (login == "admin")
                            admin_mode = true;
                        window2_close = true;
                        this.Dispose();

                    }
                    else
                    {
                        MessageBox.Show("Неверный пароль", "Провал");
                        window2_close = false;
                    }
                }
                else
                {
                    MessageBox.Show("Вы успешно зарегистрировались", "Успех");

                    int ID0 = GetLastID();
                    ID0 += 1;

                    Log u = new Log { login = login, password = password };
                    db.Logs.Add(u);
                    db.SaveChanges();

                    if (login == "admin")
                        admin_mode = true;
                    window2_close = true;
                    this.Dispose();

                }
            }
            else
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка");
            }
        }
    }
}
