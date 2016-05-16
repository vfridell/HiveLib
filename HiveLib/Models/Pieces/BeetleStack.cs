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
            this.bottom = bottom;
            this.secondLevel = secondLevel;
            _topLevel = 1;
        }

        internal BeetleStack(Beetle newPiece, BeetleStack originalStack)
            : base(originalStack.color, originalStack.number)
        {
            _topLevel = originalStack._topLevel + 1;
            this.bottom = originalStack.bottom;
            this.secondLevel = originalStack.secondLevel;
            this.thirdLevel = originalStack.thirdLevel;
            this.fourthLevel = originalStack.fourthLevel;
            this.fifthLevel = originalStack.fifthLevel;
            _pieces[_topLevel] = newPiece;
        }

        private int _topLevel = 1;

        private Piece[] _pieces = new Piece[5];
        internal Piece bottom { get { return _pieces[0]; } set { _pieces[0] = value; } }
        internal Beetle secondLevel { get { return (Beetle)_pieces[1]; } set { _pieces[1] = value; } }
        internal Beetle thirdLevel { get { return (Beetle)_pieces[2]; } set { _pieces[2] = value; } }
        internal Beetle fourthLevel { get { return (Beetle)_pieces[3]; } set { _pieces[3] = value; } }
        internal Beetle fifthLevel { get { return (Beetle)_pieces[4]; } set { _pieces[4] = value; } }

        internal override int height { get { return _topLevel; } }

        /// <summary>
        /// the top of the stack.  Pull from whatever level is the top of the stack
        /// </summary>
        internal Piece top 
        { 
            get 
            {
                return _pieces[_topLevel];
            } 
        }

        internal bool Contains(Piece piece)
        {
            Piece foundPiece = _pieces.Where(p => p != null)
                                      .FirstOrDefault(p => p.Equals(piece));
            if (null == foundPiece)
                return false;
            else
                return true;
        }

        internal override PieceColor color
        {
            get
            {
                return top.color;
            }
        }

        internal override int number
        {
            get
            {
                return top.number;
            }
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
