using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace RiftShark
{
    public class PEFinder
    {
        public uint SearchedAddress = 0;
        public IMAGE_SECTION_HEADER[] ImageSectionHeaders;

        #region PE Structures

        [StructLayout(LayoutKind.Explicit)]
        public struct Misc
        {
            [FieldOffset(0)]
            public System.UInt32 PhysicalAddress;
            [FieldOffset(0)]
            public System.UInt32 VirtualSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_SECTION_HEADER
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
            public Misc Misc;
            public UInt32 VirtualAddress;
            public UInt32 SizeOfRawData;
            public UInt32 PointerToRawData;
            public UInt32 PointerToRelocations;
            public UInt32 PointerToLinenumbers;
            public UInt16 NumberOfRelocations;
            public UInt16 NumberOfLinenumbers;
            public UInt32 Characteristics;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_IMPORT_DESCRIPTOR
        {
            //public UInt32 Characteristics;            // 0 for terminating null import descriptor
            public UInt32 OriginalFirstThunk;         // RVA to original unbound IAT (PIMAGE_THUNK_DATA)
            public UInt32 TimeDateStamp;                  // 0 if not bound,
            // -1 if bound, and real date\time stamp
            //     in IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT (new BIND)
            // O.W. date/time stamp of DLL bound to (Old BIND)

            public UInt32 ForwarderChain;                 // -1 if no forwarders
            public UInt32 Name;
            public UInt32 FirstThunk;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_DOS_HEADER
        {
            public UInt16 e_magic;       // Magic number
            public UInt16 e_cblp;        // Bytes on last page of file
            public UInt16 e_cp;          // Pages in file
            public UInt16 e_crlc;        // Relocations
            public UInt16 e_cparhdr;     // Size of header in paragraphs
            public UInt16 e_minalloc;    // Minimum extra paragraphs needed
            public UInt16 e_maxalloc;    // Maximum extra paragraphs needed
            public UInt16 e_ss;          // Initial (relative) SS value
            public UInt16 e_sp;          // Initial SP value
            public UInt16 e_csum;        // Checksum
            public UInt16 e_ip;          // Initial IP value
            public UInt16 e_cs;          // Initial (relative) CS value
            public UInt16 e_lfarlc;      // File address of relocation table
            public UInt16 e_ovno;        // Overlay number
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public UInt16[] e_res1;        // Reserved words
            public UInt16 e_oemid;       // OEM identifier (for e_oeminfo)
            public UInt16 e_oeminfo;     // OEM information; e_oemid specific
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public UInt16[] e_res2;        // Reserved words
            public Int32 e_lfanew;      // File address of new exe header
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_OPTIONAL_HEADER32
        {
            public UInt16 Magic;
            public Byte MajorLinkerVersion;
            public Byte MinorLinkerVersion;
            public UInt32 SizeOfCode;
            public UInt32 SizeOfInitializedData;
            public UInt32 SizeOfUninitializedData;
            public UInt32 AddressOfEntryPoint;
            public UInt32 BaseOfCode;
            public UInt32 BaseOfData;
            public UInt32 ImageBase;
            public UInt32 SectionAlignment;
            public UInt32 FileAlignment;
            public UInt16 MajorOperatingSystemVersion;
            public UInt16 MinorOperatingSystemVersion;
            public UInt16 MajorImageVersion;
            public UInt16 MinorImageVersion;
            public UInt16 MajorSubsystemVersion;
            public UInt16 MinorSubsystemVersion;
            public UInt32 Win32VersionValue;
            public UInt32 SizeOfImage;
            public UInt32 SizeOfHeaders;
            public UInt32 CheckSum;
            public UInt16 Subsystem;
            public UInt16 DllCharacteristics;
            public UInt32 SizeOfStackReserve;
            public UInt32 SizeOfStackCommit;
            public UInt32 SizeOfHeapReserve;
            public UInt32 SizeOfHeapCommit;
            public UInt32 LoaderFlags;
            public UInt32 NumberOfRvaAndSizes;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public IMAGE_DATA_DIRECTORY[] DataDirectory;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_DATA_DIRECTORY
        {
            public UInt32 VirtualAddress;
            public UInt32 Size;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_OPTIONAL_HEADER64
        {
            public UInt16 Magic;
            public Byte MajorLinkerVersion;
            public Byte MinorLinkerVersion;
            public UInt32 SizeOfCode;
            public UInt32 SizeOfInitializedData;
            public UInt32 SizeOfUninitializedData;
            public UInt32 AddressOfEntryPoint;
            public UInt32 BaseOfCode;
            public UInt64 ImageBase;
            public UInt32 SectionAlignment;
            public UInt32 FileAlignment;
            public UInt16 MajorOperatingSystemVersion;
            public UInt16 MinorOperatingSystemVersion;
            public UInt16 MajorImageVersion;
            public UInt16 MinorImageVersion;
            public UInt16 MajorSubsystemVersion;
            public UInt16 MinorSubsystemVersion;
            public UInt32 Win32VersionValue;
            public UInt32 SizeOfImage;
            public UInt32 SizeOfHeaders;
            public UInt32 CheckSum;
            public UInt16 Subsystem;
            public UInt16 DllCharacteristics;
            public UInt64 SizeOfStackReserve;
            public UInt64 SizeOfStackCommit;
            public UInt64 SizeOfHeapReserve;
            public UInt64 SizeOfHeapCommit;
            public UInt32 LoaderFlags;
            public UInt32 NumberOfRvaAndSizes;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_FILE_HEADER
        {
            public UInt16 Machine;
            public UInt16 NumberOfSections;
            public UInt32 TimeDateStamp;
            public UInt32 PointerToSymbolTable;
            public UInt32 NumberOfSymbols;
            public UInt16 SizeOfOptionalHeader;
            public UInt16 Characteristics;
        }

        #endregion File Header Structures

        #region Private Fields

        // The DOS header
        private IMAGE_DOS_HEADER dosHeader;
        // The file header
        private IMAGE_FILE_HEADER fileHeader;
        // Optional 32 bit file header
        private IMAGE_OPTIONAL_HEADER32 optionalHeader32;
        // Optional 64 bit file header
        private IMAGE_OPTIONAL_HEADER64 optionalHeader64;

        #endregion Private Fields

        public PEFinder(string filePath, string dllName, string APIName)
        {
            using (FileStream stream = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(stream);
                dosHeader = FromBinaryReader<IMAGE_DOS_HEADER>(reader);

                stream.Seek(dosHeader.e_lfanew, SeekOrigin.Begin);

                UInt32 ntHeadersSignature = reader.ReadUInt32();
                fileHeader = FromBinaryReader<IMAGE_FILE_HEADER>(reader);

                if (this.Is32BitHeader)
                {
                    optionalHeader32 = FromBinaryReader<IMAGE_OPTIONAL_HEADER32>(reader);
                }
                else
                {
                    optionalHeader64 = FromBinaryReader<IMAGE_OPTIONAL_HEADER64>(reader);
                }

                ImageSectionHeaders = new IMAGE_SECTION_HEADER[fileHeader.NumberOfSections];
                for (int i = 0; i < fileHeader.NumberOfSections; i++)
                {
                    ImageSectionHeaders[i] = FromBinaryReader<IMAGE_SECTION_HEADER>(reader);
                }

                var IMAGE_DIRECTORY_ENTRY_IMPORT = 1;

                var VAImageDir = this.optionalHeader32.DataDirectory[IMAGE_DIRECTORY_ENTRY_IMPORT].VirtualAddress;
                var IATSection = -1;

                for (int i = 0; i < fileHeader.NumberOfSections; i++)
                {
                    if (ImageSectionHeaders[i].VirtualAddress <= VAImageDir &&
                            ImageSectionHeaders[i].VirtualAddress +
                                     ImageSectionHeaders[i].SizeOfRawData > VAImageDir)
                    {
                        IATSection = i;
                        break;
                    }

                }

                if (IATSection != -1)
                {
                    var section = ImageSectionHeaders[IATSection];
                    reader.BaseStream.Position = (VAImageDir - section.VirtualAddress) + section.PointerToRawData;
                    var pImportDescriptor = FromBinaryReader<IMAGE_IMPORT_DESCRIPTOR>(reader);

                    while (pImportDescriptor.FirstThunk != 0)
                    {
                        var nextID = reader.BaseStream.Position;

                        reader.BaseStream.Position = (pImportDescriptor.Name - section.VirtualAddress) + section.PointerToRawData;
                        var name = new string(reader.ReadChars(dllName.Length));
                        if (name.ToLower() == dllName.ToLower())
                        {
                            reader.BaseStream.Position = (pImportDescriptor.FirstThunk - section.VirtualAddress) + section.PointerToRawData;
                            var AddressOfData = reader.ReadUInt32();
                            var Offset = 0;
                            while (AddressOfData != 0)
                            {
                                var nextI = reader.BaseStream.Position;
                                reader.BaseStream.Position = (AddressOfData - section.VirtualAddress) + section.PointerToRawData;
                                ushort hint = reader.ReadUInt16();
                                var tempName = new string(reader.ReadChars(APIName.Length));
                                if (APIName.ToLower() == tempName.ToLower())
                                {
                                    SearchedAddress = (uint)((pImportDescriptor.FirstThunk - section.VirtualAddress) + section.VirtualAddress + this.OptionalHeader32.ImageBase + Offset);
                                    return;
                                }
                                Offset += 4;
                                reader.BaseStream.Position = nextI;
                                AddressOfData = reader.ReadUInt32();
                            }
                        }

                        reader.BaseStream.Position = nextID;
                        pImportDescriptor = FromBinaryReader<IMAGE_IMPORT_DESCRIPTOR>(reader);
                    }
                }
            }
        }

        public static T FromBinaryReader<T>(BinaryReader reader)
        {
            // Read in a byte array
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            // Pin the managed memory while, copy it out the data, then unpin it
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        public bool Is32BitHeader
        {
            get
            {
                UInt16 IMAGE_FILE_32BIT_MACHINE = 0x0100;
                return (IMAGE_FILE_32BIT_MACHINE & FileHeader.Characteristics) == IMAGE_FILE_32BIT_MACHINE;
            }
        }

        public IMAGE_FILE_HEADER FileHeader
        {
            get
            {
                return fileHeader;
            }
        }

        public IMAGE_OPTIONAL_HEADER32 OptionalHeader32
        {
            get
            {
                return optionalHeader32;
            }
        }

        public IMAGE_OPTIONAL_HEADER64 OptionalHeader64
        {
            get
            {
                return optionalHeader64;
            }
        }

        public DateTime TimeStamp
        {
            get
            {
                // Timestamp is a date offset from 1970
                DateTime returnValue = new DateTime(1970, 1, 1, 0, 0, 0);

                // Add in the number of seconds since 1970/1/1
                returnValue = returnValue.AddSeconds(fileHeader.TimeDateStamp);
                // Adjust to local timezone
                returnValue += TimeZone.CurrentTimeZone.GetUtcOffset(returnValue);

                return returnValue;
            }
        }

    }
}