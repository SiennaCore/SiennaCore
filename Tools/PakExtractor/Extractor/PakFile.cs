using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using PakExtractor;

public class PakFile
{
    public Thread DelistThread = null;
    public bool Decoded = false;
    public int Ids = 0;
    public int GetId()
    {
        return System.Threading.Interlocked.Increment(ref Ids);
    }

    public string FileName = "";
    public PakHeaders Headers;

    public ExtendedFileStream Stream;
    public List<PakElement> _Elements = new List<PakElement>();

    public PakFile(FileInfo Info)
    {
        FileName = Info.Name;
        Headers = ExtractorMgr.GetHeader(FileName);
        try
        {
            Stream = new ExtendedFileStream(Info.FullName);
        }
        catch (Exception e)
        {
            Extractor.Instance.Tool(e.ToString());
        }
    }

    public void DeList()
    {
        if (_Elements.Count > 0)
            return;

        Decoded = false;
        Extractor.Instance.Progress(0);

        if (Headers == null)
        {
            Headers = new PakHeaders();
            Headers.FileName = FileName;
            Headers.One = Stream.GetInt();
            Headers.FileSize = Stream.GetInt();
            Headers.Padding = Stream.GetInt();

            Headers.Hsize = Stream.GetInt();

            Headers.HeaderSize = Headers.Hsize / 60;

            for (int i = 0; i < Headers.HeaderSize; i++)
            {
                FileHeader Header = new FileHeader();

                string Unk1 = Stream.GetString(20);

                Header.ZSize = Stream.GetInt();
                Header.Size = Stream.GetInt();
                Header.Unk2 = Stream.GetInt();
                Header.Start = Stream.GetInt();

                string Unk3 = Stream.GetString(24);

                Headers.Files.Add(Header);

                Extractor.Instance.Tool("Reading header :" + i + "/" + Headers.HeaderSize + ": " + Headers.FileName);
                Extractor.Instance.Progress((i * 100) / Headers.HeaderSize);
            }
        }

        int Total = Headers.Files.Count;
        for (int i = 0; i < Total; ++i)
        {
            Extractor.Instance.Progress((i * 100) / Total);
            FileHeader Header = Headers.Files[i];
            PakElement Ep = new PakElement(this, Header, Stream);
            Ep.GetExtention();
            _Elements.Add(Ep);
        }

        ExtractorMgr.SaveHeader(Headers);

        Extractor.Instance.Progress(100);
        Decoded = true;
    }

    public void PrintHeaders()
    {
        Extractor.Instance.ClearData();

        foreach (PakElement Ep in _Elements.ToArray())
        {
            Extractor.Instance.AddData("" + Ep.Id, Ep.GetExtention(), "" + Ep.Header.ZSize, "" + Ep.Header.Size, "Extract" , Ep.Header.Software);
        }
    }
}