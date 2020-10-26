using System;
using System.Collections.Generic;
using System.Text;

namespace SuppLocals.Utilities.Helpers
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }
    }
}
