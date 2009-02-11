using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NetGore
{
    public delegate void DArrayModifyEventHandler<T>(object sender, DArrayModifyEventArgs<T> e);

    public class DArrayModifyEventArgs<T> : EventArgs
    {
        public readonly int Index;
        public readonly T Item;

        public DArrayModifyEventArgs(T item, int index)
        {
            Item = item;
            Index = index;
        }
    }
}