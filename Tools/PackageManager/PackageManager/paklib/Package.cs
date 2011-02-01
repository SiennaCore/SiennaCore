using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using System.Threading;

namespace Sienna.PakLib
{
    public class Package
    {
        private string _File;
        private ExtendedFileStream _Efs;
        private Dictionary<int, PackedFile> _Files = new Dictionary<int,PackedFile>();

        private OnFinishedReading _OfrCb;
        private OnReadingProgress _OrpCb;

        public delegate void OnFinishedReading();
        public delegate void OnReadingProgress(PackedFile pf);
        
        public Package(string file, OnFinishedReading ofr, OnReadingProgress orp)
        {
            _File = file;
            _Efs = new ExtendedFileStream(file);

            _OfrCb = ofr;
            _OrpCb = orp;

            Thread t = new Thread(new ThreadStart(ReadHeaders));
            t.Start();
        }

        protected void ReadHeaders()
        {
            int One = _Efs.GetInt();
            int FileSize = _Efs.GetInt();
            int Padding = _Efs.GetInt();

            int Hsize = _Efs.GetInt();

            int HeaderSize = Hsize / 60;

            for (int i = 0; i < HeaderSize; i++)
            {
                string Unk1 = _Efs.GetString(20);

                int ZSize = _Efs.GetInt();
                int Size = _Efs.GetInt();
                int Unk2 = _Efs.GetInt();
                int Ptr = _Efs.GetInt();

                string Unk3 = _Efs.GetString(24);

                PackedFile pf = new PackedFile(i, Ptr, ZSize, Size);
                pf._Unk1 = Unk1;
                pf._Unk3 = Unk3;
                _Files.Add(i, pf);

                GetFile(i);

                _OrpCb.Invoke(pf);
            }

            _OfrCb.Invoke();
        }

        public List<int> GetFileNames()
        {
            return new List<int>(_Files.Keys.ToArray());
        }

        public byte[] GetFile(int Idx)
        {
            if (_Files.ContainsKey(Idx))
                return _Files[Idx].GetBytes(_Efs);
            else
                return null;
        }

        public void Close()
        {
            _Efs.Close();
        }
    }
}
