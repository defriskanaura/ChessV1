namespace ChessLib
{
    public class ChessKing : IPiece
    {
        public int Id { get; }
        public PieceType Type => PieceType.King;
        public ColorType Color { get; }
        public bool HasMoved { get; set; }

        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.North,
            Direction.South,
            Direction.East,
            Direction.West,
            Direction.NorthWest,
            Direction.NorthEast,
            Direction.SouthWest,
            Direction.SouthEast
        };

        public ChessKing(ColorType pieceColor)
        {
            Color = pieceColor;
            GenerateID.Piece++;
            Id = GenerateID.Piece;
        }
        public IPiece Copy()
        {
            ChessKing copy = new ChessKing(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }
        private IEnumerable<Position> PositionsInDir(Position from, IBoard board)
        {
            foreach (Direction dir in dirs)
            {
                Position to = from + dir;

                if (!board.IsInside(to, board))
                {
                    continue;
                }

                if (board.IsEmpty(to, board) || board[to].Color != Color)
                {
                    yield return to;
                }
            }
        }
        public IEnumerable<Position> PossiblePosition(Position from, IBoard board)
        {
            foreach (Position to in PositionsInDir(from, board))
            {
                yield return to;
            }
        }
    }
}
