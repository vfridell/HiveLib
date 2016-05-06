namespace HiveLib.Models.Pieces
{
    class Ant : Piece
    {
        internal Ant(PieceColor color, int number) : base(color, number) { }
        public override string GetPieceNotation()
        {
            return "A";
        }
    }
}
