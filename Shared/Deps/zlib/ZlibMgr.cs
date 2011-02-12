using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Shared;

namespace Shared.zlib
{
    static public class ZlibMgr
    {
        static public byte[] GetResult(byte[] Input, bool compress)
        {
            return compress ? Compress(Input,zlibConst.Z_BEST_COMPRESSION, zlibConst.Z_NO_FLUSH) : Decompress(Input);
        }

        static public byte[] Compress(byte[] Input,int Compression,int Flush)
        {
            MemoryStream OutPut = new MemoryStream();
            ZOutputStream ZStream = new ZOutputStream(OutPut,Compression);
            ZStream.FlushMode = Flush;

            Process(ZStream,Input);

            return OutPut.ToArray();
        }

        static public byte[] Decompress(byte[] Input)
        {
            MemoryStream OutPut = new MemoryStream();
            ZOutputStream ZStream = new ZOutputStream(OutPut);
            Process(ZStream,Input);

            return OutPut.ToArray();
        }

        static private void Process(ZOutputStream ZStream,byte[] Input)
        {
            try
            {
                ZStream.Write(Input, 0, Input.Length);
                ZStream.Flush();
                ZStream.Close();
            }
            catch (Exception e)
            {
                Log.Error("Zlib", "Process Error : " + e.ToString());
            }
        }
    }
}
