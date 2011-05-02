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
    public class LobbyCharacterDelete : ISerializablePacket
    {
        [ArrayBit(0)]
        public string UnkText;

        [Raw8Bit(1)]
        public long GUID;

        public override void OnRead(RiftClient From)
        {
            Log.Debug("LobbyCharacterDelete", "GUID = " + GUID);

            Character Char = CharacterMgr.Instance.GetCharacter((int)GUID);
            if (Char != null && Char.AccountId == From.Acct.Id)
            {
                CharacterMgr.Instance.RemoveObject(Char);

                foreach (Character_Item Itm in CharacterMgr.Instance.GetPlayerItems(Char.Id))
                    CharacterMgr.Instance.RemoveObject(Itm);
            }
            
            ISerializablePacket DeleteResult = new ISerializablePacket();
            DeleteResult.Opcode = (long)Opcodes.LobbyCharacterDeleteResponse;
            DeleteResult.AddField(0, EPacketFieldType.Unsigned7BitEncoded, (long)0); // Result, 15 Error must wait logout, 0 OK
            From.SendSerialized(DeleteResult);
        }
    }
}
