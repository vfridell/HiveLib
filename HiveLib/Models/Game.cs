using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.ViewModels;

namespace HiveLib.Models
{
    [Serializable]
    public class Game
    {
        private List<Move> _movesMade = new List<Move>();
        public IReadOnlyList<Move> movesMade { get { return _movesMade.AsReadOnly(); } }
        
        private List<Board> _boards = new List<Board>();
        public IReadOnlyList<Board> boards { get { return _boards.AsReadOnly(); } }
        
        private Board _currentBoard;

        public readonly string whitePlayerName;
        public readonly string blackPlayerName;

        public bool whiteToPlay { get { return _currentBoard.whiteToPlay; } }
        public GameResult gameResult { get { return _currentBoard.gameResult; } }
        public string lastError { get { return _currentBoard.lastError; } }
        public int turnNumber { get { return _currentBoard.turnNumber; } }
        public int currentBoardIndex { get { return _currentBoard.turnNumber - 2; } }

        private Game(string whitePlayerName, string blackPlayerName) 
        { 
            this.whitePlayerName = whitePlayerName;
            this.blackPlayerName = blackPlayerName;
        }

        public bool TryMakeMove(Move move)
        {
            move.FixNotation(_currentBoard);
            if (_currentBoard.TryMakeMove(move))
            {
                _boards.Add(_currentBoard.Clone());
                _movesMade.Add(move);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Board GetCurrentBoard()
        {
            return _currentBoard.Clone();
        }

        public static Game GetNewGame(string whitePlayerName, string blackPlayerName)
        {
            Game game = new Game(whitePlayerName, blackPlayerName);
            game._currentBoard = Board.GetNewBoard();
            return game;
        }

        public string GetMoveTranscript()
        {
            StringBuilder sb = new StringBuilder();
            _movesMade.ForEach(m => sb.Append(m.notation).Append("\n"));
            return sb.ToString();
        }

    }
}
