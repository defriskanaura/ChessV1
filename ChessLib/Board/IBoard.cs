using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib
{
    public abstract class Board1
    {
        public int row;
        public int column;
        public Piece[,] square;
        public Piece this[int row, int col]
        {
            get { return square[row, col]; }
            set { square[row, col] = value; }
        }

        public Piece this[Position pos]
        {
            get { return this[pos.Row, pos.Column]; }
            set { this[pos.Row, pos.Column] = value; }
        }
    }
}
