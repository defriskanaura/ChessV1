using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib
{
    public class ChessBoard : Board1
    {
        public new int row;
        public new int column;
        public new Piece[,] square;
        public ChessBoard(int rows, int cols)
        {
            row = rows;
            column = cols;
            Piece[,] pieces = new Piece[rows, cols];
        }
        public new Piece this[int row, int col]
        {
            get { return square[row, col]; }
            set { square[row, col] = value; }
        }

        public new Piece this[Position pos]
        {
            get { return this[pos.Row, pos.Column]; }
            set { this[pos.Row, pos.Column] = value; }
        }
    }
}
