using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Shared
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
                    if (Class.BaseType == typeof(ISerializablePacket) && Class.GetCustomAttributes(typeof(ISerializableAttribute), true) != null)
                    {
                        ISerializableAttribute PacketAttribute = Class.GetCustomAttributes(typeof(ISerializableAttribute), true)[0] as ISerializableAttribute;
                        PacketHandlerDefinition PacketDef = new PacketHandlerDefinition(PacketAttribute.GetOpcode(), Class);

                        Log.Success("RegisterDefinitions", "Registering Handler : " + PacketAttribute.GetOpcode().ToString("X8"));
                        if(!Definitions.ContainsKey(PacketAttribute.GetOpcode()))
                            Definitions.Add(PacketAttribute.GetOpcode(), PacketDef);
                        else
                            Log.Error("RegisterDefinitions","Handler already registered : " + PacketAttribute.GetOpcode().ToString("X8"));
                    }
                }
            }

            FieldsTypes.Add(EPacketFieldType.ByteArray, new ArrayBitField());
            FieldsTypes.Add(EPacketFieldType.True, new BoolBitField());
            FieldsTypes.Add(EPacketFieldType.False, new BoolBitField());
            FieldsTypes.Add(EPacketFieldType.List, new ListBitField());
            FieldsTypes.Add(EPacketFieldType.Packet, new PacketBitField());
            FieldsTypes.Add(EPacketFieldType.Raw4Bytes, new Raw4BitField());
            FieldsTypes.Add(EPacketFieldType.Raw8Bytes, new Raw8BitBitField());
            FieldsTypes.Add(EPacketFieldType.Signed7BitEncoded, new Encoded7BitField());
            FieldsTypes.Add(EPacketFieldType.Unsigned7BitEncoded, new Unsigned7BitField());
            FieldsTypes.Add(EPacketFieldType.Dictionary, new DicBitField());
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
            if(Field != null)
                return Activator.CreateInstance(Field.GetType()) as ISerializableField;

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

            long FieldResult;
            PacketOutStream.Encode2Parameters(out FieldResult, FieldType, FieldIndex);
            Stream.WriteEncoded7Bit(FieldResult);

            if(Field != null)
                Field.Serialize(ref Stream);

            return true;
        }

        public static bool WriteField(ref PacketOutStream Stream, EPacketFieldType FieldType, object Value)
        {
            ISerializableField Field = GetFieldType(FieldType);
            if (Field != null)
            {
                Field.val = Value;
                Field.Serialize(ref Stream);
                return true;
            }

            return false;
        }

        public static bool WritePacket(ref PacketOutStream Stream,ISerializablePacket Packet, bool WithSize = true , bool WithTerminator = true, bool WithOpcode = true)
        {
            if (Packet == null)
                return false;

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
