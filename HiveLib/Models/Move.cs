using System;
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
        private string _notation;
        public string notation { get { return _notation; } }

        private Piece _pieceToMove;
        public Piece pieceToMove { get { return _pieceToMove; } }

        private Piece _referencePiece;
        public Piece referencePiece { get { return _referencePiece; } }

        private Position _targetPosition;
        public Position targetPosition { get { return _targetPosition; } }

        private Hex _hex;
        public Hex hex { get { return _hex; } }

        private Move() { _hex = Board.invalidHex; }

        public static bool TryGetMove(string notation, out Move move)
        {
            if (NotationParser.TryParseNotation(notation, out move))
            {
                move._notation = notation;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Move GetMove(string notation)
        {
            Move move;
            if (NotationParser.TryParseNotation(notation, out move))
            {
                move._notation = notation;
                return move;
            }
            else
            {
                throw new Exception("Bad move notation");
            }
        }

        public static Move GetMove(Piece pieceToMove, Piece referencePiece, Position targetPosition)
        {
            Move move = new Move();
            move._pieceToMove = pieceToMove;
            move._referencePiece = referencePiece;
            move._targetPosition = targetPosition;
            return move;
        }

        public static Move GetMove(Piece pieceToMove, Hex hex)
        {
            Move move = new Move();
            move._pieceToMove = pieceToMove;
            move._hex = hex;
            return move;
        }

        internal static Move GetMove(Move move, Hex hex)
        {
            Move newMove = new Move();
            newMove._pieceToMove = move._pieceToMove;
            newMove._targetPosition = move.targetPosition;
            newMove._notation = move._notation;
            newMove._referencePiece = move._referencePiece;
            newMove._hex = hex;
            return newMove;
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
