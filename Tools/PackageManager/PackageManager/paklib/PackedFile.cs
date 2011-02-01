using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sienna.zlib;

namespace Sienna.PakLib
{
    public class PackedFile
    {
        protected int _FileAddr;
        protected int _ZSize;
        protected int _Size;
        protected int _Idx;

        public string _Unk1;
        public string _Unk3;

        public PackedFile(int Idx, int FileAddress, int ZSize, int Size)
        {
            _Idx = Idx;
            _FileAddr = FileAddress;
            _Size = Size;
            _ZSize = ZSize;
        }

        public bool IsCompressed()
        {
            return _ZSize != _Size;
        }

        public byte[] GetBytes(ExtendedFileStream Efs)
        {
            long EfsPos = Efs.Position;
            Efs.Position = _FileAddr;

            byte[] dat = new byte[_ZSize];
            Efs.Read(dat, 0, (int)_ZSize);

            Efs.Position = EfsPos;

            if (!IsCompressed())
                return dat;

            byte[] extdat = ZlibMgr.Decompress(dat);

            if (extdat.Length != _Size)
                System.Windows.Forms.MessageBox.Show("ERROR WHILE DECOMPRESSING FILE : " + extdat.Length + " " + _Size);

            return extdat;
        }

        public int GetIndex()
        {
            return _Idx;
        }
    }
}
