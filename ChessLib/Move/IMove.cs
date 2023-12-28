namespace ChessLib
{
    public interface IMove
    {
        public int Id { get; }
        public MoveType Type { get; }
        public Position FromPos { get; }
        public Position ToPos { get; }
        public IPiece MovedPiece { get; set; }
        public IPiece CapturedPiece { get; set; }

        public bool CaptureOrPawnMove { get; set; }

        public void Execute(IBoard board);

    }
}
