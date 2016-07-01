using HiveLib.AI;
using HiveLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveDisplay
{
    public abstract class DisplayState
    {
        public abstract void ReadyForMove(IHiveAI AI, Game game);
        public abstract Task GameStart(IHiveAI AI, Game game);
        public abstract void GameEnd(IHiveAI AI, Game game);
    }

    public class ViewOnly : DisplayState
    {

        public override void ReadyForMove(IHiveAI AI, Game game)
        {
            // do nothing
        }

        public override Task GameStart(IHiveAI AI, Game game)
        {
            // do nothing
            return Task.FromResult<object>(null);
        }

        public override void GameEnd(IHiveAI AI, Game game)
        {
            // do nothing
        }
    }

    public class PlayGame : DisplayState
    {
        public override void ReadyForMove(IHiveAI AI, Game game)
        {
            //

        }

        public override Task GameStart(IHiveAI AI, Game game)
        {
            return Task.Run(() =>
            {
                Move move = AI.PickBestMove(game.GetCurrentBoard());
                game.TryMakeMove(move);
            });
        }

        public override void GameEnd(IHiveAI AI, Game game)
        {
            throw new NotImplementedException();
        }
    }

}
