using System.Collections.Generic;
using System.Linq;

namespace HiveLib.Models.Pieces
{
    class Spider : Piece
    {
        internal Spider(PieceColor color, int number) : base(color, number) { }
        public override string GetPieceNotation()
        {
            return "S";
        }

        internal override IList<Move> GetMoves(Hex start, Board board)
        {
            throw new System.NotImplementedException();
        }
    }
}
