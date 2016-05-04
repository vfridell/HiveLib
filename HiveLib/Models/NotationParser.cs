﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using HiveLib.Models.Pieces;

namespace HiveLib.Models
{
    class NotationParser
    {
        private static Regex _notationStringRegex = new Regex(@"([bw]([BS][12]|[GA][123]|Q)) (([-\\/]?)([bw]([BS][12]|[GA][123]|Q))([-\\/]?))");

        internal static bool TryParseNotation(string notation, out Move move)
        {
            move = null;

            if (!_notationStringRegex.IsMatch(notation)) return false;

            MatchCollection matches = _notationStringRegex.Matches(notation);
            string pieceToMoveNotation = matches[0].Groups[1].Value;
            string targetLocation = matches[0].Groups[3].Value;
            string referencePieceNotation = matches[0].Groups[5].Value;
            string targetpositionLeft = matches[0].Groups[4].Value;
            string targetpositionRight = matches[0].Groups[7].Value;

            if (!NotationValid(pieceToMoveNotation, targetLocation, referencePieceNotation, targetpositionLeft, targetpositionRight)) return false;

            Piece pieceToMove = GetPieceByNotation(pieceToMoveNotation);
            Piece referencePiece = GetPieceByNotation(referencePieceNotation);
            Neighborhood.Position targetPosition;

            if (!string.IsNullOrEmpty(targetpositionLeft))
            {
                // left
                if (targetpositionLeft == @"\")
                    targetPosition = Neighborhood.Position.bottomleft;
                else if (targetpositionLeft == @"-")
                    targetPosition = Neighborhood.Position.left;
                else if (targetpositionLeft == @"/")
                    targetPosition = Neighborhood.Position.topleft;
                else
                    return false;
            }
            else if (!string.IsNullOrEmpty(targetpositionRight))
            {
                // right
                if (targetpositionRight == @"\")
                    targetPosition = Neighborhood.Position.bottomright;
                else if (targetpositionRight == @"-")
                    targetPosition = Neighborhood.Position.right;
                else if (targetpositionRight == @"/")
                    targetPosition = Neighborhood.Position.topright;
                else
                    return false;
            }
            else
            {
                // beetle on top
                targetPosition = Neighborhood.Position.center;
            }

            move = Move.GetMove(pieceToMove, referencePiece, targetPosition);
            return true;
        }

        private static bool NotationValid(string pieceToMoveNotation, string targetLocation, string referencePieceNotation, string targetpositionLeft, string targetpositionRight)
        {
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
    }
}
