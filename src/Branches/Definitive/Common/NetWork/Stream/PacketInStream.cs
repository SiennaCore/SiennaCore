/*
 * Copyright (C) 2011 APS
 *	http://AllPrivateServer.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    public class PacketInStream : PacketIn
    {

        public PacketInStream(byte[] buf, int Size)
            : base(buf, 0, Size)
        {
        }

        public long ReadEncoded7Bit()
        {
            long Value = 0;
            int ValueSize = 0;

            while (true)
            {
                if (Position >= Length)
                    return -1;

                byte Bits = GetUint8();

                Value |= ((long)(Bits & 0x7F) << (ValueSize * 7));
                ValueSize++;

                if ( (Bits & 0x80) == 0)
                    break;
            }

            return Value;
        }
        public bool ReadEncodedLong(out long Value)
        {
            Value = 0;
            int ValueSize = 0;

            int Bit = 0;
            int Offset = 0;

            while (Offset < Length && Bit < 70)
            {
                byte current = GetUint8();
                long maskedAndShifted = (current & 0x7F) << Bit;
                Bit += 7;
                Value |= maskedAndShifted;
                ++ValueSize;
                ++Offset;

                if ((current & 0x80) == 0) 
                    return true;
            }

            return false;
        }

        public static bool Decode2Parameters(long pValue, out int pParameter1, out int pParameter2)
        {
            pParameter1 = (int)(pValue & 0x07);
            pParameter2 = (int)(pValue >> 3);
            if (pParameter1 < 0x07) return true;
            pParameter1 = pParameter2 & 0x07;
            pParameter2 = (int)(pValue >> 6);
            if (pParameter1 < 0x07)
            {
                pParameter1 += 0x08;
                return true;
            }
            return false;
        }
        public static bool Decode3Parameters(long pValue, out int pParameter1, out int pParameter2, out int pParameter3)
        {
            pParameter1 = 0;
            pParameter2 = 0;
            pParameter3 = 0;
            return Decode2Parameters(pValue, out pParameter1, out pParameter2) &&
                   Decode2Parameters(pParameter2, out pParameter2, out pParameter3);
        }

        public override byte[] ToArray()
        {
            byte[] Dest = new byte[Length - Position];
            Buffer.BlockCopy(base.ToArray(), (int)Position, Dest, 0, (int)(Length - Position));

            return Dest;
        }
        public bool IsDone()
        {
            return Position >= Length;
        }
    }
}
