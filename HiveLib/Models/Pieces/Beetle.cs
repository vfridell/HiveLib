using System.Collections.Generic;
using System.Linq;

namespace HiveLib.Models.Pieces
{
    class Beetle : Piece
    {
        internal Beetle(PieceColor color, int number) : base(color, number) { }
        public override string GetPieceNotation()
        {
            return "B";
        }

        internal override IList<Move> GetMoves(Hex start, Board board)
        {
            throw new System.NotImplementedException();
        }
    }
}
