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
using System.Reflection;

using FrameWork;

namespace Common
{
    public class ListBitAttribute : ISerializableFieldAttribute
    {
        public ListBitAttribute(int Index)
            : base(Index)
        {
        }

        public override Type GetSerializableType()
        {
            return typeof(ListBitField);
        }
    }

    [Serializable]
    public class ListBitField : ISerializableField
    {
        public override void Deserialize(ref PacketInStream Data)
        {

            long ListData = Data.ReadEncoded7Bit();
            int ListType;
            int ListCount;
            PacketInStream.Decode2Parameters(ListData, out ListType, out ListCount);
            List<ISerializableField> Fields = new List<ISerializableField>();

            Log.Debug("Packet", "------> List : " + ListType + "("+ListCount+")");
            for (int i = 0; i < ListCount; ++i)
            {
                ISerializableField Field = PacketProcessor.GetFieldType((EPacketFieldType)ListType);
                if (Field != null)
                {
                    Field.Deserialize(ref Data);
                    Fields.Add(Field);
                }
            }

            val = Fields;
        }

        public override bool Serialize(ref PacketOutStream Data,bool Force)
        {
            Log.Debug("WriteList", "Serialize : " + val);

            if (val == null)
                return false;

            if (val is List<ISerializablePacket>)
            {
                List<ISerializablePacket> Packets = val as List<ISerializablePacket>;

                /*if (Packets.Count <= 0)
                    return false;*/

                long ListData;
                PacketOutStream.Encode2Parameters(out ListData, (int)EPacketFieldType.Packet, Packets.Count);
                Data.WriteEncoded7Bit(ListData);

                foreach (ISerializablePacket Packet in Packets)
                    PacketProcessor.WritePacket(ref Data, Packet, false, true, true);
            }
            else if (val is ISerializablePacket[])
            {
                ISerializablePacket[] Packets = val as ISerializablePacket[];

                long ListData;
                PacketOutStream.Encode2Parameters(out ListData, (int)EPacketFieldType.Packet, Packets.Length);
                Data.WriteEncoded7Bit(ListData);

                foreach (ISerializablePacket Packet in Packets)
                    PacketProcessor.WritePacket(ref Data, Packet, false, true, true);
            }
            else if (val is List<ulong>)
            {
                List<ulong> Values = val as List<ulong>;

                long ListData;
                PacketOutStream.Encode2Parameters(out ListData, (int)EPacketFieldType.Unsigned7BitEncoded, Values.Count);
                Data.WriteEncoded7Bit(ListData);

                for (int i = 0; i < Values.Count; ++i)
                    PacketProcessor.WriteField(ref Data, EPacketFieldType.Unsigned7BitEncoded, (ulong)Values[i], true);
            }
            else if (val is List<long>)
            {
                List<long> Values = val as List<long>;

                long ListData;
                PacketOutStream.Encode2Parameters(out ListData, (int)EPacketFieldType.Unsigned7BitEncoded, Values.Count);
                Data.WriteEncoded7Bit(ListData);

                for (int i = 0; i < Values.Count; ++i)
                    PacketProcessor.WriteField(ref Data, EPacketFieldType.Unsigned7BitEncoded, (long)Values[i],true);
            }
            else if (val is List<uint>)
            {
                List<uint> Values = val as List<uint>;

                /*if (Values.Count <= 0)
                    return false;*/

                long ListData;
                PacketOutStream.Encode2Parameters(out ListData, (int)EPacketFieldType.Raw4Bytes, Values.Count);
                Data.WriteEncoded7Bit(ListData);

                for (int i = 0; i < Values.Count; ++i)
                    PacketProcessor.WriteField(ref Data, EPacketFieldType.Raw4Bytes, (uint)Values[i], true);
            }
            else if (val is List<float>)
            {
                List<float> Values = val as List<float>;

                /*if (Values.Count <= 0)
                    return false;*/

                long ListData;
                PacketOutStream.Encode2Parameters(out ListData, (int)EPacketFieldType.Raw4Bytes, Values.Count);
                Data.WriteEncoded7Bit(ListData);

                for (int i = 0; i < Values.Count; ++i)
                    PacketProcessor.WriteField(ref Data, EPacketFieldType.Raw4Bytes, Values[i], true);
            }
            else if (val is List<string>)
            {
                List<string> Strs = val as List<string>;

                /*if (Strs.Count <= 0)
                    return false;*/

                long ListData;
                PacketOutStream.Encode2Parameters(out ListData, (int)EPacketFieldType.ByteArray, Strs.Count);
                Data.WriteEncoded7Bit(ListData);

                for (int i = 0; i < Strs.Count; ++i)
                    PacketProcessor.WriteField(ref Data, EPacketFieldType.ByteArray, (string)Strs[i], true);

                return true;
            }
            else if (val is List<ISerializableField>)
            {
                List<ISerializableField> Strs = val as List<ISerializableField>;

                if (Strs.Count <= 0)
                    return false;

                ISerializableField Field = Strs[0];

                long ListData;
                PacketOutStream.Encode2Parameters(out ListData, (int)Field.PacketType, Strs.Count);
                Data.WriteEncoded7Bit(ListData);

                for (int i = 0; i < Strs.Count; ++i)
                    PacketProcessor.WriteField(ref Data, Strs[i].PacketType, Strs[i].val, true);

                return true;
            }
            else
                return false;

            return true;
        }

        public override void ApplyToFieldInfo(FieldInfo Info, ISerializablePacket Packet, Type Field)
        {
            if (Field.Equals(typeof(List<uint>)))
            {
                List<uint> Luint = new List<uint>();
                foreach (ISerializableField Value in (List<ISerializableField>)val)
                    Luint.Add(Value.GetUint());
                Info.SetValue(Packet, Luint);
            }
            else if(Field.Equals(typeof(List<ISerializablePacket>)))
            {
                List<ISerializablePacket> Packets = new List<ISerializablePacket>();
                foreach (ISerializableField Value in (List<ISerializableField>)val)
                    Packets.Add(Value.GetPacket());
                Info.SetValue(Packet, Packets);
            }
            else if (Field.Equals(typeof(ISerializablePacket[])))
            {
                List<ISerializablePacket> Packets = new List<ISerializablePacket>();
                foreach (ISerializableField Value in (List<ISerializableField>)val)
                    Packets.Add(Value.GetPacket());
                Info.SetValue(Packet, Packets.ToArray());
            }
            else if (Field.Equals(typeof(List<bool>)))
            {
                List<bool> Bools = new List<bool>();
                foreach (ISerializableField Value in (List<ISerializableField>)val)
                    Bools.Add((bool)Value.val);
                Info.SetValue(Packet, Bools);
            }
            else if (Field.Equals(typeof(List<float>)))
            {
                List<float> floats = new List<float>();
                foreach (ISerializableField Value in (List<ISerializableField>)val)
                    floats.Add(Value.GetFloat());

                Info.SetValue(Packet, floats);
            }
        }
    }
}
