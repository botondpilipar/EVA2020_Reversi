using System;
using System.Collections.Generic;
using System.Text;

namespace kd417d.eva.test.utility
{
    class EventSpy<EventArgT>
    {
        public List<EventArgT> CapturedArguments { get; }

        public EventSpy()
        {
            CapturedArguments = new List<EventArgT>();
        }
        public void OnEventRaised(object sender, EventArgT arg)
        {
            CapturedArguments.Add(arg);
        }
        public bool IsEventRaised()
        {
            return CapturedArguments.Count != 0;
        }
        public bool IsEmpty()
        {
            return CapturedArguments.Count == 0;
        }
    }
}
