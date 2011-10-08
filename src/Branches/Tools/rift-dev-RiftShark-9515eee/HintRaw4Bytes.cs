using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiftShark
{
    public sealed class HintRaw4Bytes
    {
        private byte[] mValue;
        public string Hex { get { return BitConverter.ToString(mValue).Replace('-', ' '); } }
        public int Int32 { get { return BitConverter.ToInt32(mValue, 0); } }
        public uint UInt32 { get { return BitConverter.ToUInt32(mValue, 0); } }
        public float Single { get { return BitConverter.ToSingle(mValue, 0); } }

        public HintRaw4Bytes(byte[] pValue) { mValue = pValue; }
    }
}
