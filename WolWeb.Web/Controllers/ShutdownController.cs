using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web.Http;

namespace WolWeb.Controllers {
    [AuthorizeRemoteOnly]
    public class ShutdownController : ApiController {

        [HttpGet]
        public string Restart(string id) {
            return RunProcess(id, "/r");
        }


        [HttpGet]
        public string Index(string id) {
            return RunProcess(id, "/s");
        }


        private string RunProcess(string id, string command) {
            try {
                IntPtr dupedToken = new IntPtr(0);

                SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
                sa.bInheritHandle = false;
                sa.Length = Marshal.SizeOf(sa);
                sa.lpSecurityDescriptor = (IntPtr)0;

                var token = WindowsIdentity.GetCurrent().Token;

                const uint GENERIC_ALL = 0x10000000;

                const int SecurityImpersonation = 2;
                const int TokenType = 1;

                var ret = DuplicateTokenEx(token, GENERIC_ALL, ref sa, SecurityImpersonation, TokenType, ref dupedToken);
                if (ret == false)
                    throw new Exception("DuplicateTokenEx failed (" + Marshal.GetLastWin32Error() + ")");

                STARTUPINFO si = new STARTUPINFO();
                si.cb = Marshal.SizeOf(si);
                si.lpDesktop = "";
                PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

                uint exitCode;
                try {
                    ret = CreateProcessAsUser(dupedToken, null, @"c:\windows\system32\shutdown.exe " + command + " /t 0 /m " + id, ref sa, ref sa, false, 0, (IntPtr)0, "c:\\", ref si, out pi);

                    if (ret == false)
                        throw new Exception("CreateProcessAsUser failed (" + Marshal.GetLastWin32Error() + ")");

                    WaitForSingleObject(pi.hProcess, 10000);
                    GetExitCodeProcess(pi.hProcess, out exitCode);
                }
                catch (Exception ex) {
                    throw ex;
                }
                finally {
                    CloseHandle(pi.hProcess);
                    CloseHandle(pi.hThread);
                    CloseHandle(dupedToken);
                }

                if (exitCode == 0)
                    return "";
                return "Exit code: " + exitCode;
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }



        //took most of the code from http://support.microsoft.com/kb/889251/en-us?fr=1
        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO {
            public int cb;
            public String lpReserved;
            public String lpDesktop;
            public String lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES {
            public int Length;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public extern static bool CloseHandle(IntPtr handle);

        [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static bool CreateProcessAsUser(IntPtr hToken, String lpApplicationName, String lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandle, int dwCreationFlags, IntPtr lpEnvironment,
            String lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx")]
        public extern static bool DuplicateTokenEx(IntPtr ExistingTokenHandle, uint dwDesiredAccess,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, int TokenType,
            int ImpersonationLevel, ref IntPtr DuplicateTokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetExitCodeProcess(IntPtr hProcess, out uint lpExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);



    }
}
