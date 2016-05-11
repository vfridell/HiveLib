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
            throw new System.NotImplementedException();
        }
    }
}
