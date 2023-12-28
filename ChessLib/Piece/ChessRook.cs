namespace ChessLib
{
    public class ChessRook : IPiece
    {
        public int Id { get; }
        public PieceType Type => PieceType.Rook;
        public ColorType Color { get; }
        public bool HasMoved { get; set; }

        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.North,
            Direction.South,
            Direction.East,
            Direction.West
        };

        public ChessRook(ColorType pieceColor)
        {
            Color = pieceColor;
            GenerateID.Piece++;
            Id = GenerateID.Piece;
        }
        public IPiece Copy()
        {
            ChessRook copy = new ChessRook(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }
        private IEnumerable<Position> PositionsInDir(Position from, IBoard board, Direction dir)
        {
            for (Position pos = from + dir; board.IsInside(pos, board); pos += dir)
            {
                if (board.IsEmpty(pos, board))
                {
                    yield return pos;
                    continue;
                }

                IPiece piece = board[pos];

                if (piece.Color != Color)
                {
                    yield return pos;
                }

                yield break;
            }
        }
        private IEnumerable<Position> PositionsInDirs(Position from, IBoard board, Direction[] dirs)
        {
            return dirs.SelectMany(dir => PositionsInDir(from, board, dir));
        }
        public IEnumerable<Position> PossiblePosition(Position from, IBoard board)
        {
            IEnumerable<Position> possiblePosition = PositionsInDirs(from, board, dirs);
            return possiblePosition;
        }
    }
}
