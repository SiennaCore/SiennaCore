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

    [Serializable]
    public class PacketBitField : ISerializableField
    {
        public override void Deserialize(ref PacketInStream Data)
        {
            long Opcode = Data.ReadEncoded7Bit();
            PacketHandlerDefinition Handler = PacketProcessor.GetPacketHandler(Opcode);
            ISerializablePacket Packet = Activator.CreateInstance(Handler.GetClass()) as ISerializablePacket;

            ISerializableField Field = null;

            Log.Debug("Packet", "----------------------> New " + Opcode.ToString("X8"));
            Packet.Opcode = Opcode;

            while ((Field = PacketProcessor.ReadField(ref Data)) != null)
            {
                Log.Debug("Packet", "------> ++T : " + Field.PacketType);
                Packet.AddField(Field.Index, Field);
            }

            Log.Debug("Packet", "----------------------> End ");

            Packet.ApplyToFieldInfo();
            val = Packet;
        }

        public override bool Serialize(ref PacketOutStream Data, bool Force)
        {
            if (val == null)
                return false;

            return PacketProcessor.WritePacket(ref Data, (ISerializablePacket)val, false, true, true);
        }

        public override void ApplyToFieldInfo(FieldInfo Info, ISerializablePacket Packet, Type Field)
        {
            if (Field.IsSubclassOf(typeof(ISerializablePacket)))
                Info.SetValue(Packet, val);
        }
    }
}
