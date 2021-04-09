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
using System.Timers;
using Microsoft.Toolkit.Uwp.Notifications;

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
        List<ProcessAndDevices> currentProcessFiles;
        List<ProcessAndDevices> previousProcessFiles;
        private static Timer timer;
        String currentContext;

        public MainWindow()
        {
            InitializeComponent();

            deviceList = deviceFetcher.getAllDevices();
            currentProcessFiles = new List<ProcessAndDevices>();
            checkBoxes = new List<CheckBox>();

            timer = new Timer();
            timer.Elapsed += UpdateTimer;
            timer.Interval = 10000;
            timer.Start();

            DeviceButtonClicked(null, null);

        }

        private void UpdateTimer(object sender, EventArgs e)
        {
            if (String.Compare("Devices", currentContext) == 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    DeviceButtonClicked(null, null);
                });
            }
            else if (String.Compare("Processes", currentContext) == 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    ProcessButtonClicked(null, null);
                });
            }
        }

        private void NotifyOfNewItems(List<ProcessAndDevices> currentProcessFiles, List<ProcessAndDevices> previousProcessFiles)
        {
            Console.WriteLine("currentProcessFiles: " + currentProcessFiles.ToString());
            Console.WriteLine("previousProcessFiles: " + previousProcessFiles.ToString());
            if (!currentProcessFiles.Equals(previousProcessFiles))
            {
                Console.WriteLine("lists are different");
                List<ProcessAndDevices> newProcessFiles = currentProcessFiles.Except(previousProcessFiles).ToList();

                foreach (ProcessAndDevices processFile in newProcessFiles)
                {
                    String newProcess = processFile.processName;
                    List<Device> newDevices = processFile.devices;

                    foreach (Device device in newDevices)
                    {
                        /*new ToastContentBuilder()
                        .AddText(newProcess + " has started using " + device.Name + ".")
                        .Show();*/
                    }
                }
            }
            else
            {
                Console.WriteLine("Lists are the same");
            }
        }

        private void ConnectProcessesAndDevices()
        {
            ProcessUtility processUtility = new WindowsProcessUtility();
            processList = System.Diagnostics.Process.GetProcesses().ToList();
            previousProcessFiles = currentProcessFiles;
            currentProcessFiles = processUtility.GetProcessAndDevices(processList, deviceList);

            NotifyOfNewItems(currentProcessFiles, previousProcessFiles);
        }

        private void DeviceButtonClicked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Device Button Clicked");
            DeviceID_LB.Items.Clear();
            TextBox_Page.Text = "Devices";
            currentContext = "Devices";

            Devices_Refresh.Visibility = Visibility.Visible;
            Processes_Refresh.Visibility = Visibility.Hidden;
            ConfirmSelection_Button.Visibility = Visibility.Hidden;

            ConnectProcessesAndDevices();

            foreach (Device device in deviceList)
            {
                DeviceID_LB.Items.Add(device.Name + " is being used by the processes:");

                for (int i = 0; i < currentProcessFiles.Count; i++)
                {
                    foreach (Device processDevice in currentProcessFiles[i].devices)
                    {
                        if (String.Compare(device.Name, processDevice.Name) == 0)
                            DeviceID_LB.Items.Add("     " + currentProcessFiles[i].processName);
                    }
                    
                }
            }

        }

        private void ProcessButtonClicked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Process Button Clicked");

            TextBox_Page.Text = "Processes";
            currentContext = "Processes";

            Devices_Refresh.Visibility = Visibility.Hidden;
            Processes_Refresh.Visibility = Visibility.Visible;
            ConfirmSelection_Button.Visibility = Visibility.Hidden;

            DeviceID_LB.Items.Clear();

            ConnectProcessesAndDevices();

            for (int i = 0; i < currentProcessFiles.Count; i++)
            {
                /*
                Console.WriteLine("\n\nProcess name: " + currentProcessFiles[i].processName + " Devices: ");
                foreach (Device device in currentProcessFiles[i].devices)
                    Console.WriteLine(device.Name);
                */

                DeviceID_LB.Items.Add(currentProcessFiles[i].processName + " is using the devices:");
                foreach (Device device in currentProcessFiles[i].devices)
                    DeviceID_LB.Items.Add("     " + device.Name);
            }
        }

        private void SettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Settings Button Clicked");

            TextBox_Page.Text = "Settings";
            currentContext = "Settings";
            ConfirmSelection_Button.Visibility = Visibility.Hidden;

            DeviceID_LB.Items.Clear();
        }

        private void SelectDevicesButton(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Select Devices Button Clicked");

            TextBox_Page.Text = "Select Devices";
            currentContext = "Select Devices";

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
