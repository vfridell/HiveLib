using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models.Pieces;

namespace HiveLib.Models
{
    // Axial coordinates
    // ref: http://www.redblobgames.com/grids/hexagons/
    class Neighborhood
    {
        public enum Position { center, topleft, topright, right, bottomright, bottomleft, left };
        public static readonly Hex[] neighborDirections = { new Hex(0,0), new Hex(0,-1), new Hex(1,-1), new Hex(1,0), new Hex(0,1), new Hex(-1,1), new Hex(-1,0), };
        public static readonly string[] neighborDirectionNotationTemplates = { "{0}", "/{0}", "{0}/", "{0}-", @"{0}\", @"\{0}", "-{0}", };

        public static Hex GetDirectionHex(Position position)
        {
            return neighborDirections[(int)position];
        }

        public static Hex GetNeighborHex(Hex hex, Position position)
        {
            Hex dir = GetDirectionHex(position);
            return new Hex(hex.column + dir.column, hex.row + dir.row);
        }

        public static Position GetOpposite(Position position)
        {
            switch(position)
            {
                case Position.center:
                    return Position.center;
                case Position.topleft:
                    return Position.bottomright;
                case Position.topright:
                    return Position.bottomleft;
                case Position.right:
                    return Position.left;
                case Position.bottomright:
                    return Position.topleft;
                case Position.bottomleft:
                    return Position.topright;
                case Position.left:
                    return Position.right;
                default:
                    throw new Exception("Invalid position");
            }
        }

        public static Hex GetOppositeHex(Position position)
        {
            Hex vector = neighborDirections[(int)position];
            return new Hex(-vector.x, -vector.y, -vector.z);
        }

        public static Hex ClockwiseDelta(Position position)
        {
            Hex vector = neighborDirections[(int)position];
            return new Hex(-vector.z, -vector.x, -vector.y);
        }

        public static Hex CounterClockwiseDelta(Position position)
        {
            Hex vector = neighborDirections[(int)position];
            return new Hex(-vector.y, -vector.z, -vector.x);
        }
    }
}
