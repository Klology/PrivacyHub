using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;

namespace PrivacyHub
{
    class SystemProcesses
    {
        public void BindToRunningProcesses()
        {
            System.Diagnostics.Debug.WriteLine("-----------------Entered SystemProcesses-----------------");
            // Get all processes running on the local computer.
            Process[] localAll = Process.GetProcesses();

            //print process Name and ID for all processes
            foreach(var process in localAll)
            {
                System.Diagnostics.Debug.WriteLine("Process: " + process.ProcessName + " ID: " + process.Id);
            }

            System.Diagnostics.Debug.WriteLine("-----------------Exited SystemProcesses-----------------");
        }
    }
}
