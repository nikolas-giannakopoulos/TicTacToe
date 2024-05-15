using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicTacToe.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;

namespace TicTacToe
{
    public partial class Form4 : Form
    {
        Form1 form1;
        String connectionString = "Data source=players.db;Version=3";
        SQLiteConnection connection;

        int N, pNum;//number of rows/columns and number of players
        int buttonHeight, buttonWidth;
        Point loca;//button location
        List<Game> buttons= new List<Game>();//list of buttons
        int turn;//turn counter (X/O)
        bool close= false, finished= false;
        bool newGameFlag = false; //bool for new game menu strip choice
        Random r = new Random();

        public Form4(Form1 form1, int N, int pNum)
        {
            InitializeComponent();
            this.form1 = form1;
            this.N = N;
            this.pNum = pNum;
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            connection = new SQLiteConnection(connectionString);//connect with database
            
            this.WindowState = FormWindowState.Maximized;//maximize window

            buttons.Clear();//empty buttons list

            buttonHeight = ((this.Height - 70) / N) - 5;
            buttonWidth = (this.Width /N)-5;            

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++) {
                    loca = new Point(j * buttonWidth + (j + 1) * 5, i * buttonHeight + (i + 1) * 5 + 20);
                    Game game = new Game(Game_Click, buttonWidth, buttonHeight, i,j, loca,""); 
                    buttons.Add(game);
                    this.Controls.Add(game.b);
                }
            }

            turn = 0;

            this.FormClosing += new FormClosingEventHandler(Form_Closing); //changing what the form does when closing
        }

        private void Form4_Resize(object sender, EventArgs e)
        {
            try
            {
                buttonHeight = ((this.Height - 70) / N) - 5;
                buttonWidth = (this.Width / N) - 5;
            }
            catch { }
            foreach (Game game in buttons)
            {
                game.b.Width = buttonWidth;
                game.b.Height = buttonHeight;
                game.b.Location = new Point(game.j * buttonWidth + (game.j + 1) * 5, game.i * buttonHeight + (game.i + 1) * 5 + 20);
            }
        }

        public void Game_Click(object sender, EventArgs e)
        {
            int choice;
            bool result=false;
            if (turn % 2 == 0 && ((Game)((Button)sender).Tag).played == "")
            {
                ((Button)sender).BackgroundImage = Resources.Box_X;
                ((Button)sender).Refresh();
                ((Game)((Button)sender).Tag).played = "X";
                result=Winner_Check(((Game)((Button)sender).Tag).i, ((Game)((Button)sender).Tag).j,"X");
                if (result)
                {
                    MessageBox.Show("Player 1 Wins");
                    Info_Save(1);
                }
                
                if (pNum == 1 && !result)
                {
                    choice = 0;
                    do
                    {
                        choice = r.Next(buttons.Count);
                    } while (buttons[choice].played != "");
                    System.Threading.Thread.Sleep(150);
                    buttons[choice].b.BackgroundImage = Resources.Box_Circle;
                    buttons[choice].played = "O";
                    result = Winner_Check(buttons[choice].i, buttons[choice].j, "O");
                    if (result)
                    {
                        MessageBox.Show("You lost");
                        Info_Save(2);
                    }
                    turn += 2;
                }
                else
                {
                    turn++;
                }
            }
            else if (((Game)((Button)sender).Tag).played == "" && pNum==2)
            {
                ((Button)sender).BackgroundImage = Resources.Box_Circle;
                ((Game)((Button)sender).Tag).played = "O";
                result=Winner_Check(((Game)((Button)sender).Tag).i, ((Game)((Button)sender).Tag).j, "O");
                if (result)
                {
                    MessageBox.Show("Player 2 Wins");
                    Info_Save(2);
                }
                turn++;
            }
            
        }

        private bool Winner_Check(int bi, int bj, string mark)
        {
            bool flag=false;
            int count = 0;
            if (turn < (N * N))
            {
                if (bi==bj)//first diagonal
                {
                    for (int i  = 0; i < buttons.Count; i+=(N+1))
                    {
                        if (buttons[i].played== mark)
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    flag = (count == N) ? true : false;
                    if (flag)
                    {
                        finished= true;
                        return flag;
                    }
                }
                if (bi ==(N-1) - bj)//second diagonal
                {
                    count = 0;
                    for (int i = N-1; i < (buttons.Count-(N -1)); i += (N - 1))
                    {
                        if (buttons[i].played == mark)
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    flag = (count == N) ? true : false;
                    if (flag)
                    {
                        finished = true;
                        return flag;
                    }
                }

                //check row
                count = 0;
                for (int i =bi*N; i<bi*N+N; i++ )
                {
                    if (buttons[i].played == mark)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                flag = (count == N) ? true : false;
                if (flag)
                {
                    finished = true;
                    return flag;
                }

                //check collumn
                count = 0;
                for (int i = bj; i < buttons.Count; i += N)
                {
                    if (buttons[i].played == mark)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                flag = (count == N) ? true : false;

                if (turn==(N*N)-1 && !flag)//last box and noone has won then it is a draw
                {
                    finished = true;
                    MessageBox.Show("The game is a draw", "Game Ended");               
                    Info_Save(0);
                    return flag;
                }
                finished =flag;
                return flag;
            }
            return false;
        }

        private void Info_Save(int winner)
        {
            if (pNum == 1)
            {//insert player 1 details

                if (!form1.p1.exists) //if he didn't exist
                {
                    form1.p1.exists = true;
                    connection.Open();
                    String insertSQL = "Insert into Players(" +
                            "Player_Name,Wins,Loses,Draws,Games_Played) " +
                            "values(@name,@wins,@loses,@draws,@games_played)";
                    SQLiteCommand command = new SQLiteCommand(insertSQL, connection);
                    command.Parameters.AddWithValue("name", form1.p1.name);
                    command.Parameters.AddWithValue("wins", (winner == 0) ? 0 : (winner == 1) ? 1 : 0);
                    command.Parameters.AddWithValue("loses", (winner == 0) ? 0 : (winner == 1) ? 0 : 1);
                    command.Parameters.AddWithValue("draws", (winner == 0) ? 1 : (winner == 1) ? 0 : 0);
                    command.Parameters.AddWithValue("games_played", 1);
                    int rowsInserted = command.ExecuteNonQuery();
                    connection.Close();
                }
                else//if he did exist
                {
                    connection.Open();
                    String updateSQL = "Update Players " +
                            "set Wins=@wins,"+
                            "Loses=@loses,"+
                            "Draws=@draws,"+
                            "Games_Played= Games_Played+1 " +
                            "where Player_Name==@name";
                    SQLiteCommand command = new SQLiteCommand(updateSQL, connection);
                    command.Parameters.AddWithValue("name", form1.p1.name);
                    command.Parameters.AddWithValue("wins", form1.p1.wins + ((winner == 0) ? 0 : (winner == 1) ? 1 : 0));
                    command.Parameters.AddWithValue("loses", form1.p1.loses + ((winner == 0) ? 0 : (winner == 1) ? 0 : 1));
                    command.Parameters.AddWithValue("draws", form1.p1.draws + ((winner == 0) ? 1 : (winner == 1) ? 0 : 0));
                    int rowsInserted = command.ExecuteNonQuery();
                    connection.Close(); 
                }
            }
            else
            {//insert player details for both players

                if (!form1.p1.exists)//player 1 didnt exist
                {
                    form1.p1.exists = true;
                    connection.Open();
                    String insertSQL = "Insert into Players(" +
                            "Player_Name,Wins,Loses,Draws,Games_Played) " +
                            "values(@name,@wins,@loses,@draws,@games_played)";
                    SQLiteCommand command = new SQLiteCommand(insertSQL, connection);
                    command.Parameters.AddWithValue("name", form1.p1.name);
                    command.Parameters.AddWithValue("wins", (winner == 0) ? 0 : (winner == 1) ? 1 : 0);
                    command.Parameters.AddWithValue("loses", (winner == 0) ? 0 : (winner == 1) ? 0 : 1);
                    command.Parameters.AddWithValue("draws", (winner == 0) ? 1 : (winner == 1) ? 0 : 0);
                    command.Parameters.AddWithValue("games_played", 1);
                    int rowsInserted = command.ExecuteNonQuery();
                    connection.Close();
                }
                else
                {//player 1 already existed
                    connection.Open();
                    String updateSQL = "Update Players " +
                            "set Wins=@wins," +
                            "Loses=@loses," +
                            "Draws=@draws," +
                            "Games_Played= Games_Played+1 " +
                            "where Player_Name==@name";
                    SQLiteCommand command = new SQLiteCommand(updateSQL, connection);
                    command.Parameters.AddWithValue("name", form1.p1.name);
                    command.Parameters.AddWithValue("wins", form1.p1.wins + ((winner == 0) ? 0 : (winner == 1) ? 1 : 0));
                    command.Parameters.AddWithValue("loses", form1.p1.loses + ((winner == 0) ? 0 : (winner == 1) ? 0 : 1));
                    command.Parameters.AddWithValue("draws", form1.p1.draws + ((winner == 0) ? 1 : (winner == 1) ? 0 : 0));
                    int rowsInserted = command.ExecuteNonQuery();
                    connection.Close();
                }
                if (!form1.p2.exists)//player 2 didnt exist
                {
                    form1.p2.exists = true;
                    connection.Open();
                    String insertSQL = "Insert into Players(" +
                            "Player_Name,Wins,Loses,Draws,Games_Played) " +
                            "values(@name,@wins,@loses,@draws,@games_played)";
                    SQLiteCommand command = new SQLiteCommand(insertSQL, connection);
                    command.Parameters.AddWithValue("name", form1.p2.name);
                    command.Parameters.AddWithValue("wins", (winner == 0) ? 0 : (winner == 2) ? 1 : 0);
                    command.Parameters.AddWithValue("loses", (winner == 0) ? 0 : (winner == 2) ? 0 : 1);
                    command.Parameters.AddWithValue("draws", (winner == 0) ? 1 : (winner == 2) ? 0 : 0);
                    command.Parameters.AddWithValue("games_played", 1);
                    int rowsInserted = command.ExecuteNonQuery();
                    connection.Close();
                }
                else //player 2 already existed
                {
                    connection.Open();
                    String updateSQL = "Update Players " +
                            "set Wins=@wins," +
                            "Loses=@loses," +
                            "Draws=@draws," +
                            "Games_Played= Games_Played+1 " +
                            "where Player_Name=@name";
                    SQLiteCommand command = new SQLiteCommand(updateSQL, connection);
                    command.Parameters.AddWithValue("name", form1.p2.name);
                    command.Parameters.AddWithValue("wins", form1.p2.wins + ((winner == 0) ? 0 : (winner == 2) ? 1 : 0));
                    command.Parameters.AddWithValue("loses", form1.p2.loses + ((winner == 0) ? 0 : (winner == 2) ? 0 : 1));
                    command.Parameters.AddWithValue("draws", form1.p2.draws + ((winner == 0) ? 1 : (winner == 2) ? 0 : 0));
                    int rowsInserted = command.ExecuteNonQuery();
                    connection.Close();
                }
                //update class in case of multiple games played in a row
                form1.p2.wins += (winner == 0) ? 0 : (winner == 2) ? 1 : 0; 
                form1.p2.loses += (winner == 0) ? 0 : (winner == 2) ? 0 : 1;
                form1.p2.draws += (winner == 0) ? 1 : (winner == 2) ? 0 : 0;

            }
            //update class in case of multiple games played in a row
            form1.p1.wins += (winner == 0) ? 0 : (winner == 1) ? 1 : 0; 
            form1.p1.loses += (winner == 0) ? 0 : (winner == 1) ? 0 : 1;
            form1.p1.draws += (winner == 0) ? 1 : (winner == 1) ? 0 : 0;

            this.Close();  
        }

        private void Form_Closing(object sender, CancelEventArgs e)
        {
            if (close)
            {
                if (MessageBox.Show("Would you like to go back to the homepage? If you go back no information about this game will be saved. ", "Quit Game", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                    close = false;
                }
                else
                {                   
                    form1.Visible = true;
                    form1.Focus();                   
                }
            }else if (newGameFlag && !close)
            {
                if (MessageBox.Show("Would you like to start a new game? No information about this game will be saved.", "New Game", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                    newGameFlag = false;
                }
                else
                {
                    Form4 newGame = new Form4(form1, N, pNum);//create new form(game)
                    newGame.Show();
                }
            }else if (finished)
            {
                if (MessageBox.Show("Would you like a rematch?", "Rematch", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    form1.Visible = true;
                    form1.Focus();
                }
                else
                {
                    Form4 newGame1 = new Form4(form1, N, pNum);//create new form(game)
                    newGame1.Show();
                }
            }
            else
            {
                if (MessageBox.Show("Would you like to quit this app? If you quit no information about this game will be saved. ", "Quit App", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    form1.Close();                    
                }
            }            
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //through message box for warning and then create new game with the same players and N
            newGameFlag = true;
            this.Close();
        }

        private void quitGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            close = true;           
            this.Close();
        }

        private void playerDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pNum == 2)
            {
                MessageBox.Show("Player 1: " + form1.p1.name + " -> X" + "\n" + "Player 2: " + form1.p2.name + " -> O" + "\n", "Player Details");
            }
            else
            {
                MessageBox.Show("Player : " + form1.p1.name + " -> X" + "\n" + "Computer -> O" + "\n", "Player Details");
            }

        }
    }
}