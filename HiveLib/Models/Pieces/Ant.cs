using System.Collections.Generic;
using System.Linq;

namespace HiveLib.Models.Pieces
{
    class Ant : Piece
    {
        internal Ant(PieceColor color, int number) : base(color, number) { }
        public override string GetPieceNotation()
        {
            return "A";
        }

        internal override IList<Move> GetMoves(Hex start, Board board)
        {
            throw new System.NotImplementedException();
        }
    }
}
