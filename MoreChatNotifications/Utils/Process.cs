using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

namespace MoreChatNotifications.Utils {
    public static class ProcessUtils {
        [DllImport("psapi.dll", SetLastError = true)]
        static extern bool EnumProcesses(IntPtr lpProcesses, uint cb, out uint lpcbNeeded);

        const int MAX_NAME_LENGTH = 256;
        const string PROCESS_NAME = "YourProcessName";

        static void Main() {
            IntPtr processesBuffer = Marshal.AllocHGlobal(65536);
            uint bytesNeeded;
            EnumProcesses(processesBuffer, 65536, out bytesNeeded);

            foreach (uint processId in GetProcessIdsFromBuffer(processesBuffer)) {
                // Removed the assignment to Process.Id since it's read-only
                if (GetProcessNameById(processId) == PROCESS_NAME) {
                    Console.WriteLine($"Found {PROCESS_NAME}");
                }
            }

            Marshal.FreeHGlobal(processesBuffer);
        }

        static IEnumerable<uint> GetProcessIdsFromBuffer(IntPtr buffer) {
            int size = 0;
            uint count = 0;
            while (size < 65536) {
                int bytesRead = 0;
                GetProcessImageFileName((IntPtr)(buffer.ToInt64() + size), MAX_NAME_LENGTH, out bytesRead);
                if (bytesRead > 0) {
                    var byteArray = new byte[65536];
                    Marshal.Copy(buffer, byteArray, 0, 65536);
                    yield return BitConverter.ToUInt32(byteArray, size);
                    size += bytesRead; // Ensure this addition is correct
                } else {
                    break;
                }
            }
        }

        [DllImport("psapi.dll")]
        static extern bool GetProcessImageFileName(IntPtr baseAddress, int size, out int bytesReturned);

        // Helper method to get the process name by ID
        private static string GetProcessNameById(uint processId) {
            var handle = OpenProcess((int)ProcessAccess.QueryInformation, false, processId);
            if (handle != IntPtr.Zero) {
                var processName = new StringBuilder(MAX_NAME_LENGTH);
                if (GetProcessName(handle, processName)) {
                    return processName.ToString();
                }
            }
            return null;
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("psapi.dll")]
        static extern bool GetProcessName(IntPtr processHandle, StringBuilder processName);

        [Flags]
        enum ProcessAccess : uint {
            QueryInformation = 0x400,
            // Add other flags as needed
        }
    }
}

