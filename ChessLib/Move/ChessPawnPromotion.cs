namespace ChessLib
{
    public class ChessPawnPromotion : IMove
    {
        public int Id { get; }
        public MoveType Type => MoveType.PawnPromotion;
        public Position FromPos { get; }
        public Position ToPos { get; }
        public IPiece MovedPiece { get; set; }
        public IPiece CapturedPiece { get; set; }
        public bool CaptureOrPawnMove { get; set; }

        private readonly PieceType newType;

        public ChessPawnPromotion(Position from, Position to, PieceType newType)
        {
            FromPos = from;
            ToPos = to;
            this.newType = newType;
            GenerateID.Move++;
            Id = GenerateID.Move;
        }

        private IPiece CreatePromotionPiece(ColorType pieceColor)
        {
            return newType switch
            {
                PieceType.Knight => new ChessKnight(pieceColor),
                PieceType.Bishop => new ChessBishop(pieceColor),
                PieceType.Rook => new ChessRook(pieceColor),
                _ => new ChessQueen(pieceColor)
            };
        }

        public void Execute(IBoard board)
        {
            MovedPiece = board[FromPos];
            CapturedPiece = board[ToPos];
            IPiece pawn = board[FromPos];
            board[FromPos] = null;
            IPiece promotionPiece = CreatePromotionPiece(pawn.Color);
            promotionPiece.HasMoved = true;
            board[ToPos] = promotionPiece;
            CaptureOrPawnMove = true;
        }
    }
}
