using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            pieces[0] = bottom;
            pieces[1] = secondLevel;
        }

        private int _topLevel = 1;

        internal Piece[] pieces = new Piece[5];
        internal Piece bottom { get; set; }
        internal Beetle secondLevel { get; set; }
        internal Beetle thirdLevel { get; set; }
        internal Beetle fourthLevel { get; set; }
        internal Beetle fifthLevel { get; set; }

        /// <summary>
        /// the top of the stack.  Pull from whatever level is the top of the stack
        /// </summary>
        internal Piece top 
        { 
            get 
            {
                return pieces[_topLevel];
            } 
        }

        internal void Push(Piece piece)
        {
            if ((_topLevel + 1) > 4) throw new OverflowException("You cannot have a stack taller than 5 pieces");

            _topLevel++;
            pieces[_topLevel] = piece;
        }

        internal Piece Pop()
        {
            if ((_topLevel - 1) < 1) throw new Exception("You cannot have a stack with fewer than 2 pieces");
            Piece returnValue = pieces[_topLevel];
            _topLevel--;
            return returnValue;
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
    }
}
