using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PrivacyHub.WindowsDeviceFetcherPackage
{
    class WindowsWebcamAndMicrophoneFetcher : DeviceFetcher
    {
        public List<Device> getAllDevices()
        {
            ManagementObjectCollection collection;
            List<Device> deviceList = new List<Device>(); ;

            using (var searcher = new ManagementObjectSearcher(@"Select * from Win32_USBControllerDevice"))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                string curDeviceInfo = (string)device.GetPropertyValue("Dependent");
                string usbAddress = (curDeviceInfo.Split(new String[] { "DeviceID=" }, 2, StringSplitOptions.None)[1]);

                ManagementObjectCollection devices;
                using (var searcher = new ManagementObjectSearcher("Select * from Win32_PnPEntity where PNPDeviceID = " + usbAddress))
                    devices = searcher.Get();

                foreach (var usbDevice in devices)
                {
                    //Ignore Bluetooth devices (add Bluetooth compatibility later perhaps)
                    if (usbDevice.GetPropertyValue("PNPClass") == null)
                    {
                        continue;
                    }

                    String pnpClass = usbDevice.GetPropertyValue("PNPClass").ToString();
                    if (pnpClass.Equals("AudioEndpoint") || pnpClass.Equals("MEDIA") || pnpClass.Equals("Image") || pnpClass.Equals("Camera"))
                    {

                        Device newDevice = new Device(usbDevice);

                        if (newDevice.HasSearchableSubstring)
                        {
                            deviceList.Add(newDevice);
                        }

                    }
                }
                devices.Dispose();
            }
            collection.Dispose();
            return deviceList;
        }
    }
}
