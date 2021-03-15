using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PrivacyHub.WindowsProcessUtilityPackage
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct OBJECT_NAME_INFORMATION
    { // Information Class 1
        public UNICODE_STRING Name;
    }
}
