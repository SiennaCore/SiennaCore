using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace WorldServer
{
    [ISerializableAttribute((long)Opcodes.WorldChatMessage)]
    public class WorldChatMessage : ISerializablePacket
    {
        [ArrayBit(0)]
        public string ChannelName;

        [ArrayBit(3)]
        public string Message;

        public override void OnRead(RiftClient From)
        {
            if (Message.StartsWith(".teleport"))
            {
                string CmdArgs = Message.Substring(10);
                string[] pos = CmdArgs.Split(new char[] { ' ' });

                WorldServerPositionUpdate WPos = new WorldServerPositionUpdate();

                WPos.GUID = 123456;

                WPos.Position = new List<float>();
                WPos.Position.Add(float.Parse(pos[0]));
                WPos.Position.Add(float.Parse(pos[1]));
                WPos.Position.Add(float.Parse(pos[2]));

                WPos.Orientation = new List<float>();
                WPos.Orientation.Add(0.0f);
                WPos.Orientation.Add(0.0f);
                WPos.Orientation.Add(0.0f);

                From.SendSerialized(WPos);
            }
        }
    }
}
