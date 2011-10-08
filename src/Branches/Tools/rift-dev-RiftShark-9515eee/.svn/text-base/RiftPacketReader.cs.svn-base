using System;
using System.Collections.Generic;

namespace RiftShark
{
    public sealed class RiftPacketReader
    {
        private DateTime mTimestamp = DateTime.MinValue;
        private bool mOutbound = false;
        private byte[] mBuffer = null;
        private int mLength = 0;
        private int mCursor = 0;

        public RiftPacketReader(DateTime pTimestamp, bool pOutbound, byte[] pBuffer, int pStart, int pLength)
        {
            mTimestamp = pTimestamp;
            mOutbound = pOutbound;
            mBuffer = new byte[pLength];
            Buffer.BlockCopy(pBuffer, pStart, mBuffer, 0, pLength);
            mLength = pLength;
        }

        public bool ReadPacket(out RiftPacket pPacket, out int pSizeOfPacket)
        {
            long opcode;
            int sizeOfOpcode;
            pPacket = null;
            pSizeOfPacket = 0;
            if (!Decode7BitValue(out opcode, out sizeOfOpcode, mBuffer, mCursor, mLength)) return false;
            pSizeOfPacket += sizeOfOpcode;
            mCursor += sizeOfOpcode;
            mLength -= sizeOfOpcode;
            pPacket = new RiftPacket(mTimestamp, mOutbound, (int)opcode);
            long fieldData;
            int sizeOfFieldData;
            while (Decode7BitValue(out fieldData, out sizeOfFieldData, mBuffer, mCursor, mLength))
            {
                pSizeOfPacket += sizeOfFieldData;
                mCursor += sizeOfFieldData;
                mLength -= sizeOfFieldData;
                int fieldType;
                int fieldIndex;
                if (!Decode2Parameters(fieldData, out fieldType, out fieldIndex)) return false;
                RiftPacketField field = new RiftPacketField((ERiftPacketFieldType)fieldType, fieldIndex);
                int sizeOfFieldValue;
                if (!ReadFieldValue((ERiftPacketFieldType)fieldType, out field.Value, out sizeOfFieldValue)) return false;
                pSizeOfPacket += sizeOfFieldValue;
                if (field.Type == ERiftPacketFieldType.Terminator) break;
                pPacket.Fields.Add(field);
            }
            return true;
        }
        public static bool Decode7BitValue(out long pValue, out int pSizeOfValue, byte[] pBuffer, int pStart, int pLength)
        {
            pValue = 0;
            pSizeOfValue = 0;
            bool carry = false;
            do
            {
                if (pSizeOfValue >= pLength) return false;
                byte bits = pBuffer[pStart + pSizeOfValue];
                carry = (bits & 0x80) != 0;
                pValue |= ((long)(bits & 0x7F) << (pSizeOfValue * 7));
                ++pSizeOfValue;
            } while (carry);
            return true;
        }
        public static bool Decode2Parameters(long pValue, out int pParameter1, out int pParameter2)
        {
            pParameter1 = (int)(pValue & 0x07);
            pParameter2 = (int)(pValue >> 3);
            if (pParameter1 < 0x07) return true;
            pParameter1 = pParameter2 & 0x07;
            pParameter2 = (int)(pValue >> 6);
            if (pParameter1 < 0x07)
            {
                pParameter1 += 0x08;
                return true;
            }
            return false;
        }
        public static bool Decode3Parameters(long pValue, out int pParameter1, out int pParameter2, out int pParameter3)
        {
            pParameter1 = 0;
            pParameter2 = 0;
            pParameter3 = 0;
            return Decode2Parameters(pValue, out pParameter1, out pParameter2) &&
                   Decode2Parameters(pParameter2, out pParameter2, out pParameter3);
        }
        private bool ReadFieldValue(ERiftPacketFieldType pFieldType, out RiftPacketFieldValue pFieldValue, out int pSizeOfFieldValue)
        {
            pFieldValue = null;
            pSizeOfFieldValue = 0;
            switch (pFieldType)
            {
                case ERiftPacketFieldType.False:
                case ERiftPacketFieldType.True:
                case ERiftPacketFieldType.Invalid:
                case ERiftPacketFieldType.Terminator:
                    break;
                case ERiftPacketFieldType.Unsigned7BitEncoded:
                case ERiftPacketFieldType.Signed7BitEncoded:
                    {
                        long value;
                        int sizeOfValue;
                        if (!Decode7BitValue(out value, out sizeOfValue, mBuffer, mCursor, mLength)) return false;
                        pSizeOfFieldValue += sizeOfValue;
                        mCursor += sizeOfValue;
                        mLength -= sizeOfValue;
                        pFieldValue = new RiftPacketFieldValue(pFieldType, value);
                        break;
                    }
                case ERiftPacketFieldType.Raw4Bytes:
                    {
                        if (mLength < 4) return false;
                        byte[] value = new byte[4];
                        Buffer.BlockCopy(mBuffer, mCursor, value, 0, 4);
                        pSizeOfFieldValue += 4;
                        mCursor += 4;
                        mLength -= 4;
                        pFieldValue = new RiftPacketFieldValue(pFieldType, value);
                        break;
                    }
                case ERiftPacketFieldType.Raw8Bytes:
                    {
                        if (mLength < 8) return false;
                        byte[] value = new byte[8];
                        Buffer.BlockCopy(mBuffer, mCursor, value, 0, 8);
                        pSizeOfFieldValue += 8;
                        mCursor += 8;
                        mLength -= 8;
                        pFieldValue = new RiftPacketFieldValue(pFieldType, value);
                        break;
                    }
                case ERiftPacketFieldType.ByteArray:
                    {
                        long length;
                        int sizeOfLength;
                        if (!Decode7BitValue(out length, out sizeOfLength, mBuffer, mCursor, mLength)) return false;
                        pSizeOfFieldValue += sizeOfLength;
                        mCursor += sizeOfLength;
                        mLength -= sizeOfLength;
                        if (mLength < length) return false;
                        byte[] value = new byte[length];
                        Buffer.BlockCopy(mBuffer, mCursor, value, 0, (int)length);
                        pSizeOfFieldValue += (int)length;
                        mCursor += (int)length;
                        mLength -= (int)length;
                        pFieldValue = new RiftPacketFieldValue(pFieldType, value);
                        break;
                    }
                case ERiftPacketFieldType.Packet:
                    {
                        RiftPacket value;
                        int sizeOfValue;
                        if (!ReadPacket(out value, out sizeOfValue)) return false;
                        pSizeOfFieldValue += sizeOfValue;
                        pFieldValue = new RiftPacketFieldValue(pFieldType, value);
                        break;
                    }
                case ERiftPacketFieldType.List:
                    {
                        long listData;
                        int sizeOfListData;
                        if (!Decode7BitValue(out listData, out sizeOfListData, mBuffer, mCursor, mLength)) return false;
                        pSizeOfFieldValue += sizeOfListData;
                        mCursor += sizeOfListData;
                        mLength -= sizeOfListData;
                        int listType;
                        int listCount;
                        if (!Decode2Parameters(listData, out listType, out listCount)) return false;
                        RiftPacketFieldValueList value = new RiftPacketFieldValueList((ERiftPacketFieldType)listType);
                        for (int listIndex = 0; listIndex < listCount; ++listIndex)
                        {
                            RiftPacketFieldValue listValue;
                            int sizeOfListValue;
                            if (!ReadFieldValue((ERiftPacketFieldType)listType, out listValue, out sizeOfListValue)) return false;
                            pSizeOfFieldValue += sizeOfListValue;
                            if (listValue != null) value.Add(listValue);
                        }
                        pFieldValue = new RiftPacketFieldValue(pFieldType, value);
                        break;
                    }
                case ERiftPacketFieldType.Dictionary:
                    {
                        long dictionaryData;
                        int sizeOfDictionaryData;
                        if (!Decode7BitValue(out dictionaryData, out sizeOfDictionaryData, mBuffer, mCursor, mLength)) return false;
                        pSizeOfFieldValue += sizeOfDictionaryData;
                        mCursor += sizeOfDictionaryData;
                        mLength -= sizeOfDictionaryData;
                        int dictionaryKeyType;
                        int dictionaryValueType;
                        int dictionaryCount;
                        if (!Decode3Parameters(dictionaryData, out dictionaryKeyType, out dictionaryValueType, out dictionaryCount)) return false;
                        RiftPacketFieldValueDictionary value = new RiftPacketFieldValueDictionary((ERiftPacketFieldType)dictionaryKeyType, (ERiftPacketFieldType)dictionaryValueType);
                        for (int dictionaryIndex = 0; dictionaryIndex < dictionaryCount; ++dictionaryIndex)
                        {
                            RiftPacketFieldValue dictionaryKeyValue = null;
                            if (Enum.IsDefined(typeof(ERiftPacketFieldType), dictionaryKeyType))
                            {
                                int sizeOfDictionaryKeyValue;
                                if (!ReadFieldValue((ERiftPacketFieldType)dictionaryKeyType, out dictionaryKeyValue, out sizeOfDictionaryKeyValue)) return false;
                                pSizeOfFieldValue += sizeOfDictionaryKeyValue;
                            }
                            RiftPacketFieldValue dictionaryValueValue = null;
                            if (Enum.IsDefined(typeof(ERiftPacketFieldType), dictionaryValueType))
                            {
                                int sizeOfDictionaryValueValue;
                                if (!ReadFieldValue((ERiftPacketFieldType)dictionaryValueType, out dictionaryValueValue, out sizeOfDictionaryValueValue)) return false;
                                pSizeOfFieldValue += sizeOfDictionaryValueValue;
                            }
                            if (dictionaryKeyValue != null || dictionaryValueValue != null) value.Add(new KeyValuePair<RiftPacketFieldValue, RiftPacketFieldValue>(dictionaryKeyValue, dictionaryValueValue));
                        }
                        pFieldValue = new RiftPacketFieldValue(pFieldType, value);
                        break;
                    }
                default: break;
            }
            return true;
        }
    }
}
