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
        public ISerializablePacket()
        {
            ISerializableAttribute[] Attr = GetType().GetCustomAttributes(typeof(ISerializableAttribute), true) as ISerializableAttribute[];
            if (Attr == null || Attr.Length <= 0)
                return;

            Opcode = Attr[0].GetOpcode();
        }

        public virtual void OnRead(RiftClient From)
        {
            Log.Error("OnRead", "Unknown ISerialized packet : " + GetType().Name);
        }

        public long Opcode = 0;
        public long GetOpcode()
        {
            return Opcode;
        }

        #region AttribuedFieldInfo

        public void ApplyToFieldInfo()
        {
            FieldInfo[] Fields = GetType().GetFields();
            foreach (ISerializableField Field in GetFields().Values)
            {
                FieldInfo Info = GetFieldInfo(Fields, Field.Index);
                if (Info != null)
                {
                    Log.Success("ApplyToFieldInfo", "" + Info.Name + ", Index=" + Field.Index);
                    Field.ApplyToFieldInfo(Info, this, Info.FieldType);
                }
                else
                    Log.Error("ApplyToFieldInfo", GetType().Name + ", Invalid Index : " + Field.Index);
            }
        }

        private FieldInfo GetFieldInfo(FieldInfo[] Fields, int Index)
        {
            foreach (FieldInfo Info in Fields)
            {
                ISerializableFieldAttribute[] FieldsAttr = Info.GetCustomAttributes(typeof(ISerializableFieldAttribute), true) as ISerializableFieldAttribute[];
                if (FieldsAttr != null && FieldsAttr.Length > 0 && FieldsAttr[0].Index == Index)
                    return Info;
            }

            return null;
        }

        public void ConvertToField()
        {
            FieldInfo[] Fields = GetType().GetFields();
            foreach (FieldInfo Info in Fields)
            {
                ISerializableFieldAttribute[] FieldsAttr = Info.GetCustomAttributes(typeof(ISerializableFieldAttribute), true) as ISerializableFieldAttribute[];
                if (FieldsAttr != null && FieldsAttr.Length > 0)
                {
                    ISerializableField Field = Activator.CreateInstance(FieldsAttr[0].GetSerializableType()) as ISerializableField;
                    Field.Index = FieldsAttr[0].Index;
                    Field.val = Info.GetValue(this);
                    Field.PacketType = PacketProcessor.GetFieldType(Field);
                    AddField(Field.Index, Field);
                }
            }
        }

        #endregion

        #region Fields

        private SortedDictionary<int, ISerializableField> Fields = new SortedDictionary<int, ISerializableField>();

        public SortedDictionary<int, ISerializableField> GetFields()
        {
            return Fields;
        }
        
        public ISerializableField GetField(int Index)
        {
            ISerializableField Field;
            Fields.TryGetValue(Index, out Field);
            return Field;
        }

        public void AddField(int Index, ISerializableField Field)
        {
            if (!Fields.ContainsKey(Index))
                Fields.Add(Index, Field);
            else
            {
                Log.Error("ISerializablePacket", "Duplicate Field Index : " + GetType().Name);

                Fields.Remove(Index);
                AddField(Index, Field);
            }

        }

        public void AddField(int Index, EPacketFieldType Type, object Value)
        {
            ISerializableField Field = PacketProcessor.GetFieldType(Type);
            if (Field != null)
            {
                Field.Index = Index;
                Field.val = Value;
                Field.PacketType = Type;
                AddField(Index, Field);
            }
        }

        #endregion
    }
}
