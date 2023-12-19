using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChessLib;

namespace ChessUI
{
    public static class Images
    {
        private static readonly Dictionary<PieceType, ImageSource> whiteSources = new()
        {
            { PieceType.Pawn, LoadImage("PinkyChessAsset/PawnW.png") },
            { PieceType.Bishop, LoadImage("PinkyChessAsset/BishopW.png") },
            { PieceType.Knight, LoadImage("PinkyChessAsset/KnightW.png") },
            { PieceType.Rook, LoadImage("PinkyChessAsset/RookW.png") },
            { PieceType.Queen, LoadImage("PinkyChessAsset/QueenW.png") },
            { PieceType.King, LoadImage("PinkyChessAsset/KingW.png") }
        };

        private static readonly Dictionary<PieceType, ImageSource> blackSources = new()
        {
            { PieceType.Pawn, LoadImage("PinkyChessAsset/PawnB.png") },
            { PieceType.Bishop, LoadImage("PinkyChessAsset/BishopB.png") },
            { PieceType.Knight, LoadImage("PinkyChessAsset/KnightB.png") },
            { PieceType.Rook, LoadImage("PinkyChessAsset/RookB.png") },
            { PieceType.Queen, LoadImage("PinkyChessAsset/QueenB.png") },
            { PieceType.King, LoadImage("PinkyChessAsset/KingB.png") }
        };

        private static ImageSource LoadImage(string filePath)
        {
            return new BitmapImage(new Uri(filePath, UriKind.Relative));
        }

        public static ImageSource GetImage(ColorType color, PieceType type)
        {
            return color switch
            {
                ColorType.White => whiteSources[type],
                ColorType.Black => blackSources[type],
                _ => null
            };
        }

        public static ImageSource GetImage(Piece piece)
        {
            if (piece == null)
            {
                return null;
            }

            return GetImage(piece.Color, piece.Type);
        }
    }
}
