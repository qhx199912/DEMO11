using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Text;

namespace BIW.Common.CrashReport
{
    internal static class HelperMethods
    {
        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr LoadLibrary(string libraryName);

        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr GetProcAddress(IntPtr hwnd, string procedureName);

        private static bool IsOS64Bit()
        {
            return IntPtr.Size == 8 || (IntPtr.Size == 4 && Is32BitProcessOn64BitProcessor());
        }

        private static IsWow64ProcessDelegate GetIsWow64ProcessDelegate()
        {
            IntPtr handle = LoadLibrary("kernel32");

            if (handle != IntPtr.Zero)
            {
                IntPtr fnPtr = GetProcAddress(handle, "IsWow64Process");

                if (fnPtr != IntPtr.Zero)
                {
                    return
                        (IsWow64ProcessDelegate)
                        Marshal.GetDelegateForFunctionPointer(fnPtr, typeof(IsWow64ProcessDelegate));
                }
            }

            return null;
        }

        private static bool Is32BitProcessOn64BitProcessor()
        {
            IsWow64ProcessDelegate fnDelegate = GetIsWow64ProcessDelegate();

            if (fnDelegate == null)
            {
                return false;
            }

            bool isWow64;
            bool retVal = fnDelegate.Invoke(Process.GetCurrentProcess().Handle, out isWow64);

            if (retVal == false)
            {
                return false;
            }

            return isWow64;
        }

        private static string HKLM_GetString(string key, string value)
        {
            try
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key);
                if (registryKey == null) return "";
                return (string)registryKey.GetValue(value);
            }
            catch { return ""; }
        }

        private delegate bool IsWow64ProcessDelegate([In] IntPtr handle, [Out] out bool isWow64Process);

        public static string GetWindowsVersion()
        {
            string osArchitecture;
            try
            {
                osArchitecture = IsOS64Bit() ? "64-bit" : "32-bit";
            }
            catch (Exception)
            {
                osArchitecture = "32/64-bit (Undetermined)";
            }
            string productName = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            string csdVersion = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
            string currentBuild = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild");
            if (productName != "")
            {
                return string.Format("{0}{1}{2}{3} (OS Build {4})", (productName.StartsWith("Microsoft") ? "" : "Microsoft "), productName, (csdVersion != "" ? " " + csdVersion : ""), osArchitecture, currentBuild);
            }
            return "";
        }

        public static string GetBase64FromImage(string imagefile)
        {
            string strbaser64 = "";
            try
            {
                Image bmp = Image.FromFile(imagefile);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                strbaser64 = Convert.ToBase64String(arr);
            }
            catch (Exception)
            {
                return null;
            }
            return strbaser64;
        }
        public static string EncodeJsString(string s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"");
            foreach (char c in s)
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        int i = (int)c;
                        if (i < 32 || i > 127)
                        {
                            sb.AppendFormat("\\u{0:X04}", i);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append("\"");

            return sb.ToString();
        }
        public static string GetBase64FormString(string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        public static string GetStringFromBase64(string str)
        {
            byte[] data = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(data);
        }
    }
}
