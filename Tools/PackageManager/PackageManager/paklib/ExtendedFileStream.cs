using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace Sienna.PakLib
{
    public class ExtendedFileStream : FileStream
    {
        public ExtendedFileStream(string Filename) : base(Filename, FileMode.Open) { }

        public short GetShort()
        {
            int typesize = sizeof(short);

            byte[] dat = new byte[typesize];
            Read(dat, 0, typesize);

            return BitConverter.ToInt16(dat, 0);
        }

        public int GetInt()
        {
            int typesize = sizeof(int);

            byte[] dat = new byte[typesize];
            Read(dat, 0, typesize);

            return BitConverter.ToInt32(dat, 0);
        }

        public long GetLong()
        {
            int typesize = sizeof(long);

            byte[] dat = new byte[typesize];
            Read(dat, 0, typesize);

            return BitConverter.ToInt64(dat, 0);
        }

        public string GetString(int size)
        {
            byte[] dat = new byte[size];
            Read(dat, 0, size);

            return Encoding.UTF8.GetString(dat);
        }
    }
}
