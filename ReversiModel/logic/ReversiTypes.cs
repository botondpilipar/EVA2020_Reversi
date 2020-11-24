using System;
using System.Collections.Generic;
using System.Text;

namespace kd417d.eva
{
    public class Dimension<T> : Tuple<T, T>
    {
        public T Vertical { get; set; }
        public T Horizontal { get; set; }
        public Dimension(T horizontal, T vertical) : base(horizontal, vertical)
        {
            Horizontal = base.Item1;
            Vertical = base.Item2;
        }

        public override bool Equals(object o)
        {
            if(!(o is Dimension<T>)) { return false; }
            var other = (Dimension<T>)o;
            return other.Vertical.Equals(this.Vertical) &&
                other.Horizontal.Equals(this.Horizontal);
        }
    }
    public class ReversiBlock : Tuple<Dimension<uint>, ReversiDisk>
    {
        public uint Vertical { get; set; }
        public uint Horizontal { get; set; }
        public ReversiDisk Disk { get; set; }

        public ReversiBlock(Dimension<uint> dimension, ReversiColor color)
            : this(dimension.Horizontal, dimension.Vertical, color)
        { }
        public ReversiBlock(uint horizontal, uint vertical, ReversiColor color)
            : this(horizontal, vertical, new ReversiDisk(color))
        { }
        public ReversiBlock(uint horizontal, uint vertical, ReversiDisk disk) : base(new Dimension<uint>(horizontal, vertical), disk)
        {
            this.Vertical = base.Item1.Item2;
            this.Horizontal = base.Item1.Item1;
            this.Disk = base.Item2;
        }

        public override bool Equals(object obj)
        {
            if(!(obj is ReversiBlock)) { return false; }

            var other = (ReversiBlock)obj;
            return other.Disk.Equals(this.Disk) &&
                other.Horizontal.Equals(this.Horizontal) &&
                other.Vertical.Equals(this.Vertical);
        }
    }
   
}
