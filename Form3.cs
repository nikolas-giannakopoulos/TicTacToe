using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TicTacToe
{
    public partial class Form3 : Form
    {
        Form1 form1; 
        String connectionString = "Data source=players.db;Version=3";
        SQLiteConnection connection;
        bool exist =false;
        bool success = false;

        public Form3(Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1; //connect with form1 
        }

        private void Form3_Load(object sender, EventArgs e)
        {           
            connection = new SQLiteConnection(connectionString);

            form1.Enabled= false; //dissable form1 when this form opens

            //all players in combobox
            connection.Open();
            String selectIDsSQL = "Select Player_Name from Players";
            SQLiteCommand commandAvailable = new SQLiteCommand(selectIDsSQL, connection);
            SQLiteDataReader sQLiteDataReaderreader = commandAvailable.ExecuteReader();
            comboBox1.Items.Clear();
            while (sQLiteDataReaderreader.Read())
            {
                comboBox1.Items.Add(sQLiteDataReaderreader.GetString(0));
            }
            connection.Close();

            this.FormClosing += new FormClosingEventHandler(Form_Closing); //changing what the form does when closing

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox1.Visible = false;
                comboBox1.Visible = true;
                textBox1.Text = null;
                exist = true;
            }
            else
            {
                textBox1.Visible = true;
                comboBox1.Visible = false;
                comboBox1.Text = null;
                exist = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int N;
            if (int.TryParse(textBox3.Text, out N) && N % 2 == 1 && N>3 ) //check if rows are number, odd and greater than 3
            {
                if ((comboBox1.Text == "") && (textBox1.Text == "")) //check if the player has been given a name
                {
                    MessageBox.Show("Please choose or give player a name");
                }
                else
                {
                    string check = "";
                    try
                    {
                        connection.Open();
                        String nameCheck = "Select * from Players where Player_Name== @name";
                        SQLiteCommand command = new SQLiteCommand(nameCheck, connection);
                        command.Parameters.AddWithValue("name", textBox1.Text);
                        SQLiteDataReader sQLiteDataReaderreader = command.ExecuteReader();
                        check = sQLiteDataReaderreader.GetString(0);
                        connection.Close();
                    }
                    catch { }                  
                    if (check != "" && check == textBox1.Text) //check if the new player has the name of a previous player
                    {
                        MessageBox.Show("The name given to the player already exists. Please choose another.");
                    }
                    else //go to game 
                    {
                        success = true;
                        form1.createPlayer(exist, (exist) ? comboBox1.Text : textBox1.Text, 1);
                        //open game
                        Form4 form4 = new Form4(form1, Convert.ToInt32(textBox3.Text),1);
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

        private void Form_Closing(object sender, CancelEventArgs e)
        {
            if (success)
            {
                form1.Enabled = true;
                form1.Visible = false;
            }
            else
            {
                if (MessageBox.Show("Would you like to go back to the homepage?", "Return", MessageBoxButtons.YesNo) == DialogResult.No)
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
