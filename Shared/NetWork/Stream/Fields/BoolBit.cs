using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    public class BoolBitAttribute : ISerializableFieldAttribute
    {
        public BoolBitAttribute(int Index)
            : base(Index)
        {

        }

        public override Type GetSerializableType()
        {
            return typeof(BoolBitField);
        }
    }

    public class BoolBitField : ISerializableField
    {
        public BoolBitField()
        {

        }

        public override void Deserialize(ref PacketInStream Data, Type Field)
        {

        }

        public override void Serialize(ref PacketOutStream Data, Type Field)
        {

        }
    }
}