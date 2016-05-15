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
            _pieces[0] = bottom;
            _pieces[1] = secondLevel;
        }

        private int _topLevel = 1;

        private Piece[] _pieces = new Piece[5];
        internal Piece bottom { get { return _pieces[0]; } set { _pieces[0] = value; } }
        internal Beetle secondLevel { get { return (Beetle)_pieces[1]; } set { _pieces[1] = value; } }
        internal Beetle thirdLevel { get { return (Beetle)_pieces[2]; } set { _pieces[2] = value; } }
        internal Beetle fourthLevel { get { return (Beetle)_pieces[3]; } set { _pieces[3] = value; } }
        internal Beetle fifthLevel { get { return (Beetle)_pieces[4]; } set { _pieces[4] = value; } }

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

        internal void Push(Piece piece)
        {
            if ((_topLevel + 1) > 4) throw new OverflowException("You cannot have a stack taller than 5 pieces");

            _topLevel++;
            _pieces[_topLevel] = piece;
        }

        internal Piece Pop()
        {
            if ((_topLevel - 1) < 1) throw new Exception("You cannot have a stack with fewer than 2 pieces");
            Piece returnValue = _pieces[_topLevel];
            _topLevel--;
            return returnValue;
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

        internal BeetleStack Clone()
        {
            BeetleStack bs = new BeetleStack(this.bottom, this.secondLevel);
            for (int i = 0; i <= this._topLevel; i++)
            {
                bs._pieces[i] = this._pieces[i];
            }
            bs._topLevel = this._topLevel;
            return bs;
        }

        public override bool Equals(Piece obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            if(this._topLevel != ((BeetleStack)obj)._topLevel) return false;

            for (int i = 0; i < _topLevel; i++)
            {
                if(!this._pieces[i].Equals(((BeetleStack)obj)._pieces[i])) return false;
            }
            return true;
        }

        public override string GetPieceNotation()
        {
            throw new Exception("BeetleStacks do not have notation");
        }

        internal override IList<Move> GetMoves(Hex start, Board board)
        {
            Beetle beetle = (Beetle)top;
            var validMoves = new List<Move>();
            beetle.GetClimbingMoves(start, board, validMoves);
            return validMoves;
        }
    }
}
