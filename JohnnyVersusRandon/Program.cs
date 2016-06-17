using System;
using System.IO;
using System.Runtime.Serialization;
using BinaryFormatter = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;
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

namespace JohnnyVersusRandom
{
    class Program
    {
        public enum YesNo { Yes, No };

        static void Main(string[] args)
        {
            do
            {
                IHiveAI AI = new JohnnyDeep(BoardAnalysisWeights.winningWeights, "JohnnyDeep winningWeights");
                //IHiveAI AI = new RandomAI();
                //IHiveAI AI2 = new RandomAI();
                IHiveAI AI2 = new JohnnyHive(BoardAnalysisWeights.winningWeights, "JohnnyHive winningWeights");

                YesNo yn = PromptYesOrNo(string.Format("Is {0} playing white? ", AI.Name));

                Game game;
                if (yn == YesNo.Yes)
                    game = Game.GetNewGame(AI.Name, AI2.Name);
                else
                    game = Game.GetNewGame(AI2.Name, AI.Name);

                AI.BeginNewGame(yn == YesNo.Yes);
                AI2.BeginNewGame(yn == YesNo.No);

                do
                {
                    Board currentBoard = game.GetCurrentBoard();
                    Move move;
                    IHiveAI currentAI;
                    if (game.whiteToPlay == AI.playingWhite)
                    {
                        currentAI = AI;
                    }
                    else
                    {
                        currentAI = AI2;
                    }
                    DateTime beginTimestamp = DateTime.Now;
                    move = currentAI.MakeBestMove(game);
                    TimeSpan timespan = DateTime.Now.Subtract(beginTimestamp);
                    Console.WriteLine(string.Format("{0} seconds {1} Moved: {2}", timespan.TotalSeconds, currentAI.Name, Move.GetMoveWithNotation(move, currentBoard).notation));
                } while (game.gameResult == GameResult.Incomplete);

                Console.WriteLine(GetWinnerString(game));
                if (PromptYesOrNo("Write out game transcript?") == YesNo.Yes)
                {
                    string filename = WriteGameTranscript(game);
                    Console.WriteLine("Written to " + filename);
                }

            } while (PromptYesOrNo("Play again?") == YesNo.Yes);
        }

        private static string WriteGameTranscript(Game game)
        {
            string filename = string.Format("transcript_{0}", DateTime.Now.ToString("yyyy.MM.dd.HHmmss"));
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filename + ".txt"))
            {
                writer.Write(game.GetMoveTranscript());
            }

            BinaryFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(filename + ".bin", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, game);
            }
            return filename;
        }

        private static string GetWinnerString(Game game)
        {
            switch(game.gameResult)
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
                if (!Move.TryGetMove(response, out move))
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
