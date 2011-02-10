using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Threading;

using System.Runtime.InteropServices;

namespace Sienna
{
    public class Log
    {
        public enum LogLevel
        {
            Error,
            Warning,
            Debug,
            Info          
        }

        private static string _Filename;
        private static Mutex _LockFile;
        private static FileStream _LogFile;
        private static LogLevel _FileLogLevel;
        private static LogLevel _ConsoleLogLevel;

        public static void Init(String Filename, LogLevel FileLogLevel, LogLevel ConsoleLogLevel)
        {
            _LockFile = new Mutex();
            _Filename = Filename;
            _LogFile = null;
            _FileLogLevel = FileLogLevel;
            _ConsoleLogLevel = ConsoleLogLevel;
        }

        public static void Info(String Message)
        {
            Console.ForegroundColor = ConsoleColor.White;

            if(_ConsoleLogLevel >= LogLevel.Info)
                Console.WriteLine(Message);

            if (_FileLogLevel >= LogLevel.Info)
                WriteToFile(Message);

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Warning(String Message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            if (_ConsoleLogLevel >= LogLevel.Warning)
                Console.WriteLine(Message);

            if (_FileLogLevel >= LogLevel.Warning)
                WriteToFile("[Warning] " + Message);

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Error(String Message)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            if (_ConsoleLogLevel >= LogLevel.Error)
                Console.WriteLine(Message);

            if (_FileLogLevel >= LogLevel.Error)
                WriteToFile("[Error] " + Message);

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Debug(String Message)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            if (_ConsoleLogLevel >= LogLevel.Debug)
                Console.WriteLine(Message);

            if (_FileLogLevel >= LogLevel.Debug)
                WriteToFile("[Debug] " + Message);

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Dump(String Message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(Message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PacketDump(byte[] Data)
        {
            Dump(BytesToHexString(Data));
            Console.WriteLine();
        }

        private static void WriteToFile(String Message)
        {
            _LockFile.WaitOne();

            _LogFile = new FileStream(_Filename, FileMode.Append);

            byte[] Msg = Encoding.UTF8.GetBytes(Message);
            _LogFile.Write(Msg, 0, Msg.Length);

            _LogFile.WriteByte(BitConverter.GetBytes('\r')[0]);
            _LogFile.WriteByte(BitConverter.GetBytes('\n')[0]);

            _LogFile.Close();

            _LockFile.ReleaseMutex();
        }

        public static string BytesToHexString(byte[] ba)
        {
            int bytesPerLine = 16;

            string hexDump = ba.Select((c, i) => new { Char = c, Chunk = i / bytesPerLine })
                .GroupBy(c => c.Chunk)
                .Select(g => g.Select(c => String.Format("{0:X2} ", c.Char))
                  .Aggregate((s, i) => s + i))
                .Select((s, i) => String.Format("{0:d6}: {1}", i * bytesPerLine, s))
                    .Aggregate("", (s, i) => s + i + Environment.NewLine);

            return hexDump;
        }
    }
}
