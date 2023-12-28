using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

namespace ChessLib
{
    public class GameManager
    {
        private IBoard _Board { get; }
        private Dictionary<IPlayer, ColorType> _playerList = new Dictionary<IPlayer, ColorType>();
        private ColorType _CurrentPlayerColor { get; set; }
        private List<IPiece> _killedPieceList = new List<IPiece>();
        private IEnumerable<IMove> _moveCandidates;
        private Result _Result { get; set; }
        private int _noCaptureOrPawnMoves = 0;
        private Direction _kingMoveDir;
        private Dictionary<ColorType, Position> _pawnSkipPositions = new Dictionary<ColorType, Position>
                                                                    {
                                                                        { ColorType.White, null },
                                                                        { ColorType.Black, null }
                                                                    };
        private readonly ILogger<GameManager>? _logger;
        /// <summary>
        /// create a new game with just board as parameter
        /// </summary>
        /// <param name="board">the used board for chess</param>
        /// <param name="logger"></param>
        public GameManager(IBoard board, ILogger<GameManager>? logger = null)
        {
            _Board = board;
            _logger = logger;
            _logger?.LogInformation("New chess game has been made");
        }
        /// <summary>
        /// create a new game with board and the first color to play as parameter
        /// </summary>
        /// <param name="playerColor">the first color to play</param>
        /// <param name="board">the used board for chess</param>
        /// <param name="logger"></param>
        public GameManager(ColorType playerColor, IBoard board, ILogger<GameManager>? logger = null) : base()
        {
            _CurrentPlayerColor = playerColor;
            _Board = board;
            
            _logger = logger;
            _logger?.LogInformation("New chess game has been made");
            _logger?.LogInformation("Current player is {playercolor}", playerColor);
        }
        /// <summary>
        /// adding player with player and color as parameter
        /// </summary>
        /// <param name="player">the added player instance</param>
        /// <param name="playerColor">color assigned for the added player</param>
        /// <returns>true if the player can be added and not in the player list</returns>
        public bool AddPlayer(IPlayer player, ColorType playerColor)
        {
            if (_playerList.ContainsKey(player) || _playerList.ContainsValue(playerColor))
            {
                _logger?.LogWarning("New player {name} can't be added", player.Name);
                return false;
            }
            _playerList.Add(player, playerColor);
            _logger?.LogInformation("New player {name} with color {color} has been added", player.Name, playerColor);
            return true;
        }
        /// <summary>
        /// adding player with name and color as parameter
        /// </summary>
        /// <param name="name">the name of the added player</param>
        /// <param name="playerColor">color assigned for the added player</param>
        /// <returns>true if the player can be added and not in the player list</returns>
        public bool AddPlayer(string name, ColorType playerColor)
        {
            if (_playerList.Any(item => item.Key.Name == name) || _playerList.ContainsValue(playerColor))
            {
                _logger?.LogWarning("New player {name} can't be added", name);
                return false;
            }
            Player player = new Player(name);
            _playerList.Add(player, playerColor);

            _logger?.LogInformation("New player {name} with color {color} has been added", name, playerColor);
            return true;
        }
        /// <summary>
        /// get all player list
        /// </summary>
        /// <returns>all available player</returns>
        public IEnumerable<IPlayer> GetPlayer()
        {
            _logger?.LogInformation("Show player list");
            return _playerList.Where(item => item.Key.Id != 0).Select(item => item.Key);
        }
        /// <summary>
        /// try to get player based from its assigned color
        /// </summary>
        /// <param name="playerColor">the player color</param>
        /// <returns>null if the player has not added yet</returns>
        public IPlayer GetPlayer(ColorType playerColor)
        {
            foreach (var item in _playerList)
            {
                if (item.Value == playerColor)
                {
                    _logger?.LogInformation("Player {player} color is {color}", item.Key, playerColor);
                    return item.Key;
                }
            }
            _logger?.LogWarning("Player with assigned color {color}has not been added yet", playerColor);
            return null;
        }
        /// <summary>
        /// try to get the current player who's playing now
        /// </summary>
        /// <returns>return null if there is no current playing player</returns>
        public IPlayer GetCurrentPlayer()
        {
            foreach (var item in _playerList)
            {
                if (item.Value == GetCurrentColor())
                {
                    _logger?.LogInformation("The current playing player is {player}", item.Key);
                    return item.Key;
                }
            }
            _logger?.LogWarning("There is no current playing player");
            return null;
        }
        /// <summary>
        /// set current player with player instance as parameter
        /// </summary>
        /// <param name="player">the player instance that you want to set as current player</param>
        /// <returns>false if player has not been added yet and true as long as the color is not none</returns>
        public bool SetCurrentPlayer(IPlayer player)
        {
            if (!_playerList.ContainsKey(player))
            {
                _logger?.LogWarning("Player {player} has not been added to the game", player);
                return false;
            }
            SetCurrentColor(GetPlayerColor(player));
            _logger?.LogInformation("Player {player} has been assigned as the current player", player);
            return true;
        }
        /// <summary>
        /// get current color of the current playing player
        /// </summary>
        /// <returns>the current color of the current playing player</returns>
        public ColorType GetCurrentColor()
        {
            _logger?.LogInformation("Obtaining the current player color which is {color}", _CurrentPlayerColor);
            return _CurrentPlayerColor;
        }
        /// <summary>
        /// set certain color as the color for current player
        /// </summary>
        /// <param name="playerColor">it can't be ColorType.None</param>
        /// <returns>true if succeeded to set the current color</returns>
        public bool SetCurrentColor(ColorType playerColor)
        {
            //Enum.IsDefined(typeof(ColorType), playerColor)
            if (playerColor == ColorType.None)
            {
                _logger?.LogWarning("Current player is assigned in color {color}", playerColor);
                return false;
            }
            _logger?.LogInformation("Current player is assigned in color {color}", playerColor);
            _CurrentPlayerColor = playerColor;
            return true;
        }
        /// <summary>
        /// get color from the opponent of the current playing player
        /// </summary>
        /// <returns>the opponent's color</returns>
        public ColorType GetOpponentColor()
        {
            _logger?.LogInformation("Obtaining the current opponent color which is {color}", _CurrentPlayerColor.Opponent());
            return _CurrentPlayerColor.Opponent();
        }
        /// <summary>
        /// get the color from certain player instance 
        /// </summary>
        /// <param name="player">player instance as parameter</param>
        /// <returns>the color of the picked player instance</returns>
        public ColorType GetPlayerColor(IPlayer player)
        {
            _logger?.LogInformation("Player {player} is assigned in color {color}", player, _playerList[player]);
            return _playerList[player];
        }
        /// <summary>
        /// gaining access to the board of the game
        /// </summary>
        /// <returns>board of the game</returns>
        public IBoard GetBoard()
        {
            _logger?.LogInformation("Showing Board {board} of the game", _Board);
            return _Board;
        }
        /// <summary>
        /// copying used board, the board is copied so we can check if the player is in checkmate or not
        /// </summary>
        /// <param name="board">the current used board</param>
        /// <returns>the copy of the current used board</returns>
        public IBoard Copy(IBoard board)
        {
            _logger?.LogInformation("The board is getting copied");
            foreach (Position pos in GetActivePiecePosition())
            {
                board[pos] = _Board[pos].Copy();
            }
            return board;
        }
        /// <summary>
        /// adding the starter piece to the board
        /// </summary>
        public void AddStartPieces()
        {
            _Board[0, 0] = new ChessRook(ColorType.Black);
            _Board[0, 1] = new ChessKnight(ColorType.Black);
            _Board[0, 2] = new ChessBishop(ColorType.Black);
            _Board[0, 3] = new ChessQueen(ColorType.Black);
            _Board[0, 4] = new ChessKing(ColorType.Black);
            _Board[0, 5] = new ChessBishop(ColorType.Black);
            _Board[0, 6] = new ChessKnight(ColorType.Black);
            _Board[0, 7] = new ChessRook(ColorType.Black);

            _Board[7, 0] = new ChessRook(ColorType.White);
            _Board[7, 1] = new ChessKnight(ColorType.White);
            _Board[7, 2] = new ChessBishop(ColorType.White);
            _Board[7, 3] = new ChessQueen(ColorType.White);
            _Board[7, 4] = new ChessKing(ColorType.White);
            _Board[7, 5] = new ChessBishop(ColorType.White);
            _Board[7, 6] = new ChessKnight(ColorType.White);
            _Board[7, 7] = new ChessRook(ColorType.White);

            for (int c = 0; c < 8; c++)
            {
                _Board[1, c] = new ChessPawn(ColorType.Black);
                _Board[6, c] = new ChessPawn(ColorType.White);
            }
            _logger?.LogInformation("Starter piece is added to the game");
        }
        /// <summary>
        /// adding the starter piece to the board manually in the wished position
        /// </summary>
        /// <param name="row">which row the starter piece gonna be</param>
        /// <param name="col">which column the starter piece gonna be</param>
        /// <param name="piece">which piece it would be be</param>
        /// <returns>true if the wished position is not occupied yet with the same piece</returns>
        public bool AddStartPieces(int row, int col, IPiece piece)
        {
            Position pos = new(row, col);
            if (_Board[pos].Id == piece.Id)
            {
                _logger?.LogWarning("Starter piece {piece} is already added to the position {pos} on the board", piece, pos);
                return false;
            }
            _Board[row, col] = piece;
            _logger?.LogInformation("Starter piece {piece} is added to the position {pos} on the board", piece, pos);
            return true;
        }
        /// <summary>
        /// getting positions from all active pieces regardless its colors
        /// </summary>
        /// <returns>enumerable of all active piece position regardless its color</returns>
        public IEnumerable<Position> GetActivePiecePosition()
        {
            _logger?.LogInformation("Obtaining the active piece position");
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Position pos = new Position(r, c);

                    if (!_Board.IsEmpty(pos, _Board))
                    {
                        yield return pos;
                    }
                }
            }
        }
        /// <summary>
        /// get position of an active piece
        /// for certain color only
        /// </summary>
        /// <param name="playerColor">the chosen color as parameter</param>
        /// <returns>enumerable of active piece position for the picked color</returns>
        public IEnumerable<Position> GetActivePiecePosition(ColorType playerColor)
        {
            _logger?.LogInformation("Obtaining the active piece position for color {color}", playerColor);
            return GetActivePiecePosition().Where(pos => _Board[pos].Color == playerColor);
        }
        /// <summary>
        /// finding position of certain piece type from the certain color
        /// </summary>
        /// <param name="pieceColor">the chosen color</param>
        /// <param name="type">the chosen piece type</param>
        /// <returns>the position of picked piece type from the picked color</returns>
        public Position FindPiecePosition(ColorType pieceColor, PieceType type)
        {
            _logger?.LogInformation("Searching position of piece {type} of color {color}", type, pieceColor);
            return GetActivePiecePosition(pieceColor).First(pos => _Board[pos].Type == type);
        }
        /// <summary>
        /// getting all the active pieces
        /// </summary>
        /// <returns>enumerable of active piece</returns>
        public IEnumerable<IPiece> GetActivePiece()
        {
            _logger?.LogInformation("Getting all active piece");
            return GetActivePiecePosition().Select(pos => _Board[pos]);
        }
        /// <summary>
        /// getting all the active pieces of certain color only
        /// </summary>
        /// <param name="playerColor">the chosen color</param>
        /// <returns>enumerable of active piece of the picked color</returns>
        public IEnumerable<IPiece> GetActivePiece(ColorType playerColor)
        {
            _logger?.LogInformation("Getting active piece for {color}", playerColor);
            return GetActivePiecePosition(playerColor).Select(pos => _Board[pos]);
        }
        /// <summary>
        /// adding the last killed piece to the killed piece list
        /// </summary>
        /// <param name="move">movement of the piece</param>
        /// <returns>true if the piece is succesfully added to the killed piece list</returns>
        public bool AddKilledPieceList(IMove move)
        {
            if (move.Type == MoveType.DoublePawn || move.Type == MoveType.CastleKS || move.Type == MoveType.CastleQS)
            {
                _logger?.LogWarning("Add killed piece to the list failed");
                return false;
            }
            if (!_Board.IsEmpty(move.ToPos, _Board))
            {
                _logger?.LogWarning("Add killed piece to the list failed");
                return false;
            }
            _killedPieceList.Add(move.CapturedPiece);
            _logger?.LogInformation("Add killed piece to the list succeeded");
            return true;
        }
        /// <summary>
        /// get the list of all killed piece
        /// </summary>
        /// <returns>enumerable of killed piece list</returns>
        public IEnumerable<IPiece> GetKilledPieceList()
        {
            _logger?.LogInformation("Requestiong for a list of killed pieces");
            return _killedPieceList;
        }
        /// <summary>
        /// get the last killed piece from the list
        /// </summary>
        /// <returns>the last killed piece</returns>
        public IPiece GetLastKilledPiece()
        {
            _logger?.LogInformation("The last killed piece is {piece}", GetKilledPieceList().LastOrDefault());
            return GetKilledPieceList().LastOrDefault();
        }
        /// <summary>
        /// checking if the rook is unmoved or not for castling
        /// </summary>
        /// <param name="pos">position of the rook</param>
        /// <returns>true if rook is unmoved</returns>
        private bool IsUnmovedRook(Position pos)
        {
            if (_Board.IsEmpty(pos, _Board))
            { 
                _logger?.LogWarning("The rook has moved");
                return false;
            }
            IPiece piece = _Board[pos];
            _logger?.LogInformation("The rook for castling is unmoved");
            return piece.Type == PieceType.Rook && !piece.HasMoved;
        }
        /// <summary>
        /// checking whether the spaces for king to castling are empty or not
        /// </summary>
        /// <param name="positions">the checked positions</param>
        /// <returns>true if the spaces are empty</returns>
        private bool AllEmptyForCastle(IEnumerable<Position> positions)
        {
            return positions.All(pos => _Board.IsEmpty(pos, _Board));
        }
        /// <summary>
        /// check whether if castling in king side is possible or not
        /// </summary>
        /// <param name="from">position of the king</param>
        /// <returns>true if possible</returns>
        private bool CanCastleKingSide(Position from)
        {
            if (_Board[from].HasMoved)
            {
                _logger?.LogWarning("Castling from King Side can't be done");
                return false;
            }
            Position rookPos = new Position(from.Row, 7);
            Position[] betweenPositions = new Position[] { new(from.Row, 5), new(from.Row, 6) };
            _logger?.LogInformation("Castling from King Side can be done");
            return IsUnmovedRook(rookPos) && AllEmptyForCastle(betweenPositions);
        }
        /// <summary>
        /// check whether if castling in king side is possible or not
        /// </summary>
        /// <param name="from">position of the king</param>
        /// <returns>true if possible</returns>
        private bool CanCastleQueenSide(Position from)
        {
            if (_Board[from].HasMoved)
            {
                _logger?.LogWarning("Castling from Queen Side can't be done");
                return false;
            }
            Position rookPos = new Position(from.Row, 0);
            Position[] betweenPositions = new Position[] { new(from.Row, 1), new(from.Row, 2), new(from.Row, 3) };
            _logger?.LogInformation("Castling from Queen Side can be done");
            return IsUnmovedRook(rookPos) && AllEmptyForCastle(betweenPositions);
        }
        /// <summary>
        /// get list of the skip pawn position for enpassant
        /// </summary>
        /// <param name="playerColor">the chosen color</param>
        /// <returns>the list of the skip pawn position for enpassant</returns>
        private Position GetPawnSkipPosition(ColorType playerColor)
        {
            _logger?.LogInformation("Requesting for pawnSkipPositions list");
            return _pawnSkipPositions[playerColor];
        }
        /// <summary>
        /// store the skip pawn position for enpassant to the list
        /// </summary>
        /// <param name="playerColor">the chosen color</param>
        /// <param name="pos">the skip pawn position</param>
        private void SetPawnSkipPosition(ColorType playerColor, Position pos)
        {
            _logger?.LogInformation("Requesting to set the pawnSkipPositions list");
            _pawnSkipPositions[playerColor] = pos;
        }
        /// <summary>
        /// creating promotion piece move for pawn
        /// </summary>
        /// <param name="from">position of the pawn</param>
        /// <param name="to">position where the promoted pawn is</param>
        /// <returns>enumerable move for promotion movements</returns>
        private IEnumerable<IMove> PromotionPawn(Position from, Position to)
        {
            _logger?.LogInformation("Creating promotion piece for pawn");
            yield return new ChessPawnPromotion(from, to, PieceType.Knight);
            yield return new ChessPawnPromotion(from, to, PieceType.Bishop);
            yield return new ChessPawnPromotion(from, to, PieceType.Rook);
            yield return new ChessPawnPromotion(from, to, PieceType.Queen);
        }
        /// <summary>
        /// possible moves that can be done from the possible positions within each class of the piece
        /// </summary>
        /// <param name="from">the position of the piece</param>
        /// <returns>enumerable possible move of a piece from certain position</returns>
        public IEnumerable<IMove> GetPossibleMove(Position from)
        {
            _logger?.LogInformation("Checking possible movement from position {pos}", from);
            if (_Board[from].Type == PieceType.Bishop || _Board[from].Type == PieceType.Rook || _Board[from].Type == PieceType.Queen || _Board[from].Type == PieceType.Knight)
            {
                foreach (Position to in _Board[from].PossiblePosition(from, _Board))
                {
                    yield return new ChessNormalMove(from, to);
                }
            }
            else if (_Board[from].Type == PieceType.King)
            {
                foreach (Position to in _Board[from].PossiblePosition(from, _Board))
                {
                    yield return new ChessNormalMove(from, to);
                }

                if (CanCastleKingSide(from))
                {
                    yield return new ChessCastle(MoveType.CastleKS, from);
                }

                if (CanCastleQueenSide(from))
                {
                    yield return new ChessCastle(MoveType.CastleQS, from);
                }
            }
            else if (_Board[from].Type == PieceType.Pawn)
            {
                foreach (Position to in _Board[from].PossiblePosition(from, _Board))
                {
                    if (!_Board[from].HasMoved)
                    {
                        Position doubleStep = new Position(0, 0);
                        if (_Board[from].Color == ColorType.White)
                        {
                            doubleStep = to + Direction.North;
                        }
                        else if (_Board[from].Color == ColorType.Black)
                        {
                            doubleStep = to + Direction.South;
                        }
                        ChessDoublePawn doublePawn = new ChessDoublePawn(from, doubleStep);
                        SetPawnSkipPosition(GetCurrentColor(), doublePawn.skippedPos);
                        yield return doublePawn;
                    }
                    foreach (Direction dir in new Direction[] { Direction.West, Direction.East })
                    {
                        Position toDiagonal = to + dir;
                        if (toDiagonal == GetPawnSkipPosition(GetOpponentColor()))
                        {
                            yield return new ChessEnpassant(from, toDiagonal);
                        }
                        else if (_Board.IsInside(toDiagonal, _Board) && !_Board.IsEmpty(toDiagonal, _Board) && _Board[toDiagonal].Color == GetOpponentColor())
                        {
                            if (toDiagonal.Row == 0 || toDiagonal.Row == 7)
                            {
                                foreach (IMove promMove in PromotionPawn(from, toDiagonal))
                                {
                                    yield return promMove;
                                }
                            }
                            yield return new ChessNormalMove(from, toDiagonal);
                        }
                    }
                    yield return new ChessNormalMove(from, to);
                }
            }
            yield break;
        }
        /// <summary>
        /// to test if certain player is in check or not based from its color
        /// </summary>
        /// <param name="playerColor">the tested player</param>
        /// <param name="board">board that is used</param>
        /// <returns>true if player is in check</returns>
        public bool IsInCheck(ColorType playerColor, IBoard board)
        {
            _logger?.LogInformation("Checking whether color {color} is in check or not",playerColor);
            return GetActivePiecePosition(playerColor.Opponent()).Any(pos =>
            {
                IPiece piece = board[pos];
                return CanCaptureOpponentKing(pos, board);
            });
        }
        /// <summary>
        /// check if a piece from certain position can capture the opponent's king
        /// </summary>
        /// <param name="from">the checked certain position</param>
        /// <param name="board">board that is used</param>
        /// <returns>true if the piece can capture opponent king</returns>
        public bool CanCaptureOpponentKing(Position from, IBoard board)
        {
            _logger?.LogInformation("Checking whether piece in position {from} can capture the opponent's king or not", from);
            return GetPossibleMove(from).Any(move =>
            {
                IPiece piece = board[move.ToPos];
                return piece != null && piece.Type == PieceType.King;
            });
        }
        /// <summary>
        /// check if every movement is legal or not because every movement from the piece should not make their king gets in danger
        /// </summary>
        /// <param name="move">the checked movement</param>
        /// <returns>true if the move is legal</returns>
        public bool IsMoveLegal(IMove move)
        {
            _logger?.LogInformation("Checking whether move {move} is legal or not", move);
            if (move.Type == MoveType.CastleKS || move.Type == MoveType.CastleQS)
            {
                if (IsInCheck(_Board[move.FromPos].Color, _Board))
                {
                    return false;
                }
                ChessBoard chessB = new ChessBoard(8, 8);
                var chessB2 = Copy(chessB);
                Position kingPosInCopy = move.FromPos;
                if (move.Type == MoveType.CastleKS)
                {
                    _kingMoveDir = Direction.East;
                }
                else if (move.Type == MoveType.CastleQS)
                {
                    _kingMoveDir = Direction.West;
                }
                for (int i = 0; i < 2; i++)
                {
                    new ChessNormalMove(kingPosInCopy, kingPosInCopy + _kingMoveDir).Execute(chessB2);
                    kingPosInCopy += _kingMoveDir;

                    if (IsInCheck(_Board[move.FromPos].Color, chessB2))
                    {
                        return false;
                    }
                }
                return true;
            }
            ChessBoard chessB3 = new ChessBoard(8, 8);
            var chessB4 = Copy(chessB3);
            move.Execute(chessB4);
            return ! IsInCheck(_Board[move.FromPos].Color, chessB4);
        }
        /// <summary>
        /// get the list of all legal movements for a piece
        /// </summary>
        /// <param name="from">the position of the piece</param>
        /// <returns>enumerable of legal movements for a piece</returns>
        public IEnumerable<IMove> LegalMovesForPiece(Position from)
        {
            _logger?.LogInformation("Getting all legal movements for a piece in position {pos}", from);
            if (_Board.IsEmpty(from, _Board) || _Board[from].Color != _CurrentPlayerColor)
            {
                return Enumerable.Empty<IMove>();
            }
            IPiece piece = _Board[from];
            _moveCandidates = GetPossibleMove(from);
            _moveCandidates.Where(move => IsMoveLegal(move));
            return _moveCandidates;
        }
        /// <summary>
        /// move the piece in certain movement
        /// </summary>
        /// <param name="move">the chosen movement</param>
        /// <returns>true if succeeded</returns>
        public bool MovePiece(IMove move)
        {
            if (!IsMoveLegal(move))
            {
                _logger?.LogWarning("Movepiece failed because the movement is not legal");
                return false;
            }
            AddKilledPieceList(move);
            move.Execute(_Board);
            SetPawnSkipPosition(GetCurrentColor(), null);
            if (move.CaptureOrPawnMove)
            {
                _noCaptureOrPawnMoves = 0;
            }
            else
            {
                _noCaptureOrPawnMoves++;
            }
            _CurrentPlayerColor = GetOpponentColor();
            CheckForGameOver();
            _logger?.LogInformation("Movepiece succeded");
            return true;
        }
        /// <summary>
        /// get the list of the all legal movements that can be done based from the player color
        /// </summary>
        /// <param name="playerColor"> the chosen player color</param>
        /// <returns>enumerable of all legal movements</returns>
        public IEnumerable<IMove> AllLegalMovesFor(ColorType playerColor)
        {
            _logger?.LogInformation("Checking all legal movements for color {color}", playerColor);
            _moveCandidates = GetActivePiecePosition(playerColor).SelectMany(pos =>
            {
                IPiece piece = _Board[pos];
                return GetPossibleMove(pos);
            });
            return _moveCandidates.Where(move => IsMoveLegal(move));
        }
        /// <summary>
        /// count how many pieces are left on the board
        /// </summary>
        /// <returns>the counting of how many pieces are left on the board</returns>
        private Counting CountPieces()
        {
            _logger?.LogInformation("Count the pieces");
            Counting counting = new Counting();

            foreach (Position pos in GetActivePiecePosition())
            {
                IPiece piece = _Board[pos];
                counting.Increment(piece.Color, piece.Type);
            }

            return counting;
        }
        /// <summary>
        /// checking if there are just king left on the board
        /// </summary>
        /// <param name="counting"> counting instance as parameter</param>
        /// <returns>true if there are just king left on the board</returns>
        private bool IsKingVKing(Counting counting)
        {
            _logger?.LogInformation("Count the king");
            return counting.TotalCount == 2 && (counting.White(PieceType.King) == 1 && counting.Black(PieceType.King) == 1);
        }
        /// <summary>
        /// checking if there are just king and bishop left on the board
        /// </summary>
        /// <param name="counting">counting instance as parameter</param>
        /// <returns>true if there are just king and bishop left on the board</returns>
        private bool IsKingBishopVKing(Counting counting)
        {
            _logger?.LogInformation("Count the king and bishop");
            return counting.TotalCount == 3 && (counting.White(PieceType.Bishop) == 1 || counting.Black(PieceType.Bishop) == 1) && (counting.White(PieceType.King) == 1 && counting.Black(PieceType.King) == 1);
        }
        /// <summary>
        /// checking if there are just king and knight left on the board
        /// </summary>
        /// <param name="counting">counting instance as parameter</param>
        /// <returns>true if there are just king and knight left on the board</returns>
        private bool IsKingKnightVKing(Counting counting)
        {
            _logger?.LogInformation("Count the king and knight");
            return counting.TotalCount == 3 && (counting.White(PieceType.Knight) == 1 || counting.Black(PieceType.Knight) == 1) && (counting.White(PieceType.King) == 1 && counting.Black(PieceType.King) == 1);
        }
        /// <summary>
        /// checking if there are just king and bishop left on the board
        /// </summary>
        /// <param name="counting">counting instance as parameter</param>
        /// <returns>true if there are just king and bishop left on the board</returns>
        private bool IsKingBishopVKingBishop(Counting counting)
        {
            _logger?.LogInformation("Count the king and bishop");
            if (counting.TotalCount != 4)
            {
                return false;
            }

            if (counting.White(PieceType.Bishop) != 1 || counting.Black(PieceType.Bishop) != 1)
            {
                return false;
            }

            Position wBishopPos = FindPiecePosition(ColorType.White, PieceType.Bishop);
            Position bBishopPos = FindPiecePosition(ColorType.Black, PieceType.Bishop);

            return wBishopPos.SquareColor() == bBishopPos.SquareColor();
        }
        /// <summary>
        /// checking if the board contains insufficient material to continue the game
        /// </summary>
        /// <returns>true if the materials are insufficient</returns>
        public bool InsufficientMaterial()
        {
            _logger?.LogInformation("Checking insufficient material on the board");
            Counting counting = CountPieces();

            return IsKingVKing(counting) || IsKingBishopVKing(counting) ||
                   IsKingKnightVKing(counting) || IsKingBishopVKingBishop(counting);
        }
        /// <summary>
        /// checking if Fifty Move Rule has happened when there's no capture and pawn moves for 50 repetitive times
        /// </summary>
        /// <returns> true if Fifty Move Rule has happened</returns>
        public bool FiftyMoveRule()
        {
            _logger?.LogInformation("Checking Fifty Move Rule");
            int fullMoves = _noCaptureOrPawnMoves / 2;
            return fullMoves == 50;
        }
        /// <summary>
        /// check for game over
        /// </summary>
        public void CheckForGameOver()
        {
            _logger?.LogInformation("The game is over");
            if (!AllLegalMovesFor(GetCurrentColor()).Any())
            {
                if (IsInCheck(_CurrentPlayerColor, _Board))
                {
                    _Result = Result.Win(GetOpponentColor(), EndReason.Checkmate);
                }
                else
                {
                    _Result = Result.Draw(EndReason.Stalemate);
                }
            }
            else if (InsufficientMaterial())
            {
                _Result = Result.Draw(EndReason.InsufficientMaterial);
            }
            else if (FiftyMoveRule())
            {
                _Result = Result.Draw(EndReason.FiftyMoveRule);
            }
        }
        /// <summary>
        /// checking is the game really over?
        /// </summary>
        /// <returns>true if the game result is not null</returns>
        public bool IsGameOver()
        {
            _logger?.LogInformation("Checking whether the game is over or not...");
            return _Result != null;
        }
        /// <summary>
        /// get the winner of the game
        /// </summary>
        /// <returns>the winner of the game based from its color</returns>
        public ColorType GetWinner()
        {
            return _Result.Winner;
        }
        /// <summary>
        /// get the reason behind the game endings
        /// </summary>
        /// <returns>the reason behind the game endings</returns>
        public EndReason GetReason()
        {
            return _Result.Reason;
        }
    }
}
