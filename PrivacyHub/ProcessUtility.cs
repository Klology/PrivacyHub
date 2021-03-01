using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

public class ProcessUtility
{
    /// <summary>
    /// https://www.geoffchappell.com/studies/windows/km/ntoskrnl/api/ex/sysinfo/handle_table_entry.htm?ts=0,242
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct SYSTEM_HANDLE_INFORMATION
    { // Information Class 16
        public ushort ProcessID;
        public ushort CreatorBackTrackIndex;
        public byte ObjectType;
        public byte HandleAttribute;
        public ushort Handle;
        public IntPtr Object_Pointer;
        public IntPtr AccessMask;
    }

    private enum OBJECT_INFORMATION_CLASS : int
    {
        ObjectBasicInformation = 0,
        ObjectNameInformation = 1,
        ObjectTypeInformation = 2,
        ObjectAllTypesInformation = 3,
        ObjectHandleInformation = 4
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct OBJECT_NAME_INFORMATION
    { // Information Class 1
        public UNICODE_STRING Name;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct UNICODE_STRING
    {
        public ushort Length;
        public ushort MaximumLength;
        public IntPtr Buffer;
    }

    [Flags]
    private enum PROCESS_ACCESS_FLAGS : uint
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

    private enum FileType : uint
    {
        FileTypeChar = 0x0002,
        FileTypeDisk = 0x0001,
        FileTypePipe = 0x0003,
        FileTypeRemote = 0x8000,
        FileTypeUnknown = 0x0000,
    }

    [DllImport("ntdll.dll")]
    private static extern uint NtQuerySystemInformation(int SystemInformationClass, IntPtr SystemInformation, int SystemInformationLength, ref int returnLength);

    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenProcess(PROCESS_ACCESS_FLAGS dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle, out IntPtr lpTargetHandle, uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwOptions);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();

    [DllImport("ntdll.dll")]
    private static extern int NtQueryObject(IntPtr ObjectHandle, int ObjectInformationClass, IntPtr ObjectInformation, int ObjectInformationLength, ref int returnLength);

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);

    [DllImport("kernel32.dll")]
    private static extern bool GetHandleInformation(IntPtr hObject, out uint lpdwFlags);

    [DllImport("kernel32.dll")]
    private static extern FileType GetFileType(IntPtr hFile);

    private const int MAX_PATH = 260;
    private const uint STATUS_INFO_LENGTH_MISMATCH = 0xC0000004;
    private const int DUPLICATE_SAME_ACCESS = 0x2;
    private const uint FILE_SEQUENTIAL_ONLY = 0x00000004;
    private const int CNST_SYSTEM_HANDLE_INFORMATION = 0x10;
    private const int OBJECT_TYPE_FILE = 0x24;

    public List<string> GetProcessHandles(List<Process> target_processes)
    {
        List<string> aFiles = new List<string>();
        int count = 0;
        
        //This gets a list of all the handels for the system
        List<SYSTEM_HANDLE_INFORMATION> aHandles = GetHandles().ToList();

        foreach (Process process in target_processes)
        {
            Console.WriteLine(process.ProcessName);
            //Only worry about this if you want to check for a specific process's handles     //if(process.ProcessName.Equals("NVIDIA RTX Voice"))
            
            //Go through all the handles
            foreach (SYSTEM_HANDLE_INFORMATION handle_info in aHandles)
            {
                string file_path = GetHandleName(handle_info, process, count, target_processes.Count);
                if (!string.IsNullOrEmpty(file_path))
                {
                    aFiles.Add(file_path);
                }
            }
            count++;
        }
        return aFiles;
    }

    private static IEnumerable<SYSTEM_HANDLE_INFORMATION> GetHandles()
    {
        List<SYSTEM_HANDLE_INFORMATION> aHandles = new List<SYSTEM_HANDLE_INFORMATION>();
        int handle_info_size = Marshal.SizeOf(new SYSTEM_HANDLE_INFORMATION()) * 20000; //Allocate enough memory to hold 200000 handles
        IntPtr ptrHandleData = IntPtr.Zero;
        try
        {
            ptrHandleData = Marshal.AllocHGlobal(handle_info_size);
            int nLength = 0;

            //Keep testing handle info lengths until you find out how much memory to allocate to hold all handles
            while (NtQuerySystemInformation(CNST_SYSTEM_HANDLE_INFORMATION, ptrHandleData, handle_info_size, ref nLength) == STATUS_INFO_LENGTH_MISMATCH)
            {
                handle_info_size = nLength;
                Marshal.FreeHGlobal(ptrHandleData);
                ptrHandleData = Marshal.AllocHGlobal(nLength);
            }

            //Now that we know how much data to store, actually store the data
            long handle_count = Marshal.ReadIntPtr(ptrHandleData).ToInt64();
            IntPtr ptrHandleItem = ptrHandleData + Marshal.SizeOf(ptrHandleData);

            //Go through all the handles to extract individual information for each
            for (long lIndex = 0; lIndex < handle_count; lIndex++)
            {
                //Put the information for a handle into the struct SYSTEM_HANDLE_INFORMATION
                SYSTEM_HANDLE_INFORMATION oSystemHandleInfo = Marshal.PtrToStructure<SYSTEM_HANDLE_INFORMATION>(ptrHandleItem);

                //Move to next handle
                ptrHandleItem += Marshal.SizeOf(new SYSTEM_HANDLE_INFORMATION());
                
                //Only care about this if you want to filter handles releveant to a specific process ID //if (oSystemHandleInfo.ProcessID != 8988) { continue; }

                //Add the handle info to the list of handles
                aHandles.Add(oSystemHandleInfo);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            //Avoid memory leaks by freeing up managed memory
            Marshal.FreeHGlobal(ptrHandleData);
        }
        return aHandles;
    }

    private static string GetHandleName(SYSTEM_HANDLE_INFORMATION systemHandleInformation, Process process, int count, int handleCount)
    {
        IntPtr ipHandle = IntPtr.Zero;
        IntPtr openProcessHandle = IntPtr.Zero;
        IntPtr hObjectName = IntPtr.Zero;
        try
        {
            //Set up flags for and then get a process handle for the current process being checked
            PROCESS_ACCESS_FLAGS flags = PROCESS_ACCESS_FLAGS.DupHandle | PROCESS_ACCESS_FLAGS.VMRead;  
            openProcessHandle = OpenProcess(flags, false, process.Id);

            //Duplicate the process handle into ipHandle, if this failes return null
            if (!DuplicateHandle(openProcessHandle, new IntPtr(systemHandleInformation.Handle), GetCurrentProcess(), out ipHandle, 0, false, DUPLICATE_SAME_ACCESS))
            {
                return null;
            }


            int nLength = 0;
            hObjectName = Marshal.AllocHGlobal(256 * 1024); //Allocate memory for a max length handle name

            //Try to find out exactly how long the object name is, then once you've allocated the proper amount of memory, copy it into hObjectName
            while ((uint)(NtQueryObject(ipHandle, (int)OBJECT_INFORMATION_CLASS.ObjectNameInformation, hObjectName, nLength, ref nLength)) == STATUS_INFO_LENGTH_MISMATCH)
            {
                Marshal.FreeHGlobal(hObjectName);
                if (nLength == 0)
                {
                    Console.WriteLine("Length returned at zero!");
                    return null;
                }
                hObjectName = Marshal.AllocHGlobal(nLength);
            }
            //Move the infromation in hObjectName to an easier to use structure OBJECT_NAME_INFORMATION
            OBJECT_NAME_INFORMATION objObjectName = Marshal.PtrToStructure<OBJECT_NAME_INFORMATION>(hObjectName);

            //Check if we have a proper name
            if (objObjectName.Name.Buffer != IntPtr.Zero)
            {
                //Convert objObjectName to a normal string, this is the handle's name
                string strObjectName = Marshal.PtrToStringUni(objObjectName.Name.Buffer);

                //Check the handle name for if it contains anything releveant (in this case it's checking for a device ID) if it does, return it
                if (strObjectName.ToLower().Contains("{1F4B9709-47AC-4E25-A247-E6466E076D7C}".ToLower())//Put a proper device ID here)
                    return strObjectName;

                //If it doesnt, return null
                Console.WriteLine("(" + count + " / " + handleCount + "): " + strObjectName);
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {

            //Clean up managed memory
            Marshal.FreeHGlobal(hObjectName);

            CloseHandle(ipHandle);
            CloseHandle(openProcessHandle);
        }
        return null;
    }
}