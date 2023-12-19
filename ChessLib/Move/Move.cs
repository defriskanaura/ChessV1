namespace ChessLib
{
    public abstract class Move
    {
        public abstract MoveType Type { get; }
        public abstract Position FromPos { get; }
        public abstract Position ToPos { get; }

        public abstract bool Execute(Board board);

        public virtual bool IsLegal(Board board)
        {
            ColorType playerColor = board[FromPos].Color;
            Board boardCopy = board.Copy();
            Execute(boardCopy);
            return !boardCopy.IsInCheck(playerColor);
        }
    }
}
