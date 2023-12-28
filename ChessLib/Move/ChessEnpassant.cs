namespace ChessLib
{
    public class ChessEnpassant : IMove
    {
        public int Id { get; }
        public MoveType Type => MoveType.EnPassant;
        public Position FromPos { get; }
        public Position ToPos { get; }
        public IPiece MovedPiece { get; set; }
        public IPiece CapturedPiece { get; set; }
        public bool CaptureOrPawnMove { get; set; }

        private readonly Position capturePos;

        public ChessEnpassant(Position from, Position to)
        {
            FromPos = from;
            ToPos = to;
            capturePos = new Position(from.Row, to.Column);
            GenerateID.Move++;
            Id = GenerateID.Move;
        }

        public void Execute(IBoard board)
        {
            MovedPiece = board[FromPos];
            CapturedPiece = board[capturePos];
            new ChessNormalMove(FromPos, ToPos).Execute(board);
            board[capturePos] = null;

            CaptureOrPawnMove = true;
        }
    }
}
