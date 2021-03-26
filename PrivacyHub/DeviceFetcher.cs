using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivacyHub
{
    interface DeviceFetcher
    {
        List<Device> getAllDevices();
    }
}
