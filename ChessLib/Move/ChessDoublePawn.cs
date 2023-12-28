namespace ChessLib
{
    public class ChessDoublePawn : IMove
    {
        public int Id { get; }
        public MoveType Type => MoveType.DoublePawn;
        public Position FromPos { get; }
        public Position ToPos { get; }
        public IPiece MovedPiece { get; set; }
        public IPiece CapturedPiece { get; set; }
        public bool CaptureOrPawnMove { get; set;  }

        public readonly Position skippedPos;

        public ChessDoublePawn(Position from, Position to)
        {
            FromPos = from;
            ToPos = to;
            skippedPos = new Position((from.Row + to.Row) / 2, from.Column);
            GenerateID.Move++;
            Id = GenerateID.Move;
        }

        public void Execute(IBoard board)
        {
            MovedPiece = board[FromPos];
            CapturedPiece = null;
            ColorType playerColor = board[FromPos].Color;
            new ChessNormalMove(FromPos, ToPos).Execute(board);

            CaptureOrPawnMove = true;
        }
    }
}
