using System;
using System.Collections.Generic;
using System.Linq;

namespace HiveLib.Models.Pieces
{
    [Serializable]
    public class QueenBee : Piece
    {
        public QueenBee(PieceColor color, int number) : base(color, number) { }
        public override string GetPieceNotation()
        {
            return "Q";
        }

        internal override IList<Move> GetMoves(Hex start, Board board)
        {
            Dictionary<int, List<Move>> moveDictionary = base.GetMoves(start, board, 1);
            List<Move> validMoves;
            if (!moveDictionary.TryGetValue(1, out validMoves)) validMoves = new List<Move>();
            return validMoves;
        }
    }
}
