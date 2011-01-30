using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Remoting.Messaging;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

using System.IO;

using System.Reflection;

namespace Sienna.Network
{
    public class PacketFormatter
    {
        public object Deserialize(Stream serializationStream)
        {
            return null;
        }

        public byte[] Serialize(object graph)
        {
            MemoryStream _Writer = new MemoryStream();

            Type t = graph.GetType();
            FieldInfo[] Fields = t.GetFields();

            foreach (FieldInfo Field in Fields)
            {
                Type FieldType = Field.FieldType;
                String TypeName = FieldType.Name;
                Console.WriteLine(TypeName);
                Object Val = Field.GetValue(graph);

                SerializeType(TypeName, Val, FieldType, _Writer);
            }

            return _Writer.ToArray();
        }

        protected void SerializeType(String TypeName, Object Val, Type FieldType, MemoryStream _Writer)
        {
            if (TypeName == "String" || TypeName == "string")
            {
                UInt32 StringLen = (uint)(Val as String).Length;
                Byte[] String = Encoding.UTF8.GetBytes(Val as String);

                Byte[] LenBytes = BitConverter.GetBytes(StringLen);
                _Writer.Write(LenBytes, 0, LenBytes.Length);

                _Writer.Write(String, 0, String.Length);
            }
            else if (FieldType.IsArray)
            {
                object[] objs = (object[])Val;
                Type ArrayType = null;

                if (objs.Length > 0)
                {
                    ArrayType = objs[0].GetType();

                    foreach (object obj in objs)
                        SerializeType(ArrayType.Name, obj, ArrayType, _Writer);
                }
            }
            else
            {
                if (!FieldType.IsSerializable)
                    return;

                Type bc = typeof(BitConverter);
                MethodInfo mi = bc.GetMethod("GetBytes", new Type[] { FieldType });
                byte[] Data = (byte[])mi.Invoke(null, new Object[] { Val });

                _Writer.Write(Data, 0, Data.Length);
            }
        }
    }
}
