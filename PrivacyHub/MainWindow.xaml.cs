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

        private List<string> searchableSubstrings;

        public MainWindow()
        {
            InitializeComponent();

            
            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * from Win32_USBControllerDevice"))
                collection = searcher.Get();

            searchableSubstrings = new List<string>();

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

            collection.Dispose();

        }

        private void DeviceButtonClicked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Device Button Clicked");

            DeviceID_LB.Items.Clear();
            TextBox_Page.Text = "Devices";

            foreach (string deviceID in searchableSubstrings)
            {
                DeviceID_LB.Items.Add(deviceID);
            }

            SystemProcesses systemProcesses = new SystemProcesses();
            systemProcesses.BindToRunningProcesses();


            ProcessUtility processUtility = new ProcessUtility();

            List<Process> processList = System.Diagnostics.Process.GetProcesses().ToList();

            List<ProcessUtility.ProcessAndDevices> processFiles = processUtility.GetProcessHandles(processList, searchableSubstrings);

            for (int i = 0; i < processFiles.Count; i++)
            {
                Console.Write("\n\nProcess name: " + processFiles[i].processName + " Devices: ");
                foreach (string device in processFiles[i].devices)
                    Console.Write(device);
            }
        }

        private void ProcessButtonClicked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Process Button Clicked");

            TextBox_Page.Text = "Processes";

            DeviceID_LB.Items.Clear();
        }

        private void SettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Settings Button Clicked");

            TextBox_Page.Text = "Settings";

            DeviceID_LB.Items.Clear();


        }

        private void SelectDevicesButton(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Select Devices Button Clicked");

            TextBox_Page.Text = "Select Devices";

            DeviceID_LB.Items.Clear();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * from Win32_USBControllerDevice"))
                collection = searcher.Get();

            List<string> searchableSubstrings = new List<string>();

            foreach (var device in collection)
            {
                string curDeviceInfo = (string)device.GetPropertyValue("Dependent");
                string usbAddress = (curDeviceInfo.Split(new String[] { "DeviceID=" }, 2, StringSplitOptions.None)[1]);

                ManagementObjectCollection devices;
                using (var searcher = new ManagementObjectSearcher("Select * from Win32_PnPEntity where PNPDeviceID = " + usbAddress))
                    devices = searcher.Get();

                foreach (var usbDevice in devices)
                {
                    String pnpClass = usbDevice.GetPropertyValue("PNPClass").ToString();
                    if (pnpClass.Equals("AudioEndpoint") || pnpClass.Equals("MEDIA") || pnpClass.Equals("Image") || pnpClass.Equals("Camera"))
                    {

                        Device newDevice = new Device(usbDevice);

                        if (newDevice.HasSearchableSubstring)
                        {
                            searchableSubstrings.Add(newDevice.PNPDeviceIDSubstring);
                        }

                    }
                }

                devices.Dispose();
            }

            foreach (string deviceID in searchableSubstrings)
            {
              //  RadioButton radioButton = new RadioButton();
               // radioButton.Name = deviceID;

                DeviceID_LB.Items.Add(deviceID);
            }
        }
    }
}
