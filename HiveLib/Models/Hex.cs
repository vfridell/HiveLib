using System;
namespace HiveLib.Models
{
    [Serializable]
    public struct Hex
    {
        public Hex(int column, int row)
        {
            this.column = column;
            this.row = row;

            this.x = column;
            this.z = row;
            this.y = -x-z;
        }

        public Hex(int x, int y, int z)
        {
            this.x = x;
            this.z = y;
            this.y = z;

            this.column = x;
            this.row = z;

            if (x + y + z != 0) throw new Exception(string.Format("Bad Hex cube coordinates: {0}, {1}, {2}", x,y,z));
        }

        public readonly int column;
        public readonly int row;

        public readonly int x;
        public readonly int y;
        public readonly int z;

        public static bool operator != (Hex hex1, Hex hex2)
        {
            return hex1.column != hex2.column || hex1.row != hex2.row;
        }

        public static bool operator ==(Hex hex1, Hex hex2)
        {
            return hex1.column == hex2.column && hex1.row == hex2.row;
        }

        public static Hex operator +(Hex hex1, Hex hex2)
        {
            return new Hex(hex1.column + hex2.column, hex1.row + hex2.row);
        }

        public static int Distance(Hex hex1, Hex hex2) => Math.Max(Math.Max(Math.Abs(hex1.x - hex2.x), Math.Abs(hex1.y - hex2.y)), Math.Abs(hex1.z - hex2.z));
    }
}
