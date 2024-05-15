using System;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        SQLiteConnection connection;
        private void Form5_Load(object sender, EventArgs e)
        {
            connection = new SQLiteConnection("Data source=players.db;Version=3");
            connection.Open();
            String RecentString = "Select * from Players"; 
            SQLiteCommand LoadAll = new SQLiteCommand(RecentString, connection);
            SQLiteDataReader AllReader = LoadAll.ExecuteReader();
            int count = 0;
            while (AllReader.Read())
            {
                Label name = new Label();                               
                name.Location = new Point(0, 43 + 25 * count);          
                name.Text = AllReader.GetString(0);                 
                name.AutoSize = false;                                 
                name.Size = new Size(80, 20);              
                name.TextAlign = ContentAlignment.MiddleCenter;        
                this.Controls.Add(name);                                

                Label wins = new Label();
                wins.Location = new Point(100, 43 + 25 * count);
                wins.Text = AllReader.GetInt32(1).ToString();
                wins.AutoSize = false;
                wins.Size = new Size(50, 18);
                wins.TextAlign = ContentAlignment.MiddleCenter;
                this.Controls.Add(wins);

                Label loses = new Label();
                loses.Location = new Point(175, 43 + 25 * count);
                loses.Text = AllReader.GetInt32(2).ToString();
                loses.AutoSize = false;
                loses.TextAlign = ContentAlignment.MiddleCenter;
                loses.Size = new Size(50, 20);
                this.Controls.Add(loses);

                Label draws = new Label();
                draws.Location = new Point(265, 43 + 25 * count);
                draws.Text = AllReader.GetInt32(3).ToString();
                draws.AutoSize = false;
                draws.Size = new Size(50, 18);
                draws.TextAlign = ContentAlignment.MiddleCenter;
                this.Controls.Add(draws);

                Label gamesPlayed = new Label();
                gamesPlayed.Location = new Point(395, 43 + 25 * count);
                gamesPlayed.Text = AllReader.GetInt32(4).ToString();
                gamesPlayed.AutoSize = false;
                gamesPlayed.TextAlign = ContentAlignment.MiddleCenter;
                gamesPlayed.Size = new Size(25, 20);
                this.Controls.Add(gamesPlayed);

                count++;
            }         
            connection.Close();

            this.Size = new Size(this.Width, count * 25 + 81);
        }


    }
}
