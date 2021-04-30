using System;
using System.IO;

namespace PrivacyHub
{
    class TrustedProcessesFileHandler
    {
        private string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"../../TrustedProcesses.txt");
        private string[] trustedProcesses;

        public string Path { get { return Path; } }
        public string[] TrustedProcesses { get { return trustedProcesses; } set { trustedProcesses = value; } }

        public TrustedProcessesFileHandler() {

            if (!File.Exists(path)) CreateTrustedProcessFile();

            RefreshTrustedProcesses();
        }

        private void CreateTrustedProcessFile() {
            var file = File.Create(path);

            file.Close();
        }

        public void RefreshTrustedProcesses() {
            trustedProcesses = File.ReadAllLines(path);
        }

        public void SaveTrustedProcesses() {
            Console.WriteLine(String.Join("\n", trustedProcesses));
            File.WriteAllText(path, String.Join("\n", trustedProcesses));
        }
    }
}
