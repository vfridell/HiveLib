using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models;
using HiveLib.ViewModels;

namespace HiveLib.AI
{
    public interface IHiveAI
    {
        void BeginNewGame(bool PlayingWhite);
        Move MakeBestMove(Game game);
        Move PickBestMove(Board board);
        bool playingWhite { get; }

        string Name { get; }
    }
}
