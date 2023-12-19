namespace ChessLib
{
    public enum ColorType
    {
        None,
        White,
        Black
    }

    public static class PlayerExtensions
    {
        public static ColorType Opponent(this ColorType playerColor)
        {
            return playerColor switch
            {
                ColorType.White => ColorType.Black,
                ColorType.Black => ColorType.White,
                _ => ColorType.None,
            };
        }
    }
}
