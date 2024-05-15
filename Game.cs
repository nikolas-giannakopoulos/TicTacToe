using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicTacToe.Properties;

namespace TicTacToe
{
    public class Game
    {
        public Button b = new Button();
        public int i;
        public int j;
        public string played;
        
        public Game(EventHandler Game_Click, int W, int H, int i, int j, Point loca, string played) {
            this.b.Size = new Size(W,H);
            this.b.Location = loca;
            this.b.Click += Game_Click;
            this.i = i;
            this.j = j;
            this.played = played;
            this.b.Tag = this;
            //image
            this.b.BackgroundImageLayout = ImageLayout.Stretch;
            this.b.BackgroundImage = Resources.DefaultBox;
            this.b.FlatStyle = FlatStyle.Flat;
            this.b.FlatAppearance.BorderSize = 0;
        }
    }
}
