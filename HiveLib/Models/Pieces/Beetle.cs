namespace HiveLib.Models.Pieces
{
    class Beetle : Piece
    {
        internal Beetle(PieceColor color, int number) : base(color, number) { }
        public override string GetPieceNotation()
        {
            return "B";
        }
    }
}
