using System;
using System.Collections.Generic;
using System.Text;

namespace kd417d.eva
{
    class ReversiTableSettings
    {
        public static Dimension<uint> Dimension { get; set; } = new Dimension<uint>(10, 10);
        public static ReversiColor FirstSteppingColor { get; set; } = ReversiColor.WHITE;
    }
}
