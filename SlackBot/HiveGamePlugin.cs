using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noobot.Core.Plugins;
using HiveLib.Models;
using HiveLib.AI;
using HiveLib.ViewModels;

namespace SlackBot
{
    public class HiveGamePlugin : IPlugin
    {
        private Game _game;
        private JohnnyDeep _jd;
        private string _user;

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void BeginNewGame(bool aiPlayingWhite, string otherPlayerName)
        {
            _jd = new JohnnyDeep(BoardAnalysisWeights.winningWeights, 3);
            _jd.BeginNewGame(aiPlayingWhite);
            if (aiPlayingWhite) _game = Game.GetNewGame(_jd.Name, otherPlayerName);
            else _game = Game.GetNewGame(otherPlayerName, _jd.Name);
        }

        public bool TryMakeOtherPlayerMove(Move move)
        {
            return _game.TryMakeMove(move);
        }

        public bool TryMakeAIMove(out Move move)
        {
            move = _jd.MakeBestMove(_game);
            if (null == move) return false;
            else return true;
        }

        public void SaveUsername(string userhandle)
        {
            _user = userhandle;
        }

        public string GetUser()
        {
            return _user;
        }

        public string GetLastGameError()
        {
            return _game.lastError;
        }

        public bool GameStarted()
        {
            return _game != null;
        }

        public void QuitGame()
        {
            if (_game != null)
            {
                _game = null;
                _jd = null;
            }
        }
    }
}
