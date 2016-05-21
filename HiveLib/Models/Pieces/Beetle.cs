using System.Collections.Generic;
using System.Linq;
using HiveLib.Models;

namespace HiveLib.Models.Pieces
{
    public class Beetle : Piece
    {
        internal Beetle(PieceColor color, int number) : base(color, number) { }
        public override string GetPieceNotation()
        {
            return "B";
        }

        internal override IList<Move> GetMoves(Hex start, Board board)
        {
            Dictionary<int, List<Move>> moveDictionary = base.GetMoves(start, board, 1);
            List<Move> validMoves;
            if (!moveDictionary.TryGetValue(1, out validMoves)) validMoves = new List<Move>();

            GetClimbingMoves(start, board, validMoves);

            return validMoves;
        }

        internal void GetClimbingMoves(Hex start, Board board, List<Move> validMoves)
        {
            // TODO check climbing gates
            var hivailability = Hivailability.GetHivailability(board, start);
            for (int i = 1; i < 7; i++)
            {
                Hex neighborHex = Neighborhood.GetNeighborHex(start, (Position)i);
                if (hivailability.NeighborStatusArray[i] != Hivailability.NeighborStatus.Empty)
                {
                    if (!IsDirectionClimbingBlocked(start, board, (Position)i))
                    {
                        validMoves.Add(Move.GetMove(this, neighborHex));
                    }
                }
            }
        }

        /*
            >-<
         >-< A >-<
        < C >-< D >
         >-< B >-<
            >-<
    Let's say the beetle is at B and wants to move to A. 
 * Take the beetle temporarily off of B. If the shortest stack of tiles of C and D is 
 * taller than the tallest stack of tiles of A and B, then the beetle can't move to A. 
 * In all other scenarios the beetle is free to move from B to A.

 *   For those who prefer a math formula:
 *   If
 *   height(A) < height(C) and 
 *   height(A) < height(D) and
 *   height(B) < height(C) and 
 *   height(B) < height(D)
 *   then
 *  moving between A and B (in either direction) is illegal, because the beetle 
 *  cannot slip through the "gate" formed by C and D, which are both strictly higher than A and B.
 *  Otherwise, movement between A and B is legal.
 * */
        private bool IsDirectionClimbingBlocked(Hex start, Board board, Position directionToCheck)
        {
            Hex directionHex = Neighborhood.GetNeighborHex(start, directionToCheck);
            Hex counterClockwiseHex = Neighborhood.CounterClockwiseDelta(directionToCheck) + start;
            Hex clockwiseHex = Neighborhood.ClockwiseDelta(directionToCheck) + start;

            int clockwiseHeight, counterClockwiseHeight, destinationHeight, startHeight;

            Piece p;
            startHeight = board.TryGetPieceAtHex(start, out p) ? p.height - 1 : 0;
            destinationHeight = board.TryGetPieceAtHex(directionHex, out p) ? p.height : 0;
            counterClockwiseHeight = board.TryGetPieceAtHex(counterClockwiseHex, out p) ? p.height : 0;
            clockwiseHeight = board.TryGetPieceAtHex(clockwiseHex, out p) ?  p.height : 0;

            if (destinationHeight < clockwiseHeight &&
                destinationHeight < counterClockwiseHeight &&
                startHeight < clockwiseHeight &&
                startHeight < counterClockwiseHeight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
