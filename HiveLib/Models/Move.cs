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
        internal string notation;
        internal Piece pieceToMove { get; set; }
        internal Piece referencePiece { get; set; }
        internal Neighborhood.Position targetPosition { get; set; }
        internal Hex hex;

        private Move() { hex = Board.invalidHex; }

        internal void Execute(Board board)
        {
            throw new NotImplementedException();
        }

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
    }
}
