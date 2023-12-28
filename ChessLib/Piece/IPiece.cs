namespace ChessLib
{
    public interface IPiece
    {
        
        public int Id { get; }
        public PieceType Type { get; }
        public ColorType Color { get; }
        public bool HasMoved { get; set; }
        public IPiece Copy();

        public IEnumerable<Position> PossiblePosition(Position from, IBoard board);
    }
}
