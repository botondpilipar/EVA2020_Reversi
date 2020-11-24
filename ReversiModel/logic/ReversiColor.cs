using System;
using System.Collections.Generic;
using System.Text;

namespace kd417d.eva
{
    public enum ReversiColor
    {
        BLACK, WHITE

    }
    public class ReversiColorFunctions
    {
        public static ReversiColor NextColor(ReversiColor color)
        {
            switch (color)
            {
                case ReversiColor.BLACK:
                    return ReversiColor.BLACK;
                case ReversiColor.WHITE:
                    return ReversiColor.WHITE;
                default:
                    return ReversiColor.WHITE;
            }
        }
    }
}
