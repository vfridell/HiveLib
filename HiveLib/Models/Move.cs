﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using HiveLib.Models.Pieces;

namespace HiveLib.Models
{
    public class Move
    {
        internal string notation;
        internal Piece pieceToMove { get; set; }
        internal Piece referencePiece { get; set; }
        internal Neighborhood.Position targetPosition { get; set; }
        internal Hex hex;

        private Move() { hex = Board.invalidHex; }

        internal static bool TryGetMove(string notation, out Move move)
        {
            if (NotationParser.TryParseNotation(notation, out move))
            {
                move.notation = notation;
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static Move GetMove(string notation)
        {
            Move move;
            if (NotationParser.TryParseNotation(notation, out move))
            {
                move.notation = notation;
                return move;
            }
            else
            {
                throw new Exception("Bad move notation");
            }
        }

        internal static Move GetMove(Piece pieceToMove, Piece referencePiece, Neighborhood.Position targetPosition)
        {
            Move move = new Move();
            move.pieceToMove = pieceToMove;
            move.referencePiece = referencePiece;
            move.targetPosition = targetPosition;
            return move;
        }

        internal static Move GetMove(Piece pieceToMove, Hex hex)
        {
            Move move = new Move();
            move.pieceToMove = pieceToMove;
            move.hex = hex;
            return move;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            return Equals((Move)obj);
        }

        public virtual bool Equals(Move obj)
        {
            if (obj == null) return false;
            return this.pieceToMove.Equals(obj.pieceToMove) && this.hex.Equals(obj.hex);
        }

        public override int GetHashCode()
        {
            // this probably sucks
            return (this.hex.column ^ this.hex.row) + this.pieceToMove.GetHashCode();
        }
    }
}
