using System.Collections.Generic;

namespace HiveLib.Models.Pieces
{
    class QueenBee : Piece
    {
        internal QueenBee(PieceColor color, int number) : base(color, number) { }
        public override string GetPieceNotation()
        {
            return "Q";
        }

        //public static IList<Move> GetMoves(Hex start, Board board, List<Hex> visited)
        //{
        //    List<Move> returnList = new List<Move>();
        //    Hivailability hivailableCenter = Hivailability.GetHivailability(board, start);
        //    hivailableCenter.
        //    foreach (Hex deltaHex in Neighborhood.neighborDirections)
        //    {

        //        Hex nHex = deltaHex + start;
        //        if (nHex.Equals(start)) continue;
        //        Hivailability hivailable;
        //        if(!hivailableHexes.TryGetValue(nHex, out hivailable)) continue;

        //    }
        //}
    }
}
