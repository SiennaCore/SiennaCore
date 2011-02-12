using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class FileHeader
{
    //public string Unk1 = "";
    public int ZSize = 0;
    public int Size = 0;
    public int Unk2 = 0;
    public int Start = 0;
    //public string Unk3 = "";
    public string Ext = "";
    public string Software = "";
}

[Serializable]
public class PakHeaders
{
    public string FileName = "";
    public int One = 0;
    public int FileSize = 0;
    public int Padding = 0;
    public int Hsize = 0;
    public int HeaderSize = 0;

    public List<FileHeader> Files = new List<FileHeader>();
}