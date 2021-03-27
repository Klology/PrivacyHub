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
using PrivacyHub.WindowsDeviceFetcherPackage;

namespace PrivacyHub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Device> deviceList;
        List<CheckBox> checkBoxes;
        DeviceFetcher deviceFetcher = new WindowsWebcamAndMicrophoneFetcher();
        List<Process> processList;
        List<ProcessAndDevices> processFiles;

        public MainWindow()
        {
            InitializeComponent();

            deviceList = deviceFetcher.getAllDevices();
            checkBoxes = new List<CheckBox>();

            DeviceButtonClicked(null, null);

        }

        private void ConnectProcessesAndDevices()
        {
            ProcessUtility processUtility = new WindowsProcessUtility();
            processList = System.Diagnostics.Process.GetProcesses().ToList();
            processFiles = processUtility.GetProcessAndDevices(processList, deviceList);
        }

        private void DeviceButtonClicked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Device Button Clicked");

            DeviceID_LB.Items.Clear();
            TextBox_Page.Text = "Devices";

            Devices_Refresh.Visibility = Visibility.Visible;
            Processes_Refresh.Visibility = Visibility.Hidden;
            ConfirmSelection_Button.Visibility = Visibility.Hidden;

            ConnectProcessesAndDevices();

            foreach (Device device in deviceList)
            {
                DeviceID_LB.Items.Add(device.Name + " is being used by the processes:");

                for (int i = 0; i < processFiles.Count; i++)
                {
                    foreach (Device processDevice in processFiles[i].devices)
                    {
                        if (String.Compare(device.Name, processDevice.Name) == 0)
                            DeviceID_LB.Items.Add("     " + processFiles[i].processName);
                    }
                    
                }
            }

        }

        private void ProcessButtonClicked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Process Button Clicked");

            TextBox_Page.Text = "Processes";

            Devices_Refresh.Visibility = Visibility.Hidden;
            Processes_Refresh.Visibility = Visibility.Visible;
            ConfirmSelection_Button.Visibility = Visibility.Hidden;

            DeviceID_LB.Items.Clear();

            ConnectProcessesAndDevices();

            for (int i = 0; i < processFiles.Count; i++)
            {
                Console.WriteLine("\n\nProcess name: " + processFiles[i].processName + " Devices: ");
                foreach (Device device in processFiles[i].devices)
                    Console.WriteLine(device.Name);

                DeviceID_LB.Items.Add(processFiles[i].processName + " is using the devices:");
                foreach (Device device in processFiles[i].devices)
                    DeviceID_LB.Items.Add("     " + device.Name);
            }
        }

        private void SettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Settings Button Clicked");

            TextBox_Page.Text = "Settings";
            ConfirmSelection_Button.Visibility = Visibility.Hidden;

            DeviceID_LB.Items.Clear();
        }

        private void SelectDevicesButton(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Select Devices Button Clicked");

            TextBox_Page.Text = "Select Devices";

            DeviceID_LB.Items.Clear();

            deviceList = deviceFetcher.getAllDevices();

            checkBoxes.Clear();

            foreach (Device device in deviceList)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = device.Name;
                checkBox.Tag = device;
                checkBox.IsChecked = true;

                checkBoxes.Add(checkBox);

                DeviceID_LB.Items.Add(checkBox);
            }

            Devices_Refresh.Visibility = Visibility.Hidden;
            Processes_Refresh.Visibility = Visibility.Hidden;
            ConfirmSelection_Button.Visibility = Visibility.Visible;
        }

        private void ConfirmSelection_Click(object sender, RoutedEventArgs e)
        {
            deviceList.Clear();

            foreach(CheckBox checkBox in checkBoxes)
            {
                if ((bool)checkBox.IsChecked)
                    deviceList.Add((Device)checkBox.Tag);
            }

            Console.WriteLine("searchableSubstrings Updated");
        }
    }
}
