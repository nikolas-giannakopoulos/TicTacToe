using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicTacToe;

namespace TicTacToe
{
    public partial class Form2 : Form
    {
        Form1 form1;
        String connectionString = "Data source=players.db;Version=3";
        SQLiteConnection connection;
        bool exists1 = false;
        bool exists2=false;
        bool success = false;

        public Form2(Form1 form1)
        {
            this.form1 = form1; //connect with form1
            InitializeComponent();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            connection = new SQLiteConnection(connectionString);

            form1.Enabled = false; //dissable form1 when this form opens

            //insert all the players into combo boxes
            connection.Open();
            String selectIDsSQL = "Select Player_Name from Players";
            SQLiteCommand commandAvailable = new SQLiteCommand(selectIDsSQL, connection);
            SQLiteDataReader sQLiteDataReaderreader = commandAvailable.ExecuteReader();
            comboBox1.Items.Clear();
            while (sQLiteDataReaderreader.Read())
            {
                comboBox1.Items.Add(sQLiteDataReaderreader.GetString(0));
                comboBox2.Items.Add(sQLiteDataReaderreader.GetString(0));
            }
            connection.Close();

            this.FormClosing += new FormClosingEventHandler(Form_Closing);//changing what the form does when closing
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox1.Visible = false;
                comboBox1.Visible = true;
                textBox1.Text = null;
                exists1 = true;
            }
            else
            {
                textBox1.Visible = true;
                comboBox1.Visible = false;
                comboBox1.Text = null;
                exists1 = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                textBox2.Visible = false;
                comboBox2.Visible = true;
                textBox2.Text = null;
                exists2 = true;
            }
            else
            {
                textBox2.Visible = true;
                comboBox2.Visible = false;
                comboBox2.Text = null;
                exists2 = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int N;
            if (int.TryParse(textBox3.Text, out N) && N % 2 == 1 && N>3) //check if rows are number, odd and greater than 3 
            {
                if ((comboBox1.Text == "") && (textBox1.Text == "")) //check if the player 1 has been given a name
                {
                    MessageBox.Show("Please choose or give player 1 a name");
                }
                else if ((comboBox2.Text == "") && (textBox2.Text == "")) //check if the player 2 has been given a name
                {
                    MessageBox.Show("Please choose or give player 2 a name");
                }
                else
                {
                    string check1 = "";
                    string check2 = "";
                    try
                    {
                        connection.Open();

                        //for player1
                        String nameCheck1 = "Select * from Players where Player_Name== @name1";
                        SQLiteCommand command1 = new SQLiteCommand(nameCheck1, connection);
                        command1.Parameters.AddWithValue("name1", textBox1.Text);
                        SQLiteDataReader sQLiteDataReaderreader1 = command1.ExecuteReader();
                        check1 = sQLiteDataReaderreader1.GetString(0);

                        //for player2
                        String nameCheck2 = "Select * from Players where Player_Name== @name2";
                        SQLiteCommand command2 = new SQLiteCommand(nameCheck2, connection);
                        command2.Parameters.AddWithValue("name2", textBox2.Text);
                        SQLiteDataReader sQLiteDataReaderreader2 = command2.ExecuteReader();
                        check2 = sQLiteDataReaderreader2.GetString(0);

                        connection.Close();
                    }
                    catch { }

                    if (check1 != "" && check1 == textBox1.Text) //check if the new player 1 has the name of a previous player
                    {
                        MessageBox.Show("The name given to the player 1 already exists. Please choose another.");
                    }
                    else if (check2 != "" && check2 == textBox2.Text) //check if the new player 2 has the name of a previous player
                    {
                        MessageBox.Show("The name given to the player 2 already exists. Please choose another.");
                    }
                    else
                    {
                        form1.createPlayer(exists1, (exists1) ? comboBox1.Text : textBox1.Text, 1);
                        form1.createPlayer(exists2, (exists2) ? comboBox2.Text : textBox2.Text, 2);
                        success = true;
                        //open game                   
                        Form4 form4 = new Form4(form1, Convert.ToInt32(textBox3.Text),2);
                        form4.Show();
                        //close this form
                        this.Close();
                    }

                }
            }
            else
            {
                MessageBox.Show("Please give an odd number greater than 3 in order to begin");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) //make sure the same player can't be chosen for both
        {
            connection.Open();
            String selectIDsSQL = "Select * from Players where Player_Name!=@name";
            SQLiteCommand commandAvailable = new SQLiteCommand(selectIDsSQL, connection);
            commandAvailable.Parameters.AddWithValue("name", comboBox1.Text);
            SQLiteDataReader sQLiteDataReaderreader = commandAvailable.ExecuteReader();
            comboBox2.Items.Clear();
            while (sQLiteDataReaderreader.Read())
            {
                comboBox2.Items.Add(sQLiteDataReaderreader.GetString(0));
            }
            connection.Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) //make sure the same player can't be chosen for both
        {
            connection.Open();
            String selectIDsSQL = "Select * from Players where Player_Name!=@name";
            SQLiteCommand commandAvailable = new SQLiteCommand(selectIDsSQL, connection);
            commandAvailable.Parameters.AddWithValue("name", comboBox2.Text);
            SQLiteDataReader sQLiteDataReaderreader = commandAvailable.ExecuteReader();
            comboBox1.Items.Clear();
            while (sQLiteDataReaderreader.Read())
            {
                comboBox1.Items.Add(sQLiteDataReaderreader.GetString(0));
            }
            connection.Close();
        }

        private void Form_Closing(object sender, CancelEventArgs e)
        {
            if (success)
            {
                form1.Enabled = true;
                form1.Visible=false;
            }
            else
            {
                if (MessageBox.Show("Θέλετε σίγουρα να επιστρέψετε στην αρχική οθόνη;","Return", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    form1.Enabled = true;
                    form1.Focus();
                }
            }
        }
    }
}
