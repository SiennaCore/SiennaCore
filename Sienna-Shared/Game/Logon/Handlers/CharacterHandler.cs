using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml.Serialization;

using Sienna.Database;
using Sienna.Network;

namespace Sienna.Game
{
    public static class CharacterHandler
    {
        private static byte GetPlayerByte(ref PacketStream Data, ref bool bytefound)
        {
            bytefound = false;

            byte InstructionType = (byte)Data.ReadByte();
            byte DataByte = 0x00;

            // Sex infos
            if (InstructionType == 0x12)
                DataByte = (byte)Data.ReadByte();

            // Race infos
            if (InstructionType == 0x0A)
                DataByte = (byte)Data.ReadByte();

            // Class infos
            if (InstructionType == 0x6A)
                DataByte = (byte)Data.ReadByte();

            if (DataByte != 0)
                bytefound = true;

            return DataByte != 0 ? DataByte : InstructionType;
        }

        [LogonPacket((ushort)LogonOpcodes.Client_CreateCharacter)]
        public static void HandleCharacterCreation(LogonClient From, PacketStream Data)
        {
            // Unk1
            Data.ReadByte();

            // Get name and check name existance
            String RequestedName = Data.ReadString();
            RequestedName = LogonMgr.LDatabase.EscapeString(RequestedName);

            byte Sex = 0x01;
            byte Race = 0x01;
            byte Class = 0x01;

            // Not found yet
            int RealmId = 0x00;

            bool bfound = false;

            byte dat = GetPlayerByte(ref Data, ref bfound);

            // Fetch Race / Sex informations
            if (bfound)
            {
                Race = dat;

                // Check if it's renegate
                int isRenegate = Race - 0xD0;
                if (isRenegate >= 0)
                {
                    Race = (byte)((int)Race - 0xD0);

                    // Skip 0x0F
                    Data.ReadByte();
                }

                dat = GetPlayerByte(ref Data, ref bfound);

                if (bfound)
                {
                    Sex = dat;
                    dat = GetPlayerByte(ref Data, ref bfound);
                }
            }

            // Unk block
            Data.Skip(20);

            // Fetch class
            dat = GetPlayerByte(ref Data, ref bfound);
            if (bfound)
            {
                Class = dat;
                dat = GetPlayerByte(ref Data, ref bfound);
            }

            // End of block ?
            Data.Skip(2);

            byte[] Appearance = Data.Read((int)Data.Length());
            String HexAppearance = BitConverter.ToString(Appearance);
            
            // Put whitespaces
            HexAppearance = HexAppearance.Replace("-", " ");

            // Remove junk
            int junkpos = HexAppearance.IndexOf("00 00 00 00");

            if(junkpos > 0)
                HexAppearance = HexAppearance.Substring(0, junkpos);

            LogonMgr.LDatabase.DelayedExecute("SELECT guid FROM characters WHERE name = \"" + RequestedName + "\" AND realm = " + RealmId + "", (List<Row> Results, object e) =>
            {
                // A Character with that name already exist
                if (Results.Count != 0)
                {
                    PacketStream ps = new PacketStream();
                    ps.WriteByte(0x01);
                    ps.WriteByte(0x07);
                    From.Send(LogonOpcodes.Server_CreateCharacterResult, ps);
                    return;
                }

                LogonMgr.LDatabase.Execute("INSERT INTO characters (account, realm, name, sex, class, race, level, data) VALUES (" + From.Acct.ID + "," + 0 + ",\"" + RequestedName + "\"," + Sex + "," + Class + "," + Race + "," + 1 + ",\"" + HexAppearance + "\")");

                // OK
                PacketStream psb = new PacketStream();
                psb.WriteByte(0x00);
                psb.WriteByte(0x07);
                From.Send(LogonOpcodes.Server_CreateCharacterResult, psb);
            },
            null);
        }
    }
}
