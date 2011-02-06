using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;

namespace Sienna.zlib
{
    static public class ZlibMgr
    {
        static public byte[] GetResult(byte[] Input, bool compress)
        {
            return compress ? Compress(Input) : Decompress(Input);
        }

        static public byte[] Compress(byte[] Input)
        {
            MemoryStream OutPut = new MemoryStream();
            ZOutputStream ZStream = new ZOutputStream(OutPut,zlibConst.Z_DEFAULT_COMPRESSION);
            ZStream.FlushMode = zlibConst.Z_SYNC_FLUSH;

            Process(ZStream,Input);

            return OutPut.ToArray();
        }

        static public byte[] Decompress(byte[] Input)
        {
            MemoryStream OutPut = new MemoryStream();
            ZOutputStream ZStream = new ZOutputStream(OutPut);
            ZStream.FlushMode = zlibConst.Z_SYNC_FLUSH;
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
                Log.Error(e.Message + " " + e.Source + " " + e.StackTrace);
            }
        }
    }
}
