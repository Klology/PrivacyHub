using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivacyHub.WindowsProcessUtility
{
    public struct ProcessAndDevices
    {
        public string processName;
        public string handlePath;
        public List<string> devices;

        public ProcessAndDevices(string processName, string handlePath, List<string> devices)
        {
            this.processName = processName;
            this.handlePath = handlePath;
            this.devices = devices;
        }
    }
}
