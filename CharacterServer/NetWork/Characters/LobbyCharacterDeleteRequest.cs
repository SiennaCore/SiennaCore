using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using Shared;
using Shared.Database;
using Shared.NetWork;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyCharacterDeleteRequest)]
    public class LobbyCharacterDeleteRequest : ISerializablePacket
    {
        [ArrayBit(0)]
        public string UnkText;

        [Raw8Bit(1)]
        public long GUID;

        public override void OnRead(RiftClient From)
        {
            // Client requested delete character with guid

            Log.Dump("LobbyCharacterDelete", "GUID = " + GUID + ",Text = " + UnkText);

            Character Char = CharacterMgr.Instance.GetCharacter((int)GUID);
            if (Char != null)
                CharacterMgr.Instance.RemoveObject(Char);

            ISerializablePacket Packet = new ISerializablePacket();
            Packet.Opcode = (long)Opcodes.LobbyCharacterDeleteResponse;
            From.SendSerialized(Packet);
        }
    }
}
