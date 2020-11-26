using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace kd417d.eva.reversi.logic
{
    public interface ITableLogic
    {
        public static List<ReversiBlock> GetInitial(uint xDim, uint yDim)
        {
            var initial = new List<ReversiBlock>(4);
            uint horizontalMiddle = xDim / 2;
            uint verticalMiddle = yDim / 2;
            initial.Add(new ReversiBlock(horizontalMiddle, verticalMiddle, ReversiColor.BLACK));
            initial.Add(new ReversiBlock(horizontalMiddle + 1, verticalMiddle, ReversiColor.WHITE));
            initial.Add(new ReversiBlock(horizontalMiddle + 1, verticalMiddle + 1, ReversiColor.BLACK));
            initial.Add(new ReversiBlock(horizontalMiddle, verticalMiddle + 1, ReversiColor.WHITE));
            return initial;
        }
        public static uint GetCoordinateDistance(Dimension<uint> a, Dimension<uint> b)
        {
            return (uint)Math.Abs(a.Horizontal - b.Horizontal) + (uint)Math.Abs(a.Vertical - b.Vertical);
        }

        public static uint GetHorizontalDistance(Dimension<uint> a, Dimension<uint> b)
        {
            return (uint)Math.Abs((int)a.Horizontal - (int)b.Horizontal);
        }
        public static uint GetVerticalDistance(Dimension<uint> a, Dimension<uint> b)
        {
            return (uint)Math.Abs((int)a.Vertical - (int)b.Vertical);
        }
        public static uint GetLargerDistance(Dimension<uint> a, Dimension<uint> b)
        {
            return Math.Max(GetHorizontalDistance(a, b), GetVerticalDistance(a, b));
        }
        public static bool IsHorizontallyAlignedWith(Dimension<uint> a, Dimension<uint> b)
        {
            return GetVerticalDistance(a, b) == 0;
        }
        public static bool IsVerticallyAlignedWith(Dimension<uint> a, Dimension<uint> b)
        {
            return GetHorizontalDistance(a, b) == 0;
        }
        public static bool IsDiagonallyAlignedWith(Dimension<uint> a, Dimension<uint> b)
        {
            return GetVerticalDistance(a, b) == GetHorizontalDistance(a, b);
        }
        public static IEnumerable<Dimension<uint>> TopSideFrom(Dimension<uint> from, IEnumerable<Dimension<uint>> dimensions)
        {
            return Enumerable.Where(dimensions, (key => key.Vertical < from.Vertical));
        }
        public static IEnumerable<Dimension<uint>> BottomSideFrom(Dimension<uint> from, IEnumerable<Dimension<uint>> dimensions)
        {
            return dimensions.Where((key => key.Vertical > from.Vertical));
        }
        public static IEnumerable<Dimension<uint>> LeftSideFrom(Dimension<uint> from, IEnumerable<Dimension<uint>> dimensions)
        {
            return dimensions.Where((key => key.Horizontal < from.Horizontal));
        }
        public static IEnumerable<Dimension<uint>> RightSideFrom(Dimension<uint> from, IEnumerable<Dimension<uint>> dimensions)
        {
            return dimensions.Where((key => key.Horizontal > from.Horizontal));
        }
        public static IEnumerable<Dimension<uint>> GetNeighBoringPositions(Dimension<uint> a)
        {
            return new Dimension<uint>[]
            {
                new Dimension<uint>(a.Horizontal, a.Vertical + 1),
                new Dimension<uint>(a.Horizontal, a.Vertical - 1),
                new Dimension<uint>(a.Horizontal + 1, a.Vertical),
                new Dimension<uint>(a.Horizontal - 1, a.Vertical),
                new Dimension<uint>(a.Horizontal + 1, a.Vertical + 1),
                new Dimension<uint>(a.Horizontal -1, a.Vertical - 1),
                new Dimension<uint>(a.Horizontal + 1, a.Vertical - 1),
                new Dimension<uint>(a.Horizontal -1, a.Vertical + 1) };
        }

        public bool IsOnTable(Dimension<uint> a);
        public bool IsTableFull();    }
}
