using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Shared
{
    public class PacketBitAttribute : ISerializableFieldAttribute
    {
        public PacketBitAttribute(int Index)
            : base(Index)
        {

        }

        public override Type GetSerializableType()
        {
            return typeof(PacketBitField);
        }
    }

    public class PacketBitField : ISerializableField
    {
        public override void Deserialize(ref PacketInStream Data)
        {
            long Opcode = Data.ReadEncoded7Bit();
            PacketHandlerDefinition Handler = PacketProcessor.GetPacketHandler(Opcode);
            ISerializablePacket Packet = Activator.CreateInstance(Handler.GetClass()) as ISerializablePacket;

            ISerializableField Field = null;

            Log.Debug("Packet", "----------------------> New " + Opcode.ToString("X8"));

            while ((Field = PacketProcessor.ReadField(ref Data)) != null)
            {
                Log.Success("Packet", "------> ++T : " + Field.PacketType);
                Packet.AddField(Field.Index, Field);
            }

            Log.Debug("Packet", "----------------------> End ");

            Packet.ApplyToFieldInfo();
            val = Packet;
        }

        public override bool Serialize(ref PacketOutStream Data)
        {
            return PacketProcessor.WritePacket(ref Data, (ISerializablePacket)val, false, true, true);
        }

        public override void ApplyToFieldInfo(FieldInfo Info, ISerializablePacket Packet, Type Field)
        {
            if (Field.IsSubclassOf(typeof(ISerializablePacket)))
                Info.SetValue(Packet, val);
        }
    }
}
