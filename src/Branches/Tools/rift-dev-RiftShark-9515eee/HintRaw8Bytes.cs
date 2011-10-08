using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiftShark
{
    public sealed class HintRaw8Bytes
    {
        private byte[] mValue;
        public string Hex { get { return BitConverter.ToString(mValue).Replace('-', ' '); } }
        public long Int64 { get { return BitConverter.ToInt64(mValue, 0); } }
        public ulong UInt64 { get { return BitConverter.ToUInt64(mValue, 0); } }
        public double Double { get { return BitConverter.ToDouble(mValue, 0); } }

        public HintRaw8Bytes(byte[] pValue) { mValue = pValue; }
    }
}
