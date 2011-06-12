using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Shared.NetWork
{
    public class PacketIn : MemoryStream
    {
        // Opcode du packet
        // Opcode of packet
        private UInt64 _Opcode = 0;
        private UInt64 _Size = 0;

        public UInt64 Opcode
        {
            get { return _Opcode; }
            set { _Opcode = value; }
        }
        public UInt64 Size
        {
            get { return _Size; }
            set { _Size = value; }
        }

        public PacketIn(int size)
            : base(size)
        {
        }

        public PacketIn(byte[] buf, int start, int size)
            : base(buf, start, size)
        {
        }

        public byte[] Read(int size)
        {
            byte[] buf = new byte[size];
            Read(buf, 0, size);

            return buf;
        }
        public void Skip(long num)
        {
            Seek(num, SeekOrigin.Current);
        }
        public long Remain()
        {
            return Length - Position;
        }

        public virtual byte GetUint8()
        {
            return (byte)ReadByte();
        }

        public virtual UInt16 GetUint16()
        {
            return Marshal.ConvertToUInt16(GetUint8(), GetUint8());
        }
        public virtual ushort GetUint16R()
        {
            byte v1 = GetUint8();
            byte v2 = GetUint8();

            return Marshal.ConvertToUInt16(v2, v1);
        }

        public virtual Int16 GetInt16()
        {
            byte[] tmp = { GetUint8() , GetUint8()  };
            return BitConverter.ToInt16(tmp, 0);
        }
        public virtual Int16 GetInt16R()
        {
            byte v1 = GetUint8();
            byte v2 = GetUint8();

            byte[] tmp = { v2, v1 };
            return BitConverter.ToInt16(tmp, 0);
        }

        public virtual uint GetUint32()
        {
            byte v1 = GetUint8();
            byte v2 = GetUint8();
            byte v3 = GetUint8();
            byte v4 = GetUint8();

            return Marshal.ConvertToUInt32(v1, v2, v3, v4);
        }
        public virtual uint GetUint32R()
        {
            byte v1 = GetUint8();
            byte v2 = GetUint8();
            byte v3 = GetUint8();
            byte v4 = GetUint8();

            return Marshal.ConvertToUInt32(v4, v3, v2, v1);
        }

        public virtual Int32 GetInt32()
        {
            byte[] tmp = { GetUint8(), GetUint8(), GetUint8(), GetUint8() };
            return BitConverter.ToInt32(tmp, 0);
        }
        public virtual Int32 GetInt32R()
        {
            byte v1 = GetUint8();
            byte v2 = GetUint8();
            byte v3 = GetUint8();
            byte v4 = GetUint8();

            byte[] tmp = { v4, v3, v2, v1 };
            return BitConverter.ToInt32(tmp, 0);
        }

        public UInt64 GetUint64()
        {

            UInt64 value = (GetUint32() << 24) + (GetUint32());
            return value;
        }
        public UInt64 GetUint64R()
        {

            UInt64 value = (GetUint32()) + (GetUint32() << 24);
            return value;
        }

        public float GetFloat()
        {
            byte[] b = new byte[4];
            b[0] = (byte)ReadByte();
            b[1] = (byte)ReadByte();
            b[2] = (byte)ReadByte();
            b[3] = (byte)ReadByte();

            return BitConverter.ToSingle(b, 0);
        }

        private char ReadPs()
        {
            if (Length >= Position + 2)
                return BitConverter.ToChar(new byte[] { GetUint8(), GetUint8() }, 0);

            return (char)0;
        }
        public virtual string GetString()
        {
            int len = (int)GetUint32();

            var buf = new byte[len];
            Read(buf, 0, len);

            return Marshal.ConvertToString(buf);
        }
        public virtual string GetString(int maxlen)
        {
            var buf = new byte[maxlen];
            Read(buf, 0, maxlen);

            return Marshal.ConvertToString(buf);
        }
        public virtual string GetPascalString()
        {
            return GetString(GetUint8());
        }
        public virtual string GetUnicodeString()
        {
            string tmp = "";

            char tmp2 = ReadPs();
            while (tmp2 != 0x00)
            {
                tmp += tmp2;
                tmp2 = ReadPs();
            }

            return tmp;
        }
        public virtual string GetStringToZero()
        {
            string value = "";

            while (Position < Length)
            {
                char c = (char)ReadByte();
                if (c == 0)
                    break;
                value += c;

            }

            return value;
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        #region Mythic

        public int DecodeMythicSize()
        {
            int mSize = 0;
            int mByteCount = 0;
            int mByte = 0;

            mByte = ReadByte();
            while ((mByte & 0x80) == 0x80)
            {
                Log.Debug("readSize", "mByte = " + mByte);
                mByte ^= 0x80;
                mSize = (mSize | (mByte << (7 * mByteCount)));

                if (Length == Capacity)
                    return 0;


                mByte = ReadByte();
                mByteCount++;
            }

            mSize = (mSize | (mByte << (7 * mByteCount)));
            return mSize;
        }
        public PacketIn DecryptMythicRC4(byte[] Key)
        {
            // Putin ... j'en ai chié pour réussir a faire sa...
            // Putin ... I have shit to do to succeed his ... (?)
            try
            {
                byte[] Packet = new byte[Length];
                Read(Packet, (int)Position, Packet.Length);

                int x, y, midpoint, pos;
                byte tmp = 0;

                x = y = 0;

                midpoint = Packet.Length / 2;

                for (pos = midpoint; pos < Packet.Length; ++pos)
                {
                    x = (x + 1) & 255;
                    y = (y + Key[x]) & 255;

                    tmp = Key[x];

                    Key[x] = Key[y];
                    Key[y] = tmp;

                    tmp = (byte)(( Key[x] + Key[y] ) & 255);
                    Packet[pos] ^= Key[tmp];
                    y = (y + Packet[pos]) & 255;
                }

                for (pos = 0; pos < midpoint; ++pos)
                {
                    x = (x + 1) & 255;
                    y = (y + Key[x]) & 255;

                    tmp = Key[x];

                    Key[x] = Key[y];
                    Key[y] = tmp;

                    tmp = (byte)((Key[x] + Key[y]) & 255);
                    Packet[pos] ^= Key[tmp];
                    y = (y + Packet[pos]) & 255; 
                }

                return new PacketIn(Packet,0,Packet.Length);
            }
            catch(Exception e)
            {
                Log.Error("PacketIn","DecryptMythicRC4 : Failed !" + e.ToString());
            }

            return null;
        }

        #endregion

        #region GameBryo

        public int DecodeGamebryoSize()
        {
            int Size = GetUint8();

            if (Size >= 128)
                Size += ((GetUint8() - 1) * 128) + 2;

            return Size;
        }

        #endregion
    }
}
