using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models;
using HiveLib.ViewModels;
using System.Threading;

namespace HiveLib.AI
{
    public interface IHiveAI
    {
        void BeginNewGame(bool PlayingWhite);
        Move MakeBestMove(Game game);
        Move PickBestMove(Board board);
        Task<Move> PickBestMoveAsync(Board board, CancellationToken aiCancelToken);
        bool playingWhite { get; }

        string Name { get; }
    }
}
