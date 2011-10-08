using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace RiftShark
{
    public sealed class PacketDescriptions
    {
        public sealed class PacketDescription
        {
            public sealed class PacketField
            {
                public int Index = 0;
                public string Name = "";
            }

            public bool Outbound = false;
            public int Opcode = 0;
            public string Name = "";
            public bool Ignore = false;

            public List<PacketField> Fields = new List<PacketField>();
        }

        public List<PacketDescription> Descriptions = new List<PacketDescription>();

        private static PacketDescriptions sInstance = null;
        internal static PacketDescriptions Instance
        {
            get
            {
                if (sInstance == null)
                {
                    if (!File.Exists(Config.Instance.PacketDescriptions))
                    {
                        sInstance = new PacketDescriptions();
                        sInstance.Save();
                    }
                    else
                    {
                        using (XmlReader xr = XmlReader.Create(Config.Instance.PacketDescriptions))
                        {
                            XmlSerializer xs = new XmlSerializer(typeof(PacketDescriptions));
                            sInstance = xs.Deserialize(xr) as PacketDescriptions;
                        }
                    }
                }
                return sInstance;
            }
        }

        internal void Save()
        {
            XmlWriterSettings xws = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "  ",
                NewLineOnAttributes = true,
                OmitXmlDeclaration = true
            };
            using (XmlWriter xw = XmlWriter.Create(Config.Instance.PacketDescriptions, xws))
            {
                XmlSerializer xs = new XmlSerializer(typeof(PacketDescriptions));
                xs.Serialize(xw, this);
            }
        }
    }
}
