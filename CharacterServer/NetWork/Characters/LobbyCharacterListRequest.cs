using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyCharacterListRequest)]
    public class LobbyCharacterListRequest : ISerializablePacket
    {

        public override void OnRead(RiftClient From)
        {
            Log.Success("CharacterListRequest", "Characters Requested For : " + From.GetIp);

            Character[] Chars = CharacterMgr.Instance.GetCharacters(From.Acct.Id, From.Realm.RealmId);

            LobbyCharacterListResponse Rp = new LobbyCharacterListResponse();
            foreach (Character Char in Chars)
            {
                LobbyCharacterEntry Entry = new LobbyCharacterEntry();
                Entry.AccountId = Char.AccountId;
                Entry.Email = "email@hotmail.com";
                Entry.CharacterId = Char.Id;
                Entry.CharacterName = Char.Name;

                Entry.Field5 = new LobbyCharacterInfoBase();

                if (Char.Sex > 1)
                    Entry.Field5.AddField(2, EPacketFieldType.False , (bool)false);

                Entry.Field5.Race = Char.Race;
                Entry.Field5.AddField(15, EPacketFieldType.Unsigned7BitEncoded, (long)Char.Class);
                Entry.Field5.AddField(9, EPacketFieldType.Unsigned7BitEncoded, (long)Char.Level);

                Entry.Field5.Field6 = (uint)CharacterMgr.Instance.GetMaskForRaceSex(Char.Race, Char.Sex).Mask;

                byte[] CustomData = StringToUTF8ByteArray(Char.Data);
                PacketInStream CustomStream = new PacketInStream(CustomData,CustomData.Length);
                Entry.Field5.Custom = PacketProcessor.ReadPacket(ref CustomStream) as LobbyCharacterCustom;

                foreach(Model_Info model in CharacterMgr.Instance.GetModelsForRaceSex(Char.Race, Char.Sex))
                {
                    // Don't add hair now
                    if (model.SlotID == 46)
                        continue;

                    LobbyCharacterDesc Desc = new LobbyCharacterDesc();

                    Desc.AddField(4, EPacketFieldType.Raw4Bytes, (uint)model.Field_4);

                    if(model.SlotID == 10)
                        Desc.AddField(7, EPacketFieldType.Unsigned7BitEncoded, (long)model.Field_7);
                    else
                        Desc.AddField(7, EPacketFieldType.Raw4Bytes, (uint)model.Field_7);

                    Desc.Field8 = new LobbyCharacterInfoCache();
                    Desc.Field8.CacheIdentifier = (uint)model.CacheID;

                    Entry.Field5.Field7.Add((long)model.SlotID, Desc);
                }

                Model_Info HairEntry = CharacterMgr.Instance.GetModelForCacheID(Char.HairModelID);

                if (HairEntry != null)
                {
                    LobbyCharacterDesc DescHair = new LobbyCharacterDesc();

                    DescHair.AddField(4, EPacketFieldType.Raw4Bytes, (uint)HairEntry.Field_4);
                    DescHair.AddField(7, EPacketFieldType.Raw4Bytes, (uint)HairEntry.Field_7);

                    DescHair.Field8 = new LobbyCharacterInfoCache();
                    DescHair.Field8.CacheIdentifier = (uint)HairEntry.CacheID;

                    Entry.Field5.Field7.Add((long)46, DescHair);
                }

                // Same head for race / sex pair
                /*Model_Info HeadEntry = CharacterMgr.Instance.GetModelForCacheID(Char.HeadModelID);

                if (HeadEntry != null)
                {
                    LobbyCharacterDesc DescHead = new LobbyCharacterDesc();

                    DescHead.AddField(4, EPacketFieldType.Raw4Bytes, (uint)HeadEntry.Field_4);
                    DescHead.AddField(7, EPacketFieldType.Unsigned7BitEncoded, (long)HeadEntry.Field_7);

                    DescHead.Field8 = new LobbyCharacterInfoCache();
                    DescHead.Field8.CacheIdentifier = (uint)HeadEntry.CacheID;

                    Entry.Field5.Field7.Add((long)10, DescHead);
                }*/

                Rp.Characters.Add(Entry);
            }

            From.SendSerialized(Rp);
        }

        static public byte[] StringToUTF8ByteArray(string data)
        {
            List<byte> bytes = new List<byte>();
            foreach (string Str in data.Split(' '))
                bytes.Add(byte.Parse(Str));
            return bytes.ToArray();
        } 

    }
}
