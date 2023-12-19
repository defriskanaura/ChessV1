namespace ChessLib
{
    public class Board
    {
        private readonly Piece[,] pieces = new Piece[8, 8];

        private readonly Dictionary<ColorType, Position> pawnSkipPositions = new Dictionary<ColorType, Position>
        {
            { ColorType.White, null },
            { ColorType.Black, null }
        };

        public Piece this[int row, int col]
        {
            get { return pieces[row, col]; }
            set { pieces[row, col] = value; }
        }

        public Piece this[Position pos]
        {
            get { return this[pos.Row, pos.Column]; }
            set { this[pos.Row, pos.Column] = value; }
        }

        public Position GetPawnSkipPosition(ColorType playerColor)
        {
            return pawnSkipPositions[playerColor];
        }

        public void SetPawnSkipPosition(ColorType playerColor, Position pos)
        {
            pawnSkipPositions[playerColor] = pos;
        }

        public static Board Initial()
        {
            Board board = new Board();
            board.AddStartPieces();
            return board;
        }

        public void AddStartPieces()
        {
            this[0, 0] = new Rook(ColorType.Black);
            this[0, 1] = new Knight(ColorType.Black);
            this[0, 2] = new Bishop(ColorType.Black);
            this[0, 3] = new Queen(ColorType.Black);
            this[0, 4] = new King(ColorType.Black);
            this[0, 5] = new Bishop(ColorType.Black);
            this[0, 6] = new Knight(ColorType.Black);
            this[0, 7] = new Rook(ColorType.Black);

            this[7, 0] = new Rook(ColorType.White);
            this[7, 1] = new Knight(ColorType.White);
            this[7, 2] = new Bishop(ColorType.White);
            this[7, 3] = new Queen(ColorType.White);
            this[7, 4] = new King(ColorType.White);
            this[7, 5] = new Bishop(ColorType.White);
            this[7, 6] = new Knight(ColorType.White);
            this[7, 7] = new Rook(ColorType.White);

            for (int c = 0; c < 8; c++)
            {
                this[1, c] = new Pawn(ColorType.Black);
                this[6, c] = new Pawn(ColorType.White);
            }
        }

        public static bool IsInside(Position pos)
        {
            return pos.Row >= 0 && pos.Row < 8 && pos.Column >= 0 && pos.Column < 8;
        }

        public bool IsEmpty(Position pos)
        {
            return this[pos] == null;
        }

        public IEnumerable<Position> PiecePositions()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Position pos = new Position(r, c);

                    if (!IsEmpty(pos))
                    {
                        yield return pos;
                    }
                }
            }
        }

        public IEnumerable<Position> PiecePositionsFor(ColorType playerColor)
        {
            return PiecePositions().Where(pos => this[pos].Color == playerColor);
        }

        public bool IsInCheck(ColorType playerColor)
        {
            return PiecePositionsFor(playerColor.Opponent()).Any(pos =>
            {
                Piece piece = this[pos];
                return piece.CanCaptureOpponentKing(pos, this);
            });
        }

        public Board Copy()
        {
            Board copy = new Board();

            foreach (Position pos in PiecePositions())
            {
                copy[pos] = this[pos].Copy();
            }

            return copy;
        }

        public Counting CountPieces()
        {
            Counting counting = new Counting();

            foreach (Position pos in PiecePositions())
            {
                Piece piece = this[pos];
                counting.Increment(piece.Color, piece.Type);
            }

            return counting;
        }

        public bool InsufficientMaterial()
        {
            Counting counting = CountPieces();

            return IsKingVKing(counting) || IsKingBishopVKing(counting) ||
                   IsKingKnightVKing(counting) || IsKingBishopVKingBishop(counting);
        }

        private static bool IsKingVKing(Counting counting)
        {
            return counting.TotalCount == 2;
        }

        private static bool IsKingBishopVKing(Counting counting)
        {
            return counting.TotalCount == 3 && (counting.White(PieceType.Bishop) == 1 || counting.Black(PieceType.Bishop) == 1);
        }

        private static bool IsKingKnightVKing(Counting counting)
        {
            return counting.TotalCount == 3 && (counting.White(PieceType.Knight) == 1 || counting.Black(PieceType.Knight) == 1);
        }

        private bool IsKingBishopVKingBishop(Counting counting)
        {
            if (counting.TotalCount != 4)
            {
                return false;
            }

            if (counting.White(PieceType.Bishop) != 1 || counting.Black(PieceType.Bishop) != 1)
            {
                return false;
            }

            Position wBishopPos = FindPiece(ColorType.White, PieceType.Bishop);
            Position bBishopPos = FindPiece(ColorType.Black, PieceType.Bishop);

            return wBishopPos.SquareColor() == bBishopPos.SquareColor();
        }

        private Position FindPiece(ColorType pieceColor, PieceType type)
        {
            return PiecePositionsFor(pieceColor).First(pos => this[pos].Type == type);
        }

        private bool IsUnmovedKingAndRook(Position kingPos, Position rookPos)
        {
            if (IsEmpty(kingPos) || IsEmpty(rookPos))
            {
                return false;
            }

            Piece king = this[kingPos];
            Piece rook = this[rookPos];

            return king.Type == PieceType.King && rook.Type == PieceType.Rook &&
                   !king.HasMoved && !rook.HasMoved;
        }

        public bool CastleRightKS(ColorType playerColor)
        {
            return playerColor switch
            {
                ColorType.White => IsUnmovedKingAndRook(new Position(7, 4), new Position(7, 7)),
                ColorType.Black => IsUnmovedKingAndRook(new Position(0, 4), new Position(0, 7)),
                _ => false
            };
        }

        public bool CastleRightQS(ColorType playerColor)
        {
            return playerColor switch
            {
                ColorType.White => IsUnmovedKingAndRook(new Position(7, 4), new Position(7, 0)),
                ColorType.Black => IsUnmovedKingAndRook(new Position(0, 4), new Position(0, 0)),
                _ => false
            };
        }

        private bool HasPawnInPosition(ColorType playerColor, Position[] pawnPositions, Position skipPos)
        {
            foreach (Position pos in pawnPositions.Where(IsInside))
            {
                Piece piece = this[pos];
                if (piece == null || piece.Color != playerColor || piece.Type != PieceType.Pawn)
                {
                    continue;
                }

                EnPassant move = new EnPassant(pos, skipPos);
                if (move.IsLegal(this))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CanCaptureEnPassant(ColorType playerColor)
        {
            Position skipPos = GetPawnSkipPosition(playerColor.Opponent());

            if (skipPos == null)
            {
                return false;
            }

            Position[] pawnPositions = playerColor switch
            {
                ColorType.White => new Position[] { skipPos + Direction.SouthWest, skipPos + Direction.SouthEast },
                ColorType.Black => new Position[] { skipPos + Direction.NorthWest, skipPos + Direction.NorthEast },
                _ => Array.Empty<Position>()
            };

            return HasPawnInPosition(playerColor, pawnPositions, skipPos);
        }
    }
}
