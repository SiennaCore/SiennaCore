using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    [Serializable]
    public class ISerializablePacket
    {
        private Dictionary<int, ISerializableField> Fields = new Dictionary<int, ISerializableField>();
        public Dictionary<int, ISerializableField> GetFields()
        {
            return Fields;
        }

        public void AddField(int Index, ISerializableField Field)
        {
            if (!Fields.ContainsKey(Index))
                Fields.Add(Index, Field);
        }
        public void AddField(int Index, EPacketFieldType Type, object Value)
        {
            ISerializableField Field = PacketProcessor.GetFieldType(Type);
            if (Field != null)
            {
                Field.value = Value;
                AddField(Index, Field);
            }
        }

        public ISerializableField GetField(int Index)
        {
            ISerializableField Field;
            Fields.TryGetValue(Index, out Field);
            return Field;
        }

        public virtual void OnRead(RiftClient From)
        {
            Log.Error("OnRead", "Unknown ISerialized packet : " + GetType().Name);
        }

        public long GetOpcode()
        {
            ISerializableAttribute[] Atrib = GetType().GetCustomAttributes(typeof(ISerializableAttribute), true) as ISerializableAttribute[];

            if (Atrib != null && Atrib.Length > 0)
                return Atrib[0].GetOpcode();
            else
                return 0;
        }
    }
}
