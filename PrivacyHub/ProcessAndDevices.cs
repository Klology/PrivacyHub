using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivacyHub
{
    public struct ProcessAndDevices
    {
        public string processName;
        public List<Device> devices;

        public ProcessAndDevices(string processName, List<Device> devices)
        {
            this.processName = processName;
            this.devices = devices;
        }
    }
}
