
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kd417d.eva.reversi
{
    public class TableLogic
    {
        private Dictionary<Dimension<uint>, ReversiDisk> _table;
        private Dimension<uint> _dimension;
        public TableLogic(Dictionary<Dimension<uint>, ReversiDisk> table, Dimension<uint> dimension)
        {
            this._table = table;
            this._dimension = dimension;
        }

        #region Static Methods
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
        #endregion

        #region NonStatic Methods
        public IEnumerable<Dimension<uint>> GetSurroundedBy(Dimension<uint> a, Dimension<uint> b)
        {
            var horizontallyOrdered = new List<Dimension<uint>>(2); horizontallyOrdered.Add(a); horizontallyOrdered.Add(b);
            horizontallyOrdered.Sort((a, b) => a.Horizontal.CompareTo(b.Horizontal));
            var verticallyOrdered = new List<Dimension<uint>>(2); verticallyOrdered.Add(a); verticallyOrdered.Add(b);
            verticallyOrdered.Sort((a, b) => a.Vertical.CompareTo(b.Vertical));

            var result = new List<Dimension<uint>>();
            if(IsVerticallyAlignedWith(a, b))
            {
                // Traverse from top to bottom
                // Exclude starting and ending point, since only the surrounded part is required
                for(uint i = verticallyOrdered[0].Vertical + 1; i < verticallyOrdered[1].Vertical; ++i)
                {
                    Dimension<uint> place = new Dimension<uint>(a.Horizontal, i);
                    if(_table.ContainsKey(place))
                    {
                        result.Add(place);
                    }
                }
            }
            else if (IsHorizontallyAlignedWith(a, b))
            {
                // Traverse from left to right
                // Exclude starting and ending point, since only the surrounded part is required
                for (uint i = horizontallyOrdered[0].Horizontal + 1; i < horizontallyOrdered[1].Horizontal; ++i)
                {
                    Dimension<uint> place = new Dimension<uint>(i, a.Vertical);
                    if (_table.ContainsKey(place))
                    {
                        result.Add(place);
                    }
                }
            }
            else if(IsDiagonallyAlignedWith(a, b) && horizontallyOrdered[0] == verticallyOrdered[0])
            {
                // Traverse from decreasing diagonal top to bottom
                // Exclude starting and ending point, since only the surrounded part is required
                uint horizontalIt = horizontallyOrdered[0].Horizontal + 1;
                uint verticalIt = verticallyOrdered[0].Vertical + 1;
                while(horizontalIt < horizontallyOrdered[1].Horizontal
                    && verticalIt < verticallyOrdered[1].Vertical)
                {
                    Dimension<uint> place = new Dimension<uint>(horizontalIt, verticalIt);
                    if(_table.ContainsKey(place))
                    {
                        result.Add(place);
                    }
                    horizontalIt++;
                    verticalIt++;
                }
            }
            else
            {
                // Traverse from increasing diagon bottom to top 
                // Exclude starting and ending point, since only the surrounded part is required
                uint horizontalIt = horizontallyOrdered[0].Horizontal + 1;
                uint verticalIt = verticallyOrdered[1].Vertical - 1;
                while (horizontalIt < horizontallyOrdered[1].Horizontal
                    && verticalIt > verticallyOrdered[0].Vertical)
                {
                    Dimension<uint> place = new Dimension<uint>(horizontalIt, verticalIt);
                    if(_table.ContainsKey(place))
                    {
                        result.Add(place);
                    }
                    horizontalIt++;
                    verticalIt--;
                }
            }
            return result;
        }

        public IEnumerable<Dimension<uint>> GetClosestNeighBoursOfSameType(Dimension<uint> from)
        {
            // Split table to 8 parts and filter the positions
            // where it is aligned vertically, horizontally or diagonally with the original poisiton
            ReversiColor color = _table[from].color;
            var top = TopSideFrom(from, _table.Keys).Where(p => _table[p].color == color);
            var bottom = BottomSideFrom(from, _table.Keys).Where(p => _table[p].color == color);
            var left = LeftSideFrom(from, _table.Keys).Where(p => _table[p].color == color);
            var right = RightSideFrom(from, _table.Keys).Where(p => _table[p].color == color);
            var topLeft = LeftSideFrom(from, top).Where(p => IsDiagonallyAlignedWith(from, p));
            var topRight = RightSideFrom(from, top).Where(p => IsDiagonallyAlignedWith(from, p));
            var bottomLeft = LeftSideFrom(from, bottom).Where(p => IsDiagonallyAlignedWith(from, p));
            var bottomRight = RightSideFrom(from, bottom).Where(p => IsDiagonallyAlignedWith(from, p));
            top = top.Where(p => IsVerticallyAlignedWith(p, from));
            bottom = bottom.Where(p => IsVerticallyAlignedWith(p, from));
            left = left.Where(p => IsHorizontallyAlignedWith(p, from));
            right = right.Where(p => IsHorizontallyAlignedWith(p, from));

            Func<Dimension<uint>, uint> horizontalSelector = (p => p.Horizontal);
            Func<Dimension<uint>, uint> verticalSelector = (p => p.Vertical);

            // From each segment, select the closest point to the original one
            Dimension<uint>[] closest = new Dimension<uint>[] { 
                top.Count() == 0 ? null : new Dimension<uint>(from.Horizontal, top.Max(verticalSelector)),
                bottom.Count() == 0 ? null : new Dimension<uint>(from.Horizontal, bottom.Min(verticalSelector)),
                left.Count() == 0 ? null : new Dimension<uint>(left.Max(horizontalSelector), from.Vertical),
                right.Count() == 0 ? null : new Dimension<uint>(right.Min(horizontalSelector), from.Vertical),
                topLeft.Count() == 0 ? null : new Dimension<uint>(topLeft.Max(horizontalSelector), topLeft.Max(verticalSelector)),
                topRight.Count() == 0 ? null : new Dimension<uint>(topRight.Min(horizontalSelector), topRight.Max(verticalSelector)),
                bottomLeft.Count() == 0 ? null : new Dimension<uint>(bottomLeft.Max(horizontalSelector), bottomLeft.Min(verticalSelector)),
                bottomRight.Count() == 0 ? null : new Dimension<uint>(bottomRight.Min(horizontalSelector), bottomRight.Min(verticalSelector)) 
            };
            return closest.Where(p => p != null);
        }
        public IEnumerable<Dimension<uint>> GetAffectedDisksOnStep(Dimension<uint> from)
        {
            // Find the closest positions in 8 direction and determine the disks surrounded by it
            // If only looking at the disks with color opposite of the parameter, the number of
            // surrounded disks equal to the correct distance (larger) - 1, then these disks are completely
            // surrounded by them, thus should change color
            ReversiColor color = _table[from].color;
            return GetClosestNeighBoursOfSameType(from)
                    .Select(p => new Tuple<Dimension<uint>, IEnumerable<Dimension<uint>>>(p, GetSurroundedBy(p, from)
                                                                                                .Where(c => _table[c].color != color)))
                    .Where(s => GetLargerDistance(from, s.Item1) - 1 == s.Item2.Count())
                    .SelectMany(s => s.Item2);
        }
        public bool IsPossiblyAllowedStep(Dimension<uint> a)
        {
            // There is at least one enemy disk that can be surrounded by this step
            return GetNeighBoringPositions(a)
                .Where(p => IsOnTable(p)
                    && (_table[p].color != _table[a].color))
                .Count() != 0;
        }
        public bool IsOnTable(Dimension<uint> a)
        {
            return a.Horizontal >= 0 && a.Horizontal < _dimension.Horizontal
                && a.Vertical >= 0 && a.Vertical < _dimension.Vertical;
        }

        public bool IsTableFull()
        {
            return _table.Count == (_dimension.Horizontal * _dimension.Vertical);
        }
        public bool IsGameOverScenario()
        {
            return IsTableFull()
                || Enumerable.All(_table.Values, disk => disk.color == ReversiColor.BLACK)
                || Enumerable.All(_table.Values, disk => disk.color == ReversiColor.WHITE);
        }

        #endregion
    }
}
