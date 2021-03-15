using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PrivacyHub
{
    interface ProcessUtility
    {
        List<ProcessAndDevices> GetProcessHandles(List<Process> target_processes, List<string> searchableSubstrings);
    }
}
