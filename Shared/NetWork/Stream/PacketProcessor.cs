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
        private static Dictionary<long, PacketHandlerDefinition> Definitions;

        public static void RegisterDefinitions()
        {
            Definitions = new Dictionary<long, PacketHandlerDefinition>();

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
                        Definitions.Add(PacketAttribute.GetOpcode(), PacketDef);
                    }
                }
            }
        }

        public static ISerializablePacket ProcessGameDataStream(ref PacketInStream Stream)
        {
            long Opcode = Stream.ReadEncoded7Bit();

            if (Definitions.ContainsKey(Opcode))
            {
                PacketHandlerDefinition Handler = Definitions[Opcode];
                ISerializablePacket Packet = Activator.CreateInstance(Handler.GetClass()) as ISerializablePacket;
                Deserialize(ref Stream, Handler.GetClass(), ref Packet);
                return Packet;
            }
            else
                Log.Error("ProcessGameDataStream","Received unhandled packet with opcode " + Opcode);

            return null;
        }

        public static void Deserialize(ref PacketInStream Stream, Type Class, ref ISerializablePacket Packet)
        {
            FieldInfo[] Fields = Class.GetFields();

            long FieldData;
            while ((FieldData = Stream.ReadEncoded7Bit()) >= 0)
            {
                int FieldType;
                int FieldIndex;

                if (!PacketInStream.Decode2Parameters(FieldData, out FieldType, out FieldIndex))
                    return;

                if (FieldType == (int)EPacketFieldType.Terminator)
                    return;

                FieldInfo Info = GetField(Fields, FieldIndex);

                if (Info == null)
                {
                    Log.Error("Deserialize", "Index not Found on class");
                    return;
                }
                else
                {
                    ISerializableFieldAttribute[] FieldsAttr = Info.GetCustomAttributes(typeof(ISerializableFieldAttribute), true) as ISerializableFieldAttribute[];

                    ISerializableField IField = Activator.CreateInstance(FieldsAttr[0].GetSerializableType()) as ISerializableField;
                    IField.Deserialize(ref Stream,Info.FieldType);

                    if (FieldType == (int)EPacketFieldType.True || (int)FieldType == (int)EPacketFieldType.False)
                        Info.SetValue(Packet, FieldType == (int)EPacketFieldType.True);
                    else
                        Info.SetValue(Packet, IField.value);
                }
            }
        }

        public static FieldInfo GetField(FieldInfo[] Fields, int Index)
        {
            foreach (FieldInfo Field in Fields)
            {
                ISerializableFieldAttribute[] FieldsAttr = Field.GetCustomAttributes(typeof(ISerializableFieldAttribute), true) as ISerializableFieldAttribute[];
                if (FieldsAttr != null && FieldsAttr.Length > 0 && FieldsAttr[0].Index == Index)
                    return Field;
            }

            return null;
        }

        public static void WritePacket(ref PacketOutStream Stream, Type Class, ISerializablePacket Packet,bool WriteSize=true)
        {
            PacketOutStream Temp = new PacketOutStream();
            Temp.WriteEncoded7Bit(Packet.GetOpcode());
            Serialize(ref Temp, Packet.GetType(), Packet);

            if (Packet.GetOpcode() != (long)Opcodes.ProtocolHandshakeCompression)
            {
                long Terminator;
                PacketOutStream.Encode2Parameters(out Terminator, (int)EPacketFieldType.Terminator, 0);
                Temp.WriteEncoded7Bit(Terminator);
            }

            if (WriteSize)
                Stream.WriteEncoded7Bit(Temp.Length);

            Stream.Write(Temp.ToArray());
        }

        public static int Serialize(ref PacketOutStream Stream, Type Class, ISerializablePacket Packet)
        {
            FieldInfo[] Fields = Class.GetFields();
            int MaxIndex = 0;

            foreach (FieldInfo Field in Fields)
            {
                ISerializableFieldAttribute[] FieldsAttr = Field.GetCustomAttributes(typeof(ISerializableFieldAttribute), true) as ISerializableFieldAttribute[];

                if (FieldsAttr != null && FieldsAttr.Length > 0)
                {
                    ISerializableField IField = Activator.CreateInstance(FieldsAttr[0].GetSerializableType()) as ISerializableField;
                    IField.value = Field.GetValue(Packet);

                    long FieldResult;
                    int FieldType = (int)IField.PacketType;
                    int FieldIndex = (int)FieldsAttr[0].Index;

                    if (IField is BoolBitField)
                        FieldType = (bool)IField.value ? (int)EPacketFieldType.True : (int)EPacketFieldType.False;

                    PacketOutStream.Encode2Parameters(out FieldResult, FieldType, FieldIndex);
                    Stream.WriteEncoded7Bit(FieldResult);

                    if( !(IField is BoolBitField))
                        IField.Serialize(ref Stream, Field.FieldType);

                    if (FieldsAttr[0].Index > MaxIndex)
                        MaxIndex = FieldsAttr[0].Index;
                }
            }

            return MaxIndex;
        }
    }
}
