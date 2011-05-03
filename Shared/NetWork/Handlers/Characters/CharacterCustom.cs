using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    [Serializable]
    [ISerializableAttribute((long)Opcodes.CharacterCustom)]
    public class CharacterCustom : ISerializablePacket
    {

    }
}
