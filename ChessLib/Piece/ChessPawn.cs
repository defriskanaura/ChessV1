namespace ChessLib
{
    public class ChessPawn : IPiece
    {
        public int Id { get; }
        public PieceType Type => PieceType.Pawn;
        public ColorType Color { get; }
        public bool HasMoved { get; set; }
        public ChessPawn(ColorType pieceColor)
        {
            Color = pieceColor;
            GenerateID.Piece++;
            Id = GenerateID.Piece;
        }
        public IPiece Copy()
        {
            ChessPawn copy = new ChessPawn(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }
        public IEnumerable<Position> PossiblePosition(Position from, IBoard board)
        {
            Position pos = new Position(0,0);
            if (Color == ColorType.White)
            {
                pos = from + Direction.North;
            }
            if (Color == ColorType.Black)
            {
                pos = from + Direction.South;
            }
            if (board.IsInside(pos, board) && board.IsEmpty(pos, board))
            {
                yield return pos;
            }
            yield break;
        }
    }
}
