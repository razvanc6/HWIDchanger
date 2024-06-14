using Microsoft.Win32;
using Siticone.Desktop.UI.WinForms;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.InteropServices;
using MetroFramework.Controls;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Media;
using System.IO;

namespace SecHex_GUI
{
    public partial class Spoofy : MetroFramework.Forms.MetroForm
    {
        private System.Windows.Forms.Timer timer;
        private bool isAnimationRunning = false;

        public Spoofy()
        {
            InitializeComponent();

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 100;

            this.DoubleBuffered = true;
            timer.Start();
        }

        string globalSerialNumber = null;
        string globalIdentifier = null;
        string globalGUID = null;
        string globalMachineGuid = null;
        string globalSerialNumberKey = null;
        private bool isAfricaToggleOn = false;
        private south_africa WindowSouthAfrica;
        private logs logWindow;

        private void OpenAfrica()
        {
            if (WindowSouthAfrica == null || WindowSouthAfrica.IsDisposed)
            {
                Console.WriteLine("OpenAfrica() called");
                WindowSouthAfrica = new south_africa();
                WindowSouthAfrica.Show();
            }
            else
            {
                WindowSouthAfrica.Show();
            }
        }

        private void CloseAfrica()
        {
            if (WindowSouthAfrica != null && !WindowSouthAfrica.IsDisposed)
            {
                WindowSouthAfrica.Close();
                WindowSouthAfrica.Dispose();
                WindowSouthAfrica = null;
            }
        }

        private void OpenLogWindow()
        {
            if (logWindow == null || logWindow.IsDisposed)
            {
                logWindow = new logs();
                logWindow.Show();
            }
            else
            {
                logWindow.Show();
            }
        }

        private void CloseLogWindow()
        {
            if (logWindow != null && !logWindow.IsDisposed)
            {
                logWindow.Hide();
            }
        }

        private void SaveLogs(string id, string logBefore, string logAfter)
        {
            string logsFolderPath = Path.Combine(Application.StartupPath, "Logs");
            if (!Directory.Exists(logsFolderPath))
                Directory.CreateDirectory(logsFolderPath);

            string logFileName = Path.Combine(logsFolderPath, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
            string logEntryBefore = $"{DateTime.Now:HH:mm:ss}: ID {id} -  {logBefore}";
            string logEntryAfter = $"{DateTime.Now:HH:mm:ss}: ID {id} -  {logAfter}";

            File.AppendAllText(logFileName, logEntryBefore + Environment.NewLine);
            File.AppendAllText(logFileName, logEntryAfter + Environment.NewLine);

            AppendLogEntryToWindow(logEntryBefore, logEntryAfter);
        }

        private void AppendLogEntryToWindow(string logEntryBefore, string logEntryAfter)
        {
            if (logWindow != null && !logWindow.IsDisposed)
            {
                logWindow.AddLogEntry(logEntryBefore, logEntryAfter);
            }
        }


        public static string RandomId(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string result = "";
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                result += chars[random.Next(chars.Length)];
            }

            return result;
        }

        private string RandomIdprid2(int length)
        {
            const string digits = "0123456789";
            const string letters = "abcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            var id = new char[32];
            int letterIndex = 0;

            for (int i = 0; i < 32; i++)
            {
                if (i == 8 || i == 13 || i == 18 || i == 23)
                {
                    id[i] = '-';
                }
                else if (i % 5 == 4)
                {
                    id[i] = letters[random.Next(letters.Length)];
                    letterIndex++;
                }
                else
                {
                    id[i] = digits[random.Next(digits.Length)];
                }
            }

            return new string(id);
        }


        public static string RandomMac()
        {
            string chars = "ABCDEF0123456789";
            string windows = "26AE";
            string result = "";
            Random random = new Random();

            result += chars[random.Next(chars.Length)];
            result += windows[random.Next(windows.Length)];

            for (int i = 0; i < 5; i++)
            {
                result += "-";
                result += chars[random.Next(chars.Length)];
                result += chars[random.Next(chars.Length)];

            }

            return result;
        }


        private void spoofall_Click(object sender, EventArgs e)
        {
            bool registryEntriesExist = false;

            try
            {
                req_Click(sender, e);
                registryEntriesExist = true;
            }
            catch (Exception ex)
            {
                ShowNotification("Error executing functions: " + ex.Message, NotificationType.Error);
            }

            if (registryEntriesExist)
            {
                disk_Click(sender, e); //asta
                GUID_Click(sender, e); //asta
                winid_Click(sender, e); //asta
                profile_Save(sender, e);



                ShowNotification("All functions executed successfully.", NotificationType.Success);
            }
            else
            {
                ShowNotification("Error: One or more required registry entries are missing.", NotificationType.Error);
            }
        }


        private List<string> diskNames = new List<string>()
{
    "Samsung SSD 870 QVO 1TB",
    "NVMe KINGSTON SA2000M2105",
    "Crucial MX500 1TB",
    "WD Blue 2TB",
    "Seagate Barracuda 4TB",
    "Intel 660p 1TB",
    "SanDisk Ultra 3D 2TB",
    "Toshiba X300 6TB",
    "Adata XPG SX8200 Pro 1TB",
    "HP EX920 512GB",
    "Kingston A2000 500GB",
    "Corsair MP600 2TB",
    "Western Digital Black 6TB",
    "Crucial P1 1TB",
    "Seagate FireCuda 2TB",
    "Samsung 970 EVO Plus 1TB",
    "ADATA Swordfish 500GB",
    "Toshiba N300 8TB",
    "WD Red Pro 10TB",
    "Kingston KC600 256GB",
};

        private async void disk_Click(object sender, EventArgs e)
        {
            try
            {
                using (RegistryKey ScsiPorts = Registry.LocalMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\Scsi"))
                {
                    if (ScsiPorts != null)
                    {
                        foreach (string port in ScsiPorts.GetSubKeyNames())
                        {
                            using (RegistryKey ScsiBuses = Registry.LocalMachine.OpenSubKey($"HARDWARE\\DEVICEMAP\\Scsi\\{port}"))
                            {
                                if (ScsiBuses != null)
                                {
                                    foreach (string bus in ScsiBuses.GetSubKeyNames())
                                    {
                                        using (RegistryKey ScsuiBus = Registry.LocalMachine.OpenSubKey($"HARDWARE\\DEVICEMAP\\Scsi\\{port}\\{bus}\\Target Id 0\\Logical Unit Id 0", true))
                                        {
                                            if (ScsuiBus != null)
                                            {
                                                object deviceTypeValue = ScsuiBus.GetValue("DeviceType");
                                                if (deviceTypeValue != null && deviceTypeValue.ToString() == "DiskPeripheral")
                                                {
                                                    string identifierBefore = ScsuiBus.GetValue("Identifier").ToString();
                                                    string serialNumberBefore = ScsuiBus.GetValue("SerialNumber").ToString();

                                                    string identifierAfter = GetRandomDiskName();
                                                    string serialNumberAfter = RandomId(14);
                                                    string logBefore = $"DiskPeripheral {bus}\\Target Id 0\\Logical Unit Id 0 - Identifier: {identifierBefore}, SerialNumber: {serialNumberBefore}";
                                                    string logAfter = $"DiskPeripheral {bus}\\Target Id 0\\Logical Unit Id 0 - Identifier: {identifierAfter}, SerialNumber: {serialNumberAfter}";
                                                    SaveLogs("disk", logBefore, logAfter);
                                                    globalSerialNumber = serialNumberAfter;
                                                    globalIdentifier = identifierAfter;

                                                    ScsuiBus.SetValue("DeviceIdentifierPage", Encoding.UTF8.GetBytes(serialNumberAfter));
                                                    ScsuiBus.SetValue("Identifier", identifierAfter);
                                                    ScsuiBus.SetValue("InquiryData", Encoding.UTF8.GetBytes(identifierAfter));
                                                    ScsuiBus.SetValue("SerialNumber", serialNumberAfter);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    ShowNotification("ScsiBuses key not found.", NotificationType.Error);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        ShowNotification("ScsiPorts key not found.", NotificationType.Error);
                        return;
                    }
                }

                using (RegistryKey diskKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\IDE"))
                {
                    if (diskKey != null)
                    {
                        foreach (string controllerId in diskKey.GetSubKeyNames())
                        {
                            using (RegistryKey controller = diskKey.OpenSubKey(controllerId))
                            {
                                if (controller != null)
                                {
                                    foreach (string diskId in controller.GetSubKeyNames())
                                    {
                                        using (RegistryKey disk = controller.OpenSubKey(diskId, true))
                                        {
                                            if (disk != null)
                                            {
                                                string serialNumberBefore = disk.GetValue("SerialNumber")?.ToString();

                                                string serialNumberAfter = RandomId(14);
                                                string logBefore = $"Hard Disk {diskId} - SerialNumber: {serialNumberBefore}";
                                                string logAfter = $"Hard Disk {diskId} - SerialNumber: {serialNumberAfter}";
                                                SaveLogs("disk", logBefore, logAfter);
                                                globalSerialNumberKey = serialNumberAfter;

                                                disk.SetValue("SerialNumber", serialNumberAfter);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                ShowNotification("DISK successfully spoofed.", NotificationType.Success);
            }
            catch (Exception ex)
            {
                ShowNotification("An error occurred while spoofing the DISK: " + ex.Message, NotificationType.Error);
            }
        }

        private string GetRandomDiskName()
        {
            Random random = new Random();
            int index = random.Next(diskNames.Count);
            return diskNames[index];
        }



        private void mac_Click(object sender, EventArgs e)
        {
            try
            {
                bool spoofSuccess = SpoofMAC();

                if (!spoofSuccess)
                {
                    ShowNotification("MAC address successfully spoofed.", NotificationType.Success);
                }
            }
            catch (Exception ex)
            {
                ShowNotification("An error occurred while spoofing the MAC address: " + ex.Message, NotificationType.Error);
            }
        }


        private bool SpoofMAC()
        {
            bool err = false;

            using (RegistryKey NetworkAdapters = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Class\\{4d36e972-e325-11ce-bfc1-08002be10318}"))
            {
                foreach (string adapter in NetworkAdapters.GetSubKeyNames())
                {
                    if (adapter != "Properties")
                    {
                        try
                        {
                            using (RegistryKey NetworkAdapter = Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Control\\Class\\{{4d36e972-e325-11ce-bfc1-08002be10318}}\\{adapter}", true))
                            {
                                if (NetworkAdapter.GetValue("BusType") != null)
                                {
                                    string adapterId = NetworkAdapter.GetValue("NetCfgInstanceId").ToString();
                                    string macBefore = NetworkAdapter.GetValue("NetworkAddress")?.ToString();
                                    string macAfter = RandomMac();
                                    string logBefore = $"MAC Address {adapterId} - Before: {macBefore}";
                                    string logAfter = $"MAC Address {adapterId} - After: {macAfter}";
                                    SaveLogs("mac", logBefore, logAfter);
                                    NetworkAdapter.SetValue("NetworkAddress", macAfter);
                                    RestartNetworkAdapter(adapterId);
                                }
                            }
                        }
                        catch (System.Security.SecurityException)
                        {
                            err = true;
                            break;
                        }
                    }
                }
            }

            return err;
        }

        private void RestartNetworkAdapter(string adapterId)
        {
            string logBefore = $"MAC Address: Restarting NetAdapter...";

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "powershell.exe";
            psi.Arguments = $"-Command \"Disable-NetAdapter -Name '{adapterId}'; Start-Sleep -Seconds 5; Enable-NetAdapter -Name '{adapterId}'\"";
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;
            psi.Verb = "runas";
            string logAfter = $"MAC Address: NetAdapter restartet.";

            SaveLogs("mac", logBefore, logAfter);

            Process.Start(psi);
        }


        private void GUID_Click(object sender, EventArgs e)
        {
            try
            {
                using (RegistryKey HardwareGUID = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\IDConfigDB\\Hardware Profiles\\0001", true))
                {
                    if (HardwareGUID != null)
                    {
                        string logBefore = "HwProfileGuid - Before: " + HardwareGUID.GetValue("HwProfileGuid");
                        HardwareGUID.DeleteValue("HwProfileGuid");
                        string newguid = Guid.NewGuid().ToString();
                        HardwareGUID.SetValue("HwProfileGuid", newguid);
                        string logAfter = "HwProfileGuid - After: " + HardwareGUID.GetValue("HwProfileGuid");
                        SaveLogs("guid", logBefore, logAfter);
                        globalGUID = newguid;
                    }
                    else
                    {
                        ShowNotification("HardwareGUID key not found.", NotificationType.Error);
                        return;
                    }
                }

                ShowNotification("HwProfile successfully spoofed.", NotificationType.Success);
            }
            catch (Exception ex)
            {
                ShowNotification("An error occurred: " + ex.Message, NotificationType.Error);
            }
        }


        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly Random random = new Random();

        private static long ConvertToUnixTimestamp(DateTime dateTime)
        {
            return (long)(dateTime - UnixEpoch).TotalSeconds;
        }

        private static DateTime GetRandomDateTime(int maxDateYears = 6)
        {
            DateTime now = DateTime.UtcNow;
            DateTime minTime = now.AddYears(-maxDateYears);

            long maxUnixTime = ConvertToUnixTimestamp(now);
            long minUnixTime = ConvertToUnixTimestamp(minTime);

            long randomUnixTime = minUnixTime + (long)(random.NextDouble() * (maxUnixTime - minUnixTime));

            return UnixEpoch.AddSeconds(randomUnixTime);
        }


        private void winid_Click(object sender, EventArgs e)
        {
            try
            {
                using (RegistryKey machineGuidKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography", true))
                {
                    if (machineGuidKey != null)
                    {
                        string machineGuidBefore = machineGuidKey.GetValue("MachineGuid")?.ToString();

                        string newMachineGuid = RandomIdprid2(10);
                        machineGuidKey.SetValue("MachineGuid", newMachineGuid);
                        globalMachineGuid = newMachineGuid;
                        string logBefore = $"Machine GUID - Before: {machineGuidBefore}";
                        string logAfter = $"Machine GUID - After: {newMachineGuid}";
                        SaveLogs("ChangeMachineGuid", logBefore, logAfter);
                        ShowNotification("Machine GUID successfully spoofed.", NotificationType.Success);
                    }
                    else
                    {
                        ShowNotification("Machine GUID registry key not found.", NotificationType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowNotification("An error occurred while changing the machine GUID: " + ex.Message, NotificationType.Error);
            }
        }


        private void profile_Save(object sender, EventArgs e)
        {
            try
            {
                string programDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string profilesFolder = Path.Combine(programDirectory, "Profiles");
                string profilesPath = Path.Combine(profilesFolder, "profiles.txt");


                string filePath = profilesPath;
                List<string> lines = new List<string>();
                lines = File.ReadAllLines(filePath).ToList();

                lines.Add($"{globalSerialNumber},{globalIdentifier},{globalSerialNumberKey},{globalGUID},{globalMachineGuid}");
                File.WriteAllLines(filePath, lines);

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while creating the registry backup: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void backup_Click(object sender, EventArgs e)
        {
            try
            {
                string programDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string backupFolder = Path.Combine(programDirectory, "Backup");
                string backupPath = Path.Combine(backupFolder, "backup.reg");
                Directory.CreateDirectory(backupFolder);
                Process.Start("reg", $"export HKEY_LOCAL_MACHINE\\SYSTEM \"{backupPath}\" /y").WaitForExit();
                Process.Start("reg", $"export HKEY_LOCAL_MACHINE\\HARDWARE \"{backupPath}\" /y").WaitForExit();
                Process.Start("reg", $"export HKEY_LOCAL_MACHINE\\SOFTWARE \"{backupPath}\" /y").WaitForExit();

                MessageBox.Show("Registry backup created successfully.", "Backup Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while creating the registry backup: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void req_Click(object sender, EventArgs e)
        {
            string[] registryEntries = new string[]
            {
        "HARDWARE\\DEVICEMAP\\Scsi",
        "HARDWARE\\DESCRIPTION\\System\\MultifunctionAdapter\\0\\DiskController\\0\\DiskPeripheral",
        "SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ComputerName",
        "SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ActiveComputerName",
        "SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters",
        "SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters\\Interfaces",
        "SYSTEM\\CurrentControlSet\\Control\\IDConfigDB\\Hardware Profiles\\0001",
        "SOFTWARE\\Microsoft\\Cryptography",
        "SOFTWARE\\Microsoft\\SQMClient",
        "SYSTEM\\CurrentControlSet\\Control\\SystemInformation",
        "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WindowsUpdate",
        "SYSTEM\\CurrentControlSet\\Control\\Class\\{4d36e972-e325-11ce-bfc1-08002be10318}",
        "SYSTEM\\CurrentControlSet\\Control\\Nsi\\{eb004a03-9b1a-11d4-9123-0050047759bc}\\26",
        "HARDWARE\\DESCRIPTION\\System\\BIOS"
            };

            List<string> missingEntries = new List<string>();

            foreach (string entry in registryEntries)
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(entry))
                {
                    if (key == null)
                    {
                        missingEntries.Add(entry);
                    }
                }
            }

            if (missingEntries.Count > 0)
            {
                string errorMessage = Encoding.UTF8.GetString(Convert.FromBase64String("UmVnaXN0cnkgZW50cmllcyBub3QgZm91bmQ6"));
                foreach (string entry in missingEntries)
                {
                    errorMessage += Encoding.UTF8.GetString(Convert.FromBase64String("Cg==")) + entry;
                }
                ShowNotification(errorMessage, NotificationType.Error);
            }
            else
            {
                ShowNotification(Encoding.UTF8.GetString(Convert.FromBase64String("QWxsIHJlZ2lzdHJ5IGVudHJpZXMgZXhpc3Qu")), NotificationType.Success);
            }
        }

        private void Enable_LocalAreaConnection(string adapterId, bool enable)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "netsh";
                process.StartInfo.Arguments = $"interface set interface \"{adapterId}\" {(enable ? "enable" : "disable")}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();
            }
        }


        private bool isAnimationRunnin = false;

        private async void ShowNotification(string message, NotificationType type)
        {
            if (isAnimationRunnin)
            {
                while (isAnimationRunnin)
                {
                    await Task.Delay(20);
                }
            }

            SystemSounds.Exclamation.Play();
            isAnimationRunnin = true;

            string originalMessage = label1.Text;
            Color originalColor = label1.ForeColor;
            bool originalVisibility = label1.Visible;

            label1.Text = message;

            Color startColor = Color.Magenta;
            Color targetColor = Color.FromArgb(255, 16, 16, 17);

            await ChangeColor(label1, startColor, targetColor, 300);

            label1.Text = originalMessage;
            label1.ForeColor = originalColor;
            label1.Visible = originalVisibility;

            isAnimationRunnin = false;
        }

        private async Task ChangeColor(Label label, Color startColor, Color targetColor, int duration)
        {
            int steps = 70;
            int delay = duration / steps;

            for (int i = 0; i <= steps; i++)
            {
                int currentR = startColor.R + (int)Math.Round((double)i / steps * (targetColor.R - startColor.R));
                int currentG = startColor.G + (int)Math.Round((double)i / steps * (targetColor.G - startColor.G));
                int currentB = startColor.B + (int)Math.Round((double)i / steps * (targetColor.B - startColor.B));

                label.ForeColor = Color.FromArgb(255, currentR, currentG, currentB);
                await Task.Delay(delay);
            }
        }

        //temp 
        private MessageBoxIcon GetNotificationIcon(NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Success:
                    return MessageBoxIcon.Information;
                case NotificationType.Error:
                    return MessageBoxIcon.Error;
                case NotificationType.Warning:
                    return MessageBoxIcon.Warning;
                default:
                    return MessageBoxIcon.None;
            }
        }

        private enum NotificationType
        {
            Success,
            Error,
            Warning
        }


        private void systemcleaner_CheckedChanged(object sender, EventArgs e)
        {
            isAfricaToggleOn = systemcleaner.Checked;
            if (isAfricaToggleOn)
            {
                OpenAfrica();
            }
            else
            {
                CloseAfrica();
            }
        }

        private void logwind_CheckedChanged(object sender, EventArgs e)
        {
            if (logWindow == null || logWindow.IsDisposed)
            {
                logWindow = new logs();
                logWindow.Show();
            }
            else
            {
                logWindow.Show();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private async void disk_Save(string newSerial, string newIdentifier, string newSerialDisk)
        {
            try
            {
                using (RegistryKey ScsiPorts = Registry.LocalMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\Scsi"))
                {
                    if (ScsiPorts != null)
                    {
                        foreach (string port in ScsiPorts.GetSubKeyNames())
                        {
                            using (RegistryKey ScsiBuses = Registry.LocalMachine.OpenSubKey($"HARDWARE\\DEVICEMAP\\Scsi\\{port}"))
                            {
                                if (ScsiBuses != null)
                                {
                                    foreach (string bus in ScsiBuses.GetSubKeyNames())
                                    {
                                        using (RegistryKey ScsuiBus = Registry.LocalMachine.OpenSubKey($"HARDWARE\\DEVICEMAP\\Scsi\\{port}\\{bus}\\Target Id 0\\Logical Unit Id 0", true))
                                        {
                                            if (ScsuiBus != null)
                                            {
                                                object deviceTypeValue = ScsuiBus.GetValue("DeviceType");
                                                if (deviceTypeValue != null && deviceTypeValue.ToString() == "DiskPeripheral")
                                                {
                                                    string identifierBefore = ScsuiBus.GetValue("Identifier").ToString();
                                                    string serialNumberBefore = ScsuiBus.GetValue("SerialNumber").ToString();

                                                    string identifierAfter = newIdentifier;
                                                    string serialNumberAfter = newSerial;
                                                    string logBefore = $"DiskPeripheral {bus}\\Target Id 0\\Logical Unit Id 0 - Identifier: {identifierBefore}, SerialNumber: {serialNumberBefore}";
                                                    string logAfter = $"DiskPeripheral {bus}\\Target Id 0\\Logical Unit Id 0 - Identifier: {identifierAfter}, SerialNumber: {serialNumberAfter}";
                                                    SaveLogs("disk", logBefore, logAfter);
                                                    ScsuiBus.SetValue("DeviceIdentifierPage", Encoding.UTF8.GetBytes(serialNumberAfter));
                                                    ScsuiBus.SetValue("Identifier", identifierAfter);
                                                    ScsuiBus.SetValue("InquiryData", Encoding.UTF8.GetBytes(identifierAfter));
                                                    ScsuiBus.SetValue("SerialNumber", serialNumberAfter);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    ShowNotification("ScsiBuses key not found.", NotificationType.Error);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        ShowNotification("ScsiPorts key not found.", NotificationType.Error);
                        return;
                    }
                }

                using (RegistryKey diskKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\IDE"))
                {
                    if (diskKey != null)
                    {
                        foreach (string controllerId in diskKey.GetSubKeyNames())
                        {
                            using (RegistryKey controller = diskKey.OpenSubKey(controllerId))
                            {
                                if (controller != null)
                                {
                                    foreach (string diskId in controller.GetSubKeyNames())
                                    {
                                        using (RegistryKey disk = controller.OpenSubKey(diskId, true))
                                        {
                                            if (disk != null)
                                            {
                                                string serialNumberBefore = disk.GetValue("SerialNumber")?.ToString();

                                                string serialNumberAfter = newSerialDisk;
                                                string logBefore = $"Hard Disk {diskId} - SerialNumber: {serialNumberBefore}";
                                                string logAfter = $"Hard Disk {diskId} - SerialNumber: {serialNumberAfter}";
                                                SaveLogs("disk", logBefore, logAfter);

                                                disk.SetValue("SerialNumber", newSerialDisk);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                ShowNotification("DISK successfully spoofed.", NotificationType.Success);
            }
            catch (Exception ex)
            {
                ShowNotification("An error occurred while spoofing the DISK: " + ex.Message, NotificationType.Error);
            }
        }

        private void GUID_Save(string newGUIDSAVED)
        {
            try
            {
                using (RegistryKey HardwareGUID = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\IDConfigDB\\Hardware Profiles\\0001", true))
                {
                    if (HardwareGUID != null)
                    {
                        string logBefore = "HwProfileGuid - Before: " + HardwareGUID.GetValue("HwProfileGuid");
                        HardwareGUID.DeleteValue("HwProfileGuid");
                        string newguid = newGUIDSAVED;
                        HardwareGUID.SetValue("HwProfileGuid", newguid);
                        string logAfter = "HwProfileGuid - After: " + HardwareGUID.GetValue("HwProfileGuid");
                        SaveLogs("guid", logBefore, logAfter);
                    }
                    else
                    {
                        ShowNotification("HardwareGUID key not found.", NotificationType.Error);
                        return;
                    }
                }

                ShowNotification("HwProfile successfully spoofed.", NotificationType.Success);
            }
            catch (Exception ex)
            {
                ShowNotification("An error occurred: " + ex.Message, NotificationType.Error);
            }
        }

        private void winid_Save(string newWinID)
        {
            try
            {
                using (RegistryKey machineGuidKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography", true))
                {
                    if (machineGuidKey != null)
                    {
                        string machineGuidBefore = machineGuidKey.GetValue("MachineGuid")?.ToString();

                        string newMachineGuid = newWinID;
                        machineGuidKey.SetValue("MachineGuid", newMachineGuid);
                        string logBefore = $"Machine GUID - Before: {machineGuidBefore}";
                        string logAfter = $"Machine GUID - After: {newMachineGuid}";
                        SaveLogs("ChangeMachineGuid", logBefore, logAfter);
                        ShowNotification("Machine GUID successfully spoofed.", NotificationType.Success);
                    }
                    else
                    {
                        ShowNotification("Machine GUID registry key not found.", NotificationType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowNotification("An error occurred while changing the machine GUID: " + ex.Message, NotificationType.Error);
            }
        }
        private void profile1_click(object sender, EventArgs e)
        {
            try
            {
                string programDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string profilesFolder = Path.Combine(programDirectory, "Profiles");
                string profilesPath = Path.Combine(profilesFolder, "profiles.txt");

                string filePath = profilesPath;
                List<string> lines = new List<string>();
                lines = File.ReadAllLines(filePath).ToList();

                string input = lines[0];
                string[] words = input.Split(',');
                disk_Save(words[0], words[1], words[2]);
                GUID_Save(words[3]);
                winid_Save(words[4]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while creating the registry backup: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void profile2_Click(object sender, EventArgs e)
        {
            try
            {
                string programDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string profilesFolder = Path.Combine(programDirectory, "Profiles");
                string profilesPath = Path.Combine(profilesFolder, "profiles.txt");

                string filePath = profilesPath;
                List<string> lines = new List<string>();
                lines = File.ReadAllLines(filePath).ToList();

                string input = lines[1];
                string[] words = input.Split(',');
                disk_Save(words[0], words[1], words[2]);
                GUID_Save(words[3]);
                winid_Save(words[4]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while creating the registry backup: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void profile3_Click(object sender, EventArgs e)
        {
            try
            {
                string programDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string profilesFolder = Path.Combine(programDirectory, "Profiles");
                string profilesPath = Path.Combine(profilesFolder, "profiles.txt");

                string filePath = profilesPath;
                List<string> lines = new List<string>();
                lines = File.ReadAllLines(filePath).ToList();

                string input = lines[2];
                string[] words = input.Split(',');
                disk_Save(words[0], words[1], words[2]);
                GUID_Save(words[3]);
                winid_Save(words[4]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while creating the registry backup: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void profile4_Click(object sender, EventArgs e)
        {
            try
            {
                string programDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string profilesFolder = Path.Combine(programDirectory, "Profiles");
                string profilesPath = Path.Combine(profilesFolder, "profiles.txt");

                string filePath = profilesPath;
                List<string> lines = new List<string>();
                lines = File.ReadAllLines(filePath).ToList();

                string input = lines[3];
                string[] words = input.Split(',');
                disk_Save(words[0], words[1], words[2]);
                GUID_Save(words[3]);
                winid_Save(words[4]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while creating the registry backup: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void profile5_Click(object sender, EventArgs e)
        {
            try
            {
                string programDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string profilesFolder = Path.Combine(programDirectory, "Profiles");
                string profilesPath = Path.Combine(profilesFolder, "profiles.txt");

                string filePath = profilesPath;
                List<string> lines = new List<string>();
                lines = File.ReadAllLines(filePath).ToList();

                string input = lines[4];
                string[] words = input.Split(',');
                disk_Save(words[0], words[1], words[2]);
                GUID_Save(words[3]);
                winid_Save(words[4]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while creating the registry backup: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void profile6_Click(object sender, EventArgs e)
        {
            try
            {
                string programDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string profilesFolder = Path.Combine(programDirectory, "Profiles");
                string profilesPath = Path.Combine(profilesFolder, "profiles.txt");

                string filePath = profilesPath;
                List<string> lines = new List<string>();
                lines = File.ReadAllLines(filePath).ToList();

                string input = lines[5];
                string[] words = input.Split(',');
                disk_Save(words[0], words[1], words[2]);
                GUID_Save(words[3]);
                winid_Save(words[4]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while creating the registry backup: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }




}
