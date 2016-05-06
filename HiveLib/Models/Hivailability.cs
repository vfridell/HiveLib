using System;
using System.Linq;
using HiveLib.Models.Pieces;
using Position = HiveLib.Models.Neighborhood.Position;

namespace HiveLib.Models
{
    class Hivailability
    {
        public enum NeighborStatus {Empty, Black, White};
        private bool _whiteCanPlace;
        private bool _blackCanPlace;
        private bool[] _isBlocked = new bool[7];
        NeighborStatus[] _neighborStatus = new NeighborStatus[7];

        public bool WhiteCanPlace { get { return _whiteCanPlace; } }
        public bool BlackCanPlace { get { return _blackCanPlace; } }
        public bool[] IsBlockedArray { get { return _isBlocked; } }
        public NeighborStatus[] NeighborStatusArray { get { return _neighborStatus; } }

        internal bool CanPlace(Piece.PieceColor pieceColor)
        {
            if (pieceColor == Piece.PieceColor.Black) return _blackCanPlace;
            else return _whiteCanPlace;
        }

        internal static Hivailability GetHivailability(Board board, Hex hex, bool forceWhitePlacement = false, bool forceBlackPlacement = false)
        {
            Hivailability hivailability = new Hivailability();
            hivailability._neighborStatus[(int)Position.topright] = GetNeighborStatus(board, hex, Position.topright);
            hivailability._neighborStatus[(int)Position.right] = GetNeighborStatus(board, hex, Position.right);
            hivailability._neighborStatus[(int)Position.bottomright] = GetNeighborStatus(board, hex, Position.bottomright);
            hivailability._neighborStatus[(int)Position.bottomleft] = GetNeighborStatus(board, hex, Position.bottomleft);
            hivailability._neighborStatus[(int)Position.left] = GetNeighborStatus(board, hex, Position.left);
            hivailability._neighborStatus[(int)Position.topleft] = GetNeighborStatus(board, hex, Position.topleft);

            hivailability._isBlocked[(int)Position.topright] = IsDirectionBlocked(Position.topright, Position.topleft, Position.right, hivailability._neighborStatus);
            hivailability._isBlocked[(int)Position.right] = IsDirectionBlocked(Position.right, Position.topright, Position.bottomright, hivailability._neighborStatus);
            hivailability._isBlocked[(int)Position.bottomright] = IsDirectionBlocked(Position.bottomright, Position.right, Position.bottomleft, hivailability._neighborStatus);
            hivailability._isBlocked[(int)Position.bottomleft] = IsDirectionBlocked(Position.bottomleft, Position.bottomright, Position.left, hivailability._neighborStatus);
            hivailability._isBlocked[(int)Position.left] = IsDirectionBlocked(Position.left, Position.bottomleft, Position.topleft, hivailability._neighborStatus);
            hivailability._isBlocked[(int)Position.topleft] = IsDirectionBlocked(Position.topleft, Position.left, Position.topright, hivailability._neighborStatus);

            if(hivailability._neighborStatus.Contains(NeighborStatus.Black)) hivailability._blackCanPlace = true;
            if(hivailability._neighborStatus.Contains(NeighborStatus.White)) hivailability._whiteCanPlace = true;
            if(hivailability._whiteCanPlace && hivailability._blackCanPlace)
            {
                hivailability._blackCanPlace = false;
                hivailability._whiteCanPlace = false;
            }

            // the initial hivailable space is just for white
            if (forceWhitePlacement) hivailability._whiteCanPlace = true;
            // the second turn hivailable spaces are for black despite being next to white
            if (forceBlackPlacement) hivailability._blackCanPlace = true;
            return hivailability;
        }

        private static bool IsDirectionBlocked(Position directionToCheck, Position counterClockwiseDirection, Position clockwiseDirection, NeighborStatus[] neighborStatus)
        {
            if (neighborStatus[(int)directionToCheck] != NeighborStatus.Empty)
            {
                return true;
            }
            else if(neighborStatus[(int)counterClockwiseDirection] != NeighborStatus.Empty &&
                    neighborStatus[(int)clockwiseDirection] != NeighborStatus.Empty)
            {
                // gate
                return true;
            }
            else
            {
                return false;
            }
        }

        private static NeighborStatus GetNeighborStatus(Board board, Hex hex, Position pos)
        {
            Hex tr_hex = Neighborhood.GetNeighborHex(hex, pos);
            Piece tr_piece;
            if(board.TryGetPieceAtHex(tr_hex, out tr_piece))
            {
                if(tr_piece.color == Piece.PieceColor.Black) 
                    return NeighborStatus.Black;
                else if(tr_piece.color == Piece.PieceColor.White) 
                    return NeighborStatus.White;
                else
                    throw new Exception(string.Format("Bad color status at {0},{1}", hex.column, hex.row));
            }
            else
            {
                return NeighborStatus.Empty;
            }
        }
    }
}
