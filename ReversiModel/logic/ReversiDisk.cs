using System;
using System.Collections.Generic;
using System.Text;

namespace kd417d.eva
{
    [Serializable]
    public class ReversiDisk
    {
        public ReversiDisk(ReversiColor color)
        {
            this.color = color;
        }
        public ReversiColor color { get; private set; }

        public bool SameAs(ReversiDisk other) 
        {
            return color == other.color;
        }

        public bool DifferentFrom(ReversiDisk other)
        {
            return color != other.color;
        }

        public void Flip()
        {
            switch (color)
            {
                case ReversiColor.BLACK:
                    color = ReversiColor.WHITE;
                    break;
                case ReversiColor.WHITE:
                    color = ReversiColor.BLACK;
                    break;
                default:
                    break;
            }
        }
        public override bool Equals(object obj)
        {
            if(!(obj is ReversiDisk)) { return false; }
            var other = (ReversiDisk)obj;
            return other.color.Equals(this.color);
        }
    }
}
