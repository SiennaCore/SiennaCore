namespace RiftShark
{
    public enum ERiftPacketFieldType
    {
        False = 0,
        True = 1,
        Unsigned7BitEncoded = 2,
        Signed7BitEncoded = 3,
        Raw4Bytes = 4,
        Raw8Bytes = 5,
        ByteArray = 6,
        Invalid = 7,
        Terminator = 8,
        Entry = 9,
        Packet = 10,
        List = 11,
        Dictionary = 12
    }
}
