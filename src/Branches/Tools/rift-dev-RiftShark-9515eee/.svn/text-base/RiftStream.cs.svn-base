using OpenSSL.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using zlib;

namespace RiftShark
{
    public sealed class RiftStream
    {
        private enum EProtocolState
        {
            Normal = 0,
            Compressed = 1,
            EncryptedAndCompressed = 2
        }

        private bool mOutbound = false;
        private EProtocolState mProtocolState = EProtocolState.Normal;
        private byte[] mUnprocessedBuffer = new byte[ushort.MaxValue * 16];
        private int mUnprocessedLength = 0;
        private byte[] mProcessedBuffer = new byte[ushort.MaxValue * 16];
        private int mProcessedLength = 0;
        private MemoryStream mInflaterStream = new MemoryStream();
        private ZOutputStream mInflater = null;
        private Cipher mDecryptCipher = null;
        private CipherContext mDecryptCipherContext = null;
        private byte[] mDecryptKey = null;
        private byte[] mDecryptIV = null;
        private int mDecryptIVIndex = 0;
        private byte[] mDecryptBuffer = new byte[ushort.MaxValue * 16];
        private int mDecryptLength = 0;

        public RiftStream(bool pOutbound) { mOutbound = pOutbound; }

        public void EnableInflater()
        {
            mInflater = new ZOutputStream(mInflaterStream);
            mInflater.FlushMode = zlibConst.Z_SYNC_FLUSH;
            mProtocolState = EProtocolState.Compressed;
        }
        public void EnableEncryption(byte[] pSharedSecretKey)
        {
            mDecryptCipher = Cipher.AES_128_OFB;
            mDecryptCipherContext = new CipherContext(mDecryptCipher);
            mDecryptKey = new byte[16];
            mDecryptIV = new byte[16];
            Buffer.BlockCopy(pSharedSecretKey, 0, mDecryptKey, 0, 16);
            Buffer.BlockCopy(pSharedSecretKey, 16, mDecryptIV, 0, 16);
            mProtocolState = EProtocolState.EncryptedAndCompressed;
            AdvanceDecryptIV();
        }

        public void Append(byte[] pBuffer) { Append(pBuffer, 0, pBuffer.Length); }
        public void Append(byte[] pBuffer, int pStart, int pLength)
        {
            while ((mUnprocessedLength + pLength) > mUnprocessedBuffer.Length) Array.Resize(ref mUnprocessedBuffer, mUnprocessedBuffer.Length * 2);
            Buffer.BlockCopy(pBuffer, pStart, mUnprocessedBuffer, mUnprocessedLength, pLength);
            mUnprocessedLength += pLength;
        }

        private byte DecryptByte(byte pEncryptedByte)
        {
            byte decryptedByte = (byte)(pEncryptedByte ^ mDecryptIV[mDecryptIVIndex]);
            ++mDecryptIVIndex;
            if (mDecryptIVIndex == mDecryptIV.Length) AdvanceDecryptIV();
            return decryptedByte;
        }
        private void AdvanceDecryptIV()
        {
            mDecryptIVIndex = 0;
            mDecryptIV = mDecryptCipherContext.Decrypt(new byte[16], mDecryptKey, mDecryptIV);
        }

        public RiftPacket Read(DateTime pTransmitted)
        {
            long packetLength;
            int sizeOfPacketLength;
            switch (mProtocolState)
            {
                case EProtocolState.Normal:
                    {
                        if (ReadEncodedLong(mUnprocessedBuffer, 0, mUnprocessedLength, out packetLength, out sizeOfPacketLength) &&
                            (sizeOfPacketLength + packetLength) <= mUnprocessedLength)
                        {
                            Buffer.BlockCopy(mUnprocessedBuffer, 0, mProcessedBuffer, mProcessedLength, sizeOfPacketLength + (int)packetLength);
                            mUnprocessedLength -= (sizeOfPacketLength + (int)packetLength);
                            mProcessedLength += (sizeOfPacketLength + (int)packetLength);
                            if (mUnprocessedLength > 0) Buffer.BlockCopy(mUnprocessedBuffer, sizeOfPacketLength + (int)packetLength, mUnprocessedBuffer, 0, mUnprocessedLength);
                        }
                        break;
                    }
                case EProtocolState.Compressed:
                    {
                        for (int index = 0; index < mUnprocessedLength; ++index)
                        {
                            if (index >= 3 &&
                                mUnprocessedBuffer[index] == 0xFF &&
                                mUnprocessedBuffer[index - 1] == 0xFF &&
                                mUnprocessedBuffer[index - 2] == 0x00 &&
                                mUnprocessedBuffer[index - 3] == 0x00)
                            {
                                mInflaterStream.SetLength(0);
                                mInflater.Write(mUnprocessedBuffer, 0, index + 1);
                                mInflater.Flush();
                                mInflaterStream.Position = 0;
                                mProcessedLength += mInflaterStream.Read(mProcessedBuffer, mProcessedLength, mProcessedBuffer.Length - mProcessedLength);
                                mUnprocessedLength -= (index + 1);
                                if (mUnprocessedLength > 0) Buffer.BlockCopy(mUnprocessedBuffer, index + 1, mUnprocessedBuffer, 0, mUnprocessedLength);
                                break;
                            }
                        }
                        break;
                    }
                case EProtocolState.EncryptedAndCompressed:
                    {
                        for (int index = 0; index < mUnprocessedLength; ++index)
                        {
                            if (mDecryptLength >= mDecryptBuffer.Length || index >= mUnprocessedBuffer.Length)
                            {
                                System.Diagnostics.Debug.Write("Bad");
                            }
                            mDecryptBuffer[mDecryptLength] = DecryptByte(mUnprocessedBuffer[index]);
                            ++mDecryptLength;
                            if (mDecryptLength >= 4 &&
                                mDecryptBuffer[mDecryptLength - 1] == 0xFF &&
                                mDecryptBuffer[mDecryptLength - 2] == 0xFF &&
                                mDecryptBuffer[mDecryptLength - 3] == 0x00 &&
                                mDecryptBuffer[mDecryptLength - 4] == 0x00)
                            {
                                mInflaterStream.SetLength(0);
                                mInflater.Write(mDecryptBuffer, 0, mDecryptLength);
                                mInflater.Flush();
                                mInflaterStream.Position = 0;
                                mProcessedLength += mInflaterStream.Read(mProcessedBuffer, mProcessedLength, mProcessedBuffer.Length - mProcessedLength);
                                mDecryptLength = 0;
                            }
                        }
                        mUnprocessedLength = 0;
                        break;
                    }
                default: break;
            }

            if (!ReadEncodedLong(mProcessedBuffer, 0, mProcessedLength, out packetLength, out sizeOfPacketLength)) return null;
            if (mProcessedLength < (sizeOfPacketLength + packetLength)) return null;
            byte[] packetBuffer = new byte[packetLength];
            Buffer.BlockCopy(mProcessedBuffer, sizeOfPacketLength, packetBuffer, 0, (int)packetLength);
            mProcessedLength -= (sizeOfPacketLength + (int)packetLength);
            if (mProcessedLength > 0) Buffer.BlockCopy(mProcessedBuffer, sizeOfPacketLength + (int)packetLength, mProcessedBuffer, 0, mProcessedLength);
            //PacketDescriptions.PacketDescription description = PacketDescriptions.Instance.Descriptions.Find(d => d.Outbound == mOutbound && d.Opcode == opcode);
            RiftPacketReader reader = new RiftPacketReader(pTransmitted, mOutbound, packetBuffer, 0, packetBuffer.Length);
            RiftPacket packet;
            int sizeOfPacket;
            if (!reader.ReadPacket(out packet, out sizeOfPacket)) return null;
            packet.Raw = packetBuffer;
            return packet;
        }

        public static bool ReadEncodedLong(byte[] pBuffer, int pStart, int pLength, out long pResult, out int pSizeOfResult)
        {
            pResult = 0;
            pSizeOfResult = 0;
            int bit = 0;
            int offset = 0;
            while (offset < pLength && bit < 70)
            {
                byte current = pBuffer[pStart + offset];
                long maskedAndShifted = (current & 0x7F) << bit;
                bit += 7;
                pResult |= maskedAndShifted;
                ++pSizeOfResult;
                ++offset;
                if ((current & 0x80) == 0) return true;
            }
            return false;
        }
    }
}
