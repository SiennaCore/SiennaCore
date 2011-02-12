using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;
using Shared.NetWork;

namespace CharacterServer
{
    public class Realms : IPacketHandler
    {
        [PacketHandler(PacketHandlerType.TCP,(int)Opcodes.REQUEST_REALMS_LIST,(int)RiftState.AUTHENTIFIED,"REQUEST_REALMS_LIST")]
        static public void HandleRealmsList(BaseClient client, PacketIn Packet)
        {
            RiftClient Client = client as RiftClient;

            Realm[] Rms = Program.CharMgr.GetRealms();

            int Count = Rms.Length;
            bool Multiple = Count % 2 == 0 ? true : false;

            if (!Multiple && Count > 0)
                ++Count;

            PacketOut Out = new PacketOut((ushort)Opcodes.REALMS_LIST_RESPONSE);
            Out.WriteUInt16(0x5F97);
            Out.WriteByte((byte)(Count / 2));

            foreach (Realm Rm in Rms)
                Rm.Write(ref Out);

            if (!Multiple && Count > 0)
                Rms[0].Write(ref Out);

            Out.WriteByte(7);

            Client.SendTCP(Out);
        }

        [PacketHandler(PacketHandlerType.TCP, (int)Opcodes.SELECT_REALM, (int)RiftState.AUTHENTIFIED, "SELECT_REALM")]
        static public void HandleSelectRealm(BaseClient client, PacketIn Packet)
        {
            RiftClient Client = client as RiftClient;

            byte Unk = Packet.GetUint8();

            if (Unk != 7)
                Client.Realm = Program.CharMgr.GetRealm(Packet.GetUint16());

            if (Client.Realm == null)
            {
                Client.Disconnect();
                return;
            }

            Log.Succes("Realms", "Client '" + Client.Acct.Username + "' enter on realm : " + Client.Realm.Name);


            PacketOut Out = new PacketOut((ushort)Opcodes.SELECT_REALM_RESPONSE);
            Out.WriteByte(07);
            Client.SendTCP(Out);
        }
    }
}
