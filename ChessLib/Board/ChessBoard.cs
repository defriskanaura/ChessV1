namespace ChessLib
{
    public class ChessBoard : IBoard
    {
        public int Id { get; }
        public int Row { get; }
        public int Column { get; }
        public IPiece[,] Square { get; }
        public ChessBoard(int row, int col)
        {
            Row = row;
            Column = col;
            Square = new IPiece[Row, Column];
            GenerateID.Board++;
            Id = GenerateID.Board;
        }
        public IPiece this[int row, int col]
        {
            get { return Square[row, col]; }
            set { Square[row, col] = value; }
        }
        public IPiece this[Position pos]
        {
            get { return Square[pos.Row, pos.Column]; }
            set { Square[pos.Row, pos.Column] = value; }
        }
        
        public bool IsInside(Position pos, IBoard board)
        {
            return pos.Row >= 0 && pos.Row < Row && pos.Column >= 0 && pos.Column < Column;
        }

        public bool IsEmpty(Position pos, IBoard board)
        {
            return board[pos] == null;
        }
    }
}
