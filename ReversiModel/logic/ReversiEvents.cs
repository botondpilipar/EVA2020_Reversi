using System;
using System.Collections.Generic;
using System.Text;

namespace kd417d.eva
{
    public class TableUpdateEventArgs : EventArgs
    {
        public List<ReversiBlock> data { get; set; }
        public ReversiColor currentlyStepping { get; set; }
    }
    public class NewGameEventArgs : EventArgs
    {
        public Tuple<uint, uint> dimensions { get; set; }
        public TableUpdateEventArgs boardUpdate { get; set; }
    }

    public class GameOverEventArgs : EventArgs
    {
        public ReversiColor winningSide { get; set; }
    }

    public class UserTimeUpdatedEventArgs : EventArgs
    {
        public DateTime seconds { get; set; }
    }
}
