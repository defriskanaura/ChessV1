namespace ChessLib
{
    public class ChessKnight : IPiece
    {
        public int Id { get; }
        public PieceType Type => PieceType.Knight;
        public ColorType Color { get; }
        public bool HasMoved { get; set; }

        public ChessKnight(ColorType pieceColor)
        {
            Color = pieceColor;
            GenerateID.Piece++;
            Id = GenerateID.Piece;
        }
        public IPiece Copy()
        {
            ChessKnight copy = new ChessKnight(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }
        private static IEnumerable<Position> PositionsInDir(Position from)
        {
            foreach (Direction vDir in new Direction[] { Direction.North, Direction.South })
            {
                foreach (Direction hDir in new Direction[] { Direction.West, Direction.East })
                {
                    yield return from + 2 * vDir + hDir;
                    yield return from + 2 * hDir + vDir;
                }
            }
        }
        public IEnumerable<Position> PossiblePosition(Position from, IBoard board)
        {
            return PositionsInDir(from).Where(pos => board.IsInside(pos, board)
                && (board.IsEmpty(pos, board) || board[pos].Color != Color));
        }
    }
}
