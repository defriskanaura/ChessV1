namespace ChessLib
{
    public class ChessCastle : IMove
    {
        public int Id { get; }
        public MoveType Type { get; }
        public Position FromPos { get; }
        public Position ToPos { get; }
        public IPiece MovedPiece { get; set; }
        public IPiece CapturedPiece { get; set; }
        public bool CaptureOrPawnMove { get; set; }

        private readonly Direction kingMoveDir;
        private readonly Position rookFromPos;
        private readonly Position rookToPos;

        public ChessCastle(MoveType type, Position kingPos)
        {
            Type = type;
            FromPos = kingPos;
            GenerateID.Move++;
            Id = GenerateID.Move;

            if (type == MoveType.CastleKS)
            {
                kingMoveDir = Direction.East;
                ToPos = new Position(kingPos.Row, 6);
                rookFromPos = new Position(kingPos.Row, 7);
                rookToPos = new Position(kingPos.Row, 5);
            }
            else if (type == MoveType.CastleQS)
            {
                kingMoveDir = Direction.West;
                ToPos = new Position(kingPos.Row, 2);
                rookFromPos = new Position(kingPos.Row, 0);
                rookToPos = new Position(kingPos.Row, 3);
            }
        }

        public void Execute(IBoard board)
        {
            MovedPiece = board[FromPos];
            CapturedPiece = null;
            new ChessNormalMove(FromPos, ToPos).Execute(board);
            new ChessNormalMove(rookFromPos, rookToPos).Execute(board);

            CaptureOrPawnMove = false;
        }
    }
}
