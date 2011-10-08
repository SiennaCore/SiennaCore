using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiftShark
{
    public sealed class HintByteArray
    {
        private byte[] mValue;
        public string Hex { get { return BitConverter.ToString(mValue).Replace('-', ' '); } }
        public string String { get { return Encoding.ASCII.GetString(mValue); } }

        public HintByteArray(byte[] pValue) { mValue = pValue; }
    }
}
