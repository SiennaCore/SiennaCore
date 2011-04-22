using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace WorldServer
{
    [ISerializableAttribute((long)Opcodes.WorldCanConnect)]
    public class WorldCanConnect : ISerializablePacket
    {
        static public bool test = false;
        public override void OnRead(RiftClient From)
        {
            ISerializablePacket Packet = new ISerializablePacket();
            Packet.Opcode = 0x1E9A;
            Packet.AddField(1, EPacketFieldType.True, false);
            From.SendSerialized(Packet);
        }
    }
}
