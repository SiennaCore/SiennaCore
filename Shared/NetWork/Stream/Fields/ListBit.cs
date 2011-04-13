using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Shared
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

    public class ListBitField : ISerializableField
    {
        public override void Deserialize(ref PacketInStream Data)
        {

            long ListData = Data.ReadEncoded7Bit();
            int ListType;
            int ListCount;
            PacketInStream.Decode2Parameters(ListData, out ListType, out ListCount);
            List<ISerializableField> Fields = new List<ISerializableField>();

            Log.Success("Packet", "------> List : " + ListType + "("+ListCount+")");
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

        public override void Serialize(ref PacketOutStream Data)
        {
            if (val is List<ISerializablePacket>)
            {
                List<ISerializablePacket> Packets = val as List<ISerializablePacket>;

                long ListData;
                PacketOutStream.Encode2Parameters(out ListData, (int)EPacketFieldType.Packet, Packets.Count);
                Data.WriteEncoded7Bit(ListData);

                foreach (ISerializablePacket Packet in Packets)
                    PacketProcessor.WritePacket(ref Data, Packet, false, true, true);
            }
            else if (val is List<uint>)
            {
                List<uint> Values = val as List<uint>;

                long ListData;
                PacketOutStream.Encode2Parameters(out ListData, (int)EPacketFieldType.Raw4Bytes, Values.Count);
                Data.WriteEncoded7Bit(ListData);

                Log.Success("List", "LIST OF UINT ! Count = " + Values.Count);

                foreach (uint Value in Values)
                    PacketProcessor.WriteField(ref Data, EPacketFieldType.Raw4Bytes, Value);
            }
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
            else if (Field.Equals(typeof(List<bool>)))
            {
                List<bool> Bools = new List<bool>();
                foreach (ISerializableField Value in (List<ISerializableField>)val)
                    Bools.Add((bool)Value.val);
                Info.SetValue(Packet, Bools);
            }
        }
    }
}
