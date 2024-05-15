using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;

namespace TicTacToe
{
    public partial class Form1 : Form
    {
        String connectionString = "Data source=players.db;Version=3";
        SQLiteConnection connection;

        public Player p1;
        public Player p2;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            connection = new SQLiteConnection(connectionString);
            connection.Open();
            String createTableSQL = "Create table if not exists Players(" +
    "Player_Name Text Not Null primary key," +
    "Wins integer," +
    "Loses integer,"+
    "Draws integer,"+
    "Games_Played integer)" ;
            SQLiteCommand command = new SQLiteCommand(createTableSQL, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(this);
            form2.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(this);
            form3.Show();
        }
        
        public void createPlayer(bool exists, string name, int num)
        {
            if (exists)
            {
                connection.Open();
                String findPlayer = "Select * from Players where Player_Name== @name";
                SQLiteCommand command = new SQLiteCommand(findPlayer, connection);
                command.Parameters.AddWithValue("name", name);
                SQLiteDataReader sQLiteDataReaderreader = command.ExecuteReader();
                while (sQLiteDataReaderreader.Read())
                {
                    switch (num)
                    {
                        case 1:
                            p1 = new Player(sQLiteDataReaderreader.GetString(0), sQLiteDataReaderreader.GetInt32(1), sQLiteDataReaderreader.GetInt32(2), sQLiteDataReaderreader.GetInt32(3), sQLiteDataReaderreader.GetInt32(4), true);
                            break;
                        case 2:
                            p2 = new Player(sQLiteDataReaderreader.GetString(0), sQLiteDataReaderreader.GetInt32(1), sQLiteDataReaderreader.GetInt32(2), sQLiteDataReaderreader.GetInt32(3), sQLiteDataReaderreader.GetInt32(4), true);
                            break;
                    }
                }
                connection.Close();
            }
            else
            {
                switch (num)
                {
                    case 1:
                        p1 = new Player(name,0,0,0,0,false);                        
                        break;
                    case 2:
                        p2 = new Player(name, 0, 0, 0, 0, false);
                        break;
                }
            }
        }


        private void rulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "1. The player with the letter 'X' starts. " + "\n" + "2. Both players take turns placing their letters. " + "\n" + "3. First player to fill a row or a collumn or a diagonal with their letter wins.";
            MessageBox.Show(s,"Rules");
        }

        private void playerStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form5 = new Form5();
            form5.Show();
        }
    }
}
