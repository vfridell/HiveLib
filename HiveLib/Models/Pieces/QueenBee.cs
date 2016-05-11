using System.Collections.Generic;
using System.Linq;

namespace HiveLib.Models.Pieces
{
    class QueenBee : Piece
    {
        internal QueenBee(PieceColor color, int number) : base(color, number) { }
        public override string GetPieceNotation()
        {
            return "Q";
        }

        internal override IList<Move> GetMoves(Hex start, Board board)
        {
            List<Hex> visited = new List<Hex>();
            Dictionary<int, HashSet<Hex>> results = new Dictionary<int, HashSet<Hex>>();
            GetSlideMovesRecursive(start, board, 1, 1, visited, results);
            
            List<Move> validMoves = new List<Move>();
            HashSet<Hex> validMoveHexes;
            if(results.TryGetValue(1, out validMoveHexes))
            {
                foreach (Hex hex in validMoveHexes)
                {
                    validMoves.Add(Move.GetMove(this, hex));
                }
            }
            return validMoves;
        }

        //protected IList<Move> GetSlideMoves(Hex start, Board board)
        //{
        //    List<Move> returnList = new List<Move>();
        //    Hivailability hivailableCenter = Hivailability.GetHivailability(board, start);
        //    IList<Hex> emptyNeighbors = hivailableCenter.EmptyNeighborHexes(start);
        //    IList<Hex> nonEmptyNeighbors = hivailableCenter.NonEmptyNeighborHexes(start);

        //    IList<Hex> emptyNeighborNeighbors = new List<Hex>();
        //    foreach(Hex hex in nonEmptyNeighbors)
        //    {
        //        Hivailability hivailableNeighbor = Hivailability.GetHivailability(board, hex);
        //        emptyNeighborNeighbors = hivailableNeighbor.EmptyNeighborHexes(hex).Concat(emptyNeighborNeighbors).ToList();
        //    }

        //    HashSet<Hex> validMovementHexes = new HashSet<Hex>(emptyNeighborNeighbors.Intersect(emptyNeighbors));

        //    List<Move> validMoves = new List<Move>();

        //    foreach(Hex h in validMovementHexes)
        //    {
        //        validMoves.Add(Move.GetMove(this, h));
        //    }
        //    return validMoves;
        //}
    }
}
