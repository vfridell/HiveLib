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
        private static Regex _notationStringRegex = new Regex(@"([bw]([BS][12]|[GA][123]|Q)) (([-\\/]?)([bw]([BS][12]|[GA][123]|Q))([-\\/]?))");

        internal int referenceColumn = -1;
        internal int referenceRow = -1;
        internal int targetColumn = -1;
        internal int targetRow = -1;
        internal string notation;

        internal string pieceToMoveNotation;
        internal string targetLocation;
        internal string referencePieceNotation;
        internal string targetpositionLeft;
        internal string targetpositionRight;

        internal Piece pieceToMove { get; set; }
        internal Piece referencePiece { get; set; }
        internal Neighborhood.Position targetPosition { get; set; }

        private Move(string notation) 
        {
            this.notation = notation;
        }

        internal void Execute(Board board)
        {
            throw new NotImplementedException();
        }

        internal bool IsValid(Board board)
        {
            if (!NotationValid()) return false;
            throw new NotImplementedException();
        }

        private void ParseNotation()
        {
            if (!_notationStringRegex.IsMatch(notation)) return;

            MatchCollection matches = _notationStringRegex.Matches(notation);
            pieceToMoveNotation = matches[0].Groups[1].Value;
            targetLocation = matches[0].Groups[3].Value;
            referencePieceNotation = matches[0].Groups[5].Value;
            targetpositionLeft = matches[0].Groups[4].Value;
            targetpositionRight = matches[0].Groups[7].Value;

            if (!NotationValid()) return;

            pieceToMove = GetPieceByNotation(pieceToMoveNotation);
            referencePiece = GetPieceByNotation(referencePieceNotation);

            if(!string.IsNullOrEmpty(targetpositionLeft))
            {
                // left
                if(targetpositionLeft == @"\")
                    targetPosition = Neighborhood.Position.bottomleft;
                if(targetpositionLeft == @"=")
                    targetPosition = Neighborhood.Position.left;
                if(targetpositionLeft == @"/")
                    targetPosition = Neighborhood.Position.topleft;
            }
            else if(!string.IsNullOrEmpty(targetpositionRight))
            {
                // right
                if(targetpositionLeft == @"\")
                    targetPosition = Neighborhood.Position.bottomright;
                if(targetpositionLeft == @"=")
                    targetPosition = Neighborhood.Position.right;
                if(targetpositionLeft == @"/")
                    targetPosition = Neighborhood.Position.topright;
            }
            else
            {
                // beetle on top
                targetPosition = Neighborhood.Position.center;
            }
        }

        internal bool NotationValid()
        {
            if (!_notationStringRegex.IsMatch(notation))
                return false;

            // cannot have two position indicators on target
            if (!string.IsNullOrEmpty(targetpositionLeft) &&
                !string.IsNullOrEmpty(targetpositionRight))
            {
                return false;
            }

            // cannot be missing a position indicator on a non-beetle move
            if (!pieceToMoveNotation.Contains('B') &&
                string.IsNullOrEmpty(targetpositionLeft) &&
                string.IsNullOrEmpty(targetpositionRight))
            {
                return false;
            }

            // reference and target cannot be the same piece
            if (pieceToMoveNotation.Equals(referencePieceNotation))
            {
                return false;
            }

            return true;
        }

        internal static Move GetMove(string notation)
        {
            Move move = new Move(notation);
            move.ParseNotation();
            return move;
        }

        internal static Piece GetPieceByNotation(string pieceNotation)
        {
            Piece.PieceColor color;
            int number;
            Piece piece;

            if (pieceNotation[0] == 'w')
                color = Piece.PieceColor.White;
            else if (pieceNotation[0] == 'b')
                color = Piece.PieceColor.Black;
            else
                throw new Exception("Invalid color notation");

            if (pieceNotation.Length == 3)
            {
                if (!int.TryParse(pieceNotation[2].ToString(), out number))
                    throw new Exception("Invalid number notation");
            }
            else
            {
                number = 1;
            }

            switch (pieceNotation[1])
            {
                case 'A':
                    piece = new Ant(color, number);
                    break;
                case 'B':
                    piece = new Beetle(color, number);
                    break;
                case 'G':
                    piece = new Hopper(color, number);
                    break;
                case 'S':
                    piece = new Spider(color, number);
                    break;
                case 'Q':
                    piece = new QueenBee(color, number);
                    break;
                default:
                    throw new Exception("Invalid piece notation");
            }
            return piece;
        }

        public static bool IsValidNotationString(string notation)
        {
            try
            {
                Move move = GetMove(notation);
                return move.NotationValid();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
