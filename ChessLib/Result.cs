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

        public static Result Win(ColorType winner, EndReason reason)
        {
            return new Result(winner, reason);
        }

        public static Result Draw(EndReason reason)
        {
            return new Result(ColorType.None, reason);
        }
    }
}
