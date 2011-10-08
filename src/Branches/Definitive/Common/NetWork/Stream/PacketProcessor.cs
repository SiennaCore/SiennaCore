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
    public class PacketHandlerDefinition
    {
        protected long Opcode;
        protected Type PacketClass;

        public PacketHandlerDefinition(long opcode, Type Class)
        {
            Opcode = opcode;
            PacketClass = Class;
        }

        public long GetOpcode()
        {
            return Opcode;
        }

        public Type GetClass()
        {
            return PacketClass;
        }
    }

    public static class PacketProcessor
    {
        #region PacketHandler && Fields

        private static Dictionary<long, PacketHandlerDefinition> Definitions;
        private static Dictionary<EPacketFieldType, ISerializableField> FieldsTypes;

        public static void RegisterDefinitions()
        {
            Definitions = new Dictionary<long, PacketHandlerDefinition>();
            FieldsTypes = new Dictionary<EPacketFieldType, ISerializableField>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = null;

                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types;
                }

                foreach (Type Class in types)
                {
                    if (Class.BaseType == typeof(ISerializablePacket))
                    {
                        ISerializableAttribute[] PacketAttributes = Class.GetCustomAttributes(typeof(ISerializableAttribute), true) as ISerializableAttribute[];
                        if (PacketAttributes == null || PacketAttributes.Length <= 0)
                            continue;

                        ISerializableAttribute PacketAttribute = PacketAttributes[0];
                        PacketHandlerDefinition PacketDef = new PacketHandlerDefinition(PacketAttribute.GetOpcode(), Class);

                        Log.Debug("RegisterDefinitions", "Registering Handler : " + PacketAttribute.GetOpcode().ToString("X8"));
                        if(!Definitions.ContainsKey(PacketAttribute.GetOpcode()))
                            Definitions.Add(PacketAttribute.GetOpcode(), PacketDef);
                        else
                            Log.Error("RegisterDefinitions","Handler already registered : " + PacketAttribute.GetOpcode().ToString("X8"));
                    }
                }
            }

            FieldsTypes.Add(EPacketFieldType.True, new BoolBitField());
            FieldsTypes.Add(EPacketFieldType.False, new BoolBitField());
            FieldsTypes.Add(EPacketFieldType.List, new ListBitField());
            FieldsTypes.Add(EPacketFieldType.Packet, new PacketBitField());
            FieldsTypes.Add(EPacketFieldType.ByteArray, new ArrayBitField());
            FieldsTypes.Add(EPacketFieldType.Raw4Bytes, new Raw4BitField());
            FieldsTypes.Add(EPacketFieldType.Raw8Bytes, new Raw8BitBitField());
            FieldsTypes.Add(EPacketFieldType.Dictionary, new DicBitField());
            FieldsTypes.Add(EPacketFieldType.Signed7BitEncoded, new Encoded7BitField());
            FieldsTypes.Add(EPacketFieldType.Unsigned7BitEncoded, new Unsigned7BitField());
        }

        public static PacketHandlerDefinition GetPacketHandler(long Opcode)
        {
            PacketHandlerDefinition Handler;
            Definitions.TryGetValue(Opcode, out Handler);

            if (Handler == null)
            {
                Log.Error("PacketProcessor","Unhandled Opcode : " + Opcode.ToString("X8"));
                Handler = new PacketHandlerDefinition(Opcode, typeof(ISerializablePacket));
            }

            return Handler;
        }

        public static ISerializableField GetFieldType(EPacketFieldType Type)
        {
            ISerializableField Field;
            FieldsTypes.TryGetValue(Type, out Field);
            if (Field != null)
            {
                ISerializableField IField = Activator.CreateInstance(Field.GetType()) as ISerializableField;
                IField.PacketType = Type;
                return IField;
            }

            Log.Error("PacketProcessor", "Unhandled Field Type : " + Type);
            return null;
        }

        public static EPacketFieldType GetFieldType(ISerializableField Field)
        {
            if (Field is BoolBitField)
            {
                if (Field.val.ToString() == "0")
                    return EPacketFieldType.False;
                else
                    return EPacketFieldType.True;
            }

            foreach (KeyValuePair<EPacketFieldType, ISerializableField> F in FieldsTypes)
                if (F.Value.GetType() == Field.GetType() )
                    return F.Key;

            Log.Error("PacketProcessor", "Unhandled Field : " + Field);
            return EPacketFieldType.Invalid;
        }

        #endregion

        #region Readers

        public static void BytesToField(ISerializablePacket Packet, byte[] Bytes, string FieldName)
        {
            if (Bytes == null || Bytes.Length <= 0)
                return;

            FieldInfo Info = Packet.GetType().GetField(FieldName);
            ISerializableFieldAttribute[] FieldsAttr = Info.GetCustomAttributes(typeof(ISerializableFieldAttribute), true) as ISerializableFieldAttribute[];
            if (FieldsAttr != null && FieldsAttr.Length > 0)
            {
                ISerializableField Field = Activator.CreateInstance(FieldsAttr[0].GetSerializableType()) as ISerializableField;
                Field.Index = FieldsAttr[0].Index;
                Field.val = Info.GetValue(Packet);
                Field.PacketType = PacketProcessor.GetFieldType(Field);

                PacketInStream Str = new PacketInStream(Bytes, Bytes.Length);
                Field.Deserialize(ref Str);
                Field.ApplyToFieldInfo(Info, Packet, Info.FieldType);
            }
            else
                Info.SetValue(Packet, null);
        }

        public static byte[] FieldToBytes(ISerializablePacket Packet, string FieldName)
        {
            FieldInfo Info = Packet.GetType().GetField(FieldName);
            ISerializableFieldAttribute[] FieldsAttr = Info.GetCustomAttributes(typeof(ISerializableFieldAttribute), true) as ISerializableFieldAttribute[];
            if (FieldsAttr != null && FieldsAttr.Length > 0)
            {
                ISerializableField Field = Activator.CreateInstance(FieldsAttr[0].GetSerializableType()) as ISerializableField;
                Field.Index = FieldsAttr[0].Index;
                Field.val = Info.GetValue(Packet);
                Field.PacketType = PacketProcessor.GetFieldType(Field);

                PacketOutStream Str = new PacketOutStream();
                Field.Serialize(ref Str,true);
                byte[] Result =  Str.ToArray();
                return Result;
            }
            else
                return null;
        }

        public static ISerializablePacket ReadPacket(ref PacketInStream Stream)
        {
            return (ISerializablePacket)ReadField(ref Stream, 0, (int)EPacketFieldType.Packet).val;
        }

        public static ISerializableField ReadField(ref PacketInStream Stream)
        {
            long FieldData = Stream.ReadEncoded7Bit();
            if (FieldData < 0)
                return null;

            int FieldType;
            int FieldIndex;

            if (!PacketInStream.Decode2Parameters(FieldData, out FieldType, out FieldIndex))
                return null;

            return ReadField(ref Stream, FieldIndex, FieldType);
        }

        public static ISerializableField ReadField(ref PacketInStream Stream, int FieldIndex, int FieldType)
        {
            if (FieldType == (int)EPacketFieldType.Terminator)
                return null;

            ISerializableField Field = GetFieldType((EPacketFieldType)FieldType);
            if (Field == null)
            {
                Log.Error("PacketProcessor", "Unhandled Field : Type = " + FieldType);
                return null;
            }

            Log.Debug("ReadField", "Index = " + FieldIndex + ",Type=" + FieldType);
            Field.Index = FieldIndex;
            Field.PacketType = (EPacketFieldType)FieldType;
            Field.Deserialize(ref Stream);

            return Field;
        }

        #endregion

        #region Writers

        public static bool WriteField(ref PacketOutStream Stream, ISerializableField Field)
        {
            if (Field == null)
                return false;

            return WriteField(ref Stream, Field, Field.Index, (int)Field.PacketType);
        }

        public static bool WriteField(ref PacketOutStream Stream, ISerializableField Field, int FieldIndex, int FieldType)
        {
            if (FieldType == (int)EPacketFieldType.Invalid)
                return false;

            PacketOutStream NewStream = new PacketOutStream();

            long FieldResult;
            PacketOutStream.Encode2Parameters(out FieldResult, FieldType, FieldIndex);
            NewStream.WriteEncoded7Bit(FieldResult);

            if(Field == null || Field.Serialize(ref NewStream,false))
                Stream.Write(NewStream.ToArray());

            return true;
        }

        public static bool WriteField(ref PacketOutStream Stream, EPacketFieldType FieldType, object Value, bool Force)
        {
            ISerializableField Field = GetFieldType(FieldType);
            Log.Debug("WriteField", "Type=" + FieldType + ",Val=" + Value + ",Field="+Field);
            if (Field != null)
            {
                Field.val = Value;
                Field.Serialize(ref Stream,true);
                return true;
            }

            return false;
        }

        public static bool WritePacket(ref PacketOutStream Stream,ISerializablePacket Packet, bool WithSize = true , bool WithTerminator = true, bool WithOpcode = true)
        {
            if (Packet == null)
                return false;

            lock (Packet)
            {
                Packet.ConvertToField();

                PacketOutStream Data = new PacketOutStream();

                if (WithOpcode)
                    Data.WriteEncoded7Bit(Packet.Opcode);

                foreach (ISerializableField Field in Packet.GetFields().Values)
                    WriteField(ref Data, Field);

                if (WithTerminator && Packet.Opcode != (int)Opcodes.ProtocolHandshakeCompression)
                    WriteField(ref Data, null, 0, (int)EPacketFieldType.Terminator);

                if (WithSize)
                    Stream.WriteEncoded7Bit(Data.Length);

                Stream.Write(Data.ToArray());

                Packet.Fields.Clear();
            }

            return true;
        }

        public static PacketOutStream WritePacket(ISerializablePacket Packet)
        {
            PacketOutStream Stream = new PacketOutStream();

            WritePacket(ref Stream, Packet,true,true,true);

            return Stream;
        }

        #endregion
    }
}
