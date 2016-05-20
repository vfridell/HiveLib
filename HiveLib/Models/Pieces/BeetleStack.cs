using System;
using System.Collections.Generic;
using System.Linq;

namespace HiveLib.Models.Pieces
{
    /// <summary>
    /// We treat a beetle stack as a type of Piece for convienence
    /// It allows us to override the methods for color and type
    /// </summary>
    class BeetleStack : Piece
    {
        internal BeetleStack(Piece bottom, Beetle secondLevel)
            : base(bottom.color, bottom.number)
        {
            this._pieces[0] = bottom;
            this._pieces[1] = secondLevel;
            _topLevel = 1;
        }

        internal BeetleStack(Beetle newPiece, BeetleStack originalStack)
            : base(originalStack.color, originalStack.number)
        {
            _topLevel = originalStack._topLevel + 1;
            this._pieces[0] = originalStack.bottom;
            this._pieces[1] = originalStack.secondLevel;
            this._pieces[2] = originalStack.thirdLevel;
            this._pieces[3] = originalStack.fourthLevel;
            this._pieces[4] = originalStack.fifthLevel;
            _pieces[_topLevel] = newPiece;
        }

        internal static Piece PopBeetleStack(BeetleStack oldStack)
        {
            if(oldStack.height == 1) return oldStack.bottom;
            var newStack = new BeetleStack(oldStack.bottom, oldStack.secondLevel);
            switch (oldStack.height)
            {
                case 2:
                    return newStack;
                case 3:
                    newStack._pieces[2] = oldStack.thirdLevel;
                    newStack._topLevel = 2;
                    break;
                case 4:
                    newStack._pieces[2] = oldStack.thirdLevel;
                    newStack._pieces[3] = oldStack.fourthLevel;
                    newStack._topLevel = 3;
                    break;
                default:
                    throw new Exception("Too many levels!");
            }
            return newStack;
        }

        private int _topLevel = 1;

        private Piece[] _pieces = new Piece[5];
        internal Piece bottom { get { return _pieces[0]; } }
        internal Beetle secondLevel { get { return (Beetle)_pieces[1]; }  }
        internal Beetle thirdLevel { get { return (Beetle)_pieces[2]; } }
        internal Beetle fourthLevel { get { return (Beetle)_pieces[3]; } }
        internal Beetle fifthLevel { get { return (Beetle)_pieces[4]; } }
        internal Piece top { get { return _pieces[_topLevel]; } }
        public override PieceColor color { get { return top.color; } }
        public override int number { get { return top.number; } }
        public override int height { get { return _topLevel; } }

        internal bool Contains(Piece piece)
        {
            Piece foundPiece = _pieces.Where(p => p != null)
                                      .FirstOrDefault(p => p.Equals(piece));
            if (null == foundPiece)
                return false;
            else
                return true;
        }

        public override bool Equals(Piece obj)
        {
            if (obj == null) return false;
            if (!(obj is BeetleStack)) return false;
            BeetleStack other = (BeetleStack)obj;
            if (!(this.fifthLevel == other.fifthLevel)) return false;
            if (!(this.fourthLevel == other.fourthLevel)) return false;
            if (!(this.thirdLevel == other.thirdLevel)) return false;
            if (!(this.secondLevel == other.secondLevel)) return false;
            if (!(this.bottom == other.bottom)) return false;
            return true;
        }

        public override string GetPieceNotation()
        {
            return top.GetPieceNotation();
        }

        internal override IList<Move> GetMoves(Hex start, Board board)
        {
            Beetle beetle = (Beetle)top;
            var validMoves = new List<Move>();
            beetle.GetClimbingMoves(start, board, validMoves);

            // empty spaces around are valid moves for a beetle stack
            var hivailability = Hivailability.GetHivailability(board, start);
            foreach (Hex hex in hivailability.EmptyNeighborHexes(start))
            {
                validMoves.Add(Move.GetMove(this.top, hex));
            }

            return validMoves;
        }
    }
}
