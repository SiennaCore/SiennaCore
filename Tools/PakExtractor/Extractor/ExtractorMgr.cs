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
using System.Xml.Serialization;

using PakExtractor;

static public class ExtractorMgr
{
    #region Paks

    static public Dictionary<string, PakFile> _Paks = new Dictionary<string, PakFile>();

    static public Dictionary<string, PakHeaders> _Headers = new Dictionary<string, PakHeaders>();
    static public void LoadHeaders()
    {
        Extractor.Data.CellClick += new DataGridViewCellEventHandler(CellClick);

        try
        {
            XmlSerializer X = new XmlSerializer(typeof(PakHeaders));

            string[] Urls = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.conf", SearchOption.AllDirectories);
            foreach (string Url in Urls)
            {
                PakHeaders Hrs = X.Deserialize(new FileStream(Url, FileMode.Open)) as PakHeaders;

                FileInfo Info = new FileInfo(Url);
                _Headers.Add(Hrs.FileName, Hrs);
            }

            Extractor.Instance.Tool("Loaded " + _Headers.Count + " Headers");
        }
        catch (Exception e)
        {
            Extractor.Instance.Tool(e.ToString());
        }
    }
    static public void CellClick(object sender, EventArgs a)
    {
        DataGridViewCell Cell = Extractor.Data.CurrentCell;
        string FullName = (Extractor.Box.SelectedItem as FileInfo).FullName;
        PakFile Pak = _Paks[FullName];
        PakElement Element = Pak._Elements.Find(info => info.Id == int.Parse(Cell.OwningRow.Cells[0].Value.ToString()));

        if (Cell.OwningColumn.Name == "Extract")
        {
            if (Element != null)
                AddToExtract(Element);
        }
        else if (Cell.OwningColumn.Name == "Software")
        {
            if (Element != null)
                Element.OpenSoftware();
        }
    }
    static public void SaveHeader(PakHeaders Headers)
    {

        if (_Headers.ContainsKey(Headers.FileName))
            return;

        Extractor.Instance.Tool("Saving Header : " + Headers.FileName);

        Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Headers/");

        FileStream Stream = new FileStream(Directory.GetCurrentDirectory() + "/Headers/" + Headers.FileName + ".conf", FileMode.Create);
        XmlSerializer X = new XmlSerializer(typeof(PakHeaders));
        X.Serialize(Stream, Headers);
    }
    static public PakHeaders GetHeader(string FileName)
    {
        if (_Headers.ContainsKey(FileName))
            return _Headers[FileName];
        else
            return null;
    }

    #endregion

    #region ExtractFile

    static public Thread ExtractThread;
    static public List<PakElement> _ToExtract = new List<PakElement>();

    static public void StartExtractorThread()
    {
        Thread ExtractThread = new Thread(new ThreadStart(Extracting));
        ExtractThread.Start();
    }
    static public bool IsRunning = true;
    static public void Extracting()
    {
        while (IsRunning)
        {
            PakElement[] ToExtract;
            lock (_ToExtract)
            {
                ToExtract = _ToExtract.ToArray();
                _ToExtract.Clear();
            }

            if (ToExtract.Length > 0)
            {
                int Total = ToExtract.Length;
                for (int i = 0; i < Total; ++i)
                {
                    Extractor.Instance.Progress((i * 100) / Total);

                    PakElement Element = ToExtract[i];

                    Extractor.Instance.Tool("Extracting : " + Element.Id + " From " + Element.Owner.FileName);
                    string Folder = ExtractingFolder + "/Extracted/" + Element.Owner.FileName + "/";
                    Directory.CreateDirectory(Folder);

                    Element.GetBytes();
                    FileStream Stream = new FileStream(Folder + "/" + Element.Id + Element.Header.Ext, FileMode.Create);
                    Stream.Write(Element.dat, 0, Element.dat.Length);
                    Stream.Close();
                    Element.dat = null;

                    Thread.Sleep(20);
                }
            }

            Thread.Sleep(50);
        }
    }

    static public string ExtractingFolder = "";
    static public void AddToExtract(PakElement Element)
    {
        if (ExtractingFolder.Length <= 0)
        {
            FolderBrowserDialog Dial = new FolderBrowserDialog();
            Dial.SelectedPath = Directory.GetCurrentDirectory();
            Dial.ShowDialog();
            ExtractingFolder = Dial.SelectedPath;
            if (ExtractorMgr.ExtractingFolder.Length <= 0)
                return;

        }

        if (!Extractor.Instance.IsChecked(Element.Header.Ext))
            return;

        lock (_ToExtract)
            _ToExtract.Add(Element);
    }

    #endregion

    static public PakFile DecodePak(FileInfo Info)
    {
        if (!_Paks.ContainsKey(Info.FullName))
        {
            PakFile Pak = new PakFile(Info);
            _Paks.Add(Info.FullName, Pak);

            Pak.DelistThread = new Thread(new ThreadStart(Pak.DeList));
            Pak.DelistThread.Start();
        }

        return _Paks[Info.FullName];
    }
    static public void PrintPak(FileInfo Info)
    {
        _Paks[Info.FullName].PrintHeaders();
    }
    static public void ExtractPack()
    {
        string FullName = (Extractor.Box.SelectedItem as FileInfo).FullName;
        PakFile Pak = _Paks[FullName];

        foreach (PakElement Element in Pak._Elements)
            if (Extractor.Instance.IsChecked(Element.Header.Ext))
                AddToExtract(Element);
    }
}

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