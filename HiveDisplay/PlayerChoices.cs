using HiveLib.AI;
using HiveLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveDisplay
{
    public static class PlayerChoices
    {
        public static List<PlayerChoice> Players = new List<PlayerChoice> 
        { 
            new PlayerChoice() { IsHuman = true },
            new PlayerChoice() { IsHuman = false, AI = new JohnnyDeep(BoardAnalysisWeights.winningWeights, 3) }
        };
    }

    public class PlayerChoice
    {
        public bool IsHuman;
        public IHiveAI AI;
        public override string ToString()
        {
            if (AI == null)
                return "Human";
            else
                return AI.Name;
        }
    }
}
