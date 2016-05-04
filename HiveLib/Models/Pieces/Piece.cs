namespace HiveLib.Models.Pieces
{
    class Piece
    {
        public enum PieceColor {White, Black};

        public Piece(PieceColor color, int number)
        {
            _number = number;
            _color = color;
        }

        private readonly int _number;
        internal virtual int number { get { return _number; } }

        private readonly PieceColor _color;
        internal virtual PieceColor color { get { return _color; } }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            return Equals((Piece)obj);
        }

        public virtual bool Equals(Piece obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            return this._color.Equals(obj._color) && this._number.Equals(obj._number);
        }

        public override int GetHashCode()
        {
            // this probably sucks
            return _number + (int)_color;
        }
    }
}
