namespace ChessLib
{
    public class ChessQueen : IPiece
    {
        public int Id { get; }
        public PieceType Type => PieceType.Queen;
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
        public ChessQueen(ColorType pieceColor)
        {
            Color = pieceColor;
            GenerateID.Piece++;
            Id = GenerateID.Piece;
        }
        public IPiece Copy()
        {
            ChessQueen copy = new ChessQueen(Color);
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
