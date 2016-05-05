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

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            return this.column == ((Hex)obj).column &&
                   this.row == ((Hex)obj).row;
        }

        public override int GetHashCode()
        {
            return column ^ row;
        }

    }
}
