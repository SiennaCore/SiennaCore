using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

namespace RiftShark
{
    public class RiftGrabber
    {
        public static uint MainPointer = 0;

        /*

         * In order to find new MainPointer, Find all reference to DefWindowProcW, should be only one, follow it
         * look on the beginning of the function, should be something like MOV ECX, DWORD PTR DS:[ADR]
         * MainPointer = ADR - BaseAddress
		 
        */

        #region DllImports & Flags

        [Flags]
        enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory
        (
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            UInt32 nSize,
            ref UInt32 lpNumberOfBytesRead
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        #endregion


        public delegate void KeyEventHandler(bool pIsLobbyServer, byte[] pKey);
        public delegate void TerminatedEventHandler();

        public static event KeyEventHandler OnKey;
        public static event TerminatedEventHandler OnTerminated;

        private static bool Terminated = false;
        private static Thread KillerThread = null;
        private static List<Thread> GrabberThreads = new List<Thread>();

        public static void Start()
        {
            Thread threadStart = new Thread(new ThreadStart(InternalStart));
            threadStart.Start();
        }
        public static void Stop()
        {
            Terminated = true;
            if (KillerThread != null) KillerThread.Join();
            KillerThread = null;
            while (GrabberThreads.Count > 0)
            {
                GrabberThreads[0].Abort();
                GrabberThreads[0].Join();
                GrabberThreads.RemoveAt(0);
            }
            Thread.Sleep(1000);
            if (OnTerminated != null) OnTerminated();
        }
        private static void InternalStart()
        {
            var rifts = WaitForApp("rift");
            if (rifts.Length > 0)
            {
                KillerThread = new Thread(new ThreadStart(KillerThreadWorker));
                KillerThread.Start();
                rifts[0].EnableRaisingEvents = true;
                rifts[0].Exited += new EventHandler(Grabber_Exited);
                GrabPrivateKey(rifts[0]);
            }
        }

        private static void Grabber_Exited(object sender, EventArgs e)
        {
            Stop();
        }

        private static void KillerThreadWorker()
        {
            try
            {
                while (!Terminated)
                {
                    var s = WaitForApp("rifterrorhandler");
                    if (s.Length > 0)
                    {
                        s[0].Kill();
                        s[0].WaitForExit();
                        Debug.WriteLine("Killed RiftErrorHandler!");
                    }
                }
            }
            catch (Exception) { }
        }

        public static uint FindMainAddress(string path)
        {
            var pe = new PEFinder(path, "user32.dll", "DefWindowProcW");
            if (pe.SearchedAddress == 0)
                return 0;

            var neededBytes = BitConverter.GetBytes(pe.SearchedAddress);
            var currentPosition = -1;
            var fileBytes = File.ReadAllBytes(path);
            for (int i = 0; i < fileBytes.Length; i++)
            {
                if (CompareBytes(fileBytes, i, neededBytes, 0, 4))
                {
                    currentPosition = i;
                    currentPosition -= 0x2D;
                    Array.Copy(fileBytes, currentPosition, neededBytes, 0, 4);
                    return BitConverter.ToUInt32(neededBytes, 0) - pe.OptionalHeader32.ImageBase;
                }
            }

            return 0;
        }

        private static void GrabPrivateKey(Process proccess)
        {
            Thread.Sleep(1);
            proccess.WaitForInputIdle();

            IntPtr handle = OpenProcess((uint)(ProcessAccessFlags.VMRead | ProcessAccessFlags.QueryInformation), true, (uint)proccess.Id);

            var BaseAddress = (uint)proccess.MainModule.BaseAddress.ToInt32();

            uint mainAdr = 0;
            try
            {
                mainAdr = FindMainAddress(proccess.MainModule.FileName);
            }
            catch { }
            finally
            {
                if (mainAdr != 0)
                {
                    MainPointer = mainAdr;
                    Debug.WriteLine(string.Format("Found MainAddress! {0:X}", MainPointer));
                }
                else
                {
                    Debug.WriteLine("Didn't find MainAddress :(, shutting down...");
                }
            }

            byte[] buffer = new byte[0x1000];
            List<int> offsets = new List<int>();
            while (!Terminated)
            {
                Read(handle, GetPointer(handle, BaseAddress + MainPointer, false, 0), buffer);
                for (int i = 0; i < buffer.Length / 4; i++)
                {
                    var dh = BitConverter.ToUInt32(buffer, i * 4);
                    if (CheckPG(handle, dh))
                    {
                        if (!offsets.Contains(i))
                        {
                            if (GetPrivateKey(handle, dh) != null)
                            {
                                GrabberWorkerThreadArgs args = new GrabberWorkerThreadArgs(handle, dh, (byte)offsets.Count);
                                Thread grabberThread = new Thread(new ParameterizedThreadStart(GrabberThreadWorker));
                                GrabberThreads.Add(grabberThread);
                                grabberThread.Start(args);
                                offsets.Add(i);
                            }
                        }
                    }
                }
                Thread.Sleep(100);
            }
            Terminated = false;
        }

        private class GrabberWorkerThreadArgs
        {
            public IntPtr handle;
            public uint DHAddr;
            public byte type;
            public GrabberWorkerThreadArgs(IntPtr pHandle, uint pDHAddr, byte pType) { handle = pHandle; DHAddr = pDHAddr; type = pType; }
        }
        private static void GrabberThreadWorker(object pArgs)
        {
            GrabberWorkerThreadArgs args = pArgs as GrabberWorkerThreadArgs;
            try
            {
                Thread.Sleep(10);
                byte[] currentPrivateKey = null;
                while (!Terminated)
                {
                    var newPrivateKey = GetPrivateKey(args.handle, args.DHAddr);
                    if (newPrivateKey == null)
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    bool changed = true;
                    if (currentPrivateKey != null)
                    {
                        changed = !CompareBytes(currentPrivateKey, newPrivateKey, 128);
                    }

                    if (changed)
                    {
                        Thread.Sleep(10);
                        newPrivateKey = GetPrivateKey(args.handle, args.DHAddr);
                        byte[] key = new byte[newPrivateKey.Length];
                        Buffer.BlockCopy(newPrivateKey, 0, key, 0, newPrivateKey.Length);
                        if (OnKey != null) OnKey(args.type == 0 ? true : false, key);

                        Debug.WriteLine(string.Format("{0}PrivateKey:{1}....", args.type == 0 ? "[LoginServer]" : args.type == 1 ? "[GameServer]" : "[Unkown]", BitConverter.ToString(newPrivateKey).Replace('-', ' ').Substring(0, 3 * 8)));
                        currentPrivateKey = newPrivateKey;
                    }
                    Thread.Sleep(1);
                }
            }
            catch (Exception) { }
        }

        public static uint GetDHStruct(IntPtr handle, uint BaseAddress, uint DHOffset)
        {
            return GetPointer(handle, BaseAddress + MainPointer, DHOffset);
        }

        public static byte[] GetPrivateKey(IntPtr handle, uint DHStructAdr)
        {
            uint privateKeyAdr = GetPointer(handle, DHStructAdr + 24, 0);
            if (privateKeyAdr == 0)
                return null;
            return Read(handle, privateKeyAdr, 128);
        }

        public static bool CheckPG(IntPtr handle, uint DHStructAdr)
        {
            byte[] pBigNum = Read(handle, GetPointer(handle, DHStructAdr + 8, 0), 4);
            byte[] gBigNum = Read(handle, GetPointer(handle, DHStructAdr + 12, 0), 4);
            if (!CheckPG(pBigNum, gBigNum))
                return false;
            return true;
        }

        public static bool CheckPG(byte[] pBytes, byte[] gBytes)
        {
            byte[] GCheck = new byte[] { 0x02, 0x00, 0x00, 0x00 };
            byte[] PCheck = new byte[] { 0x03, 0x40, 0xAA, 0x7B };

            if (!CompareBytes(GCheck, gBytes, 4))
            {
                //Debug.WriteLine("Bad BigNum g!");
                return false;
            }
            if (!CompareBytes(PCheck, pBytes, 4))
            {
                //Debug.WriteLine("Bad BigNum p!");
                return false;
            }
            return true;
        }


        public static byte[] Read(IntPtr handle, uint address, UInt32 size)
        {
            return Read(handle, address, new byte[size]);
        }

        public static byte[] Read(IntPtr handle, uint address, byte[] buffer)
        {
            try
            {
                uint bytesRead = 0;
                ReadProcessMemory(handle, new IntPtr(address), buffer, (uint)buffer.Length, ref bytesRead);
                return buffer;
            }
            catch
            {
                return buffer;
            }
        }

        public static uint GetPointer(IntPtr handle, uint Address, params uint[] offsets)
        {
            return GetPointer(handle, Address, true, offsets);
        }

        public static uint GetPointer(IntPtr handle, uint Address, bool GetLastPointer, params uint[] offsets)
        {
            uint address = 0;
            var bytes = Read(handle, Address, 4);

            address = BitConverter.ToUInt32(bytes, 0);
            for (int i = 0; i < offsets.Length; i++)
            {
                if (!GetLastPointer && i == offsets.Length - 1)
                    return address;
                bytes = Read(handle, address + offsets[i], 4);
                address = BitConverter.ToUInt32(bytes, 0);
            }
            return address;
        }

        public static bool CompareBytes(byte[] arr, byte[] arr2, int len)
        {
            if (arr == null || arr2 == null)
                return false;
            for (int i = 0; i < len; i++)
            {
                if (arr[i] != arr2[i])
                    return false;
            }
            return true;
        }

        public static bool CompareBytes(byte[] arr, int pos, byte[] arr2, int pos2, int len)
        {
            for (int i = 0; i < len; i++)
            {
                if (arr[i + pos] != arr2[i + pos2])
                    return false;
            }
            return true;
        }

        public static bool IsArrayNull(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != 0)
                    return false;
            }
            return true;
        }


        private static Process[] WaitForApp(string processName, int TimeOut = -1)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var x = GetProcessesByName(processName);

            while (!Terminated && x.Length == 0 && (timer.ElapsedMilliseconds < TimeOut || TimeOut == -1))
            {
                try
                {
                    Thread.Sleep(1);
                    x = GetProcessesByName(processName);
                }
                catch
                { }
            }
            return x;
        }

        public static Process[] GetProcessesByName(string name)
        {
            Process[] processes = Process.GetProcesses();
            List<Process> foundPros = new List<Process>();
            foreach (Process p in processes)
            {
                try
                {
                    if (p.ProcessName.ToLower() == name.ToLower())
                        foundPros.Add(p);
                }
                catch { }
            }
            return foundPros.ToArray();
        }

    }
}
