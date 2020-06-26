using System.Collections.Generic;
using System.Drawing;

namespace TestAppUWP.AppShell.Samples.Chess
{
    public class ChessBoardViewModel
    {
        public IReadOnlyList<Piece> Pieces;

        public ChessBoardViewModel()
        {
            Pieces = new List<Piece>
            {
                new Piece(Color.White, Kind.Pawn) {X = 0, Y = 1},
                new Piece(Color.White, Kind.Pawn) {X = 1, Y = 1},
                new Piece(Color.White, Kind.Pawn) {X = 2, Y = 1},
                new Piece(Color.White, Kind.Pawn) {X = 3, Y = 1},
                new Piece(Color.White, Kind.Pawn) {X = 4, Y = 1},
                new Piece(Color.White, Kind.Pawn) {X = 5, Y = 1},
                new Piece(Color.White, Kind.Pawn) {X = 6, Y = 1},
                new Piece(Color.White, Kind.Pawn) {X = 7, Y = 1},
                new Piece(Color.White, Kind.Rook) {X = 0, Y = 0},
                new Piece(Color.White, Kind.Rook) {X = 7, Y = 0},
                new Piece(Color.White, Kind.Knight) {X = 1, Y = 0},
                new Piece(Color.White, Kind.Knight) {X = 6, Y = 0},
                new Piece(Color.White, Kind.Bishop) {X = 2, Y = 0},
                new Piece(Color.White, Kind.Bishop) {X = 5, Y = 0},
                new Piece(Color.White, Kind.Queen) {X = 3, Y = 0},
                new Piece(Color.White, Kind.Knight) {X = 4, Y = 0},
                new Piece(Color.Black, Kind.Pawn) {X = 0, Y = 6},
                new Piece(Color.Black, Kind.Pawn) {X = 1, Y = 6},
                new Piece(Color.Black, Kind.Pawn) {X = 2, Y = 6},
                new Piece(Color.Black, Kind.Pawn) {X = 3, Y = 6},
                new Piece(Color.Black, Kind.Pawn) {X = 4, Y = 6},
                new Piece(Color.Black, Kind.Pawn) {X = 5, Y = 6},
                new Piece(Color.Black, Kind.Pawn) {X = 6, Y = 6},
                new Piece(Color.Black, Kind.Pawn) {X = 7, Y = 6},
                new Piece(Color.Black, Kind.Rook) {X = 0, Y = 6},
                new Piece(Color.Black, Kind.Rook) {X = 7, Y = 7},
                new Piece(Color.Black, Kind.Knight) {X = 1, Y = 7},
                new Piece(Color.Black, Kind.Knight) {X = 6, Y = 7},
                new Piece(Color.Black, Kind.Bishop) {X = 2, Y = 7},
                new Piece(Color.Black, Kind.Bishop) {X = 5, Y = 7},
                new Piece(Color.Black, Kind.Queen) {X = 4, Y = 7},
                new Piece(Color.Black, Kind.Knight) {X = 3, Y = 7}
            }.AsReadOnly();
        }
    }

    public class Piece
    {
        public readonly Color Color;
        public readonly Kind Kind;

        public int X;
        public int Y;

        public Piece(Color color, Kind kind)
        {
            Color = color;
            Kind = kind;
        }
    }

    public enum Kind
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King,
    }
}