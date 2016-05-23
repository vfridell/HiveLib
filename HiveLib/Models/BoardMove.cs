using HiveLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveLib.Models
{
    public class BoardMove
    {
        internal int depth;
        internal Board board;
        internal Move move;
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(BoardMove)) return false;
            return Equals((BoardMove)obj);
        }

        // this is wrong.  Need Equals override on Board
        public bool Equals(BoardMove other)
        {
            return depth == other.depth && move.Equals(other.move);
        }

        public override int GetHashCode()
        {
            return depth.GetHashCode() ^ move.GetHashCode();
        }
    }

    public class BoardMove2
    {
        internal int depth;
        internal BoardAnalysisData board;
        internal Move move;
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(BoardMove)) return false;
            return Equals((BoardMove)obj);
        }

        // this is wrong.  Need Equals override on Board
        public bool Equals(BoardMove other)
        {
            return depth == other.depth && board.Equals(other.board);
        }

        public override int GetHashCode()
        {
            return depth.GetHashCode() ^ move.GetHashCode();
        }
    }
}
