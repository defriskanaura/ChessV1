namespace ChessLib
{
    public class Result
    {
        public ColorType Winner { get; }
        public EndReason Reason { get; }

        public Result(ColorType winner, EndReason reason)
        {
            Winner = winner;
            Reason = reason;
        }

        public static Result Win(ColorType winner)
        {
            return new Result(winner, EndReason.Checkmate);
        }

        public static Result Draw(EndReason reason)
        {
            return new Result(ColorType.None, reason);
        }
    }
}
