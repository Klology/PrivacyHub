using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PrivacyHub.WindowsProcessUtilityPackage
{
    /// <summary>
    /// https://www.geoffchappell.com/studies/windows/km/ntoskrnl/api/ex/sysinfo/handle_table_entry.htm?ts=0,242
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct SYSTEM_HANDLE_INFORMATION
    { // Information Class 16
        public ushort ProcessID;
        public ushort CreatorBackTrackIndex;
        public byte ObjectType;
        public byte HandleAttribute;
        public ushort Handle;
        public IntPtr Object_Pointer;
        public IntPtr AccessMask;
    }
}
