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
            new PlayerChoice() { IsHuman = true, GetNewAI = () => {return null;} },
            new PlayerChoice() { IsHuman = false, GetNewAI = () => { return new JohnnyDeep(BoardAnalysisWeights.winningWeights, 2);} },
            new PlayerChoice() { IsHuman = false, GetNewAI = () => { return new JohnnyDeep(BoardAnalysisWeights.winningWeights, 3);} },
            new PlayerChoice() { IsHuman = false, GetNewAI = () => { return new JohnnyDeep(BoardAnalysisWeights.winningWeights, 4);} },
            new PlayerChoice() { IsHuman = false, GetNewAI = () => { return new RandomAI();} },
            new PlayerChoice() { IsHuman = false, GetNewAI = () => { return new JohnnyHive(BoardAnalysisWeights.winningWeights);} },
        };
    }

    public class PlayerChoice
    {
        public bool IsHuman;
        public Func<IHiveAI> GetNewAI;
        public override string ToString()
        {
            if (IsHuman)
                return "Human";
            else
                return GetNewAI().Name;
        }
    }
}
