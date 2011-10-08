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
    public class DicBitAttribute : ISerializableFieldAttribute
    {
        public DicBitAttribute(int Index)
            : base(Index)
        {

        }

        public override Type GetSerializableType()
        {
            return typeof(DicBitField);
        }
    }

    [Serializable]
    public class DicBitField : ISerializableField
    {
        public override void Deserialize(ref PacketInStream Data)
        {
            long DicData = Data.ReadEncoded7Bit();

            int KeyType;
            int ValueType;
            int Count;
            PacketInStream.Decode3Parameters(DicData, out KeyType, out ValueType, out Count);
            Dictionary<ISerializableField, ISerializableField> Dic = new Dictionary<ISerializableField, ISerializableField>();


            Log.Debug("DicBitField", "KeyType = " + KeyType + ",ValueType=" + ValueType + ",Count=" + Count);

            for (int i = 0; i < Count; ++i)
            {
                ISerializableField Key = PacketProcessor.ReadField(ref Data,i,KeyType);
                ISerializableField Value = PacketProcessor.ReadField(ref Data, i, ValueType);

                if (Key != null && Value != null)
                    Dic.Add(Key, Value);
            }

            val = Dic;
        }

        public override bool Serialize(ref PacketOutStream Data, bool Force)
        {
            if (val == null)
                return false;

            if (val is Dictionary<long, ISerializablePacket>)
            {
                Dictionary<long, ISerializablePacket> Dic = val as Dictionary<long, ISerializablePacket>;
                int KeyType = (int)EPacketFieldType.Unsigned7BitEncoded;
                int ValueType = (int)EPacketFieldType.Packet;
                int Count = Dic.Count;

                if (Count <= 0)
                    return false;

                long DicData;
                PacketOutStream.Encode3Parameters(out DicData, KeyType, ValueType, Count);
                Data.WriteEncoded7Bit(DicData);

                foreach (KeyValuePair<long, ISerializablePacket> KP in Dic)
                {
                    PacketProcessor.WriteField(ref Data, (EPacketFieldType)KeyType, KP.Key,true);
                    PacketProcessor.WriteField(ref Data, (EPacketFieldType)ValueType, KP.Value,true);
                }

                return true;
            }

            return false;
        }

        public override void ApplyToFieldInfo(FieldInfo Info, ISerializablePacket Packet, Type Field)
        {
            if (Field.Equals(typeof(Dictionary<long, ISerializablePacket>)))
            {
                Dictionary<long, ISerializablePacket> Dic = new Dictionary<long, ISerializablePacket>();

                foreach (KeyValuePair<ISerializableField, ISerializableField> KP in (val as Dictionary<ISerializableField, ISerializableField>))
                {
                    Dic.Add((long)KP.Key.GetLong(), KP.Value.GetPacket());
                }

                Info.SetValue(Packet, Dic);
            }
        }
    }
}