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

        public override bool Equals(object obj)
        {
            // If the passed object is null
            if (obj == null)
            {
                return false;
            }
            if (!(obj is ProcessAndDevices))
            {
                return false;
            }
            return (this.processName == ((ProcessAndDevices)obj).processName);
        }
        public override string ToString()
        {
            return "Process and Devices: " + processName + " " + devices.ToString();
        }
    }
}
