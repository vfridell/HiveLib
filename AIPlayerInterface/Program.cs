using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HiveLib;
using HiveLib.AI;
using HiveLib.ViewModels;
using HiveLib.Services;

namespace AIPlayerInterface
{
    class Program
    {
        public enum YesNo {Yes, No};

        static void Main(string[] args)
        {
            IHiveAI AI = new JohnnyHive();

            YesNo yn = PromptYesOrNo("Is the AI playing white?");

            AI.BeginNewGame(yn == YesNo.Yes);

        }

        static YesNo PromptYesOrNo(string prompt)
        {
            Regex yesRegex = new Regex("y|yes", RegexOptions.IgnoreCase);
            Regex noRegex = new Regex("n|no", RegexOptions.IgnoreCase);
            string response;
            do
            {
                response = Console.ReadLine();

            } while (!yesRegex.IsMatch(response) && !noRegex.IsMatch(response));
            if (!yesRegex.IsMatch(response)) 
                return YesNo.Yes;
            else 
                return YesNo.No;
        }
    }
}
