using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web.Http;

namespace WolWeb.Controllers {
    public class ShutdownController : ApiController {

        [HttpGet]
        public string Index(string id) {
            try {
                CreateProcessAsUser(@"c:\windows\system32\shutdown.exe", @" /s /t 0 /m " + id);
                return "Success";
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }


        //took most of the code from http://odetocode.com/Blogs/scott/archive/2004/10/29/createprocessasuser.aspx
        private void CreateProcessAsUser(string executeable, string arguments) {
            IntPtr hToken = WindowsIdentity.GetCurrent().Token;
            IntPtr hDupedToken = IntPtr.Zero;

            ProcessUtility.PROCESS_INFORMATION pi = new ProcessUtility.PROCESS_INFORMATION();

            try {
                ProcessUtility.SECURITY_ATTRIBUTES sa = new ProcessUtility.SECURITY_ATTRIBUTES();
                sa.Length = Marshal.SizeOf(sa);

                bool result = ProcessUtility.DuplicateTokenEx(hToken, ProcessUtility.GENERIC_ALL_ACCESS, ref sa, (int)ProcessUtility.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, (int)ProcessUtility.TOKEN_TYPE.TokenPrimary, ref hDupedToken);

                if (!result)
                    throw new ApplicationException("DuplicateTokenEx failed");

                ProcessUtility.STARTUPINFO si = new ProcessUtility.STARTUPINFO();
                si.cb = Marshal.SizeOf(si);
                si.lpDesktop = String.Empty;

                result = ProcessUtility.CreateProcessAsUser(hDupedToken, executeable, arguments, ref sa, ref sa, false, 0, IntPtr.Zero, @"C:\", ref si, ref pi);

                if (!result) {
                    var error = Marshal.GetLastWin32Error();
                    var message = String.Format("Error: {0}", error);
                    throw new ApplicationException(message);
                }
            }
            finally {
                if (pi.hProcess != IntPtr.Zero)
                    ProcessUtility.CloseHandle(pi.hProcess);
                if (pi.hThread != IntPtr.Zero)
                    ProcessUtility.CloseHandle(pi.hThread);
                if (hDupedToken != IntPtr.Zero)
                    ProcessUtility.CloseHandle(hDupedToken);
            }
        }




        public class ProcessUtility {
            [StructLayout(LayoutKind.Sequential)]
            public struct STARTUPINFO {
                public Int32 cb;
                public string lpReserved;
                public string lpDesktop;
                public string lpTitle;
                public Int32 dwX;
                public Int32 dwY;
                public Int32 dwXSize;
                public Int32 dwXCountChars;
                public Int32 dwYCountChars;
                public Int32 dwFillAttribute;
                public Int32 dwFlags;
                public Int16 wShowWindow;
                public Int16 cbReserved2;
                public IntPtr lpReserved2;
                public IntPtr hStdInput;
                public IntPtr hStdOutput;
                public IntPtr hStdError;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct PROCESS_INFORMATION {
                public IntPtr hProcess;
                public IntPtr hThread;
                public Int32 dwProcessID;
                public Int32 dwThreadID;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct SECURITY_ATTRIBUTES {
                public Int32 Length;
                public IntPtr lpSecurityDescriptor;
                public bool bInheritHandle;
            }

            public enum SECURITY_IMPERSONATION_LEVEL {
                SecurityAnonymous,
                SecurityIdentification,
                SecurityImpersonation,
                SecurityDelegation
            }

            public enum TOKEN_TYPE {
                TokenPrimary = 1,
                TokenImpersonation
            }

            public const int GENERIC_ALL_ACCESS = 0x10000000;

            [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern bool CloseHandle(IntPtr handle);

            [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
            public static extern bool
               CreateProcessAsUser(IntPtr hToken, string lpApplicationName, string lpCommandLine,
                                   ref SECURITY_ATTRIBUTES lpProcessAttributes, ref SECURITY_ATTRIBUTES lpThreadAttributes,
                                   bool bInheritHandle, Int32 dwCreationFlags, IntPtr lpEnvrionment,
                                   string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo,
                                   ref PROCESS_INFORMATION lpProcessInformation);

            [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx")]
            public static extern bool DuplicateTokenEx(IntPtr hExistingToken, Int32 dwDesiredAccess,
                                ref SECURITY_ATTRIBUTES lpThreadAttributes,
                                Int32 ImpersonationLevel, Int32 dwTokenType,
                                ref IntPtr phNewToken);
        }


    }
}
