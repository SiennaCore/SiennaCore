using System;

namespace Shared.NetWork
{
    public enum PacketHandlerType
    {
        TCP = 0x01,
        UDP = 0x02
    }

    public delegate void PacketFunction(BaseClient client,PacketIn packet);

    [AttributeUsage(AttributeTargets.Method)]
    public class PacketHandlerAttribute : Attribute
    {
        protected PacketHandlerType m_type;

        // Packet Opcode
        protected int m_opcode;

        // Packet Description
        protected string m_desc;

        // Client State level for handle this packet
        protected int m_statelevel;

        public PacketHandlerAttribute(PacketHandlerType type, int opcode,int statelevel, string desc)
        {
            m_type = type;
            m_opcode = opcode;
            m_desc = desc;
            m_statelevel = statelevel;
        }

        public PacketHandlerType Type
        {
            get
            {
                return m_type;
            }
        }

        public int Opcode
        {
            get
            {
                return m_opcode;
            }
        }

        public int State
        {
            get
            {
                return m_statelevel;
            }
        }

        public string Description
        {
            get
            {
                return m_desc;
            }
        }
    }
}
