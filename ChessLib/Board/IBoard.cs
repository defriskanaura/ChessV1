namespace ChessLib
{
    public interface IBoard
    {
        public int Id { get; }
        public int Row { get; }
        public int Column { get; }
        public IPiece[,] Square { get; }
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
        public bool IsInside(Position pos, IBoard board);

        public bool IsEmpty(Position pos, IBoard board);
    }
}
