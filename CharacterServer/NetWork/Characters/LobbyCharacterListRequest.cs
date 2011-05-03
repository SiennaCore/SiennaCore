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
                Entry.Email = From.Acct.Email;
                Entry.CharacterId = Char.Id;
                Entry.CharacterName = Char.Name;

                Entry.Field5 = new LobbyCharacterInfoBase();

                if (Char.Sex > 1)
                    Entry.Field5.AddField(2, EPacketFieldType.False , (bool)false);

                Entry.Field5.Race = Char.Race;
                Entry.Field5.AddField(15, EPacketFieldType.Unsigned7BitEncoded, (long)Char.Class);
                Entry.Field5.AddField(9, EPacketFieldType.Unsigned7BitEncoded, (long)Char.Level);

                Entry.Field5.Field6 = (uint)CharacterMgr.Instance.GetMaskForRaceSex(Char.Race, Char.Sex).Mask;

                byte[] CustomData = Shared.NetWork.Marshal.StringToUTF8ByteArray(Char.Data);
                PacketInStream CustomStream = new PacketInStream(CustomData,CustomData.Length);
                Entry.Field5.Custom = PacketProcessor.ReadPacket(ref CustomStream) as CharacterCustom;

                Items_Template[] Templates = CharacterMgr.Instance.GetEquipedItems(Char.Id);

                foreach (Items_Template Template in Templates)
                {
                    Model_Info model = CharacterMgr.Instance.GetItemModel(Template.ModelEntry, Char.Race, Char.Sex);

                    CharacterDesc Desc = new CharacterDesc();

                    Desc.AddField(4, EPacketFieldType.Raw4Bytes, (uint)model.Field_4);

                    if (model.Field_5 != 0)
                        Desc.AddField(5, EPacketFieldType.Raw4Bytes, model.Field_5);

                    if (model.Field_6 != 0)
                        Desc.AddField(6, EPacketFieldType.Raw4Bytes, model.Field_6);

                    if (Template.Slot == 10)
                        Desc.AddField(7, EPacketFieldType.Unsigned7BitEncoded, (long)model.Field_7);
                    else
                        Desc.AddField(7, EPacketFieldType.Raw4Bytes, (uint)model.Field_7);

                    Desc.Field8 = new CharacterInfoCache();
                    Desc.Field8.CacheIdentifier = (uint)model.CacheID;

                    Entry.Field5.Field7.Add((long)Template.Slot, Desc);
                }

                Model_Info HairEntry = CharacterMgr.Instance.GetModelForCacheID(Char.HairModelID);

                if (HairEntry != null)
                {
                    CharacterDesc DescHair = new CharacterDesc();

                    DescHair.AddField(4, EPacketFieldType.Raw4Bytes, (uint)HairEntry.Field_4);
                    DescHair.AddField(7, EPacketFieldType.Raw4Bytes, (uint)HairEntry.Field_7);

                    DescHair.Field8 = new CharacterInfoCache();
                    DescHair.Field8.CacheIdentifier = (uint)HairEntry.CacheID;

                    Entry.Field5.Field7.Add((long)46, DescHair);
                }

                Rp.Characters.Add(Entry);
            }

            From.SendSerialized(Rp);
        }

        

    }
}
