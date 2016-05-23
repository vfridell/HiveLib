using System;
using System.Linq;
using System.Text.RegularExpressions;
using HiveLib.Models.Pieces;

namespace HiveLib.Models
{
    public class NotationParser
    {
        private static Regex _notationStringRegex = new Regex(@"([bw]([BS][12]|[GA][123]|Q)) (([-\\/]?)([bw]([BS][12]|[GA][123]|Q))([-\\/]?)|\.)");

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
            Position targetPosition;
            if (targetLocation == ".")
            {
                // the first move is always to this hex
                move = Move.GetMove(pieceToMove, new Hex(24, 24));
                return true;
            }

            Piece referencePiece = GetPieceByNotation(referencePieceNotation);

            if (!string.IsNullOrEmpty(targetpositionLeft))
            {
                // left
                if (targetpositionLeft == @"\")
                    targetPosition = Position.bottomleft;
                else if (targetpositionLeft == @"-")
                    targetPosition = Position.left;
                else if (targetpositionLeft == @"/")
                    targetPosition = Position.topleft;
                else
                    return false;
            }
            else if (!string.IsNullOrEmpty(targetpositionRight))
            {
                // right
                if (targetpositionRight == @"\")
                    targetPosition = Position.bottomright;
                else if (targetpositionRight == @"-")
                    targetPosition = Position.right;
                else if (targetpositionRight == @"/")
                    targetPosition = Position.topright;
                else
                    return false;
            }
            else
            {
                // beetle on top
                targetPosition = Position.center;
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

            // an initial move has no target
            if(targetLocation == ".")
            {
                return true;
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

        public static Piece GetPieceByNotation(string pieceNotation)
        {
            PieceColor color;
            int number;
            Piece piece;

            if (pieceNotation[0] == 'w')
                color = PieceColor.White;
            else if (pieceNotation[0] == 'b')
                color = PieceColor.Black;
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

        /// <summary>
        /// Produces valid notation, but does not validate the move for the given board.
        /// </summary>
        /// <param name="move"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        internal static string GetNotationForMove(Move move, Board board)
        {
            //TODO fix problem where beetle moves generate a "next to myself" reference piece
            string referencePieceNotation;
            string targetPieceNotation = GetNotationForPiece(move.pieceToMove);
            if (board.hivailableSpaces.Count == 1)
            {
                referencePieceNotation = ".";
            }
            else if (move.referencePiece != null)
            {
                referencePieceNotation = GetNotationForPiece(move.referencePiece);
                referencePieceNotation = string.Format(Neighborhood.neighborDirectionNotationTemplates[(int)move.targetPosition], referencePieceNotation);
            }
            else if(!move.hex.Equals(Board.invalidHex))
            {
                Piece refPiece = null;
                if(board.TryGetPieceAtHex(move.hex, out refPiece))
                {
                    // assume this is a beetle moving on top of another piece
                    referencePieceNotation = GetNotationForPiece(refPiece);
                }
                else
                {
                    Hex refHex = Board.invalidHex;
                    Position refPosition = Position.center;
                    bool found = false;
                    int i = 0;  // zero is center.  we dont care about center
                    while(!found && i < Neighborhood.neighborDirections.Length)
                    {
                        i++;
                        refHex = Neighborhood.neighborDirections[i] + move.hex;
                        refPosition = Neighborhood.GetOpposite((Position)i);
                        if(board.TryGetPieceAtHex(refHex, out refPiece))
                        {
                            found = !refPiece.Equals(move.pieceToMove);
                        }
                    }
                    // must be a first move
                    if (!found)
                    {
                        referencePieceNotation = ".";
                    }
                    else
                    {
                        referencePieceNotation = GetNotationForPiece(refPiece);
                        if(referencePieceNotation == targetPieceNotation)
                        {
                            if (refPiece is BeetleStack)
                            {
                                refPiece = BeetleStack.PopBeetleStack((BeetleStack)refPiece);
                            }
                            else
                            {
                            }
                            referencePieceNotation = GetNotationForPiece(refPiece);
                        }
                        referencePieceNotation = string.Format(Neighborhood.neighborDirectionNotationTemplates[(int)refPosition], referencePieceNotation);
                    }
                }
            }
            else
            {
                // must be a first move
               referencePieceNotation = ".";
            }
            return targetPieceNotation + " " + referencePieceNotation;
        }

        private static string GetNotationForPiece(Piece piece)
        {
            return (piece.color == PieceColor.White ? "w" : "b") +
                                             piece.GetPieceNotation() +
                                             piece.number.ToString();
        }

    }
}
