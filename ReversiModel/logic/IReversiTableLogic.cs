using System;
using System.Collections.Generic;
using System.Text;

namespace kd417d.eva.reversi.logic
{
    public interface IReversiTableLogic : ITableLogic
    {
        public Dimension<uint> Dimension { get; set; }
        public Dictionary<Dimension<uint>, ReversiDisk> Table { get; set; }

        public IEnumerable<Dimension<uint>> GetSurroundedBy(Dimension<uint> a, Dimension<uint> b);
        public IEnumerable<Dimension<uint>> GetClosestNeighBoursOfSameType(Dimension<uint> from);
        public IEnumerable<Dimension<uint>> GetAffectedDisksOnStep(Dimension<uint> from);
        public bool IsPossiblyAllowedStep(Dimension<uint> a);
        public bool IsGameOverScenario();
        public bool IsAlreadyOnTable(Dimension<uint> a);
    }
}
