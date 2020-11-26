
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kd417d.eva.reversi.logic
{
    public class TableLogic : IReversiTableLogic
    {
        public Dictionary<Dimension<uint>, ReversiDisk> Table { get; set; }
        public Dimension<uint> Dimension { get; set; }

        public TableLogic(Dictionary<Dimension<uint>, ReversiDisk> table, Dimension<uint> dimension)
        {
            Table = table;
            Dimension = dimension;
        }
        #region Override
        public IEnumerable<Dimension<uint>> GetSurroundedBy(Dimension<uint> a, Dimension<uint> b)
        {
            var horizontallyOrdered = new List<Dimension<uint>>(2); horizontallyOrdered.Add(a); horizontallyOrdered.Add(b);
            horizontallyOrdered.Sort((a, b) => a.Horizontal.CompareTo(b.Horizontal));
            var verticallyOrdered = new List<Dimension<uint>>(2); verticallyOrdered.Add(a); verticallyOrdered.Add(b);
            verticallyOrdered.Sort((a, b) => a.Vertical.CompareTo(b.Vertical));

            var result = new List<Dimension<uint>>();
            if(ITableLogic.IsVerticallyAlignedWith(a, b))
            {
                // Traverse from top to bottom
                // Exclude starting and ending point, since only the surrounded part is required
                for(uint i = verticallyOrdered[0].Vertical + 1; i < verticallyOrdered[1].Vertical; ++i)
                {
                    Dimension<uint> place = new Dimension<uint>(a.Horizontal, i);
                    if(Table.ContainsKey(place))
                    {
                        result.Add(place);
                    }
                }
            }
            else if (ITableLogic.IsHorizontallyAlignedWith(a, b))
            {
                // Traverse from left to right
                // Exclude starting and ending point, since only the surrounded part is required
                for (uint i = horizontallyOrdered[0].Horizontal + 1; i < horizontallyOrdered[1].Horizontal; ++i)
                {
                    Dimension<uint> place = new Dimension<uint>(i, a.Vertical);
                    if (Table.ContainsKey(place))
                    {
                        result.Add(place);
                    }
                }
            }
            else if(ITableLogic.IsDiagonallyAlignedWith(a, b) && horizontallyOrdered[0] == verticallyOrdered[0])
            {
                // Traverse from decreasing diagonal top to bottom
                // Exclude starting and ending point, since only the surrounded part is required
                uint horizontalIt = horizontallyOrdered[0].Horizontal + 1;
                uint verticalIt = verticallyOrdered[0].Vertical + 1;
                while(horizontalIt < horizontallyOrdered[1].Horizontal
                    && verticalIt < verticallyOrdered[1].Vertical)
                {
                    Dimension<uint> place = new Dimension<uint>(horizontalIt, verticalIt);
                    if(Table.ContainsKey(place))
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
                    if(Table.ContainsKey(place))
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
            ReversiColor color = Table[from].color;
            var top = ITableLogic.TopSideFrom(from, Table.Keys).Where(p => Table[p].color == color);
            var bottom = ITableLogic.BottomSideFrom(from, Table.Keys).Where(p => Table[p].color == color);
            var left = ITableLogic.LeftSideFrom(from, Table.Keys).Where(p => Table[p].color == color);
            var right = ITableLogic.RightSideFrom(from, Table.Keys).Where(p => Table[p].color == color);
            var topLeft = ITableLogic.LeftSideFrom(from, top).Where(p => ITableLogic.IsDiagonallyAlignedWith(from, p));
            var topRight = ITableLogic.RightSideFrom(from, top).Where(p => ITableLogic.IsDiagonallyAlignedWith(from, p));
            var bottomLeft = ITableLogic.LeftSideFrom(from, bottom).Where(p => ITableLogic.IsDiagonallyAlignedWith(from, p));
            var bottomRight = ITableLogic.RightSideFrom(from, bottom).Where(p => ITableLogic.IsDiagonallyAlignedWith(from, p));
            top = top.Where(p => ITableLogic.IsVerticallyAlignedWith(p, from));
            bottom = bottom.Where(p => ITableLogic.IsVerticallyAlignedWith(p, from));
            left = left.Where(p => ITableLogic.IsHorizontallyAlignedWith(p, from));
            right = right.Where(p => ITableLogic.IsHorizontallyAlignedWith(p, from));

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
            ReversiColor color = Table[from].color;
            return GetClosestNeighBoursOfSameType(from)
                    .Select(p => new Tuple<Dimension<uint>, IEnumerable<Dimension<uint>>>(p, GetSurroundedBy(p, from)
                                                                                                .Where(c => Table[c].color != color)))
                    .Where(s => ITableLogic.GetLargerDistance(from, s.Item1) - 1 == s.Item2.Count())
                    .SelectMany(s => s.Item2);
        }
        public bool IsPossiblyAllowedStep(Dimension<uint> a)
        {
            // There is at least one enemy disk that can be surrounded by this step
            return ITableLogic.GetNeighBoringPositions(a)
                .Where(p => IsOnTable(p)
                    && (Table[p].color != Table[a].color))
                .Count() != 0;
        }
        public bool IsOnTable(Dimension<uint> a)
        {
            return a.Horizontal >= 0 && a.Horizontal < Dimension.Horizontal
                && a.Vertical >= 0 && a.Vertical < Dimension.Vertical;
        }

        public bool IsTableFull()
        {
            return Table.Count == (Dimension.Horizontal * Dimension.Vertical);
        }
        public bool IsGameOverScenario()
        {
            return IsTableFull()
                || Enumerable.All(Table.Values, disk => disk.color == ReversiColor.BLACK)
                || Enumerable.All(Table.Values, disk => disk.color == ReversiColor.WHITE);
        }
        public bool IsAlreadyOnTable(Dimension<uint> a)
        {
            return Table.ContainsKey(a);
        }
        #endregion
    }
}
