using System;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

namespace ChessLib
{
    public class GameController
    {
        public Board Board { get; }
        private Dictionary<IPlayer, ColorType> _playerList { get; set; } = new Dictionary<IPlayer, ColorType>();
        private ColorType _CurrentPlayerColor { get; set; }
        public Result Result { get; private set; } = null;

        private int _noCaptureOrPawnMoves = 0;

        private readonly Dictionary<string, int> _stateHistory = new Dictionary<string, int>();

        public GameController(Board board)
        {
            Board = board;
        }
        public GameController(ColorType playerColor, Board board)
        {
            _CurrentPlayerColor = playerColor;
            Board = board;
        }
        public bool AddPlayer(IPlayer player, ColorType playerColor)
        {
            if (!_playerList.ContainsKey(player) || !_playerList.ContainsValue(playerColor))
            {
                return false;
            }
            _playerList.Add(player, playerColor);
            return true;
        }
        public bool AddPlayer(string name, ColorType playerColor)
        {
            if (!_playerList.Any(item => item.Key.Name == name) || !_playerList.ContainsValue(playerColor))
            {
                return false;
            }
            Player player = new Player(name);
            _playerList.Add(player, playerColor);
            return true;
        }
        public IEnumerable<IPlayer> GetPlayer()
        {
            return _playerList.Where(item => item.Key.Id != 0).Select(item => item.Key);
        }
        public IPlayer GetPlayer(ColorType playerColor)
        {
            foreach (var item in _playerList)
            {
                if (item.Value == playerColor)
                {
                    return item.Key;
                }
            }
            return null;
        }

        public IPlayer GetCurrentPlayer()
        {
            foreach (var item in _playerList)
            {
                if (item.Value == GetCurrentColor())
                {
                    return item.Key;
                }
            }
            return null;
        }
        public bool SetCurrentPlayer(IPlayer player)
        {
            if(!_playerList.ContainsKey(player))
            {
                return false;
            }
            return SetCurrentColor(GetPlayerColor(player));
        }
        public ColorType GetCurrentColor()
        {
            return _CurrentPlayerColor;
        }
        public bool SetCurrentColor(ColorType playerColor)
        {
            if (Enum.IsDefined(typeof(ColorType), playerColor) && playerColor == ColorType.None)
            {
                return false;
            }
            _CurrentPlayerColor = playerColor;
            return true;
        }
        public ColorType GetPlayerColor(IPlayer player)
        {
            return _playerList[player];
        }
        public IEnumerable<Move> LegalMovesForPiece(Position pos)
        {
            if (Board.IsEmpty(pos) || Board[pos].Color != _CurrentPlayerColor)
            {
                return Enumerable.Empty<Move>();
            }

            Piece piece = Board[pos];
            IEnumerable<Move> moveCandidates = piece.GetMoves(pos, Board);
            return moveCandidates.Where(move => move.IsLegal(Board));
        }

        public void MakeMove(Move move)
        {
            Board.SetPawnSkipPosition(_CurrentPlayerColor, null);
            bool captureOrPawn = move.Execute(Board);

            if (captureOrPawn)
            {
                _noCaptureOrPawnMoves = 0;
                _stateHistory.Clear();
            }
            else
            {
                _noCaptureOrPawnMoves++;
            }

            _CurrentPlayerColor = _CurrentPlayerColor.Opponent();
            CheckForGameOver();
        }

        public IEnumerable<Move> AllLegalMovesFor(ColorType playerColor)
        {
            IEnumerable<Move> moveCandidates = Board.PiecePositionsFor(playerColor).SelectMany(pos =>
            {
                Piece piece = Board[pos];
                return piece.GetMoves(pos, Board);
            });

            return moveCandidates.Where(move => move.IsLegal(Board));
        }

        private void CheckForGameOver()
        {
            if (!AllLegalMovesFor(_CurrentPlayerColor).Any())
            {
                if (Board.IsInCheck(_CurrentPlayerColor))
                {
                    Result = Result.Win(_CurrentPlayerColor.Opponent());
                }
                else
                {
                    Result = Result.Draw(EndReason.Stalemate);
                }
            }
            else if (Board.InsufficientMaterial())
            {
                Result = Result.Draw(EndReason.InsufficientMaterial);
            }
            else if (FiftyMoveRule())
            {
                Result = Result.Draw(EndReason.FiftyMoveRule);
            }
        }

        public bool IsGameOver()
        {
            return Result != null;
        }

        private bool FiftyMoveRule()
        {
            int fullMoves = _noCaptureOrPawnMoves / 2;
            return fullMoves == 50;
        }
    }
}
