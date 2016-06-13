using HiveLib.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveDisplay
{
    public abstract class DisplayState
    {
        public abstract void MoveMade(HiveGameWindow window);
        public abstract void GameEnd(HiveGameWindow window);
    }

    public class ViewOnly : DisplayState
    {

        public override void MoveMade(HiveGameWindow window)
        {
            // do nothing
        }

        public override void GameEnd(HiveGameWindow window)
        {
            // do nothing
        }
    }

    public class PlayGame : DisplayState
    {
        public override void MoveMade(HiveGameWindow window)
        {
            IHiveAI ai;
            if(window.TryGetAIForTurn(out ai))
            {
                window.MakeAIMove(ai);
            }
        }

        public override void GameEnd(HiveGameWindow window)
        {
            throw new NotImplementedException();
        }
    }

}
