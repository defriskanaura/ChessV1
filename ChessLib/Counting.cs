﻿namespace ChessLib
{
    public class Counting
    {
        public readonly Dictionary<PieceType, int> whiteCount = new();
        public readonly Dictionary<PieceType, int> blackCount = new();

        public int TotalCount { get; private set; }

        public Counting()
        {
            foreach (PieceType type in Enum.GetValues(typeof(PieceType)))
            {
                whiteCount[type] = 0;
                blackCount[type] = 0;
            }
        }

        public void Increment(ColorType pieceColor, PieceType type)
        {
            if (pieceColor == ColorType.White)
            {
                whiteCount[type]++;
            }
            else if (pieceColor == ColorType.Black)
            {
                blackCount[type]++;
            }

            TotalCount++;
        }

        public int White(PieceType type)
        {
            return whiteCount[type];
        }

        public int Black(PieceType type)
        {
            return blackCount[type];
        }
    }
}
