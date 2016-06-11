using System;
using System.Collections.Generic;
using System.Linq;

namespace HiveLib.Models.Pieces
{
    [Serializable]
    public class Ant : Piece
    {
        public Ant(PieceColor color, int number) : base(color, number) { }
        public override string GetPieceNotation()
        {
            return "A";
        }

        internal override IList<Move> GetMoves(Hex start, Board board)
        {
            Dictionary<int, List<Move>> moveDictionary = base.GetMoves(start, board, int.MaxValue);
            List<Move> validMoves = new List<Move>();
            foreach (var kvp in moveDictionary)
            {
                validMoves.AddRange(kvp.Value);
            }
            return validMoves;
        }
    }
}
