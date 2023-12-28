namespace ChessLib
{
    public class ChessNormalMove : IMove
    {
        public int Id { get; }
        public MoveType Type => MoveType.Normal;
        public Position FromPos { get; }
        public Position ToPos { get; }
        public IPiece MovedPiece { get; set; }
        public IPiece CapturedPiece { get; set; }
        public bool CaptureOrPawnMove { get; set; } 

        public ChessNormalMove(Position from, Position to)
        {
            FromPos = from;
            ToPos = to;
            GenerateID.Move++;
            Id = GenerateID.Move;
        }

        public void Execute(IBoard board)
        {
            MovedPiece = board[FromPos];
            CapturedPiece = board[ToPos];
            IPiece piece = board[FromPos];
            bool capture = !board.IsEmpty(ToPos, board);
            board[ToPos] = piece;
            board[FromPos] = null;
            piece.HasMoved = true;
            if (capture == true || piece.Type == PieceType.Pawn)
            {
                CaptureOrPawnMove = true;
            }
        }
    }
}
