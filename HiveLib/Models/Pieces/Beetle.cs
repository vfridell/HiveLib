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
            Dictionary<int, List<Move>> moveDictionary = base.GetMoves(start, board, 1);
            List<Move> validMoves;
            if (!moveDictionary.TryGetValue(1, out validMoves)) validMoves = new List<Move>();

            GetClimbingMoves(start, board, validMoves);

            return validMoves;
        }

        internal void GetClimbingMoves(Hex start, Board board, List<Move> validMoves)
        {
            // TODO check climbing gates
            var hivailability = Hivailability.GetHivailability(board, start);
            foreach (Hex hex in hivailability.EmptyNeighborHexes(start).Union(hivailability.NonEmptyNeighborHexes(start)))
            {
                validMoves.Add(Move.GetMove(this, hex));
            }
        }
    }
}
