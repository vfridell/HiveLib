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
        string MakeBestMove();
        bool TryAcceptMove(string notation, out string error);
    }
}
