using System.Collections.Generic;
using System.Linq;

namespace HiveLib.Models.Pieces
{
    public class Spider : Piece
    {
        internal Spider(PieceColor color, int number) : base(color, number) { }
        public override string GetPieceNotation()
        {
            return "S";
        }

        internal override IList<Move> GetMoves(Hex start, Board board)
        {
            Dictionary<int, List<Move>> moveDictionary = base.GetMoves(start, board, 3);
            List<Move> validMoves;
            if (!moveDictionary.TryGetValue(3, out validMoves)) validMoves = new List<Move>();
            return validMoves;
        }
    }
}
