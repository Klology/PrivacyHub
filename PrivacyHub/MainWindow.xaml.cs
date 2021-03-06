using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;

namespace PrivacyHub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * from Win32_USBControllerDevice"))
                collection = searcher.Get();

            List<string> searchableSubstrings = new List<string>();

            foreach(var device in collection) {
                string curDeviceInfo = (string)device.GetPropertyValue("Dependent");
                string usbAddress = (curDeviceInfo.Split(new String[] { "DeviceID=" }, 2, StringSplitOptions.None)[1]);

                ManagementObjectCollection devices;
                using (var searcher = new ManagementObjectSearcher("Select * from Win32_PnPEntity where PNPDeviceID = " + usbAddress))
                    devices = searcher.Get();

                foreach(var usbDevice in devices) {
                    String pnpClass = usbDevice.GetPropertyValue("PNPClass").ToString();
                    if (pnpClass.Equals("AudioEndpoint") || pnpClass.Equals("MEDIA") || pnpClass.Equals("Image") || pnpClass.Equals("Camera")) {

                        Device newDevice = new Device(usbDevice);

                        if (newDevice.HasSearchableSubstring)
                        {
                            searchableSubstrings.Add(newDevice.PNPDeviceIDSubstring);
                        }
                            
                    }
                }

                devices.Dispose();

            }

            SystemProcesses systemProcesses = new SystemProcesses();
            systemProcesses.BindToRunningProcesses();

            collection.Dispose();
            
            ProcessUtility processUtility = new ProcessUtility();
            
            List<Process> processList = System.Diagnostics.Process.GetProcesses().ToList();

            List<ProcessUtility.ProcessAndDevices> processFiles = processUtility.GetProcessHandles(processList, searchableSubstrings);

            Console.Write("Process name: " + processFiles[0].processName + "Handle path: " + processFiles[0].handlePath + " Devices: ");
            foreach (string device in processFiles[0].devices)
                Console.Write(device);

        }
    }
}
