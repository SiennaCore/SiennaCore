using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiftShark
{
    public sealed class HintUnsigned7BitEncoded
    {
        private ulong mValue;
        public ulong Value { get { return mValue; } }

        public HintUnsigned7BitEncoded(long pValue) { mValue = (ulong)pValue; }
    }
}
