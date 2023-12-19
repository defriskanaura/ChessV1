﻿namespace ChessLib
{
    public class Rook : Piece
    {
        public override PieceType Type => PieceType.Rook;
        public override ColorType Color { get; }

        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.North,
            Direction.South,
            Direction.East,
            Direction.West
        };

        public Rook(ColorType pieceColor)
        {
            Color = pieceColor;
        }

        public override Piece Copy()
        {
            Rook copy = new Rook(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            return MovePositionsInDirs(from, board, dirs).Select(to => new NormalMove(from, to));
        }
    }
}