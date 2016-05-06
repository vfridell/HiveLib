namespace HiveLib.Models.Pieces
{
    class Hopper : Piece
    {
        internal Hopper(PieceColor color, int number) : base(color, number) { }
        public override string GetPieceNotation()
        {
            return "G";
        }
    }
}
