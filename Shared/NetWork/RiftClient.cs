using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Shared.NetWork;
using Shared.zlib;

namespace Shared.NetWork
{
    public enum RiftState : int
    {
        NOT_CONNECTED = 0,
        CONNECTING = 1,
        AUTHENTIFIED = 2,
        LOADING = 3,
        PLAYING = 4,
    };

    public class RiftClient : BaseClient
    {
        public ZOutputStream ZStream = null;
        public MemoryStream Stream = null;

        public Account Acct = null;
        public Realm Realm = null;

        public RiftClient(TCPManager Server)
            : base(Server)
        {

        }

        public override void OnConnect()
        {
            State = (int)RiftState.CONNECTING;
        }
        public override void OnDisconnect()
        {
            State = (int)RiftState.NOT_CONNECTED;
        }

        public virtual void InitZlib()
        {
            if (ZStream != null)
                return;

            Stream = new MemoryStream();
            ZStream = new ZOutputStream(Stream, zlibConst.Z_NO_COMPRESSION);
            ZStream.FlushMode = zlibConst.Z_SYNC_FLUSH;
        }
        public virtual void RemoveZlib()
        {
            Stream = null;
            ZStream = null;
        }

        protected override void OnReceive(PacketIn Packet)
        {
            long TotalSize = (long)Packet.Length;

            while (TotalSize >= 3)
            {
                Packet.Size = (ulong)Packet.DecodeGamebryoSize();
                Packet.Opcode = Packet.GetUint16R();

                Log.Debug("OnReceive","TotalSize =" + TotalSize + ",PacketSize=" + Packet.Size);

                HandlePacket(Packet);

                Packet.Position = (long)Packet.Size+1;
                TotalSize -= (long)(Packet.Size+1);
            }
        }

        public byte[] Compress(PacketOut Out)
        {
            if (ZStream == null)
                return Out.ToArray();

            Stream.SetLength(0);

            byte[] Packet = Out.ToArray();
            ZStream.Write(Packet, 0, Packet.Length);
            ZStream.Flush();

            return Stream.ToArray();
        }
        public new void SendTCP(PacketOut Out)
        {
            Out.WriteGamebryoSize();
            base.SendTCP(Compress(Out));
        }

        public virtual void HandlePacket(PacketIn Packet)
        {
            Server.HandlePacket(this, Packet);
        }
    }
}
