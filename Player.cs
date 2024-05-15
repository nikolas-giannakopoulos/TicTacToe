using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class Player
    {
        public string name;
        public int wins;
        public int loses;
        public int draws;
        public int gamesPlayed;
        public bool exists;

        public Player(string name, int wins, int loses, int draws, int gamesPlayed, bool exists)
        {
            this.name = name;
            this.wins = wins;
            this.loses = loses;
            this.draws = draws;
            this.gamesPlayed = gamesPlayed;
            this.exists = exists;
        }
    }
}
