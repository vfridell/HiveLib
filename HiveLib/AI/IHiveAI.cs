using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models;
using HiveLib.ViewModels;

namespace HiveLib.AI
{
    interface IHiveAI
    {
        void BeginNewGame(bool PlayingWhite);
        Move GetBestMove(Board board);
    }
}
