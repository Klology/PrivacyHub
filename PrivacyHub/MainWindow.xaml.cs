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

            foreach(var device in collection) {
                string curDeviceInfo = (string)device.GetPropertyValue("Dependent");
                string usbAddress = (curDeviceInfo.Split(new String[] { "DeviceID=" }, 2, StringSplitOptions.None)[1]);

                ManagementObjectCollection devices;
                using (var searcher = new ManagementObjectSearcher("Select * from Win32_PnPEntity where PNPDeviceID = " + usbAddress))
                    devices = searcher.Get();

                foreach(var usbDevice in devices) {
                    Console.WriteLine("----------------DEVICE---------------");
                    foreach(var property in usbDevice.Properties)
                        Console.WriteLine(property.Name + ": " + property.Value);
                    Console.WriteLine("----------------END---------------");
                }

                devices.Dispose();
            }

            
            collection.Dispose();
            
        }
    }
}
