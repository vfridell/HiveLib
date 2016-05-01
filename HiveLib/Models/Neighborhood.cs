using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models.Pieces;

namespace HiveLib.Models
{
    class Neighborhood
    {
        public enum Position { center, topleft, topright, right, bottomright, bottomleft, left };

        internal Piece[] neighborhoodArray = new Piece[7];

        internal Piece center 
        { 
            get { return neighborhoodArray[(int)Position.center]; }
            set { neighborhoodArray[(int)Position.center] = value; } 
        }
        internal Piece topleft 
        { 
            get { return neighborhoodArray[(int)Position.topleft]; }
            set { neighborhoodArray[(int)Position.topleft] = value; } 
        }
        internal Piece topright 
        { 
            get { return neighborhoodArray[(int)Position.topright]; }
            set { neighborhoodArray[(int)Position.topright] = value; } 
        }
        internal Piece right 
        { 
            get { return neighborhoodArray[(int)Position.right]; }
            set { neighborhoodArray[(int)Position.right] = value; } 
        }
        internal Piece bottomright 
        {
            get { return neighborhoodArray[(int)Position.bottomright]; }
            set { neighborhoodArray[(int)Position.bottomright] = value; } 
        }
        internal Piece bottomleft 
        { 
            get { return neighborhoodArray[(int)Position.bottomleft]; }
            set { neighborhoodArray[(int)Position.bottomleft] = value; } 
        }
        internal Piece left { 
            get { return neighborhoodArray[(int)Position.left]; }
            set { neighborhoodArray[(int)Position.left] = value; } 
        }
    }
}
