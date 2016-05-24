using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HiveLib;
using HiveLib.AI;
using HiveLib.ViewModels;
using HiveLib.Models;
using HiveLib.Services;

namespace AIPlayerInterface
{
    class Program
    {
        public enum YesNo {Yes, No};

        static void Main(string[] args)
        {
            do
            {
                IHiveAI AI = new JohnnyHive(JohnnyHive._winningWeights);

                YesNo yn = PromptYesOrNo("Is this AI playing white? ");
                string opponentName = PromptForString("Enter the other player's name: ");

                Game game;
                if (yn == YesNo.Yes)
                    game = Game.GetNewGame(AI.Name, opponentName);
                else
                    game = Game.GetNewGame(opponentName, AI.Name);

                AI.BeginNewGame(yn == YesNo.Yes);

                do
                {
                    Board currentBoard = game.GetCurrentBoard();
                    Move move;
                    if (game.whiteToPlay == AI.playingWhite)
                    {
                        move = AI.MakeBestMove(game);
                        Console.WriteLine("Moved: " + Move.GetMoveWithNotation(move, currentBoard).notation);
                    }
                    else
                    {
                        move = PromptForMove("Enter your move: ", game);
                        Console.WriteLine("Moved: " + move.notation);
                    }
                } while (game.gameResult == GameResult.Incomplete);

                Console.WriteLine(GetWinnerString(game));
            } while (PromptYesOrNo("Play again?") == YesNo.Yes);
        }

        private static string GetWinnerString(Game game)
        {
            switch (game.gameResult)
            {
                case GameResult.Draw:
                    return "Draw";
                case GameResult.WhiteWin:
                    return string.Format("{0} wins!", game.whitePlayerName);
                case GameResult.BlackWin:
                    return string.Format("{0} wins!", game.blackPlayerName);
                default:
                    throw new Exception("Bad game result");
            }
        }

        static YesNo PromptYesOrNo(string prompt)
        {
            Regex yesRegex = new Regex("y|yes", RegexOptions.IgnoreCase);
            Regex noRegex = new Regex("n|no", RegexOptions.IgnoreCase);
            string response;
            do
            {
                Console.WriteLine(prompt);
                response = Console.ReadLine();

            } while (!yesRegex.IsMatch(response) && !noRegex.IsMatch(response));
            if (yesRegex.IsMatch(response)) 
                return YesNo.Yes;
            else 
                return YesNo.No;
        }


        static string PromptForString(string prompt)
        {
            string response;
            do
            {
                Console.WriteLine(prompt);
                response = Console.ReadLine();

            } while (response.Length == 0);
            return response;
        }

        static Move PromptForMove(string prompt, Game game)
        {
            int turnNumber = game.turnNumber;
            Move move;
            do
            {
                Console.WriteLine(prompt);
                string response = Console.ReadLine();
                if(!Move.TryGetMove(response, out move))
                {
                    Console.WriteLine("Invalid move notation");
                    continue;
                }
                if (!game.TryMakeMove(move))
                {
                    Console.WriteLine(string.Format("Invalid move: {0}", game.lastError));
                    continue;
                }
            } while (turnNumber == game.turnNumber && game.gameResult == GameResult.Incomplete);
            return move;
        }
    }
}
