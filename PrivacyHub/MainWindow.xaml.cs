using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        TrustedProcessesFileHandler trustedProcesses = new TrustedProcessesFileHandler();
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
            checkBoxes = new List<CheckBox>();

            timer = new Timer();
            timer.Elapsed += UpdateTimer;
            timer.Interval = 100000;
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
            if (!currentProcessFiles.Equals(previousProcessFiles) && previousProcessFiles != null)
            {
                List<ProcessAndDevices> newProcessFiles = currentProcessFiles.Except(previousProcessFiles).ToList();

                string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"../../TrustedProcesses.txt");
                string[] fileLines = File.ReadAllLines(path);

                //Filter out fileLines from newProcessFiles
                newProcessFiles = newProcessFiles.Where(processFile => !fileLines.Contains(processFile.processName)).ToList();

                foreach (ProcessAndDevices processFile in newProcessFiles)
                {
                    String newProcess = processFile.processName;
                    List<Device> newDevices = processFile.devices;

                    foreach (Device device in newDevices)
                    {
                        new ToastContentBuilder()
                        .AddText(newProcess + " has started using " + device.Name + ".")
                        .Show();
                    }
                }
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
            GUINewContext("Devices");

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

            GUINewContext("Processes");

            ConnectProcessesAndDevices();

            for (int i = 0; i < currentProcessFiles.Count; i++)
            {
                DeviceID_LB.Items.Add(currentProcessFiles[i].processName + " is using the devices:");
                foreach (Device device in currentProcessFiles[i].devices)
                    DeviceID_LB.Items.Add("     " + device.Name);
            }
        }

        private void SettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            GUINewContext("Settings");
        }

        private void SelectDevicesButton(object sender, RoutedEventArgs e)
        {
            GUINewContext("Select Devices");

            deviceList = deviceFetcher.getAllDevices();

            foreach (Device device in deviceList)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = device.Name;
                checkBox.Tag = device;
                checkBox.IsChecked = true;

                checkBoxes.Add(checkBox);

                DeviceID_LB.Items.Add(checkBox);
            }
        }

        private void ConfirmSelection_Click(object sender, RoutedEventArgs e)
        {
            if (String.Compare("Select Devices", currentContext) == 0)
                UpdateDevices();
            else if (String.Compare("Trusted Processes", currentContext) == 0)
                UpdateTrustedProcesses();
        }

        private void UpdateDevices()
        {
            deviceList.Clear();

            foreach (CheckBox checkBox in checkBoxes)
            {
                if ((bool)checkBox.IsChecked)
                    deviceList.Add((Device)checkBox.Tag);
            }

            Console.WriteLine("searchableSubstrings Updated");
        }

        private void UpdateTrustedProcesses()
        {

            List<String> newTrustedProcesses = new List<String>();

            foreach (CheckBox checkBox in checkBoxes)
            {
                if ((bool)checkBox.IsChecked)
                {
                    newTrustedProcesses.Add((String)checkBox.Tag);
                }
            }

            trustedProcesses.TrustedProcesses = newTrustedProcesses.ToArray();

            trustedProcesses.SaveTrustedProcesses();
            
        }

        private void TrustedProcesses_Click(object sender, RoutedEventArgs e)
        {

            GUINewContext("Trusted Processes");

            ConnectProcessesAndDevices();


            for (int i = 0; i < currentProcessFiles.Count; i++)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = currentProcessFiles[i].processName;
                checkBox.Tag = currentProcessFiles[i].processName;
                checkBox.IsChecked = false;
                
                checkBoxes.Add(checkBox);
                DeviceID_LB.Items.Add(checkBox);
            }

            foreach (string processFileName in trustedProcesses.TrustedProcesses) {

                bool used = false;

                foreach (CheckBox checkBox in checkBoxes) {
                    if (processFileName.Equals(checkBox.Tag)) {
                        checkBox.IsChecked = true;
                        used = true;
                        break;
                    }
                }

                if(!used) {
                    CheckBox checkBox = new CheckBox();
                    checkBox.Content = processFileName;
                    checkBox.Tag = processFileName;
                    checkBox.IsChecked = true;

                    checkBoxes.Add(checkBox);
                    DeviceID_LB.Items.Add(checkBox);
                }
            }

        }

        private void GUINewContext(string newContext)
        {
            TextBox_Page.Text = newContext;
            currentContext = newContext;

            DeviceID_LB.Items.Clear();
            checkBoxes.Clear();

            Devices_Refresh.Visibility = Visibility.Hidden;
            Processes_Refresh.Visibility = Visibility.Hidden;
            ConfirmSelection_Button.Visibility = Visibility.Hidden;

            if (String.Compare("Devices", currentContext) == 0)
                Devices_Refresh.Visibility = Visibility.Visible;
            else if (String.Compare("Processes", currentContext) == 0)
                Processes_Refresh.Visibility = Visibility.Visible;
            else if (String.Compare("Trusted Processes", currentContext) == 0 || String.Compare("Select Devices", currentContext) == 0)
                ConfirmSelection_Button.Visibility = Visibility.Visible;
        }
    }
}
