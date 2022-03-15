using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace WpfApp1
{
    class UnityHwndHost : HwndHost
    {


        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint processId);
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")] // TODO: 32/64?
        internal static extern IntPtr GetWindowLongPtr(IntPtr hWnd, Int32 nIndex);
        internal const Int32 GWLP_USERDATA = -21;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr PostMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        internal const UInt32 WM_CLOSE = 0x0010;

        private string programName;
       // private string arguments;
        private Process process = null;
        private IntPtr unityHWND = IntPtr.Zero;

        
         private const int WM_ACTIVATE = 0x0006;
         private readonly IntPtr WA_ACTIVE = new IntPtr(1);
       

        public UnityHwndHost(string programName)
        {
            this.programName = programName;
           // this.arguments = arguments;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            
            try {

                Debug.WriteLine("Going to launch Unity at: " + this.programName);

                process = new Process();
                //process.StartInfo = new ProcessStartInfo
                //{
                //    FileName = programName,
                //    Arguments = "-parentHWND " + hwndParent.Handle,
                //    UseShellExecute = true,
                //    CreateNoWindow = true
                //};
                process.StartInfo.FileName = programName;
                process.StartInfo.Arguments = "-parentHWND " + hwndParent.Handle;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                process.WaitForInputIdle(3000);
                
                int repeat = 50;
                while (unityHWND == IntPtr.Zero && repeat-- > 0)
                {
                    Thread.Sleep(100);
                    EnumChildWindows(hwndParent.Handle, WindowEnum, IntPtr.Zero);
                }
                if (unityHWND == IntPtr.Zero)
                    throw new Exception("Unable to find Unity window");

                Debug.WriteLine("Found Unity window: " + unityHWND);

                repeat += 150;
                while ((GetWindowLongPtr(unityHWND, GWLP_USERDATA).ToInt32() & 1) == 0 && --repeat > 0)
                {
                    Thread.Sleep(100);
                    Debug.WriteLine("Waiting for Unity to initialize... " + repeat);
                }
                if (repeat == 0)
                {
                    Debug.WriteLine("Timed out while waiting for Unity to initialize");
                }
                else
                {                   
                    Debug.WriteLine("Unity initialized!");
                }

                Mouse.OverrideCursor = Cursors.Arrow;

                return new HandleRef(this, unityHWND);

                // ProcessStartInfo psi = new ProcessStartInfo(programName);                
                // psi.UseShellExecute = true;
                // psi.CreateNoWindow = true;
                // process = Process.Start(psi);
                // process.WaitForInputIdle(3000); //Дает компоненту Process команду ожидать входа связанного процесса в состояние простоя.

                // // The main window handle may be unavailable for a while, just wait for it
                // while (process.MainWindowHandle == IntPtr.Zero)
                // {
                //     Thread.Yield();
                // }

                // IntPtr exeHandle = process.MainWindowHandle;                 


                // int oldstyle = GetWindowLong(exeHandle, GWL_STYLE);
                // int style = oldstyle & ~((int)WS_CAPTION) & ~((int)WS_THICKFRAME); // Removes Caption bar and the sizing border
                // style |= ((int)WS_CHILD); // Must be a child window to be hosted                                              
                // SetWindowLong(exeHandle, GWL_STYLE, style);
                // // Thread.Sleep(1000);

                // SetParent(exeHandle, hwndParent.Handle);
                //// Thread.Sleep(1000);
                // Debug.WriteLine("Parent was setted");

                // SendMessage(exeHandle, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
                // Thread.Sleep(1000);
                // //SetActiveWindow(exeHandle);
                // Debug.WriteLine("Activated");

                // HandleRef hwnd = new HandleRef(this, exeHandle); 

                // Mouse.OverrideCursor = Cursors.Arrow;
                // return hwnd;             

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
                Mouse.OverrideCursor = Cursors.Arrow;
                
                return new HandleRef(this, unityHWND);
            }                      
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            if (unityHWND != IntPtr.Zero)
                throw new Exception("Found multiple Unity windows");
            unityHWND = hwnd; 
            SendMessage(unityHWND, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
            return 0;
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            
            Debug.WriteLine("Asking Unity to exit...");
            // PostMessage(unityHWND, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            process.CloseMainWindow();
            process.WaitForExit(2000);

            int i = 30;
            while (!process.HasExited)
            {
                if (--i < 0)
                {
                    Debug.WriteLine("Process not dead yet, killing...");
                    process.Kill();
                }
                Thread.Sleep(100);
            }



            // Debug.WriteLine("Asking Unity to exit...");

            // // PostMessage(unityHWND, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            // // SendMessage(unityHWND, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);   

            // try {
            //     process.CloseMainWindow();
            //     int i = 50;
            //     while (!process.HasExited)
            //     {
            //         if (--i < 0)
            //         {
            //             Debug.WriteLine("Process not dead yet, killing...");
            //            // process.Kill();

            //         }
            //         Thread.Sleep(100);
            //         //GC.SuppressFinalize(this); ///???
            //     }
            //     //process.Close();
            // }
            // catch(Exception ex)
            // {
            //     MessageBox.Show(ex.Message, "Ошибка");

            // }
            //// this.InvalidateVisual();
            
             Debug.WriteLine("Process dead");

            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}
