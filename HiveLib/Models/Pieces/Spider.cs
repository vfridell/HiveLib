﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace HiveLib.Models.Pieces
{
    [Serializable]
    public class Spider : Piece
    {
        public Spider(PieceColor color, int number) : base(color, number) { }
        public override string GetPieceNotation()
        {
            return "S";
        }

        internal override IList<Move> GetMoves(Hex start, Board board)
        {
            Dictionary<int, List<Move>> moveDictionary = base.GetMoves(start, board, 3);
            List<Move> validMoves;
            if (!moveDictionary.TryGetValue(3, out validMoves)) validMoves = new List<Move>();
            return validMoves;
        }
    }
}
