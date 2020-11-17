
using System;
using System.Collections.Generic;
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
            return (uint)Math.Abs(a.Horizontal - b.Horizontal);
        }
        public static uint GetVerticalDistance(Dimension<uint> a, Dimension<uint> b)
        {
            return (uint)Math.Abs(a.Vertical - b.Vertical);
        }

        public List<ReversiBlock> BlockAffectedAfterStep(uint horizontal, uint vertical)
        {
            Dimension<uint> place = new Dimension<uint>(horizontal, vertical);
            var result = new List<ReversiBlock>();
            for (uint horizontalIt = 0; horizontalIt < horizontal; ++horizontalIt)
            {
                var testing = new Dimension<uint>(horizontalIt, vertical);
                if(_table.ContainsKey(testing) && !testing.Equals(place))
                {
                    result.Add(new ReversiBlock(testing, _table[testing].color));
                }
            }
            for(uint verticalIt = 0; verticalIt < vertical; ++verticalIt)
            {
                var testing = new Dimension<uint>(horizontal, verticalIt);
                if (_table.ContainsKey(testing) && !testing.Equals(place))
                {
                    result.Add(new ReversiBlock(testing, _table[testing].color));
                }
            }
            uint decreasingDiagHorizontal = 0;
            uint decreasingDiagVertical = 0;
            uint increasingDiagHorizontal = 0;
            int increasingDiagVertical = (int)vertical;
            while(decreasingDiagHorizontal < horizontal &&
                decreasingDiagVertical < vertical &&
                increasingDiagHorizontal < horizontal &&
                increasingDiagVertical > 0)
            {
                Dimension<uint> increasingDiagPlace = new Dimension<uint>(increasingDiagHorizontal, (uint)increasingDiagVertical);
                Dimension<uint> decreasingDiagPlace = new Dimension<uint>(decreasingDiagHorizontal, decreasingDiagVertical);
                if(!increasingDiagPlace.Equals(place) && _table.ContainsKey(increasingDiagPlace))
                {
                    result.Add(new ReversiBlock(increasingDiagPlace, _table[increasingDiagPlace].color));
                }
                if (!decreasingDiagPlace.Equals(place) && _table.ContainsKey(decreasingDiagPlace))
                {
                    result.Add(new ReversiBlock(decreasingDiagPlace, _table[decreasingDiagPlace].color));
                }
                decreasingDiagHorizontal += 1;
                decreasingDiagVertical += 1;
                increasingDiagHorizontal += 1;
                increasingDiagVertical -= 1;
            }
            return result;
    }
        public bool IsTableFull()
        {
            return _table.Count == (_dimension.Horizontal * _dimension.Vertical);
        }
        public bool CanStep(uint horizontal, uint vertical)
        {
            return true;
        }
        public bool IsGameOverScenario()
        {
            return true;
        }
    }
}
