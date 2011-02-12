using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Shared.NetWork
{
    public class CryptKey
    {
        private string _sKey;
        private byte[] _bKey;

        public CryptKey(string Key)
        {
            SetKey(Key);
        }

        public CryptKey(byte[] Key)
        {
            SetKey(Key);
        }

        public void SetKey(string Key)
        {
            _sKey = (string)Key.Clone();
        }

        public void SetKey(byte[] Key)
        {
            _bKey = (byte[])Key.Clone();
        }

        public byte[] GetbKey()
        {
            return (byte[])_bKey.Clone();
        }

        public string GetsKey()
        {
            return (string)_sKey.Clone();
        }
    }

    public interface ICryptHandler
    {
        PacketIn Decrypt(CryptKey Key,byte[] packet);
        byte[] Crypt(CryptKey Key,byte[] packet);

        CryptKey GenerateKey(BaseClient client);
    }
}
