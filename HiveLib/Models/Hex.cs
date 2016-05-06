namespace HiveLib.Models
{
    struct Hex
    {
        public Hex(int column, int row)
        {
            this.column = column;
            this.row = row;
        }

        public readonly int column;
        public readonly int row;

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
    }
}
