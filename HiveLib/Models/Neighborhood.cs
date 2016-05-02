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

        public static Hex GetDirectionHex(Position position)
        {
            return neighborDirections[(int)position];
        }

        public static Hex GetNeighborHex(Hex hex, Position position)
        {
            Hex dir = GetDirectionHex(position);
            return new Hex(hex.column + dir.column, hex.row + dir.row);
        }
    }
}
