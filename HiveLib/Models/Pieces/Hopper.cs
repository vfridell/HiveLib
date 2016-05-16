using System.Collections.Generic;
using System.Linq;

namespace HiveLib.Models.Pieces
{
    class Hopper : Piece
    {
        internal Hopper(PieceColor color, int number) : base(color, number) { }
        public override string GetPieceNotation()
        {
            return "G";
        }

        internal override IList<Move> GetMoves(Hex start, Board board)
        {
            var validMoves = new List<Move>();
            foreach(Hex deltaHex in Neighborhood.neighborDirections)
            {
                if (deltaHex + start == start) continue;

                Piece p;
                Hex destinationHex = start + deltaHex;
                int hopDistance = 1;
                while (board.TryGetPieceAtHex(destinationHex, out p))
                {
                    destinationHex += deltaHex;
                    hopDistance++;
                }

                if (hopDistance > 1)
                {
                    validMoves.Add(Move.GetMove(this, destinationHex));
                }
            }
            return validMoves;
        }
    }
}
