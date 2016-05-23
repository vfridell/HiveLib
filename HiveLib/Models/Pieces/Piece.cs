using System;
using System.Collections.Generic;
using System.Linq;

namespace HiveLib.Models.Pieces
{
    public enum PieceColor {White, Black};

    /// <summary>
    /// Pieces must be immutable
    /// </summary>
    public abstract class Piece
    {
        public Piece(PieceColor color, int number)
        {
            _number = number;
            _color = color;
        }

        private readonly int _number;
        public virtual int number { get { return _number; } }

        private readonly PieceColor _color;
        public virtual PieceColor color { get { return _color; } }

        public virtual int height { get { return 1; } }

        public abstract string GetPieceNotation();

        internal abstract IList<Move> GetMoves(Hex start, Board board);

        internal Dictionary<int, List<Move>> GetMoves(Hex start, Board board, int stopAtDepth)
        {
            var moveDictionary = new Dictionary<int, List<Move>>();
            if (!board.articulationPoints.Contains(this))
            {
                var visited = new List<Hex>();
                var results = new Dictionary<int, HashSet<Hex>>();
                Board boardClone = board.CloneAndRemove(this);

                GetSlideMovesRecursive(start, boardClone, stopAtDepth, 1, visited, results);

                List<Move> validMoves = new List<Move>();
                foreach (var kvp in results)
                {
                    validMoves.Clear();
                    foreach (Hex hex in kvp.Value)
                    {
                        validMoves.Add(Move.GetMove(this, hex));
                    }
                    moveDictionary[kvp.Key] = new List<Move>(validMoves);
                }
            }
            return moveDictionary;
        }

        protected void GetSlideMovesRecursive(Hex current, Board board, int stopAtDepth, int depth, IList<Hex> visited, Dictionary<int, HashSet<Hex>> results)
        {
            List<Move> returnList = new List<Move>();
            Hivailability hivailableCenter = Hivailability.GetHivailability(board, current);
            IList<Hex> blockedNeighbors = hivailableCenter.BlockedNeighborHexes(current);
            IList<Hex> emptyNeighbors = hivailableCenter.EmptyNeighborHexes(current);
            IList<Hex> nonEmptyNeighbors = hivailableCenter.NonEmptyNeighborHexes(current);
            IList<Hex> emptyNeighborNeighbors = new List<Hex>();
            foreach (Hex hex in nonEmptyNeighbors)
            {
                Hivailability hivailableNeighbor = Hivailability.GetHivailability(board, hex);
                emptyNeighborNeighbors = hivailableNeighbor.EmptyNeighborHexes(hex).Concat(emptyNeighborNeighbors).ToList();
            }
            HashSet<Hex> validMovementHexes = new HashSet<Hex>((emptyNeighborNeighbors.Intersect(emptyNeighbors)).Except(blockedNeighbors));

            visited.Add(current);
            
            HashSet<Hex> validHexSet;
            if(results.TryGetValue(depth, out validHexSet))
            {
                validHexSet.UnionWith(validMovementHexes);
            }
            else
            {
                results.Add(depth, validMovementHexes);
            }

            if (stopAtDepth == depth) return;

            foreach (Hex h in validMovementHexes)
            {
                if(!visited.Contains(h)) GetSlideMovesRecursive(h, board, stopAtDepth, depth + 1, visited, results);
            }
        }


        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            return Equals((Piece)obj);
        }

        public virtual bool Equals(Piece obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            return this._color.Equals(obj._color) && this._number.Equals(obj._number);
        }

        public override int GetHashCode()
        {
            // this probably sucks
            return _number + (int)_color;
        }
    }
}
