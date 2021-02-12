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
            using (var searcher = new ManagementObjectSearcher(@"Select * from Win32_USBHub"))
                collection = searcher.Get();

            foreach(var device in collection) {
                DeviceID_LB.Items.Add(device.GetPropertyValue("DeviceID"));
                PNPDeviceID_LB.Items.Add(device.GetPropertyValue("PNPDeviceID"));
                DeviceDescription_LB.Items.Add(device.GetPropertyValue("Description"));
            }

            collection.Dispose();

        }
    }
}
