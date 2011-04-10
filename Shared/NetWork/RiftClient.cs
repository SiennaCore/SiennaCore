using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading;


using Shared.NetWork;
using Shared.zlib;

namespace Shared
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
        public Account Acct = null;
        public Realm Realm = null;

        public RiftClient(TCPManager Server)
            : base(Server)
        {
            Random random = new Random();
            byte[] randomKey = new byte[128];
            random.NextBytes(randomKey);
            Array.Resize(ref randomKey, randomKey.Length + 1);
            mLocalPrivateKey = new BigInteger(randomKey);
            mLocalPublicKey = BigInteger.ModPow(GENERATOR, mLocalPrivateKey, MODULUS);

        }

        public override void OnConnect()
        {
            State = (int)RiftState.CONNECTING;
        }
        public override void OnDisconnect()
        {
            State = (int)RiftState.NOT_CONNECTED;
        }

        public bool Crypted = false;
        private BigInteger mLocalPrivateKey = BigInteger.Zero;
        private BigInteger mLocalPublicKey = BigInteger.Zero;
        private byte[] mKey = new byte[16];

        public byte[] LocalPublicKey
        {
            get
            {
                byte[] localPublicKey = mLocalPublicKey.ToByteArray();
                Array.Resize(ref localPublicKey, 128);
                Array.Reverse(localPublicKey);
                return localPublicKey;
            }
        }

        #region MODULUS && GENERATOR

        private static BigInteger MODULUS = new BigInteger(new byte[]
            {
                0x03, 0x40, 0xAA, 0x7B, 0x49, 0xB3, 0x24, 0xE0, 0x42, 0x61, 0x27, 0x33, 0x65, 0xED, 0xAC, 0xA2,
                0x0F, 0x81, 0xD1, 0x57, 0xFB, 0x3A, 0xE7, 0x86, 0xD2, 0x5B, 0x18, 0x72, 0x14, 0x68, 0xEB, 0xF5,
                0xEE, 0xE8, 0x96, 0x99, 0x2D, 0x58, 0x41, 0x99, 0x52, 0x54, 0xE1, 0x3C, 0x4B, 0x71, 0x7E, 0x59,
                0x6E, 0x3B, 0x7F, 0xE8, 0xAD, 0x0C, 0xE1, 0x81, 0x6F, 0x78, 0x9E, 0xDD, 0xCC, 0x69, 0xDF, 0x89,
                0x89, 0x1C, 0x6F, 0x0B, 0x42, 0x95, 0x74, 0xDE, 0x2F, 0x7F, 0xB2, 0x78, 0x3C, 0x20, 0xEF, 0x55,
                0x5F, 0x94, 0x8F, 0x86, 0xB3, 0x0C, 0x7C, 0x8C, 0x48, 0x68, 0x3D, 0x6C, 0x64, 0x11, 0x74, 0x6E,
                0x66, 0x91, 0x7B, 0xCC, 0xBD, 0x64, 0x71, 0x5B, 0x01, 0xFD, 0x8F, 0xE8, 0x77, 0x07, 0xCF, 0x70,
                0xC0, 0xD2, 0x31, 0xC5, 0xBA, 0xCA, 0xB5, 0x8C, 0x6A, 0x68, 0x99, 0x21, 0x98, 0x19, 0x04, 0xEF, 0x00
            });
        private static BigInteger GENERATOR = new BigInteger(2);

        #endregion

        public void InitCrypto(byte[] pRemotePublicKey)
        {
            byte[] remotePublicKeyCopy = new byte[pRemotePublicKey.Length];
            Buffer.BlockCopy(pRemotePublicKey, 0, remotePublicKeyCopy, 0, pRemotePublicKey.Length);
            Array.Reverse(remotePublicKeyCopy);
            Array.Resize(ref remotePublicKeyCopy, remotePublicKeyCopy.Length + 1);

            BigInteger remotePublicKey = new BigInteger(remotePublicKeyCopy);
            BigInteger finalKey = BigInteger.ModPow(remotePublicKey, mLocalPrivateKey, MODULUS);
            
            byte[] secretKey = finalKey.ToByteArray();
            Array.Resize(ref secretKey, 128);
            Array.Reverse(secretKey);

            Buffer.BlockCopy(secretKey, 0, mKey, 0, 16);
            Buffer.BlockCopy(secretKey, 16, mDecryptIV, 0, 16);
            Buffer.BlockCopy(secretKey, 16, mEncryptIV, 0, 16);

            mDecryptCipher.Init(false, new ParametersWithIV(new KeyParameter(mKey), mDecryptIV));
            mEncryptCipher.Init(false, new ParametersWithIV(new KeyParameter(mKey), mEncryptIV));

            AdvanceDecryptIV();
            AdvanceEncryptIV();
        }

        #region Receiver

        public List<byte> Received = new List<byte>();
        public List<byte> Decrypted = new List<byte>();

        private OfbBlockCipher mDecryptCipher = new OfbBlockCipher(new AesEngine(), 128);
        private byte[] mDecryptIV = new byte[16];
        private int mDecryptIVIndex = 0;

        public bool ReceiveCompress = false;
        public MemoryStream InStream = new MemoryStream();
        public ZOutputStream InCompress;

        public void EnableReceiveCompress()
        {
            Crypted = true;
            ReceiveCompress = true;
            InCompress = new ZOutputStream(InStream);
            InCompress.FlushMode = zlibConst.Z_SYNC_FLUSH;
        }

        protected override void OnReceive(byte[] Packet)
        {
            Log.Success("OnReceive", "Received Bytes =" + Packet.Length);

            Received.AddRange(Packet);

            while (ReadPacket()) ;
        }

        public bool ReadPacket()
        {
            if (Received.Count <= 0)
                return false;

            if (!ReceiveCompress && !Crypted)
            {
                PacketInStream Packet = new PacketInStream(Received.ToArray(), Received.Count);
                long Size = Packet.ReadEncoded7Bit();

                if (Size > Packet.Length)
                    return false;

                Received.RemoveRange(0, (int)(Packet.Position+Size));

                ISerializablePacket Pack = PacketProcessor.ProcessGameDataStream(ref Packet);
                if (Pack != null)
                    Pack.OnRead(this);

                return true;
            }

            if (Crypted)
            {
                byte[] Packet = Received.ToArray();
                Received.Clear();
                Decrypt(Packet);
            }

            if (ReceiveCompress)
            {
               int End = -1;
                while ((End = GetEndCompression(ref Decrypted)) >= 0)
                {
                    byte[] ToUnCompress = new byte[End];
                    Buffer.BlockCopy(Decrypted.ToArray(), 0, ToUnCompress, 0, End);
                    Decrypted.RemoveRange(0, End);

                    byte[] Result = UnCompress(ToUnCompress);

                    PacketInStream Packet = new PacketInStream(Result, Result.Length);
                    long Size = Packet.ReadEncoded7Bit();

                    if (Size > Packet.Length)
                    {
                        Log.Error("ReadPacket", "Size > Packet.Lenght,Size=" + Size + ",Lenght=" + Packet.Length);
                        continue;
                    }

                    ISerializablePacket Pack = PacketProcessor.ProcessGameDataStream(ref Packet);
                    if (Pack != null)
                        Pack.OnRead(this);
                }
            }

            return false;
        }

        private void Decrypt(byte[] Packet)
        {
            int count;
            for (count = 0; count < Packet.Length; ++count)
            {
                Packet[count] = (byte)(Packet[count] ^ mDecryptIV[mDecryptIVIndex++]);
                if (mDecryptIVIndex == mDecryptIV.Length) AdvanceDecryptIV();
            }

            Decrypted.AddRange(Packet);
        }

        private byte[] UnCompress(byte[] Packet)
        {
            InStream.SetLength(0);
            InCompress.Write(Packet, 0, Packet.Length);
            InStream.Position = 0;

            byte[] Result = new byte[InStream.Length];
            InStream.Read(Result, 0, Result.Length);
            return Result;
        }

        private int GetEndCompression(ref List<byte> Waiting)
        {
            for (int i = 3; i < Waiting.Count; ++i)
            {
                if (Waiting[i] == 0xFF
                    && Waiting[i-1] == 0xFF
                    && Waiting[i-2] == 0x00
                    && Waiting[i-3] == 0x00)
                    return i+1;
            }

            return -1;
        }

        private void AdvanceDecryptIV()
        {
            mDecryptCipher.ProcessBlock(new byte[16], 0, mDecryptIV, 0);
            mDecryptCipher.Init(false, new ParametersWithIV(new KeyParameter(mKey), mDecryptIV));
            mDecryptIVIndex = 0;
        }

        #endregion

        #region Sender

        private OfbBlockCipher mEncryptCipher = new OfbBlockCipher(new AesEngine(), 128);
        private byte[] mEncryptIV = new byte[16];
        private int mEncryptIVIndex = 0;

        public bool SendCompressed = false;
        public MemoryStream OutStream = new MemoryStream();
        public ZOutputStream OutCompress;

        public void EnableSendCompress()
        {
            SendCompressed = true;
            OutCompress = new ZOutputStream(OutStream, zlibConst.Z_BEST_SPEED);
            OutCompress.FlushMode = zlibConst.Z_SYNC_FLUSH;
        }

        public void SendSerialized(ISerializablePacket Packet)
        {
            PacketOutStream Out = new PacketOutStream();
            PacketProcessor.WritePacket(ref Out, Packet.GetType(), Packet);

            byte[] ToSend = Out.ToArray();

            Log.Dump("ToSend", ToSend, 0, ToSend.Length);

            if (SendCompressed)
            {
                OutStream.SetLength(0);
                OutCompress.Write(ToSend, 0, ToSend.Length);
                OutCompress.Flush();
                ToSend = OutStream.ToArray();
            }

            if (Crypted)
                ToSend = Crypt(ToSend);

            SendTCP(ToSend);
        }

        private void AdvanceEncryptIV()
        {
            mEncryptCipher.ProcessBlock(new byte[16], 0, mEncryptIV, 0);
            mEncryptCipher.Init(false, new ParametersWithIV(new KeyParameter(mKey), mEncryptIV));
            mEncryptIVIndex = 0;
        }

        private byte[] Crypt(byte[] Packet)
        {
            for (int index = 0; index < Packet.Length; ++index)
            {
                Packet[index] = (byte)(Packet[index] ^ mEncryptIV[mEncryptIVIndex++]);
                if (mEncryptIVIndex == mEncryptIV.Length) 
                    AdvanceEncryptIV();
            }

            return Packet;
        }

        #endregion
    }
}
