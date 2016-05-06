namespace HiveLib.Models.Pieces
{
    class QueenBee : Piece
    {
        internal QueenBee(PieceColor color, int number) : base(color, number) { }
        public override string GetPieceNotation()
        {
            return "Q";
        }
    }
}
