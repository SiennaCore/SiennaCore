using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace Sienna.Game
{
    public class Realm
    {
        protected byte[] _UnkData;
        protected byte _Lang;
        protected byte _ID;

        public Realm(byte ID, byte Lang, byte[] Unk)
        {
            _UnkData = Unk;
            _Lang = Lang;
            _ID = ID;
        }

        public byte[] ToLoginData()
        {
            MemoryStream ms = new MemoryStream();

            ms.Write(new byte[] { 0xE5, 0x03, 0x02 }, 0, 3);
            ms.WriteByte(_ID);
            ms.WriteByte(_Lang);
            ms.Write(new byte[] {  0x0A, 0xB6, 0xC3, 0x01 }, 0, 4);
            ms.Write(_UnkData, 0, _UnkData.Length);
            ms.Write(new byte[] { 0x69, 0x07 }, 0, 2);

            return ms.ToArray();
        }

        // Temp
        public static byte[] RealmInfo(byte ID, byte Lang, byte[] Unk)
        {
            Realm r = new Realm(ID, Lang, Unk);
            return r.ToLoginData();
        }
    }
}
