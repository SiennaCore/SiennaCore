using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sienna.zlib;

using System.Net.Sockets;
using System.IO;

namespace Sienna.Network
{
    public class PacketStream
    {
        MemoryStream str;

        public bool Empty()
        {
            return str.Length - str.Position <= 0;
        }

        public long Length()
        {
            return str.Length - str.Position;
        }

        /// Readers

        public int ReadByte()
        {
            return str.ReadByte();
        }

        public byte[] Read(int size)
        {
            byte[] buf = new byte[size];
            str.Read(buf, 0, size);

            return buf;
        }

        public PacketStream()
        {
            str = new MemoryStream();
        }

        public PacketStream(byte[] Data)
        {
            str = new MemoryStream();
            str.Write(Data, 0, Data.Length );
        }

        public PacketStream(byte[] Data, int Length)
        {
            str = new MemoryStream();
            str.Write(Data, 0, Length);
        }

        public PacketStream(byte[] Data, int Start, int Length)
        {
            str = new MemoryStream();
            str.Write(Data, Start, Length);
        }

        public virtual UInt16 GetUInt16Reversed()
        {
            var v1 = (byte)str.ReadByte();
            var v2 = (byte)str.ReadByte();

            return ExtendedBitConverter.ConvertToUInt16(v1, v2);
        }

        public virtual ushort GetUInt16()
        {
            var v1 = (byte)str.ReadByte();
            var v2 = (byte)str.ReadByte();

            return ExtendedBitConverter.ConvertToUInt16(v2, v1);
        }

        public virtual uint GetUInt32Reversed()
        {
            var v1 = (byte)str.ReadByte();
            var v2 = (byte)str.ReadByte();
            var v3 = (byte)str.ReadByte();
            var v4 = (byte)str.ReadByte();

            return ExtendedBitConverter.ConvertToUInt32(v1, v2, v3, v4);
        }

        public void Skip(long num)
        {
            str.Position += num;
        }

        public virtual string ReadString(int len)
        {
            var buf = new byte[len];
            str.Read(buf, 0, len);

            return ExtendedBitConverter.ConvertToString(buf);
        }

        public virtual uint GetUInt32()
        {
            var v1 = (byte)str.ReadByte();
            var v2 = (byte)str.ReadByte();
            var v3 = (byte)str.ReadByte();
            var v4 = (byte)str.ReadByte();

            return ExtendedBitConverter.ConvertToUInt32(v4, v3, v2, v1);
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        public UInt64 GetUInt64()
        {
            UInt64 value = (GetUInt32() << 24) + (GetUInt32());
            return value;
        }

        public float GetFloat()
        {
            byte[] b = new byte[4];
            b[0] = (byte)str.ReadByte();
            b[1] = (byte)str.ReadByte();
            b[2] = (byte)str.ReadByte();
            b[3] = (byte)str.ReadByte();


            return BitConverter.ToSingle(b, 0);
        }

        /// Writers

        public virtual void Write(byte[] data)
        {
            str.Write(data, 0, data.Length);
        }

        public virtual void WriteByte(byte val)
        {
            str.WriteByte(val);
        }

        public virtual void WriteUInt16Reversed(ushort val)
        {
            str.WriteByte((byte)(val >> 8));
            str.WriteByte((byte)(val & 0xff));
        }

        public virtual void WriteUInt16(ushort val)
        {
            str.WriteByte((byte)(val & 0xff));
            str.WriteByte((byte)(val >> 8));
        }

        public virtual void WriteUInt32Reversed(uint val)
        {
            str.WriteByte((byte)(val >> 24));
            str.WriteByte((byte)((val >> 16) & 0xff));
            str.WriteByte((byte)((val & 0xffff) >> 8));
            str.WriteByte((byte)((val & 0xffff) & 0xff));
        }

        public virtual void WriteUInt32(uint val)
        {
            str.WriteByte((byte)((val & 0xffff) & 0xff));
            str.WriteByte((byte)((val & 0xffff) >> 8));
            str.WriteByte((byte)((val >> 16) & 0xff));
            str.WriteByte((byte)(val >> 24));
        }

        public virtual void WriteUInt64Reversed(ulong val)
        {
            str.WriteByte((byte)(val >> 56));
            str.WriteByte((byte)((val >> 48) & 0xff));
            str.WriteByte((byte)((val >> 40) & 0xff));
            str.WriteByte((byte)((val >> 32) & 0xff));
            str.WriteByte((byte)((val >> 24) & 0xff));
            str.WriteByte((byte)((val >> 16) & 0xff));
            str.WriteByte((byte)((val >> 8) & 0xff));
            str.WriteByte((byte)(val & 0xff));
        }

        public virtual void WriteFloat(float val)
        {
            foreach (Byte b in BitConverter.GetBytes(val))
                str.WriteByte(b);
        }

        public virtual void WriteUInt64(ulong val)
        {
            str.WriteByte((byte)(val & 0xff));
            str.WriteByte((byte)((val >> 8) & 0xff));
            str.WriteByte((byte)((val >> 16) & 0xff));
            str.WriteByte((byte)((val >> 24) & 0xff));
            str.WriteByte((byte)((val >> 32) & 0xff));
            str.WriteByte((byte)((val >> 40) & 0xff));
            str.WriteByte((byte)((val >> 48) & 0xff));
            str.WriteByte((byte)(val >> 56));
        }

        public virtual void Fill(byte val, int num)
        {
            for (int i = 0; i < num; ++i)
            {
                str.WriteByte(val);
            }
        }

        public virtual void WritePascalString(string sstr)
        {
            if (sstr == null || sstr.Length <= 0)
            {
                str.WriteByte(0);
                return;
            }

            byte[] bytes = Encoding.Default.GetBytes(sstr);
            str.WriteByte((byte)bytes.Length);
            str.Write(bytes, 0, bytes.Length);
        }

        public virtual void WriteString(string str)
        {
            WriteStringBytes(str);
        }

        public virtual void WriteStringBytes(string sstr)
        {
            if (sstr.Length <= 0)
                return;

            byte[] bytes = Encoding.Default.GetBytes(sstr);
            str.Write(bytes, 0, bytes.Length);
        }

        public virtual void WriteString(string sstr, int maxlen)
        {
            if (sstr.Length <= 0)
                return;

            byte[] bytes = Encoding.Default.GetBytes(sstr);
            str.Write(bytes, 0, bytes.Length < maxlen ? bytes.Length : maxlen);
        }

        public virtual void FillString(string sstr, int len)
        {
            long pos = str.Position;

            Fill(0x0, len);

            if (sstr == null)
                return;

            str.Position = pos;

            if (sstr.Length <= 0)
            {
                str.Position = pos + len;
                return;
            }

            byte[] bytes = Encoding.Default.GetBytes(sstr);
            str.Write(bytes, 0, len > bytes.Length ? bytes.Length : len);
            str.Position = pos + len;
        }

        public byte[] ToLogonPacket(UInt16 Opcode)
        {
            PacketStream ps = new PacketStream();

            bool isLongPacket = false;

            if (str.Length + 2 >= 128)
                isLongPacket = true;

            if (isLongPacket)
            {
                byte Offset = (byte)((str.Length + 2) / 128);
                byte Size = (byte)((str.Length + 2) - (long)(Offset * 128));

                ps.WriteByte(Size);
                ps.WriteByte(Offset);
            }
            else
                ps.WriteByte(BitConverter.GetBytes(str.Length + 2)[0]);

            ps.WriteUInt16(Opcode);
            ps.Write(str.ToArray());

            return ps.ToArray();
        }

        public byte[] ToArray()
        {
            return str.ToArray();
        }

        public void Rewind(int Position)
        {
            str.Position = Position;
        }
    }
}
