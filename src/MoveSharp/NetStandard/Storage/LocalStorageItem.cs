using MoveSharp.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoveSharp.Storage
{
    class LocalStorageItem
    {
        public IStorage Storage { get; private set; }

        public LocalStorageItem(IStorage storage)
        {
            Storage = storage;
        }
    }
}
