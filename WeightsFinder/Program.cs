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

namespace WeightsFinder
{
    class Program
    {
        public enum YesNo { Yes, No };

        static void Main(string[] args)
        {
            BoardAnalysisWeights bestWeights = BoardAnalysisWeights.winningWeights;
            bool testPlayingWhite = true;
            var adjustments = GetAllAdjustments(new BoardAnalysisWeightAdjustment());
            
            // tuple is <#of moves in game, adjustments win, weights>
            var gameResultsList = new List<Tuple<int, bool, BoardAnalysisWeightAdjustment, BoardAnalysisWeights>>();
            var winningResultsList = new List<Tuple<int, bool, BoardAnalysisWeightAdjustment, BoardAnalysisWeights>>();
            bool keepGoing = true;
            do
            {
                BoardAnalysisWeights testWeights = bestWeights;
                foreach (BoardAnalysisWeightAdjustment adj in adjustments)
                {
                    var adjustedWeights = AdjustWeights(testWeights, adj);
                    IHiveAI AI = new JohnnyDeep(BoardAnalysisWeights.winningWeights, 1);
                    IHiveAI AI2 = new JohnnyDeep(adjustedWeights, 1, "TestWeights");

                    AI.BeginNewGame(!testPlayingWhite);
                    AI2.BeginNewGame(testPlayingWhite);

                    Game game = PlayGame(AI, AI2);

                    bool testWon = TestWon(game.gameResult, testPlayingWhite);
                    gameResultsList.Add(new Tuple<int, bool, BoardAnalysisWeightAdjustment, BoardAnalysisWeights>(game.turnNumber, testWon, adj, adjustedWeights));
                    if (testWon)
                    {
                        winningResultsList.Add(new Tuple<int, bool, BoardAnalysisWeightAdjustment, BoardAnalysisWeights>(game.turnNumber, testWon, adj, adjustedWeights));
                        Console.WriteLine(string.Format("We have a contender!"));
                        Console.WriteLine(adjustedWeights.ToString());
                        Console.WriteLine(string.Format("---===PLAY OFF===---"));
                        Game playoffGame;
                        int wins = 0;
                        int draws = 0;
                        bool playoffTestWhite = !testPlayingWhite;
                        for (int i = 0; i < 10; i++)
                        {
                            AI.BeginNewGame(!playoffTestWhite);
                            AI2.BeginNewGame(playoffTestWhite);

                            playoffGame = PlayGame(AI, AI2);
                            if(TestWon(playoffGame.gameResult, playoffTestWhite)) wins++;
                            if (playoffGame.gameResult == GameResult.Draw || playoffGame.gameResult == GameResult.Incomplete) draws++;
                        }
                        Console.WriteLine("Wins: {0}, Losses: {1}, Draws: {2}", wins, 10 - wins - draws, draws);
                        if (wins + draws >= 5)
                            Console.WriteLine("*_*_*_*_*_*_**CHECK IT OUT!!!");
                        else
                            Console.WriteLine("Nothing special, moving on");
                    }
                    Console.WriteLine(string.Format("Finished Game: {0}", testWon ? "Win" : "Loss"));
                }

                var bestResults = gameResultsList.OrderByDescending(t => (t.Item2 ? 10000 - t.Item1 : 1000 + t.Item1)).First();
                if (bestWeights.Equals(bestResults.Item4)) keepGoing = false;
                bestWeights = bestResults.Item4;
                adjustments = GetAllAdjustments(bestResults.Item3);

            } while (keepGoing);
        }

        static bool TestWon(GameResult result, bool testPlayingWhite)
        {
            return (testPlayingWhite && result == GameResult.WhiteWin) ||
                   (!testPlayingWhite && result == GameResult.BlackWin);
        }

        private static Game PlayGame(IHiveAI AI, IHiveAI AI2)
        {

            Game game;
            if ( (AI.playingWhite && AI2.playingWhite) || (!AI.playingWhite && !AI2.playingWhite)) throw new Exception("Please decide who will be what color!");
            
            if(AI.playingWhite)
                game = Game.GetNewGame(AI.Name, AI2.Name);
            else
                game = Game.GetNewGame(AI2.Name, AI.Name);

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
                //DateTime beginTimestamp = DateTime.Now;
                move = currentAI.MakeBestMove(game);
                //TimeSpan timespan = DateTime.Now.Subtract(beginTimestamp);
                //Console.WriteLine(string.Format("{0} seconds {1} Moved: {2}", timespan.TotalSeconds, currentAI.Name, Move.GetMoveWithNotation(move, currentBoard).notation));
            } while (game.gameResult == GameResult.Incomplete && !game.ThreeFoldRepetition() );
            return game;
        }

        private static BoardAnalysisWeights AdjustWeights(BoardAnalysisWeights oldWeights, BoardAnalysisWeightAdjustment weightsAdjustment)
        {
            BoardAnalysisWeights newWeights = oldWeights;
            newWeights.articulationPointDiffWeight += weightsAdjustment.articulationPointAdj;
            newWeights.hivailableSpaceDiffWeight += weightsAdjustment.hivailableSpaceAdj;
            newWeights.possibleMovesDiffWeight += weightsAdjustment.possibleMovesAdj;
            newWeights.queenBreathingSpaceDiffWeight += weightsAdjustment.queenBreathingAdj;
            newWeights.unplayedPiecesDiffWeight += weightsAdjustment.unplayedPiecesAdj;
            newWeights.ownedBeetleStacksWeight += weightsAdjustment.ownedBeetleAdj;
            return newWeights;
        }

        private static IList<BoardAnalysisWeightAdjustment> GetAllAdjustments(BoardAnalysisWeightAdjustment lastAdjustment)
        {
            List<BoardAnalysisWeightAdjustment> adjustments = new List<BoardAnalysisWeightAdjustment>();
            for (int i = 1; i <= 63; i++)
            {
                BoardAnalysisWeightAdjustment adj = lastAdjustment;

                adj.articulationPointAdj += (i & 1) > 0 ? 0.2 : -0.2;
                adj.hivailableSpaceAdj += (i & 2) > 0 ? 0.2 : -0.2;
                adj.possibleMovesAdj += (i & 4) > 0 ? 0.2 : -0.2;
                adj.queenBreathingAdj += (i & 8) > 0 ? 0.6 : -0.6;
                adj.unplayedPiecesAdj += (i & 16) > 0 ? 0.2 : -0.2;
                adj.ownedBeetleAdj += (i & 32) > 0 ? 0.4 : -0.4;

                adjustments.Add(adj);
            }
            return adjustments;
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
